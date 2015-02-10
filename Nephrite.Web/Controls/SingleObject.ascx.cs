using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq.Expressions;
using System.Web.UI.WebControls.WebParts;
using System.Collections;
using Nephrite.Web;


namespace Nephrite.Web.Controls
{
	public partial class SingleObject : System.Web.UI.UserControl
	{
		public SingleObject()
		{
			//DataTextField = "Title";
			//DataValueField = "ObjectID";
		}

		protected void Page_Init(object sender, EventArgs e)
		{
		}


		protected override void CreateChildControls()
		{
			base.CreateChildControls();

		}

		protected string RenderRun()
		{
			return select.RenderRun();
		}

		void select_Selected(object sender, SelectObjectHierarchicEventArgs e)
		{
			hfObjectID.Value = e.ObjectID > 0 ? e.ObjectID.ToString() : e.ObjectGUID.ToString();
			tbObject.Text = e.Title;
			OnSelected(e);
			up.Update();
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			select.OnSelect = "OnSelect_" + ClientID;
			select.Selected += new EventHandler<SelectObjectHierarchicEventArgs>(select_Selected);
			select.AllObjects = AllObjects;
			if (SearchExpression != null)
				select.SearchExpression = SearchExpression;
			select.TargetModalDialog = TargetModalDialog;
			select.Title = Title;
			select.PageSize = PageSize;
			select.SearchDataTextField = DisplayTextField;
			select.SearchQuery = SearchQuery;
			select.SearchCountQuery = SearchCountQuery;
			select.ShowFlatList = ShowFlatList;
			select.CanSelectFunc = CanSelectFunc;

			if (AutoComplete)
			{
				Nephrite.Web.AutoComplete.Apply(tbObject, new AutoCompleteOptions { AllowMultipleSelect = false, SearchQueryParams = SearchQueryParams, PostBackControl = lbAutoComplete });
				tbObject.Attributes.Add("onchange", Page.ClientScript.GetPostBackEventReference(lbAutoComplete, ""));
				if (Query.GetString("autocompleteclientid") == tbObject.ClientID)
				{
					if (SearchQuery == null)
						Response.End();
					foreach (IModelObject obj in SearchQuery(HttpUtility.UrlDecode(Query.GetString("q")), 1, Settings.PageSize))
					{
						Response.Write(DataBinder.GetPropertyValue(obj, DisplayTextField ?? DataTextField).ToString() + "\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<span style='display:none'>" + (DataValueField.IsEmpty() ? obj.ObjectID.ToString() : DataBinder.GetPropertyValue(obj, DataValueField)) + "</span>" + Environment.NewLine);
					}
					Response.End();
				}
			}
			else
				tbObject.ReadOnly = true;
			tbObject.Attributes.Add("AUTOCOMPLETE", "OFF");
			if (!Height.IsEmpty)
			{
				tbObject.TextMode = TextBoxMode.MultiLine;
				tbObject.Height = Height;
			}
		}

		protected void lbAutoComplete_Click(object sender, EventArgs e)
		{
			SelectObjectHierarchicEventArgs args = new SelectObjectHierarchicEventArgs();
			IModelObject mo = Activator.CreateInstance(Type) as IModelObject;
			if (tbObject.Text.IndexOf("<span") < 0)
			{
				ScriptManager.RegisterStartupScript(up, up.GetType(), "alert", "alert('Необходимо выбрать элемент из списка!');", true);
				Clear();
				return;
			}
			var s = tbObject.Text.Substring(tbObject.Text.IndexOf("<span"));
			s = s.Substring(0, s.IndexOf("</span>"));
			s = s.Substring(s.IndexOf('>') + 1);
			int id = s.ToInt32(0);
			var obj = id > 0 ?
				AllObjects.Cast<IModelObject>().Where(mo.FindByID<IModelObject>(id)).FirstOrDefault() :
				AllObjects.Cast<IModelObject>().Where(mo.FindByGUID<IModelObject>(s.ToGuid())).FirstOrDefault();
			if (obj != null)
			{
				SetObject(obj);
				args.AddObject(obj.ObjectID > 0 ? obj.ObjectID.ToString() : obj.ObjectGUID.ToString(), tbObject.Text);
				OnSelected(args);
			}
			else
			{
				ScriptManager.RegisterStartupScript(up, up.GetType(), "alert", "alert('Необходимо выбрать элемент из списка!');", true);
				Clear();
			}
		}

		public string SearchQueryParams { get; set; }

		public void SetObject(int objectID, string title)
		{
			hfObjectID.Value = objectID.ToString();
			tbObject.Text = title;
		}

		public void SetObject(Guid objectGUID, string title)
		{
			hfObjectID.Value = objectGUID.ToString();
			tbObject.Text = title;
		}

		public int? ObjectID
		{
			get { return hfObjectID.Value.ToInt32(); }
		}

		public Guid? ObjectGUID
		{
			get { return hfObjectID.Value.ToGuid() == Guid.Empty ? null : (Guid?)hfObjectID.Value.ToGuid(); }
		}

		public bool HasObject
		{
			get { return hfObjectID.Value.ToInt32(0) > 0 || hfObjectID.Value.ToGuid() != Guid.Empty; }
		}

		public string ObjectTitle
		{
			get { return tbObject.Text; }
		}

		public void SetObject(IModelObject obj)
		{
			if (obj != null)
			{
				hfObjectID.Value = Convert.ToString(obj.GetType().GetProperty(String.IsNullOrEmpty(DataValueField) ? "ObjectID" : DataValueField).GetValue(obj, null));
				tbObject.Text = Convert.ToString(obj.GetType().GetProperty(String.IsNullOrEmpty(DisplayTextField) ? DataTextField : DisplayTextField).GetValue(obj, null));
			}
			else
			{
				hfObjectID.Value = "";
				tbObject.Text = "";
			}
			up.Update();
		}

		public T GetObject<T>() where T : IModelObject
		{
			T empty = default(T);
			if (hfObjectID.Value.ToInt32(0) > 0)
				return AllObjects.Cast<T>().SingleOrDefault(empty.FindByID<T>(hfObjectID.Value.ToInt32(0)));
			if (hfObjectID.Value.ToGuid() != Guid.Empty)
				return AllObjects.Cast<T>().SingleOrDefault(empty.FindByGUID<T>(hfObjectID.Value.ToGuid()));
			return default(T);
		}

		public string DataTextField { get { return select.DataTextField; } set { select.DataTextField = value; } }
		public string DisplayTextField { get; set; }
		public string DataValueField { get { return select.DataValueField; } set { select.DataValueField = value; } }
		public string ParentField { get { return select.ParentField; } set { select.ParentField = value; } }
		public string IDField { get { return select.IDField; } set { select.IDField = value; } }
		public bool HighlightSearchResults { get { return select.HighlightSearchResults; } set { select.HighlightSearchResults = value; } }
		public string NotFoundMessage { get { return select.GetNotFoundMessage(); } set { select.GetNotFoundMessage = () => value; } }
		public Func<string> GetNotFoundMessage { get { return select.GetNotFoundMessage; } set { select.GetNotFoundMessage = value; } }
		public Func<dynamic, bool> CanSelectFunc { get; set; }
		public string Title { get; set; }
		public string QuickFilterValue { get { return select.QuickFilterValue; } set { select.QuickFilterValue = value; } }
		public int PageSize { get; set; }

		public Type Type { get { return select.Type; } set { select.Type = value; } }
		public bool ShowFlatList { get; set; }

		public Unit Height { get; set; }

		public IQueryable<dynamic> AllObjects { get; set; }

		public Func<string, Expression<Func<dynamic, bool>>> SearchExpression { get; set; }
		public Func<string, int, int, IEnumerable> SearchQuery { get; set; }
		public Func<string, int> SearchCountQuery { get; set; }

		public ModalDialog TargetModalDialog { get; set; }

		public event EventHandler<SelectObjectHierarchicEventArgs> Selected;
		protected internal virtual void OnSelected(SelectObjectHierarchicEventArgs e)
		{
			if (Selected != null)
			{
				Selected(this, e);
				up.Update();
			}
		}

		protected void lbClear_Click(object sender, EventArgs e)
		{
			if (Selected != null)
			{
				var args = new SelectObjectHierarchicEventArgs();
				args.AddObject("", "");
				Selected(this, args);
			}
			tbObject.Text = "";
			hfObjectID.Value = "";
			up.Update();
		}

		public bool AutoComplete { get; set; }

		public bool Enabled
		{
			get { return tbObject.Enabled; }
			set
			{
				if (tbObject.Enabled != value)
				{
					tbObject.Enabled = value;
					up.Update();
				}
			}
		}

		public void Clear()
		{
			tbObject.Text = "";
			hfObjectID.Value = "";
			up.Update();
		}

		public ModalDialog ModalDialog
		{
			get { return select.ModalDialog; }
		}

		public object RootObjectID { get { return select.RootObjectID; } set { select.RootObjectID = value; } }
	}
}