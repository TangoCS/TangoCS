using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Tango.Data;

namespace Tango.UI.Controls
{
	public interface ISelectObjectField<TRef> : IViewElement
	{
		IQueryable<TRef> AllObjects { get; set; }
		Func<string, Expression<Func<TRef, bool>>> SearchExpression { get; }
		Func<string, int, int, IQueryable<TRef>> SearchQuery { get; }
		Func<string, IQueryable<TRef>> SearchCountQuery { get; }

		Func<TRef, string> DataTextField { get; }
		Func<TRef, string> DataValueField { get;  }
		Func<TRef, string> SelectedObjectTextField { get; }
		string FilterValue { get; }
		bool Disabled { get; }
		Action<LayoutWriter> TextWhenDisabled { get; }
		Action<LayoutWriter> TextWhenNothingSelected { get; }

		Action<LayoutWriter> FieldExtensions { get; }

		bool PostOnClearEvent { get; }
		bool PostOnChangeEvent { get; }

		ISelectObjectFieldDataProvider<TRef> DataProvider { get; }

		void OnClear(ApiResponse response);
	}

	public interface ISelectSingleObjectField<TRef, TRefKey, TValue> : ISelectObjectField<TRef>
	{
		void OnChange(ApiResponse response, TValue selectedValue);
		TRef GetObjectByID(TRefKey id);
	}

	public abstract class SelectObjectField<TRef, TRefKey, TValue> : ViewComponent, ISelectObjectField<TRef>
		where TRef : class, IWithKey<TRefKey>
	{
		public IQueryable<TRef> AllObjects { get; set; }

		public Func<string, Expression<Func<TRef, bool>>> SearchExpression { get; set; } = s => o => (o as IWithTitle).Title.Contains(s);
		public Func<string, int, int, IQueryable<TRef>> SearchQuery { get; set; }
		public Func<string, IQueryable<TRef>> SearchCountQuery { get; set; }

		public Func<TRef, string> DataTextField { get; set; } = o => o != null && o is IWithTitle ? (o as IWithTitle).Title : "";
		public Func<TRef, string> DataValueField { get; set; } = o => o.ID.ToString();
		public Func<TRef, string> SelectedObjectTextField { get; set; } = o => o != null && o is IWithTitle ? (o as IWithTitle).Title : "";
		public Expression<Func<TRef, TRefKey>> ByParentSelector { get; set; }
		public TRefKey RootObjectID { get; set; }
		public bool IsHierarchic => ByParentSelector != null;

		public string FilterValue { get; set; }
		public bool Disabled { get; set; }
		public Action<LayoutWriter> TextWhenDisabled { get; set; }
		public Action<LayoutWriter> TextWhenNothingSelected { get; set; }

		public Action<LayoutWriter> FieldExtensions { get; set; }

		//public Func<T, string> IDField { get; set; }

		public event Action<ApiResponse, TValue> Change;
		public void OnChange(ApiResponse response, TValue selectedValue) => Change?.Invoke(response, selectedValue);

		public event Action<ApiResponse> Clear;
		public void OnClear(ApiResponse response) => Clear?.Invoke(response);

		public bool PostOnClearEvent => TextWhenNothingSelected != null || Clear != null;
		public bool PostOnChangeEvent => Change != null;

		public ISelectObjectFieldDataProvider<TRef> DataProvider { get; set; }

		public SelectObjectField()
		{
			DataProvider = new ORMSelectObjectFieldDataProvider<TRef>(this);
		}
	}

	public static class SelectObjectDialogHelpers
	{
		public static IQueryable<TRef> DataQuery<TRef>(this ISelectObjectField<TRef> field, Paging paging, string filterValue)
		{
			if (!filterValue.IsEmpty())
			{
				if (field.SearchQuery != null)
				{
					return field.SearchQuery(filterValue, paging.PageIndex, paging.PageSize);
				}
				else
				{
					return paging.Apply(field.AllObjects.Where(field.SearchExpression(filterValue)));
				}
			}
			else
			{
				return paging.Apply(field.AllObjects);
			}
		}

		public static SelectListItem GetListItem<TRef>(this ISelectObjectField<TRef> field,	TRef obj, string filterValue, bool highlightSearchResults)
		{
			string text = field.DataTextField(obj);
			string value = field.DataValueField(obj);

			if (highlightSearchResults && !filterValue.IsEmpty())
			{
				string fv = filterValue.Replace("<", "").
					Replace(">", "").Replace("(", "").Replace(")", "").Replace("[", "").Replace("]", "").
					Replace("{", "").Replace("}", "").Replace("?", "").Replace("*", "");
				text = Regex.Replace(text, "(?i)(?<1>" + fv + ")",
					"<span style='color:Red; font-weight:bold'>$1</span>");
			}
			return new SelectListItem(text, value);
		}

		public static IQueryable<TRef> ItemsCountQuery<TRef>(this ISelectObjectField<TRef> field, string filterValue)
		{
			return field.SearchCountQuery != null ? field.SearchCountQuery(filterValue) : (
				filterValue.IsEmpty() ?
				field.AllObjects :
				field.AllObjects.Where(field.SearchExpression(filterValue))
			);
		}
	}

	public interface ISelectObjectFieldDataProvider<TRef>
	{
		int GetCount(string filterValue);
		IEnumerable<TRef> GetData(Paging paging, string filterValue);
		IEnumerable<TRef> GetAllData();
		TRef GetObjectByID<T>(T id, Expression<Func<TRef, bool>> predicate);
		IEnumerable<TRef> GetObjectsByID<T>(IEnumerable<T> id, Expression<Func<TRef, bool>> predicate);
	}

	public class ORMSelectObjectFieldDataProvider<TRef> : ISelectObjectFieldDataProvider<TRef>
	{
		ISelectObjectField<TRef> field;

		public ORMSelectObjectFieldDataProvider(ISelectObjectField<TRef> field)
		{
			this.field = field;
		}

		public IEnumerable<TRef> GetAllData()
		{
			return field.AllObjects;
		}

		public int GetCount(string filterValue)
		{
			return field.ItemsCountQuery(filterValue).Count();
		}

		public IEnumerable<TRef> GetData(Paging paging, string filterValue)
		{
			return field.DataQuery(paging, filterValue);
		}

		public TRef GetObjectByID<T>(T id, Expression<Func<TRef, bool>> predicate)
		{
			return field.AllObjects.Where(predicate).FirstOrDefault();
		}

		public IEnumerable<TRef> GetObjectsByID<T>(IEnumerable<T> id, Expression<Func<TRef, bool>> predicate)
		{
			return field.AllObjects.Where(predicate).ToList();
		}
	}

	public class DapperSelectObjectFieldDataProvider<TRef> : ISelectObjectFieldDataProvider<TRef>
	{
		ISelectObjectField<TRef> field;
		IDatabase database;

		public string AllObjectsSql { get; set; }
		public object AllObjectsSqlParms { get; set; }
		public Func<IQueryable<TRef>, IQueryable<TRef>> PostProcessing { get; set; }

		public DapperSelectObjectFieldDataProvider(ISelectObjectField<TRef> field, IDatabase database)
		{
			this.field = field;
			this.database = database;
			if (field.AllObjects == null) field.AllObjects = Enumerable.Empty<TRef>().AsQueryable();
		}

		public IEnumerable<TRef> GetAllData()
		{
			var rep = database.Repository<TRef>().WithAllObjectsQuery(AllObjectsSql, AllObjectsSqlParms);
			var q = field.AllObjects;
			if (PostProcessing != null) q = PostProcessing(q);
			return rep.List(q.Expression);
		}

		public int GetCount(string filterValue)
		{
			var rep = database.Repository<TRef>().WithAllObjectsQuery(AllObjectsSql, AllObjectsSqlParms);
			var q = field.ItemsCountQuery(filterValue);
			return rep.Count(q.Expression);
		}

		public IEnumerable<TRef> GetData(Paging paging, string filterValue)
		{
			var rep = database.Repository<TRef>().WithAllObjectsQuery(AllObjectsSql, AllObjectsSqlParms);
			var q = field.DataQuery(paging, filterValue);
			if (PostProcessing != null) q = PostProcessing(q);
			return rep.List(q.Expression);
		}

		public TRef GetObjectByID<T>(T id, Expression<Func<TRef, bool>> predicate)
		{
			var rep = database.Repository<TRef>().WithAllObjectsQuery(AllObjectsSql, AllObjectsSqlParms);
			var q = field.AllObjects.Where(predicate);
			return rep.List(q.Expression).FirstOrDefault();
		}

		public IEnumerable<TRef> GetObjectsByID<T>(IEnumerable<T> id, Expression<Func<TRef, bool>> predicate)
		{
			var rep = database.Repository<TRef>().WithAllObjectsQuery(AllObjectsSql, AllObjectsSqlParms);
			var q = field.AllObjects.Where(predicate);
			return rep.List(q.Expression);
		}
	}

	public class SelectSingleObjectField<TRef, TRefKey> : SelectObjectField<TRef, TRefKey, TRef>, ISelectSingleObjectField<TRef, TRefKey, TRef>
		where TRef : class, IWithKey<TRef, TRefKey>, new()
	{
		public Func<TRefKey, Expression<Func<TRef, bool>>> FilterSelected { get; set; }
		public SelectSingleObjectDropDown<TRef, TRefKey> Strategy { get; set; }

		public override void OnInit()
		{
			Strategy = CreateControl<SelectSingleObjectDropDown<TRef, TRefKey>>("str", c => c.Field = this);
		}

		public TRef GetObjectByID(TRefKey id)
		{
			var keySelector = FilterSelected ?? new TRef().KeySelector;
			return DataProvider.GetObjectByID(id, keySelector(id));
		}
	}

	public class SelectMultipleObjectsField<TRef, TRefKey> : SelectObjectField<TRef, TRefKey, IEnumerable<TRef>>
		where TRef : class, IWithTitle, IWithKey<TRefKey>
	{
		public bool AllowSelectAll { get; set; }

		public Func<IEnumerable<TRefKey>, Expression<Func<TRef, bool>>> FilterSelected { get; set; }

		public SelectMultipleObjectsDialog<TRef, TRefKey> Strategy { get; set; }
		public override void OnInit()
		{
			Strategy = CreateControl<SelectMultipleObjectsDialog<TRef, TRefKey>>("str", c => c.Field = this);
		}

		public IEnumerable<string> GetSelectedObjectsTitle()
		{
			List<TRefKey> selectedIds = Context.FormData.ParseList<TRefKey>(ClientID);
			return selectedIds == null || selectedIds.Count == 0 ? null : GetObjectsByIDs(selectedIds).Select(SelectedObjectTextField);
		}

		public IEnumerable<TRef> GetObjectsByIDs(IEnumerable<TRefKey> ids)
		{
			return ids == null || ids.Count() == 0 ? null : DataProvider.GetObjectsByID(ids, FilterSelected(ids));
		}
	}

	public static class LayoutWriterSelectDialogExtensions
	{
		public static void FormFieldSelectDialog<TSelected, TSelectedKey>(this LayoutWriter w, string caption, TSelected obj, SelectSingleObjectField<TSelected, TSelectedKey> dialog, bool isRequired = false, string description = null)
			where TSelected : class, IWithKey<TSelected, TSelectedKey>, new()
		{
			w.FormField(dialog.ID, caption, () => dialog.Strategy.Render(w, obj), isRequired, description);
		}

		public static void FormFieldSelectDialog<TSelected, TSelectedKey>(this LayoutWriter w, string caption, IEnumerable<TSelected> obj, SelectMultipleObjectsField<TSelected, TSelectedKey> dialog, bool isRequired = false, string description = null)
			where TSelected : class, IWithKey<TSelected, TSelectedKey>, IWithTitle, new()
		{
			w.FormField(dialog.ID, caption, () => dialog.Strategy.Render(w, obj), isRequired, description);
		}

		public static void SelectSingleObject<TEntity, TValue, TRefClass, TRefKey>(
			this LayoutWriter w,
			EntityField<TEntity, TRefClass, TValue> field,
			SelectSingleObjectField<TRefClass, TRefKey> dialog,
			Grid grid = Grid.OneWhole)
			where TEntity : class
			where TRefClass : class, IWithTitle, IWithKey<TRefClass, TRefKey>, new()
		{
			var id = dialog.Context.GetArg<TRefKey>(dialog.ID);
			var v = dialog.Context.RequestMethod == "POST" ? dialog.GetObjectByID(id) : field.Value;
			dialog.Disabled = field.Disabled;
			w.FormField(field, grid, () => dialog.Strategy.Render(w, v));
		}

		public static void SelectSingleObject<TEntity, TRefClass, TRefKey>(
			this LayoutWriter w,
			EntityField<TEntity, TRefKey> field,
			SelectSingleObjectField<TRefClass, TRefKey> dialog,
			Grid grid = Grid.OneWhole)
			where TEntity : class
			where TRefClass : class, IWithTitle, IWithKey<TRefClass, TRefKey>, new()
		{
			var id = dialog.Context.RequestMethod == "POST" ?
				dialog.Context.GetArg<TRefKey>(dialog.ID) :
				field.Value;
			var v =  dialog.GetObjectByID(id);
			dialog.Disabled = field.Disabled;
			w.FormField(field, grid, () => dialog.Strategy.Render(w, v));
		}

		public static void SelectMultipleObjects<TEntity, TRefClass, TRefKey>(
			this LayoutWriter w,
			EntityReferenceManyField<TEntity, TRefClass, TRefKey> field,
			SelectMultipleObjectsField<TRefClass, TRefKey> dialog,
			Grid grid = Grid.OneWhole)
			where TEntity : class
			where TRefClass : class, IWithTitle, IWithKey<TRefKey>
		{
			var ids = dialog.Context.GetListArg<TRefKey>(dialog.ID);
			var v = dialog.Context.RequestMethod == "POST" ? dialog.GetObjectsByIDs(ids) : field.Value;
			dialog.Disabled = field.Disabled;
			w.FormField(field, grid, () => dialog.Strategy.Render(w, v));
		}

		public static void SelectMultipleObjects<TEntity, TRefClass, TRefKey>(
			this LayoutWriter w,
			EntityReferenceManyField<TEntity, TRefKey> field,
			SelectMultipleObjectsField<TRefClass, TRefKey> dialog,
			Grid grid = Grid.OneWhole)
			where TEntity : class
			where TRefClass : class, IWithTitle, IWithKey<TRefKey>
		{
			var ids = dialog.Context.RequestMethod == "POST" ?
				dialog.Context.GetListArg<TRefKey>(dialog.ID) :
				field.Value;
			var v = dialog.GetObjectsByIDs(ids);
			dialog.Disabled = field.Disabled;
			w.FormField(field, grid, () => dialog.Strategy.Render(w, v));
		}

		public static void SelectMultipleObjects<TEntity, TRefClass, TRefKey>(
			this LayoutWriter w,
			EntityField<TEntity, List<TRefKey>> field,
			SelectMultipleObjectsField<TRefClass, TRefKey> dialog,
			Grid grid = Grid.OneWhole)
			where TEntity : class
			where TRefClass : class, IWithTitle, IWithKey<TRefKey>
		{
			var ids = dialog.Context.RequestMethod == "POST" ?
				dialog.Context.GetListArg<TRefKey>(dialog.ID) :
				field.Value;
			var v = dialog.GetObjectsByIDs(ids);
			dialog.Disabled = field.Disabled;
			w.FormField(field, grid, () => dialog.Strategy.Render(w, v));
		}
	}
}
