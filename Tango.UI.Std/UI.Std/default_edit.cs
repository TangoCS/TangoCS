using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using Tango.AccessControl;
using Tango.Data;
using Tango.Html;
using Tango.Identity.Std;
using Tango.Logic;
using Tango.Meta;
using Tango.UI.Controls;
using Tango.UI.Std.EntityAudit;
using static Dapper.SqlMapper;
using static Tango.UI.CommonFields;

namespace Tango.UI.Std
{
	public abstract class default_edit : ViewPagePart
	{
		protected abstract string FormTitle { get; }
		protected abstract void Form(LayoutWriter w);
		protected virtual ContainerWidth FormWidth => ContainerWidth.WidthStd;
		protected virtual ContainerHeight FormHeight => ContainerHeight.HeightStd;
		protected virtual bool FormGridMode => false;
		protected virtual bool ShowResultBlock => false;

		public override ViewContainer GetContainer()
		{
			Action<LayoutWriter> header = ContentHeadersConfig.Default;
			if (ParentElement != null)
				header = ContentHeadersConfig.Nested;

			return new EditEntityContainer
			{
				ContentHeader = header,
				GridMode = FormGridMode,
				Width = FormWidth,
				Height = FormHeight,
				ShowResultBlock = ShowResultBlock
			};
		}

		[Inject]
		protected IEntityAudit EntityAudit { get; set; }

		[Inject]
		protected IRequestEnvironment RequestEnvironment { get; set; }

		protected List<IFieldGroup> groups = new List<IFieldGroup>();
		protected T AddFieldGroup<T>(T group)
			where T : IFieldGroup
		{
			group.Init(this);
			groups.Add(group);
			return group;
		}

		public default_edit()
		{
			ID = GetType().Name;
		}

		public override void OnEvent()
		{
			if (groups.Count == 0)
			{
				var grProps = GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
					.Where(o => typeof(IFieldGroup).IsAssignableFrom(o.PropertyType));
				foreach (var prop in grProps)
				{
					var gr = Activator.CreateInstance(prop.PropertyType) as IFieldGroup;
					gr.InjectProperties(Context.RequestServices);
					AddFieldGroup(gr);
					prop.SetValue(this, gr);
				}
			}
			groups.ForEach(g => {
				g.SetValueSource(Context.RequestMethod == "POST" ? ValueSource.Form : ValueSource.Model);
			});
			FieldsPreInit();
		}

		protected virtual void FieldsPreInit()
		{
		}

		protected virtual bool ObjectNotExists => false;

		public override void OnLoad(ApiResponse response)
		{
			response.AddWidget("contenttitle", FormTitle);
			if (!IsSubView && ParentElement == null)
				response.AddWidget("#title", FormTitle);

            if (ObjectNotExists)
			{
				response.AddWidget("form", w => {
					w.Div(Resources.Get("Common.ObjectNotExists"));
				});
			}
			else
			{
				response.AddWidget("form", RenderFormLayout);
				RenderButtonsBarLayout(response);

				if (EnableToolbar)
					response.AddWidget("contenttoolbar", w => Toolbar(w));
			}
		}

		protected virtual void RenderFormLayout(LayoutWriter w)
		{
			Form(w);
			w.FormValidationBlock();
		}

		protected virtual void RenderButtonsBarLayout(ApiResponse response)
		{
			if (EnableButtonsBar)
				response.AddAdjacentWidget("form", "buttonsbar", AdjacentHTMLPosition.BeforeEnd, ButtonsBar);
		}

		public bool ProcessSubmit(ApiResponse response)
		{
			if (Context.FormData == null) return false;

			var m = new ValidationMessageCollection();

			ValidateFormData(m);
			if (m.HasItems(ValidationMessageSeverity.Error)) goto err;
			if (!response.Success) goto ret;
			PreProcessFormData(response, m);
			if (m.HasItems(ValidationMessageSeverity.Error)) goto err;
			if (!response.Success) goto ret;
			ProcessFormData(m);
			if (m.HasItems(ValidationMessageSeverity.Error)) goto err;
			if (!response.Success) goto ret;
			PostProcessFormData(response, m);
			if (m.HasItems(ValidationMessageSeverity.Error)) goto err;
			if (!response.Success) goto ret;

			if (m.HasItems(ValidationMessageSeverity.Information, ValidationMessageSeverity.Warning))
				RenderValidation(response, m);
			else
			{
				response.WithNamesFor(this, () => response.AddWidget("validation", w => w.Write("")));
			}
			return true;

			err:
			RenderValidation(response, m);
			response.Success = false;
			return false;

			ret:
			if (m.Count == 0)
				response.AddWidget("validation", w => w.Write(""));
			return false;
		}

		public virtual void OnSubmit(ApiResponse response)
		{
			if (ObjectNotExists)
			{
				response.AddWidget("form", w => {
					w.Div(Resources.Get("Common.ObjectNotExists"));
				});
				return;
			}
			
			if (!ProcessSubmit(response)) 
				return;

			var m = new ValidationMessageCollection();
			var doSubmit = ProcessObjectChangeRequest(m);
			if (!doSubmit && m.Count > 0)
			{
				RenderValidation(response, m);
				response.Success = false;
				return;
			}

			if (doSubmit)
				Submit(response);

			var doAfterSubmit = PostProcessObjectChangeRequest(response);
			if (doAfterSubmit)
				AfterSubmit(response);
		}
		
		protected virtual void ValidateFormData(ValidationMessageCollection val) => groups.ForEach(g => g.ValidateFormData(val));
        protected virtual void PreProcessFormData(ApiResponse response, ValidationMessageCollection val) { }
        protected virtual void ProcessFormData(ValidationMessageCollection val) => groups.ForEach(g => g.ProcessFormData(val));
		protected virtual void PostProcessFormData(ApiResponse response, ValidationMessageCollection val) { }

		protected virtual bool ProcessObjectChangeRequest(ValidationMessageCollection m) { return true; }
		protected virtual bool PostProcessObjectChangeRequest(ApiResponse response) { return true; }

		protected abstract void Submit(ApiResponse response);
		protected virtual void AfterSubmit(ApiResponse response)
		{
			response.RedirectBack(Context, 1, !IsSubView);
		}

		protected virtual bool EnableButtonsBar => true;
		protected virtual bool EnableToolbar => false;

		protected virtual void Toolbar(LayoutWriter w)
		{
			w.Toolbar(t => ToolbarLeft(t), t => ToolbarRight(t));
		}

		protected virtual void ToolbarLeft(MenuBuilder t) { }

		protected virtual void ToolbarRight(MenuBuilder t) { }

		protected virtual void ButtonsBar(LayoutWriter w)
		{
			w.ButtonsBar_edit(this);
		}

		protected virtual void RenderValidation(ApiResponse response, ValidationMessageCollection m)
		{
			response.WithNamesFor(this).AddWidget("validation", w => w.ValidationBlock(m));
		}

		protected void RefreshField(ApiResponse response, IField field, Action<LayoutWriter> content)
		{
			response.WithNamesFor(this).ReplaceWidget(field.GetFieldID(), content);
		}
    }

	public abstract class default_edit<T> : default_edit
		where T : class
	{
		[Inject]
		protected IObjectTracker Tracker { get; set; }

		[Inject]
		public IObjectChangeRequestManager<T> ChReqManager { get; set; }

		[Inject]
		public IObjectChangeRequestView ChReqView { get; set; }

		T _viewData = null;
		List<string> _changedFields = null;
		List<FieldSnapshot> _srcFieldSnapshot = null;
		List<FieldSnapshot> _destFieldSnapshot = null;
		ObjectChangeRequestData _objectChangeRequestData = null;

		protected T ViewData
		{
			get
			{
				if (_viewData == null)
					_viewData = GetViewData();
				return _viewData;
			}
			set
			{
				_viewData = value;
			}
		}

		protected virtual T GetViewData()
		{
			if (ChReqEnabled && ChangeRequestMode)
				return GetChangeRequestData();
			else if (CreateObjectMode || BulkMode)
				return GetNewEntity();
			else 
				return GetExistingEntity();
		}

		protected T GetChangeRequestData()
		{
			var ochid = Context.GetArg(Constants.ObjectChangeRequestId);
			var data = ChReqView.Load<T>(ochid);
			if (data == null) return null;
			Tracker?.StartTracking(data.Object);
			_changedFields = data.ChangedFields;
			return data.Object;
		}

		public override void OnEvent()
		{
			base.OnEvent();
			groups.ForEach(g => {
				g.Args.Add("IsNewObject", CreateObjectMode);
				g.Args.Add("IsBulkMode", BulkMode);
				g.SetViewData(ViewData);
			});
		}

		public override void OnInit()
		{
			base.OnInit();

			if (ChReqEnabled)
			{
				ChReqView.ID = "chreqview";
				AddControl(ChReqView);
			}

		}

		protected override bool ObjectNotExists => ViewData == null;

		public override ViewContainer GetContainer()
		{
			var c = base.GetContainer();
			if (ChangeRequestMode && c is AbstractDefaultContainer ac)
				ac.Width = ContainerWidth.Width100;

			return c;
		}

		public override void OnLoad(ApiResponse response)
		{
			base.OnLoad(response);
			if (BulkMode)
			{
				response.AddAdjacentWidget("form", "selectedvalues", AdjacentHTMLPosition.BeforeEnd, w => {
					var sel = GetArg(Constants.SelectedValues);
					var cnt = sel?.Split(',').Count() ?? 0;
					if (cnt > 0) w.Hidden(Constants.SelectedValues, sel);
				});
			}
		}

		protected override void RenderFormLayout(LayoutWriter w)
		{
			if (ChReqEnabled)
			{
				if (ChangeRequestMode)
				{
					ChReqView.RenderHeader(w);
					if (ChReqView.Status != ObjectChangeRequestStatus.New)
					{
						groups.ForEach(g => {
							g.Fields.ForEach(f => {
								f.Disabled = true;
							});
						});
					}

					if (ChReqView.Status == ObjectChangeRequestStatus.New)
					{
						if (CreateObjectMode)
						{
							w.Div(a => a.Class("layout1 withwrap width100"), () => {
								w.Div(a => a.Class("sidebar"), () => {
									w.Div(a => a.Class("sidebar-panel"), () => {
										w.Div(a => a.Class("sidebar-header"), () => {
											w.H3(a => a.ID("sidebarcontenttitle"), "Целевые значения");
										});
										w.Div(a => a.Class(FormWidth.ToString().ToLower()), () => {
											Form(w);
											ChReqView.RenderFooter(w);
										});
									});
								});
							});
						}
						else
						{
							var fDisabled = new Dictionary<string, bool>();
							var fWithCB = new Dictionary<string, bool>();

							w.Div(a => a.Class("layout1 withwrap size_5"), () => {
								w.CollapsibleSidebar("Исходные значения", () => {
									groups.ForEach(g => {
										g.SetViewData(GetExistingEntity());
										g.Fields.ForEach(f => {
											fDisabled.Add(f.ID, f.Disabled);
											fWithCB.Add(f.ID, f.WithCheckBox);
											f.Disabled = true;
											f.WithCheckBox = false;
										});
									});
									var id = ID;
									ID += "_oldstate";
									w.WithPrefix(this, () => {
										w.Div(a => a.Class("contentbodypadding"), () => Form(w));
									});
									ID = id;
								});
								w.CollapsibleSidebar("Целевые значения", () => {
									groups.ForEach(g => {
										g.SetViewData(ViewData);
										g.Fields.ForEach(f => {
											if (_changedFields?.Contains(f.ID.ToLower()) ?? false)
												f.Disabled = false;
											else
												f.Disabled = fDisabled[f.ID];
											f.WithCheckBox = fWithCB[f.ID];
										});
									});
									w.Div(a => a.Class("contentbodypadding"), () => Form(w));
								});
							});
						}
					}
					else
					{
						if (CreateObjectMode)
							ChReqView.RenderDestFields(w);
						else
							ChReqView.RenderFields(w);
					}
				}
				else
				{
					Form(w);
					ChReqView.RenderFooter(w);
				}
			}
			else
			{
				if (ChangeRequestMode)
				{
					w.Div(Resources.Get("Common.ChangeRequestModeDisabledMessage"));
				}
				else
				{
					Form(w);
				}
			}
			w.FormValidationBlock();
		}

		protected virtual bool DeleteMode => Context.Action.ToLower() == "delete";
		protected virtual bool CreateObjectMode => !BulkMode && !Context.AllArgs.ContainsKey(Constants.Id);
		protected virtual bool BulkMode => Context.AllArgs.ContainsKey(Constants.SelectedValues);
		protected override string FormTitle => CreateObjectMode ? CreateNewFormTitle : EditFormTitle;
		protected virtual string BulkModeFormTitle => Resources.Get("Common.BulkModeTitle");
		protected virtual string CreateNewFormTitle => Resources.Get(ViewData.GetType().FullName);
		protected virtual string EditFormTitle => ViewData is IWithTitle ? (ViewData as IWithTitle).Title : "";
		bool ReadonlyMode = false;

		bool ChReqEnabled => (ChReqManager?.IsEnabled() ?? false) && ChReqView != null;
		protected bool ChangeRequestMode => Context.AllArgs.ContainsKey(Constants.ObjectChangeRequestId);
		protected bool CreateChangeRequestMode => ChReqEnabled && !DeleteMode && !BulkMode && !ChangeRequestMode;

		protected abstract T GetNewEntity();
		protected abstract T GetExistingEntity();

		protected override void ButtonsBar(LayoutWriter w)
		{
			var width = FormWidth;
			if (ChangeRequestMode && !CreateObjectMode && GetContainer() is AbstractDefaultContainer ac)
				width = ac.Width;

			w.ButtonsBar(a => a.Class(width.ToString().ToLower()), () => {
				w.ButtonsBarRight(() => {
					if (!ReadonlyMode && !(!ChReqEnabled && ChangeRequestMode) && 
						!(ChangeRequestMode && ChReqView.Status.In(ObjectChangeRequestStatus.Approved, ObjectChangeRequestStatus.Rejected)))
					{
						var res = "Common.OK";
						if (CreateChangeRequestMode)
						{
							if (ChReqManager.IsCurrentUserModerator())
								res = "Common.CreateAndApproveObjectChangeRequest";
							else
								res = "Common.CreateObjectChangeRequest";
						}
						if (!ChangeRequestMode || ChReqManager.IsCurrentUserModerator())
							w.SubmitAndBackButton(a => a.DataReceiver(this), Resources.Get(res));
					}
					if (!ReadonlyMode && ChReqEnabled && ChangeRequestMode && ChReqView.Status == ObjectChangeRequestStatus.New && ChReqView.CanReject())
						w.SubmitAndBackButton(a => a.DataEvent(RejectObjectChangeRequest), Resources.Get("Common.RejectObjectChangeRequest"));
					w.BackButton(this);
				});
			});
		}

		protected override void ProcessFormData(ValidationMessageCollection val)
		{
			if (CreateChangeRequestMode && !CreateObjectMode)
			{
				var moderatorMode = ChReqManager?.IsCurrentUserModerator() ?? false;
				if (moderatorMode)
				{
					_srcFieldSnapshot = new List<FieldSnapshot>();
					FillSrcFieldSnapshot(ViewData);
				}
			}

			base.ProcessFormData(val);

			if (ViewData is IWithTimeStamp withTimeStamp)
			{
				withTimeStamp.LastModifiedDate = DateTime.Now;
			}

			if (CreateChangeRequestMode && !CreateObjectMode)
			{
				
			}
		}

		void FillDestFieldSnapshot(T viewData)
		{
			_destFieldSnapshot = new List<FieldSnapshot>();
			groups.ForEach(g => {
				g.SetViewData(viewData);
				g.Fields.ForEach(f => {
					if (f is IEditableField)
					{
						var vs = f.ValueSource;
						if (f.Disabled)
							f.ValueSource = ValueSource.Model;
						else
							f.ValueSource = ValueSource.Form;

						_destFieldSnapshot.Add(new FieldSnapshot {
							ID = f.ID,
							Caption = f.Caption,
							StringValue = f.StringValue,
							Value = f.ToString()
						});
						f.ValueSource = vs;
					}
				});
			});
		}

		void FillSrcFieldSnapshot(T viewData)
		{
			_srcFieldSnapshot = new List<FieldSnapshot>();
			groups.ForEach(g => {
				g.SetViewData(viewData);
				g.Fields.ForEach(f => {
					if (f is IEditableField)
					{
						var vs = f.ValueSource;
						f.ValueSource = ValueSource.Model;
						_srcFieldSnapshot.Add(new FieldSnapshot {
							ID = f.ID,
							Caption = f.Caption,
							StringValue = f.StringValue,
							Value = f.ToString()
						});
						f.ValueSource = vs;
					}
				});
			});
		}

		protected override bool ProcessObjectChangeRequest(ValidationMessageCollection m)
		{
			if (!ChangeRequestMode)
			{
				if (!DeleteMode && !BulkMode)
				{
					var moderatorMode = ChReqManager?.IsCurrentUserModerator() ?? false;

					if (ChReqEnabled)
					{
						var changes = Context.AllArgs
							.Where(x => x.Key.EndsWith("_check"))
							.Select(x => x.Key.Replace("_check", ""))
							.ToList();
						_objectChangeRequestData = new ObjectChangeRequestData {
							Object = ViewData,
							ChangedFields = changes
						};

						ChReqView.Validate(_objectChangeRequestData, m);
						if (m.Count > 0)
							return false;

						if (moderatorMode)
							FillDestFieldSnapshot(ViewData);
					}

					return !ChReqEnabled || moderatorMode;
				}
				return true;
			}
			else
			{
				if (!CreateObjectMode)
					FillSrcFieldSnapshot(GetExistingEntity());

				FillDestFieldSnapshot(ViewData);

				return true;
			}
		}

		protected override bool PostProcessObjectChangeRequest(ApiResponse response)
		{
			if (CreateChangeRequestMode && !(ChReqManager?.IsCurrentUserModerator() ?? false))
			{
				ChReqView.Save(_objectChangeRequestData, Context.GetArg("ocr_comments"));

				groups.ForEach(g => {
					g.SetViewData(ViewData);
					g.Fields.ForEach(f => {
						f.Disabled = true;
						f.WithCheckBox = false;
					});
				});
				ReadonlyMode = true;

				response.RemoveWidget("buttonsbar");
				response.AddWidget("form", RenderFormLayout);
				RenderButtonsBarLayout(response);

				var m = new ValidationMessageCollection();
				m.Add("ochr", ChReqView.RequestCreatedMessage(), ValidationMessageSeverity.Information);
				RenderValidation(response, m);
				return false;
			}
			else
				return base.PostProcessObjectChangeRequest(response);
		}

		protected override void AfterSubmit(ApiResponse response)
		{
			foreach (var arg in Context.ReturnTarget[1].Args.ToList())
			{
				if (arg.Value.StartsWith("@"))
				{
					Context.ReturnTarget[1].Args[arg.Key] =
						typeof(T)
						.GetProperty(arg.Value.Substring(1), BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)
						.GetValue(ViewData)
						.ToString();
					// Какие-то костыли, чтобы работал возврат параметров и коллбэк из модального окна
					string retUrl = Context.GetArg("returnurl");
					if (retUrl != null)
						Url.ParseQuery(retUrl.Substring(retUrl.IndexOf("?") + 1)).ForEach(kv => Context.ReturnTarget[1].Args.AddIfNotExists(kv.Key, kv.Value[0]));
				}
			}
			base.AfterSubmit(response);
		}

		protected virtual IEnumerable<string> GetBulkEditProperties()
		{
			var updFields = new List<string>();
			groups.ForEach(g => {
				updFields.AddRange(g.Fields
					.Where(f => f is IEntityField<T> && f.IsVisible)
					.SelectMany(f => (f as IEntityField<T>).Properties));
			});
			return updFields;
		}

		protected void RejectObjectChangeRequest(ApiResponse response)
		{
			var m = new ValidationMessageCollection();
			var doSubmit = ProcessObjectChangeRequest(m);
			if (doSubmit)
				ChReqView.Reject(_srcFieldSnapshot, _destFieldSnapshot);
			response.RedirectBack(Context, 1, !IsSubView);
		}

		protected void ApproveObjectChangeRequest()
		{
			if (ChangeRequestMode)
				ChReqView.Approve(ViewData, _srcFieldSnapshot, _destFieldSnapshot);
			else if (ChReqEnabled && !BulkMode && !DeleteMode)
			{
				ChReqView.Save(_objectChangeRequestData, Context.GetArg("ocr_comments"));

				if (ChReqManager?.IsCurrentUserModerator() ?? false)
					ChReqView.CreateAndApprove(ViewData, _srcFieldSnapshot, _destFieldSnapshot);
			}
		}
	}

	public abstract class default_edit<T, TKey> : default_edit<T>
		where T : class, IWithKey<T, TKey>, new()
	{
		[Inject]
		protected IDataContext DataContext { get; set; }

		protected virtual void SetDefaultValues(T obj) { }
		protected virtual void SetDefaultValuesBulkMode(IEnumerable<T> objs) { }

		protected override T GetNewEntity()
		{
			var obj = new T();
			SetDefaultValues(obj);
			if (!BulkMode) DataContext.InsertOnSubmit(obj);
			Tracker?.StartTracking(obj);
			return obj;
		}

		protected override T GetExistingEntity()
		{
			TKey id = Context.AllArgs.Parse<TKey>(Constants.Id);
			T obj = CommonLogic.GetFiltered<T, TKey>(DataContext, id);
			Tracker?.StartTracking(obj);
			return obj;
		}

        protected override void PreProcessFormData(ApiResponse response, ValidationMessageCollection val)
        {
            base.PreProcessFormData(response, val);

			if (EntityAudit != null && ViewData != null)
            {
                if (CreateObjectMode)
                    EntityAudit.AddChanges(ViewData, EntityAuditAction.Insert);
                else
				if (DeleteMode)
					EntityAudit.AddChanges(ViewData, EntityAuditAction.Delete);
				else
					EntityAudit.AddChanges(ViewData, EntityAuditAction.Update);
            }
        }

        protected override void Submit(ApiResponse response)
		{
			if (EntityAudit != null && ViewData != null && EntityAudit.PrimaryObject != null)
			{
				EntityAudit.PrimaryObject.PropertyChanges = Tracker?.GetChanges(ViewData);
				if (CreateObjectMode)
					EntityAudit.PrimaryObject.PropertyChanges.ForEach(pc => { pc.OldValue = null; });
			}

			if (CreateObjectMode && BulkMode)
			{
				var sel = GetArg(Constants.SelectedValues);
				var cnt = sel?.Split(',').Count() ?? 0;
				var objList = new List<T>();
				for (int i = 0; i < cnt; i++)
				{
					var obj = new T();
					DataContext.InsertOnSubmit(obj);
					objList.Add(obj);
				}
				SetDefaultValuesBulkMode(objList);

				var updFields = GetBulkEditProperties();
				foreach (var obj in objList)
				{
					foreach (var f in updFields)
					{
						var prop = typeof(T).GetProperty(f);
						prop.SetValue(obj, prop.GetValue(ViewData));
					}
				}
			}
			Action writeObjectChanges = () =>
			{
				Context.GetService<IDatabase>().Transaction = DataContext.Transaction;
				EntityAudit?.WriteObjectChange();
			};
			if (!DataContext.AfterSaveActions.Contains(writeObjectChanges))
				DataContext.AfterSaveActions.Add(writeObjectChanges);
			DataContext.SubmitChanges();
		}
	}

	public abstract class default_edit_rep<T, TKey, TRep> : default_edit<T>
		where T : class, IWithKey<T, TKey>, new()
		where TRep : IRepository<T>
	{
        [Inject] protected IDatabase Database { get; set; }

        TRep _repository = default;
		protected TRep Repository { 
			get 
			{ 
				if (_repository == null)
					_repository = GetRepository();

				return _repository;
			} 
		}

        protected virtual TRep GetRepository() => RepositoryExtensions.GetRepository<TRep, T>(Context.RequestServices, Database);

		protected virtual void SetDefaultValues(T obj) { }

		protected override T GetNewEntity()
		{
			var obj = new T();
			SetDefaultValues(obj);
			Tracker?.StartTracking(obj);
			return obj;
		}

		protected override T GetExistingEntity()
		{
			var id = Context.GetArg<TKey>(Constants.Id);
			var	obj = Repository.GetById(id);
			Tracker?.StartTracking(obj);
			return obj;
		}

		protected virtual void BeforeSaveEntity() { }
		protected virtual void AfterSaveEntity()
		{
			ApproveObjectChangeRequest();
		}

		protected override void PreProcessFormData(ApiResponse response, ValidationMessageCollection val)
		{
			base.PreProcessFormData(response, val);

			if (EntityAudit != null && ViewData != null)
			{
				if (CreateObjectMode)
					EntityAudit.AddChanges(ViewData, EntityAuditAction.Insert);
				else
				if (DeleteMode)
					EntityAudit.AddChanges(ViewData, EntityAuditAction.Delete);
				else
					EntityAudit.AddChanges(ViewData, EntityAuditAction.Update);
			}
		}

		protected override void Submit(ApiResponse response)
		{
			if (EntityAudit != null && ViewData != null)
			{
				EntityAudit.PrimaryObject.PropertyChanges = Tracker?.GetChanges(ViewData);
				if (CreateObjectMode)
					EntityAudit.PrimaryObject.PropertyChanges.ForEach(pc => { pc.OldValue = null; });
			}

			if (CreateObjectMode)
			{
				InTransaction(() => {
					Repository.Create(ViewData);
				});
			}
			else if (DeleteMode)
			{
				InTransaction(() => {
					Repository.Delete(new List<TKey>() { ViewData.ID });
				});
			}
			else if (BulkMode)
			{
				var updFields = GetBulkEditProperties();
				var ids = Context.AllArgs.ParseList<TKey>(Constants.SelectedValues);

				InTransaction(() => {
					Repository.Update(s => {
						foreach (var f in updFields)
							s.Set(f, typeof(T).GetProperty(f).GetValue(ViewData));
					}, ids);
				});
			}
			else
			{
				InTransaction(() => {
					Repository.Update(ViewData);
				});
			}
		}

		public void InTransaction(Action action)
		{
			using (var tran = Database.BeginTransaction())
			{
				BeforeSaveEntity();
				action();
				AfterSaveEntity();
				EntityAudit?.WriteObjectChange();
				tran.Commit();
			}
		}
	}

	public abstract class default_edit_rep<T, TKey> : default_edit_rep<T, TKey, IRepository<T>>
		where T : class, IWithKey<T, TKey>, new()
	{
	}

	public interface IObjectChangeRequestView : IViewElement
	{
		ObjectChangeRequestStatus Status { get; }

		void Validate(ObjectChangeRequestData data, ValidationMessageCollection m);
		void Save(ObjectChangeRequestData data, string comments);
		ObjectChangeRequestData<T> Load<T>(string ochid);

		void RenderHeader(LayoutWriter w);
		void RenderFooter(LayoutWriter w);
		void RenderFields(LayoutWriter w);
		void RenderDestFields(LayoutWriter w);
		bool CanReject();
		void Reject(List<FieldSnapshot> srcFields, List<FieldSnapshot> destFields);
		void Approve(object entity, List<FieldSnapshot> srcFields, List<FieldSnapshot> destFields);
		void CreateAndApprove(object entity, List<FieldSnapshot> srcFields, List<FieldSnapshot> destFields);
		string RequestCreatedMessage();
	}

	public class ObjectChangeRequestData<T>
	{
		public List<string> ChangedFields { get; set; }
		public T Object { get; set; }
	}

	public class ObjectChangeRequestData : ObjectChangeRequestData<object>
	{
	}
}