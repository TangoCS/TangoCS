﻿using System;
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

		public override ViewContainer GetContainer() => new EditEntityContainer();

		[Inject]
		protected IEntityAudit EntityAudit { get; set; }

		[Inject]
		protected IObjectTracker Tracker { get; set; }

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

				response.AddAdjacentWidget("form", "buttonsbar", AdjacentHTMLPosition.BeforeEnd, ButtonsBar);
				if (EnableToolbar)
					response.AddWidget("contenttoolbar", w => Toolbar(w));
			}
		}

		

		protected bool ProcessSubmit(ApiResponse response)
		{
			if (Context.FormData == null) return false;

			var m = new ValidationMessageCollection();
			bool prepareResponse()
			{
				RenderValidation(response, m);
				return false;
			}

			ValidateFormData(m);
			if (m.HasItems()) return prepareResponse();
			ProcessFormData(m);
			if (m.HasItems()) return prepareResponse();
			PostProcessFormData(response, m);
			if (m.HasItems()) return prepareResponse();

			return response.Success;
		}

		public virtual void OnSubmit(ApiResponse response)
		{
			if (!ProcessSubmit(response)) 
				return;

			Submit(response);
			AfterSubmit(response);
			response.AddWidget("validation", w => w.Write(""));
		}

		protected virtual void ValidateFormData(ValidationMessageCollection val) => groups.ForEach(g => g.ValidateFormData(val));
		protected virtual void ProcessFormData(ValidationMessageCollection val) => groups.ForEach(g => g.ProcessFormData(val));
		protected virtual void PostProcessFormData(ApiResponse response, ValidationMessageCollection val) { }

		protected abstract void Submit(ApiResponse response);
		protected virtual void AfterSubmit(ApiResponse response)
		{
			response.RedirectBack(Context, 1);
		}

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
			response.Success = false;
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

		protected virtual bool CreateObjectMode => !Context.AllArgs.ContainsKey(Constants.Id);
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

		protected IEnumerable<string> GetBulkEditProperties()
		{
			var updFields = new List<string>();
			groups.ForEach(g => {
				updFields.AddRange(g.Fields
					.Where(f => f is IEntityField<T> && f.IsVisible)
					.SelectMany(f => (f as IEntityField<T>).Properties));
			});
			return updFields;
		}

		protected virtual void WriteObjectChanges<TEntity, TKey>(TEntity obj) where TEntity : IWithKey<TEntity, TKey>
		{
			if (CreateObjectMode)
				EntityAudit?.WriteObjectChange<TEntity, TKey>(obj, EntityAuditAction.Insert, null);
			else
				EntityAudit?.WriteObjectChange<TEntity, TKey>(obj, EntityAuditAction.Update, Tracker.GetChanges(ViewData));
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

		protected override void Submit(ApiResponse response)
		{
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
				WriteObjectChanges<T, TKey>(ViewData);
			};
			if (!DataContext.AfterSaveActions.Contains(writeObjectChanges))
				DataContext.AfterSaveActions.Add(writeObjectChanges);
			DataContext.SubmitChanges();
		}
	}


	public abstract class default_edit_rep<T, TKey> : default_edit<T>
		where T : class, IWithKey<T, TKey>, new()
	{
		[Inject]
		protected IDatabase Database { get; set; }

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
			var obj = Database.Repository<T>().GetById(id);
			Tracker?.StartTracking(obj);
			return obj;
		}

		protected virtual void BeforeSaveEntity() { }
		protected virtual void AfterSaveEntity() { }

		protected override void Submit(ApiResponse response)
		{
			var rep = Database.Repository<T>();

			if (CreateObjectMode)
				InTransaction(() =>
				{
					rep.Create(ViewData);
					WriteObjectChanges<T, TKey>(ViewData);
				});
			else if (BulkMode)
			{
				var updFields = GetBulkEditProperties();
				var ids = Context.AllArgs.ParseList<TKey>(Constants.SelectedValues);

				InTransaction(() => {
					rep.Update(s => {
						foreach (var f in updFields)
							s.Set(f, typeof(T).GetProperty(f).GetValue(ViewData));
					}, ids);
				});
			}
			else
			{
				InTransaction(() =>
				{
					rep.Update(ViewData);
					WriteObjectChanges<T, TKey>(ViewData);
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
				tran.Commit();
			}
		}
	}
}