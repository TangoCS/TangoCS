using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Tango.Data;
using Tango.Html;
using Tango.Localization;
using Tango.UI.Std;

namespace Tango.UI.Controls
{
	public interface ISelectObjectField<TRef> : IViewElement
	{
		Func<string> Title { get; } 

		IQueryable<TRef> AllObjects { get; set; }
		Func<string, Expression<Func<TRef, bool>>> SearchExpression { get; }
		Func<string, int, int, IQueryable<TRef>> SearchQuery { get; }
		Func<string, IQueryable<TRef>> SearchCountQuery { get; }

		Func<TRef, string> DataTextField { get; }
		Func<TRef, string> DataValueField { get;  }
		Action<LayoutWriter, TRef> DataRow { get; }
		Func<TRef, string> SelectedObjectTextField { get; }

		string FilterValue { get; }
		bool HighlightSearchResults { get; }
		string FilterFieldName { get; }

		bool Disabled { get; }
		bool ReadOnly { get; }
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

	public abstract class AbstractSelectObjectField<TRef, TRefKey, TValue> : ViewComponent, ISelectObjectField<TRef>
		where TRef : class, IWithKey<TRefKey>
	{
		public Func<string> Title { get; set; }

		public IQueryable<TRef> AllObjects { get; set; }

		public Func<string, Expression<Func<TRef, bool>>> SearchExpression { get; set; } = s => o => (o as IWithTitle).Title.Contains(s);
		public Func<string, int, int, IQueryable<TRef>> SearchQuery { get; set; }
		public Func<string, IQueryable<TRef>> SearchCountQuery { get; set; }

		public Func<TRef, string> DataTextField { get; set; } = o => o != null && o is IWithTitle ? (o as IWithTitle).Title : "";
		public Func<TRef, string> DataValueField { get; set; } = o => o.ID.ToString();
		public Action<LayoutWriter, TRef> DataRow { get; set; }
		public Func<TRef, string> SelectedObjectTextField { get; set; } = o => o != null && o is IWithTitle ? (o as IWithTitle).Title : "";
		public Expression<Func<TRef, TRefKey>> ByParentSelector { get; set; }
		public TRefKey RootObjectID { get; set; }
		public bool IsHierarchic => ByParentSelector != null;

		string _filterValue = null;
		public string FilterValue
		{
			get
			{
				if (_filterValue == null) _filterValue = Context.GetArg(FilterFieldName);
				return _filterValue;
			}
		}
		public bool HighlightSearchResults { get; set; } = false;
		public string FilterFieldName { get; set; } = "filter";

		public bool Disabled { get; set; }
		public bool ReadOnly { get; set; }
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

		public AbstractSelectObjectField()
		{
			AllObjects = Enumerable.Empty<TRef>().AsQueryable();
			DataProvider = new ORMSelectObjectFieldDataProvider<TRef>(this);
			Title = () => Resources.Get(typeof(TRef).GetResourceType().FullName + "-pl");
			DataRow = (w, o) => w.Write(GetDataText(o));
		}

		public string GetDataText(TRef o)
		{
			var text = DataTextField(o);

			if (HighlightSearchResults && !FilterValue.IsEmpty())
			{
				string fv = FilterValue.Replace("<", "").
					Replace(">", "").Replace("(", "").Replace(")", "").Replace("[", "").Replace("]", "").
					Replace("{", "").Replace("}", "").Replace("?", "").Replace("*", "");
				text = Regex.Replace(text, "(?i)(?<1>" + fv + ")",
					"<span style='color:Red; font-weight:bold'>$1</span>");
			}

			return text;
		}

		public bool DoCallbackToCurrent { get; set; } = false;
	}

	public abstract class AbstractSelectMultipleObjectsField<TRef, TRefKey> : AbstractSelectObjectField<TRef, TRefKey, IEnumerable<TRef>>
		where TRef : class, IWithKey<TRefKey>
	{
		public bool AllowSelectAll { get; set; }

		public Func<IEnumerable<TRefKey>, Expression<Func<TRef, bool>>> FilterSelected { get; set; }

		public IEnumerable<string> GetSelectedObjectsTitle()
		{
			List<TRefKey> selectedIds = Context.FormData.ParseList<TRefKey>(ClientID);
			return selectedIds == null || selectedIds.Count == 0 ? null : GetObjectsByIDs(selectedIds).Select(SelectedObjectTextField);
		}

		public IEnumerable<TRef> GetObjectsByIDs(IEnumerable<TRefKey> ids)
		{
			if (ids == null || ids.Count() == 0)
				return null;

			return DataProvider.MaterializeList(AllObjects.Where(FilterSelected(ids)));
		}
	}

	public abstract class AbstractSelectMultipleObjectsField<TRef, TRefKey, TStrategy> : AbstractSelectMultipleObjectsField<TRef, TRefKey>
		where TRef : class, IWithTitle, IWithKey<TRefKey>
		where TStrategy : SelectObjectDialog<TRef, TRefKey, IEnumerable<TRef>, AbstractSelectMultipleObjectsField<TRef, TRefKey>>, new()
	{
		public TStrategy Strategy { get; set; }
		public override void OnInit()
		{
			Strategy = CreateControl<TStrategy>("str", c => c.Field = this);
		}
	}

	public static class SelectObjectDialogHelpers
	{
		public static IQueryable<TRef> DataQuery<TRef>(this ISelectObjectField<TRef> field, Paging paging)
		{
			if (!field.FilterValue.IsEmpty())
			{
				if (field.SearchQuery != null)
				{
					return field.SearchQuery(field.FilterValue, paging.PageIndex, paging.PageSize);
				}
				else
				{
					return paging.Apply(field.AllObjects.Where(field.SearchExpression(field.FilterValue)));
				}
			}
			else
			{
				return paging.Apply(field.AllObjects);
			}
		}

		//public static SelectListItem GetListItem<TRef>(this ISelectObjectField<TRef> field,	TRef obj, string filterValue, bool highlightSearchResults)
		//{
		//	string text = field.DataTextField(obj);
		//	string value = field.DataValueField(obj);

		//	if (highlightSearchResults && !filterValue.IsEmpty())
		//	{
		//		string fv = filterValue.Replace("<", "").
		//			Replace(">", "").Replace("(", "").Replace(")", "").Replace("[", "").Replace("]", "").
		//			Replace("{", "").Replace("}", "").Replace("?", "").Replace("*", "");
		//		text = Regex.Replace(text, "(?i)(?<1>" + fv + ")",
		//			"<span style='color:Red; font-weight:bold'>$1</span>");
		//	}
		//	return new SelectListItem(text, value);
		//}

		public static IQueryable<TRef> ItemsCountQuery<TRef>(this ISelectObjectField<TRef> field)
		{
			return field.SearchCountQuery != null ? field.SearchCountQuery(field.FilterValue) : (
				field.FilterValue.IsEmpty() ?
				field.AllObjects :
				field.AllObjects.Where(field.SearchExpression(field.FilterValue))
			);
		}
	}



	public class SelectSingleObjectField<TRef, TRefKey> : AbstractSelectObjectField<TRef, TRefKey, TRef>, ISelectSingleObjectField<TRef, TRefKey, TRef>
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
			return DataProvider.MaterializeList(AllObjects.Where(keySelector(id))).FirstOrDefault();
		}
	}



	public class SelectMultipleObjectsField<TRef, TRefKey> : 
		AbstractSelectMultipleObjectsField<TRef, TRefKey, SelectMultipleObjectsDialog<TRef, TRefKey>>
		where TRef : class, IWithTitle, IWithKey<TRefKey>
	{
		
	}

	public class SelectMultipleObjectsTreeField<TRef, TRefKey, TControl> :
		AbstractSelectMultipleObjectsField<TRef, TRefKey, SelectMultipleObjectsTreeDialog<TRef, TRefKey, TControl>>
		where TRef : class, IWithTitle, IWithKey<TRefKey>, ILazyListTree
		where TControl : default_tree_rep<TRef>, new()
	{

	}


	public static class LayoutWriterSelectDialogExtensions
	{
		public static void FormFieldSelectDialog<TSelected, TSelectedKey>(this LayoutWriter w, string caption, TSelected obj, SelectSingleObjectField<TSelected, TSelectedKey> dialog, GridPosition grid = null, bool isRequired = false, string description = null, string hint = null)
			where TSelected : class, IWithKey<TSelected, TSelectedKey>, new()
		{
			w.FormField(dialog.ID, caption, () => dialog.Strategy.Render(w, obj), grid, isRequired, description, hint: hint);
		}

		public static void FormFieldSelectDialog<TSelected, TSelectedKey>(this LayoutWriter w, string caption, IEnumerable<TSelected> obj, SelectMultipleObjectsField<TSelected, TSelectedKey> dialog, GridPosition grid = null, bool isRequired = false, string description = null, string hint = null)
			where TSelected : class, IWithKey<TSelected, TSelectedKey>, IWithTitle, new()
		{
			w.FormField(dialog.ID, caption, () => dialog.Strategy.Render(w, obj), grid, isRequired, description, hint: hint);
		}

		public static void SelectSingleObject<TEntity, TValue, TRefClass, TRefKey>(
			this LayoutWriter w,
			EntityField<TEntity, TRefClass, TValue> field,
			SelectSingleObjectField<TRefClass, TRefKey> dialog,
			GridPosition grid = null, bool isViewCaption = true)
			where TEntity : class
			where TRefClass : class, IWithTitle, IWithKey<TRefClass, TRefKey>, new()
		{
			var id = dialog.Context.GetArg<TRefKey>(dialog.ID);
			var v = field.ValueSource == ValueSource.Model ? field.Value : dialog.GetObjectByID(id);		
			
			dialog.Disabled = field.Disabled;
			dialog.ReadOnly = field.ReadOnly;
			w.FormField(field, () => dialog.Strategy.Render(w, v), grid, isViewCaption);
		}

		public static void SelectSingleObject<TEntity, TRefClass, TRefKey>(
			this LayoutWriter w,
			EntityField<TEntity, TRefKey> field,
			SelectSingleObjectField<TRefClass, TRefKey> dialog,
			GridPosition grid = null)
			where TEntity : class
			where TRefClass : class, IWithTitle, IWithKey<TRefClass, TRefKey>, new()
		{
			var id = dialog.Context.RequestMethod == "POST" ?
				dialog.Context.GetArg<TRefKey>(dialog.ID) :
				field.Value;
			var v =  dialog.GetObjectByID(id);
			dialog.Disabled = field.Disabled;
			dialog.ReadOnly = field.ReadOnly;
			w.FormField(field, () => dialog.Strategy.Render(w, v), grid);
		}

		public static void SelectMultipleObjects<TEntity, TRefClass, TRefKey>(
			this LayoutWriter w,
			EntityReferenceManyField<TEntity, TRefClass, TRefKey> field,
			SelectMultipleObjectsField<TRefClass, TRefKey> dialog,
			GridPosition grid = null)
			where TEntity : class
			where TRefClass : class, IWithTitle, IWithKey<TRefKey>
		{
			var ids = dialog.Context.GetListArg<TRefKey>(dialog.ID);
			var v = dialog.Context.RequestMethod == "POST" ? dialog.GetObjectsByIDs(ids) : field.Value;
			dialog.Disabled = field.Disabled;
			dialog.ReadOnly = field.ReadOnly;
			w.FormField(field, () => dialog.Strategy.Render(w, v), grid);
		}

		public static void SelectMultipleObjects<TEntity, TRefClass, TRefKey>(
			this LayoutWriter w,
			EntityReferenceManyField<TEntity, TRefKey> field,
			SelectMultipleObjectsField<TRefClass, TRefKey> dialog,
			GridPosition grid = null)
			where TEntity : class
			where TRefClass : class, IWithTitle, IWithKey<TRefKey>
		{
			var ids = dialog.Context.RequestMethod == "POST" ?
				dialog.Context.GetListArg<TRefKey>(dialog.ID) :
				field.Value;
			var v = dialog.GetObjectsByIDs(ids);
			dialog.Disabled = field.Disabled;
			dialog.ReadOnly = field.ReadOnly;
			w.FormField(field, () => dialog.Strategy.Render(w, v), grid);
		}

		public static void SelectMultipleObjects<TEntity, TRefClass, TRefKey>(
			this LayoutWriter w,
			EntityField<TEntity, List<TRefKey>> field,
			SelectMultipleObjectsField<TRefClass, TRefKey> dialog,
			GridPosition grid = null)
			where TEntity : class
			where TRefClass : class, IWithTitle, IWithKey<TRefKey>
		{
			var ids = dialog.Context.RequestMethod == "POST" ?
				dialog.Context.GetListArg<TRefKey>(dialog.ID) :
				field.Value;
			var v = dialog.GetObjectsByIDs(ids);
			dialog.Disabled = field.Disabled;
			dialog.ReadOnly = field.ReadOnly;
			w.FormField(field, () => dialog.Strategy.Render(w, v), grid);
		}
	}
}
