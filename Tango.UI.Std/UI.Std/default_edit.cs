using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using Tango.Data;
using Tango.Html;
using Tango.Identity.Std;
using Tango.Logic;
using Tango.Meta;
using Tango.UI.Controls;
using Tango.UI.Std.EntityAudit;

namespace Tango.UI.Std
{
	public abstract class default_edit : ViewPagePart
	{
		protected abstract string Title { get; }
		protected abstract void Form(LayoutWriter w);
		protected virtual ContainerWidth FormWidth => ContainerWidth.WidthStd;
		protected virtual ContainerHeight FormHeight => ContainerHeight.HeightStd;
		protected virtual bool FormGridMode => false;
		protected virtual bool ShowResultBlock => false;

		public override ViewContainer GetContainer() => new EditEntityContainer {
			IsNested = ParentElement != null,
			GridMode = FormGridMode,
			Width = FormWidth,
			Height = FormHeight,
			ShowResultBlock = ShowResultBlock
		};

		[Inject]
		protected IEntityAudit EntityAudit { get; set; }

		[Inject]
		protected IObjectTracker Tracker { get; set; }

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

		public override void AfterInit()
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
			FieldsPreInit();
		}

		protected virtual void FieldsPreInit()
		{
		}

		protected virtual bool ObjectNotExists => false;

		public override void OnLoad(ApiResponse response)
		{
			response.AddWidget("contenttitle", Title);
			response.AddWidget("#title", Title);

            if (ObjectNotExists)
			{
				response.AddWidget("form", w => {
					w.Div(Resources.Get("Common.ObjectNotExists"));
				});
			}
			else
			{
				response.AddWidget("form", w => {
					Form(w);
					w.FormValidationBlock();
				});

				if (EnableButtonsBar)
					response.AddAdjacentWidget("form", "buttonsbar", AdjacentHTMLPosition.BeforeEnd, ButtonsBar);
				if (EnableToolbar)
					response.AddWidget("contenttoolbar", w => Toolbar(w));
			}
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
				response.AddWidget("validation", w => w.Write(""));

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
			if (!ProcessSubmit(response)) 
				return;

			Submit(response);
			AfterSubmit(response);
		}

		protected virtual void ValidateFormData(ValidationMessageCollection val) => groups.ForEach(g => g.ValidateFormData(val));
        protected virtual void PreProcessFormData(ApiResponse response, ValidationMessageCollection val) { }
        protected virtual void ProcessFormData(ValidationMessageCollection val) => groups.ForEach(g => g.ProcessFormData(val));
		protected virtual void PostProcessFormData(ApiResponse response, ValidationMessageCollection val) { }

		protected abstract void Submit(ApiResponse response);
		protected virtual void AfterSubmit(ApiResponse response)
		{
			response.RedirectBack(Context, 1);
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
			w.ButtonsBar_edit();
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
		protected IIdentityManager IdentityManager { get; set; }

		T _viewData = null;
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
			return CreateObjectMode || BulkMode ? GetNewEntity() : GetExistingEntity();
		}

		public override void AfterInit()
		{
			base.AfterInit();
			groups.ForEach(g => {
				g.Args.Add("IsNewObject", CreateObjectMode);
				g.Args.Add("IsBulkMode", BulkMode);
				g.SetViewData(ViewData);
			});
		}

		protected override bool ObjectNotExists => ViewData == null;

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

		protected virtual bool CreateObjectMode => !BulkMode && !Context.AllArgs.ContainsKey(Constants.Id);
		protected virtual bool BulkMode => Context.AllArgs.ContainsKey(Constants.SelectedValues);
		protected override string Title => CreateObjectMode ? CreateNewFormTitle : EditFormTitle;

		protected virtual string BulkModeFormTitle => Resources.Get("Common.BulkModeTitle");
		protected virtual string CreateNewFormTitle => Resources.Get(ViewData.GetType().FullName);
		protected virtual string EditFormTitle => ViewData is IWithTitle ? (ViewData as IWithTitle).Title : "";

		protected abstract T GetNewEntity();
		protected abstract T GetExistingEntity();

		protected override void ProcessFormData(ValidationMessageCollection val)
		{
			base.ProcessFormData(val);

			if (ViewData is IWithTimeStamp withTimeStamp)
			{
				withTimeStamp.LastModifiedDate = DateTime.Now;
			}

			if (ViewData is IWithUserTimeStamp withUserTimeStamp)
			{
				withUserTimeStamp.LastModifiedUserID = IdentityManager.CurrentUser.Id;
			}
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
				}
			}
			base.AfterSubmit(response);
		}

		protected void Set<TValue>(MetaAttribute<T, TValue> attr, TValue defaultValue = default(TValue))
		{
			attr.SetValue(ViewData, FormData.Parse(attr.Name, defaultValue));
		}
		protected void Set<TValue, TRefKey>(MetaReference<T, TValue, TRefKey> attr, TRefKey defaultValue = default(TRefKey))
		{
			attr.SetValueID(ViewData, FormData.Parse(attr.Name, defaultValue));
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
                    EntityAudit.AddChanges(ViewData, EntityAuditAction.Update);
            }
        }

        protected override void Submit(ApiResponse response)
		{
            if (EntityAudit != null && ViewData != null)
            {
                if (!CreateObjectMode)
                {
                    if (EntityAudit != null && EntityAudit.PrimaryObject != null)
						EntityAudit.PrimaryObject.PropertyChanges = Tracker?.GetChanges(ViewData);
                }
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
		TRep _repository = default;
		protected TRep Repository { 
			get 
			{ 
				if (_repository == null)
					_repository = RepositoryExtensions.GetRepository<TRep, T>(Context.RequestServices, Database);

				return _repository;
			} 
		}
		
		[Inject] protected IDatabase Database { get; set; }

		protected virtual void SetDefaultValues(T obj) { }

		protected override T GetNewEntity()
		{
			var obj = new T();
			SetDefaultValues(obj);
			return obj;
		}

		protected override T GetExistingEntity()
		{
			var id = Context.GetArg<TKey>(Constants.Id);
			var obj = Repository.GetById(id);
			Tracker?.StartTracking(obj);
			return obj;
		}

		protected virtual void BeforeSaveEntity() { }
		protected virtual void AfterSaveEntity()
		{
			
		}

		protected override void PreProcessFormData(ApiResponse response, ValidationMessageCollection val)
        {
            base.PreProcessFormData(response, val);

            if (EntityAudit != null && ViewData != null)
            {
                if (CreateObjectMode)
                    EntityAudit.AddChanges(ViewData, EntityAuditAction.Insert);
                else
                    EntityAudit.AddChanges(ViewData, EntityAuditAction.Update);
            }
        }

        protected override void Submit(ApiResponse response)
		{
            if (EntityAudit != null && ViewData != null)
            {
                if (!CreateObjectMode)
                {
                    if (EntityAudit != null)
						EntityAudit.PrimaryObject.PropertyChanges = Tracker?.GetChanges(ViewData);
                }
            }

            //var rep = Database.Repository<T>();

			if (CreateObjectMode)
				InTransaction(() =>
				{
					Repository.Create(ViewData);
				});
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
				InTransaction(() =>
				{
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
}