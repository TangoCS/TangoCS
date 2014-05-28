using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Linq.Expressions;
using System.Data.Linq;
using Nephrite.Web.SettingsManager;
using Nephrite.Web.TextResources;

namespace Nephrite.Web.Controls
{
	public partial class MultiObject : System.Web.UI.UserControl
	{
		public bool ReadOnly { get; set; }
		public MultiObject()
		{
			DataTextField = "Title";
			DataValueField = "ObjectID";
			SelectLinkTitle = TextResource.Get("Common.Controls.MultiObject.SelectObjects", "Выбрать объекты");
			PageSize = 20;
			RemoveUnselected = true;
			Title = TextResource.Get("Common.Controls.MultiObject.ChooseObjects", "Выберите объекты");
		}

		protected string RenderRun()
		{
			return select.RenderRun();
		}
		protected override void CreateChildControls()
		{
			base.CreateChildControls();

		}
		public bool AllowSelectAll { get; set; }
		
		protected void Page_Load(object sender, EventArgs e)
		{
			//select.OnSelect = "OnSelect_" + ClientID;
			select.Selected += new EventHandler<SelectObjectHierarchicEventArgs>(select_Selected);
			select.AllObjects = AllObjects;
			select.Type = Type;
			if (SearchExpression != null)
				select.SearchExpression = SearchExpression;
			select.TargetModalDialog = TargetModalDialog;
			select.Title = Title;
			select.PageSize = PageSize;
			select.DataTextField = DataTextField;
			select.DataValueField = DataValueField;
			select.ParentField = ParentField;
			select.SearchDataTextField = DisplayTextField;
			select.SearchQuery = SearchQuery;
			select.SearchCountQuery = SearchCountQuery;
			select.ShowFlatList = ShowFlatList;
			select.AllowSelectAll = AllowSelectAll;
			select.RootObjectID = RootObjectID;
			select.IDField = IDField;
			if (Objects.Count > 0)
				select.SelectedObjectIDs = Objects.ToArray();
			else
				select.SelectedObjectGUIDs = GuidObjects.ToArray();

			if (ListSliding)
			{
				//ScriptManager.RegisterStartupScript(Page, typeof(Page), "InitSlider" + ClientID, "InitSlider" + ClientID + "();", true);
				Page.ClientScript.RegisterClientScriptInclude("jquery.cookie", Settings.JSPath + "jquery.cookie.js");
			}
		}

		public void LoadObjects<T>(IEnumerable<T> objects) where T : IModelObject
		{
			Objects.Clear();
			ObjectTitles.Clear();
			foreach (T obj in objects)
			{
				if (obj.ObjectID > 0)
					Objects.Add(obj.ObjectID);
				else
					GuidObjects.Add(obj.ObjectGUID);
				ObjectTitles.Add(Convert.ToString(obj.GetType().GetProperty(String.IsNullOrEmpty(DisplayTextField) ? DataTextField : DisplayTextField).GetValue(obj, null)));
			}
			up.Update();
		}

		public int[] ObjectIDs { get { return Objects.ToArray(); } }
		public Guid[] ObjectGUIDs { get { return GuidObjects.ToArray(); } }

		public int Count()
		{
			return Objects.Count > 0 ? Objects.Count : GuidObjects.Count;
		}

		protected List<int> Objects
		{
			get
			{
				if (ViewState["Objects"] == null)
					ViewState["Objects"] = new List<int>();
				return ViewState["Objects"] as List<int>;
			}
		}

		protected List<Guid> GuidObjects
		{
			get
			{
				if (ViewState["GuidObjects"] == null)
					ViewState["GuidObjects"] = new List<Guid>();
				return ViewState["GuidObjects"] as List<Guid>;
			}
		}

		public List<string> ObjectTitles
		{
			get
			{
				if (ViewState["ObjectTitles"] == null)
					ViewState["ObjectTitles"] = new List<string>();
				return ViewState["ObjectTitles"] as List<string>;
			}
		}

		public string SearchQueryParams { get; set; }
		public object RootObjectID { get; set; }
		public string DataTextField { get; set; }
		public string DataValueField { get; set; }
		public string DisplayTextField { get; set; }
		public string ParentField { get; set; }
		public string SelectLinkTitle { get; set; }
		public string IDField { get; set; }
		public string Title { get; set; }

		public int PageSize { get; set; }

		public Type Type { get; set; }
		public bool ShowFlatList { get; set; }
		public bool ShowMassOperations { get; set; }
		public bool ListSliding { get; set; }
        public bool HighlightSearchResults { get { return select.HighlightSearchResults; } set { select.HighlightSearchResults = value; } }
		public string NotFoundMessage { get { return select.GetNotFoundMessage(); } set { select.GetNotFoundMessage = () => value; } }
		public Func<string> GetNotFoundMessage { get { return select.GetNotFoundMessage; } set { select.GetNotFoundMessage = value; } }
		public IQueryable<dynamic> AllObjects { get; set; }
		public string QuickFilterValue { get { return select.QuickFilterValue; } set { select.QuickFilterValue = value; } }
		public Func<string, Expression<Func<dynamic, bool>>> SearchExpression { get; set; }
		public Func<string, int, int, IEnumerable> SearchQuery { get; set; }
		public Func<string, int> SearchCountQuery { get; set; }
		public bool RemoveUnselected { get; set; }

		public ModalDialog TargetModalDialog { get; set; }

		public event EventHandler SelectedChanged;
		protected internal virtual void OnSelectedChanged(EventArgs e)
		{
			if (SelectedChanged != null)
			{
				SelectedChanged(this, e);
			}
		}

		public void Clear()
		{
			Objects.Clear();
			GuidObjects.Clear();
			ObjectTitles.Clear();
			up.Update();
		}

		protected void select_Selected(object sender, SelectObjectHierarchicEventArgs e)
		{
			for (int i = 0; i < e.ObjectIDs.Length; i++)
				if (e.ObjectIDs[i] > 0 && !Objects.Contains(e.ObjectIDs[i]))
				{
					Objects.Add(e.ObjectIDs[i]);
					ObjectTitles.Add(e.Titles[i]);
				}
			for (int i = 0; i < e.ObjectGUIDs.Length; i++)
				if (e.ObjectGUIDs[i] != Guid.Empty && !GuidObjects.Contains(e.ObjectGUIDs[i]))
				{
					GuidObjects.Add(e.ObjectGUIDs[i]);
					ObjectTitles.Add(e.Titles[i]);
				}
			if (RemoveUnselected)
			{
				if (Objects.Count > 0)
				{
					for (int i = Objects.Count - 1; i >= 0; i--)
						if (!e.ObjectIDs.Contains(Objects[i]))
						{
							Objects.RemoveAt(i);
							ObjectTitles.RemoveAt(i);
						}
				}
				if (GuidObjects.Count > 0)
				{
					for (int i = GuidObjects.Count - 1; i >= 0; i--)
						if (!e.ObjectGUIDs.Contains(GuidObjects[i]))
						{
							GuidObjects.RemoveAt(i);
							ObjectTitles.RemoveAt(i);
						}
				}
			}
			OnSelectedChanged(e);
			up.Update();
		}

		protected void plmDelete_Click(object sender, EventArgs e)
		{
			if (plmDelete.Value.ToInt32(0) > 0)
			{
				int i = Objects.IndexOf(plmDelete.Value.ToInt32(0));
				Objects.RemoveAt(i);
				ObjectTitles.RemoveAt(i);
			}
			else
			{
				int i = GuidObjects.IndexOf(plmDelete.Value.ToGuid());
				GuidObjects.RemoveAt(i);
				ObjectTitles.RemoveAt(i);
			}
			OnSelectedChanged(new SelectObjectHierarchicEventArgs());
			up.Update();
		}

		protected void lbSelectAll_Click(object sender, EventArgs e)
		{
			Objects.Clear();
			GuidObjects.Clear();
			ObjectTitles.Clear();
			foreach (var obj in AllObjects.Cast<IModelObject>())
			{
				if (obj.ObjectID > 0)
					Objects.Add(obj.ObjectID);
				else
					GuidObjects.Add(obj.ObjectGUID);
				ObjectTitles.Add(Convert.ToString(obj.GetType().GetProperty(String.IsNullOrEmpty(DisplayTextField) ? DataTextField : DisplayTextField).GetValue(obj, null)));
			}
		}

		protected void lbDeleteAll_Click(object sender, EventArgs e)
		{
			Clear();
		}

		public List<T> GetSelected<T>() where T : class, IModelObject, new()
		{
			List<T> list = new List<T>();
			var iq = (System.Web.HttpContext.Current.Items["SolutionDataContext"] as DataContext).GetTable<T>();
			var s = new T();
			foreach (int id in Objects)
				list.Add(iq.Single(s.FindByID<T>(id)));
			foreach (Guid guid in GuidObjects)
				list.Add(iq.Single(s.FindByGUID<T>(guid)));
			
			return list;
		}
	}
}