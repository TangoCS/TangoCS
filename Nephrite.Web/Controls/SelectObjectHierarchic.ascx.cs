using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq.Expressions;
using System.Data.Linq.SqlClient;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Collections;
using System.Reflection;
using System.Data.Linq.Mapping;
using Nephrite.Web.TextResources;

namespace Nephrite.Web.Controls
{
    public partial class SelectObjectHierarchic : System.Web.UI.UserControl
    {
        public string Title { get; set; }
        public string Width { get; set; }
        public string Height { get; set; }
        public int PageSize { get; set; }
        public string DataTextField { get; set; }
		public string DataValueField { get; set; }
		public string ParentField { get; set; }
		public string IDField { get; set; }
		public string SearchDataTextField { get; set; }
        public string OnSelect { get; set; }
        public Type Type { get; set; }
        public bool Enabled { get; set; }
        public bool MultipleSelect { get; set; }
		public object RootObjectID { get; set; }
		public bool AllowSelectAll { get; set; }

		protected string SearchText = TextResource.Get("Common.Controls.SelectObjectHierarchic.Search", "Поиск"); 
		public Func<string> GetNotFoundMessage;
		string[] selectedObjects { get; set; }
		Guid[] rootGuids;
		int[] rootIds;

        public int[] SelectedObjectIDs 
		{
			get
			{
				return selectedObjects.Select(o => o.ToInt32(0)).ToArray();
			}
			set
			{
				selectedObjects = value.Select(o => o.ToString()).ToArray();
			}
		}
		public Guid[] SelectedObjectGUIDs
		{
			get
			{
				return selectedObjects.Select(o => o.ToGuid()).ToArray();
			}
			set
			{
				selectedObjects = value.Select(o => o.ToString()).ToArray();
			}
		}

		public bool ShowFlatList { get; set; }
        public bool HighlightSearchResults { get; set; }

        public IQueryable AllObjects { get; set; }
		
        //public Func<string, Expression<Func<IModelObject, bool>>> SearchExpression { get; set; }
		public Func<string, Expression<Func<object, bool>>> SearchExpression { get; set; }
		public Func<string, int, int, IEnumerable> SearchQuery { get; set; }
		public Func<string, int> SearchCountQuery { get; set; }

        public SelectObjectHierarchic()
        {
            //SearchExpression = s => (o => SqlMethods.Like(o.Title, "%" + s + "%"));
            Enabled = true;
			selectedObjects = new string[0];
			DataTextField = "Title";
			DataValueField = "ObjectID";
			RootObjectID = null;
			GetNotFoundMessage = () => TextResource.Get("Common.Controls.SelectObjectHierarchic.NotFoundMessage", "В системе отсутствует информация по вашему запросу");
			//SearchText = TextResource.Get("Common.Controls.SelectObjectHierarchic.Search", "Поиск");
		}

        List<string> ids = new List<string>();
        Dictionary<string, string> titles = new Dictionary<string, string>();
		protected void Page_Init(object sender, EventArgs e)
		{
            HighlightSearchResults = false;
			select.DisableHfOkClick = true;
			select.BottomLeftContentTemplate = bottomLeftContentTemplate;
			select.SetBottomLeft();
		}

        protected void Page_Load(object sender, EventArgs e)
        {
			if (!String.IsNullOrEmpty(Title))
				select.Title = Title;
			else
				select.Title = TextResource.Get("Common.Controls.SelectObject", "Выберите объект");
            if (!String.IsNullOrEmpty(Width))
                select.Width = Width;
			if (rblItems == null)
				return;
			//rblItems.DataTextField = SearchDataTextField ?? (DataTextField ?? "Title");
			//rblItems.DataValueField = DataValueField;
			//cblItems.DataTextField = SearchDataTextField ?? (DataTextField ?? "Title");
			//cblItems.DataValueField = DataValueField;
            select.OKClientClick = "selectOK_" + ClientID + "()";

            if (MultipleSelect)
            {
                // Запомнить предыдущие выбранные
                ids.AddRange(hfSelectedIDs.Value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                string[] t = hfSelectedTitles.Value.Split('\n');
                for (int i = 0; i < ids.Count; i++)
                    titles.Add(ids[i], t[i]);

                foreach (ListItem item in cblItems.Items)
                {
                    if (item.Selected && !ids.Contains(item.Value))
                    {
                        ids.Add(item.Value);
                        titles[item.Value] = item.Text;
                    }
                    else
                    {
                        if (!item.Selected && ids.Contains(item.Value))
                        {
                            ids.Remove(item.Value);
                            titles.Remove(item.Value);
                        }
                    }
                }
				if (hfCheckType.Value == "check")
				{
					ids.Clear();
					if (IDField.IsEmpty())
					{
						foreach (PropertyInfo pi in Type.GetProperties())
						{
							object[] attr = pi.GetCustomAttributes(typeof(ColumnAttribute), true);
							if (attr.Length == 1 && ((ColumnAttribute)attr[0]).IsPrimaryKey == true)
								IDField = pi.Name;
						}
						if (IDField.IsEmpty())
							throw new Exception(ClientID + ": Не задано свойство IDField");
					}
					ids.AddRange(AllObjects.Cast<object>().ToList().Select(o => DataBinder.GetPropertyValue(o, IDField).ToString()).ToList());
					titles = AllObjects.Cast<object>().Select(o => new
					{
						id = DataBinder.GetPropertyValue(o, IDField).ToString(),
						title = DataBinder.GetPropertyValue(o, DataTextField).ToString()
					}).ToDictionary(o => o.id, o => o.title);
				}
				if (hfCheckType.Value == "uncheck")
					ids.Clear();

                hfSelectedIDs.Value = String.Join(",", ids.Distinct().ToArray());
                hfSelectedTitles.Value = "";
                for (int i = 0; i < ids.Count; i++)
                    hfSelectedTitles.Value += titles[ids[i]] + "\n";
				hfCheckType.Value = "";
            }
            
            Height = (hfHeight.Value.ToInt32(0) - 200).ToString() + "px";
        }

        protected void select_Populate(object sender, EventArgs e)
        {
			if (RootObjectID is int[])
				rootIds = RootObjectID as int[];
			if (RootObjectID is Guid[])
				rootGuids = RootObjectID as Guid[];
			if (RootObjectID is int)
				rootIds = new int[] { (int)RootObjectID };
			if (RootObjectID is Guid)
				rootGuids = new Guid[] { (Guid)RootObjectID };


			if (Type == null)
				throw new Exception(ClientID + ": Не задано свойство Type");
			if (ParentField.IsEmpty() && Type.GetProperty("ParentID") != null)
				ParentField = "ParentID";
			if (ParentField.IsEmpty() && Type.GetProperty("ParentGUID") != null)
				ParentField = "ParentGUID";

			Type idType = null;
			if (IDField.IsEmpty())
			{

				var dynamicobject = Type.GetConstructor(new Type[] { }).Invoke(new object[] { });
				var modelObject = dynamicobject as IModelObject;
				if (modelObject != null) IDField = modelObject.MetaClass.Key.Name;

				if (IDField.IsEmpty())
					throw new Exception(ClientID + ": Не задано свойство IDField");
			}
			else
				idType = Type.GetProperty(IDField).PropertyType;

			object mo = Activator.CreateInstance(Type);

			if (SearchExpression == null && mo is IModelObject)
				SearchExpression = s => (o => SqlMethods.Like((o as IModelObject).Title, "%" + s + "%"));

			if (MultipleSelect && select.IsFirstPopulate)
            {
                ids.Clear();
                titles.Clear();
                
                foreach (var id in selectedObjects)
                {
					object typedid = id.ToGuid() == Guid.Empty ? (object)id.ToInt32(0) : (object)id.ToGuid();
					var obj = AllObjects.OfType<object>().SingleOrDefault(mo.FindByProperty<object>(IDField, typedid));
                    if (obj != null)
                    {
                        ids.Add(id);
						titles[id] = DataBinder.GetPropertyValue(obj, DataTextField).ToString();
                    }
                }
                hfSelectedIDs.Value = String.Join(",", ids.Distinct().ToArray());
                hfSelectedTitles.Value = "";
                for (int i = 0; i < ids.Count; i++)
                    hfSelectedTitles.Value += titles[ids[i]] + "\n";
            }

			IQueryable<object> items = AllObjects.Cast<object>();
            if (select.IsFirstPopulate)
            {
                hfParentID.Value = "0";
            }
            
            if (PageSize == 0)
                PageSize = 20;

            if (Type != null && hfQuickFilter.Value.Trim() == String.Empty)
            {
				if (!ParentField.IsEmpty() && !ShowFlatList)
                {
                    if (hfParentID.Value.ToInt32(0) > 0 || hfParentID.Value.ToGuid() != Guid.Empty)
                    {
						object obj = null;
						if (hfParentID.Value.ToInt32(0) > 0)
							obj = items.SingleOrDefault(mo.FindByProperty<object>(IDField, hfParentID.Value.ToInt32(0)));
						else
							obj = items.SingleOrDefault(mo.FindByProperty<object>(IDField, hfParentID.Value.ToGuid()));
						if (obj != null)
						{
							sled.Text = DataBinder.GetPropertyValue(obj, DataTextField).ToString();

							object parentid = DataBinder.GetPropertyValue(obj, ParentField);

							while (parentid != null)
							{
								obj = items.SingleOrDefault(mo.FindByProperty<object>(IDField, parentid));
								if (obj == null || (rootIds != null && rootIds.Contains((int)DataBinder.GetPropertyValue(obj, DataValueField))) || (rootGuids != null && rootGuids.Contains((Guid)DataBinder.GetPropertyValue(obj, DataValueField))))
									break;
								sled.Text = "<a href='#' onclick=\"godeeper_" + ClientID + "('" + DataBinder.GetPropertyValue(obj, IDField).ToString() + "');return false;\">" + DataBinder.GetPropertyValue(obj, DataTextField) + "</a> / " + sled.Text;
								parentid = DataBinder.GetPropertyValue(obj, ParentField);
							}

							if (sled.Text != String.Empty)
								sled.Text = "<a href='#' onclick=\"godeeper_" + ClientID + "('0');return false;\">Объекты</a> / " + sled.Text;
						}
                    }

					if (hfParentID.Value.ToInt32(0) == 0 && hfParentID.Value.ToGuid() == Guid.Empty)
					{
						if (RootObjectID == null)
						{
							items = items.Where(mo.FindByProperty<object>(ParentField, null));
						}
						else
						{
							items = items.Where(mo.FindByProperty<object>(ParentField, RootObjectID));
						}
					}
					else
					{

						if (idType == typeof(int))
						{
							items = items.Where(mo.FindByProperty<object>(ParentField, hfParentID.Value.ToInt32(0)));
						}
						else if (idType == typeof(Guid))
						{
							items = items.Where(mo.FindByProperty<object>(ParentField, hfParentID.Value.ToGuid()));
						}
					}
                }
            }
			IEnumerable list;
			if (hfQuickFilter.Value.Trim() != String.Empty)
			{
				if (SearchQuery != null && SearchCountQuery != null)
				{
					select.PageCount = (int)Math.Ceiling((decimal)SearchCountQuery(hfQuickFilter.Value.Trim()) / (decimal)PageSize);
					list = SearchQuery(hfQuickFilter.Value.Trim(), select.PageIndex, PageSize);
				}
				else
				{
					items = items.Where(SearchExpression(hfQuickFilter.Value.Trim()));
					select.PageCount = (int)Math.Ceiling((decimal)items.Count() / (decimal)PageSize);

					list = items.Skip((select.PageIndex - 1) * PageSize).Take(PageSize);
				}
			}
			else
			{
				select.PageCount = (int)Math.Ceiling((decimal)items.Count() / (decimal)PageSize);
				list = items.Skip((select.PageIndex - 1) * PageSize).Take(PageSize);
			}

			cblItems.Items.Clear();
			rblItems.Items.Clear();

			foreach (object obj in list)
			{
				ListItem li = new ListItem();
				li.Text = (DataBinder.GetPropertyValue(obj, SearchDataTextField ?? DataTextField) ?? "").ToString();
				li.Value = (DataBinder.GetPropertyValue(obj, DataValueField) ?? "").ToString();
				if (ids.Contains(li.Value))
					li.Selected = true;

                if (HighlightSearchResults && hfQuickFilter.Value.Trim().Length > 0)
				{
					li.Text = Regex.Replace(li.Text, "(?i)(?<1>" + hfQuickFilter.Value.Trim().Replace("<", "").
						Replace(">", "").Replace("(", "").Replace(")", "").Replace("[", "").Replace("]", "").
						Replace("{", "").Replace("}", "").Replace("?", "").Replace("*", "") + ")",
						"<span style='color:Red; font-weight:bold'>$1</span>");
					//li.Text = li.Text.Replace(hfQuickFilter.Value.Trim(), "<span style='color:Red; font-weight:bold'>" + hfQuickFilter.Value.Trim() + "</span>");
				}

				if (!ParentField.IsEmpty() && !ShowFlatList && hfQuickFilter.Value.Trim() == String.Empty)
				{
					object id = DataBinder.GetPropertyValue(obj, IDField);
					int childcnt = AllObjects.Cast<object>().Where(mo.FindByProperty<object>(ParentField, id)).Count();
					if (childcnt > 0)
						li.Text = "<a href='#' onclick=\"godeeper_" + ClientID + "('" + id + "');return false;\">" + li.Text + "</a>";
				}

				if (MultipleSelect)
					cblItems.Items.Add(li);
				else
					rblItems.Items.Add(li);
			}

			if (MultipleSelect)
            {
				if (cblItems.Items.Count == 1 && (ParentField.IsEmpty() || hfQuickFilter.Value.Trim() != String.Empty))
                    cblItems.Items[0].Selected = true;
            }
            else
            {
				if (rblItems.Items.Count == 1 && (ParentField.IsEmpty() || hfQuickFilter.Value.Trim() != String.Empty))
                    rblItems.Items[0].Selected = true;
            }

            AllObjects = items;
			//if (!select.IsFirstPopulate && hfQuickFilter.Value != "")
			//	ScriptManager.RegisterStartupScript(select.UpdatePanel, select.UpdatePanel.GetType(), "focusSearch", "window.setTimeout(\"document.getElementById('text_" + ClientID + "').focus();\", 100);", true);
			if(rblItems.Items.Count == 0 && cblItems.Items.Count == 0)
			{
				sled.Text = "<p style='text-align: center; font-style: italic; color:Gray;'>" + GetNotFoundMessage() + "</p>";
			} 
        }

		public string QuickFilterValue
		{
			get { return hfQuickFilter.Value; }
			set { hfQuickFilter.Value = value; }
		}

        public string RenderRun()
        {
            return Enabled ? select.RenderRun() : "";
        }

        public event EventHandler<SelectObjectHierarchicEventArgs> Selected;
        protected internal virtual void OnSelected(EventArgs e)
        {
            if (Selected != null)
            {
                SelectObjectHierarchicEventArgs args = new SelectObjectHierarchicEventArgs();
                if (MultipleSelect)
                {
                    for (int i = 0; i < ids.Count; i++)
                    {
						args.AddObject(ids[i], Regex.Replace(titles[ids[i]], "<.*?>", ""));
                    }
                }
                else
                    args.AddObject(hfSelectedID.Value, hfSelectedTitle.Value);

                Selected(this, args);
            }
        }

        public ModalDialog TargetModalDialog
        {
            get { return select.TargetModalDialog; }
            set { select.TargetModalDialog = value; }
        }

        protected void lbOKClick_Click(object sender, EventArgs e)
        {
            OnSelected(e);
            if (TargetModalDialog != null)
                ScriptManager.RegisterStartupScript(select.UpdatePanel, select.UpdatePanel.GetType(), "showwnd" + TargetModalDialog.ID, "loaded" + TargetModalDialog.ClientID + "();", true);
        }

        protected bool HasSelectedHandler
        {
            get { return Selected != null; }
        }

		public ModalDialog ModalDialog
		{
			get { return select; }
		}

		ITemplate bottomLeftContentTemplate;
		[TemplateInstance(TemplateInstance.Single)]
		[Browsable(false)]
		public virtual ITemplate BottomLeftContentTemplate
		{
			get { return bottomLeftContentTemplate; }
			set { bottomLeftContentTemplate = value; }
		}
    }

    public class SelectObjectHierarchicEventArgs : EventArgs
    {
        public string Title 
        {
            get { return Titles[0]; }
        }

        public int ObjectID 
        {
            get { return ObjectIDs[0]; }
        }

		public Guid ObjectGUID
		{
			get { return ObjectGUIDs[0]; }
		}

		public int[] ObjectIDs
		{
			get { return objects.Select(o => o.ToInt32(0)).ToArray(); }
		}
		public Guid[] ObjectGUIDs
		{
			get { return objects.Select(o => o.ToGuid()).ToArray(); }
		}

		public string[] Titles { get { return titles.ToArray(); } }

		List<string> objects = new List<string>();
		List<string> titles = new List<string>();
		public void AddObject(string id, string title)
		{
			objects.Add(id);
			titles.Add(title);
		}
    }
}
