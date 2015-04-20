using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.ComponentModel;
using System.Web.UI.WebControls;
using System.Drawing;
using Nephrite.Data;



namespace Nephrite.Web.Controls
{
    [ParseChildren(true)]
    [PersistChildren(false)]
	public class ObjectListExEd<T> : ObjectListExEd where T : class, new()
	{
        List<Func<T, object>> expressions = new List<Func<T, object>>();
        List<string> titles = new List<string>();
        List<bool> encode = new List<bool>();

        public ModalDialog EditorDialog { get; set; }

        HiddenField action = new HiddenField { ID = "Action" };
        HiddenField editvisible = new HiddenField { ID = "EditVisible", Value = "0" };
        Label lMess = new Label { ID = "lMess", ForeColor = Color.Red };
        LinkButton go = new LinkButton { ID = "Go" };

        public IDataContext DataContext { get; set; }
        public bool ReadOnly { get; set; }

        List<ObjectListObject<T>> Data
        {
            get
            {
                var list = ViewState["data"] as List<ObjectListObject<T>>;
                if (list == null)
                    ViewState["data"] = list = new List<ObjectListObject<T>>();
                return list;
            }
        }

        List<T> DeletedData
        {
            get
            {
                var list = ViewState["deleted"] as List<T>;
                if (list == null)
                    ViewState["deleted"] = list = new List<T>();
                return list;
            }
        }

        /// <summary>
        /// Добавить столбец
        /// </summary>
        /// <param name="title">Заголовок</param>
        /// <param name="expression">Значение</param>
        public void AddColumn(string title, Func<T, object> expression, bool htmlEncode)
        {
            titles.Add(title);
            expressions.Add(expression);
            encode.Add(htmlEncode);
        }

        public void AddColumn(string title, Func<T, object> expression)
        {
            AddColumn(title, expression, true);
        }

        /// <summary>
        /// Загрузить исходные данные
        /// </summary>
        /// <param name="data"></param>
        public void LoadData(IEnumerable<T> data)
        {
            var list = Data;
            list.Clear();
            foreach (var item in data)
                list.Add(new ObjectListObject<T> { Original = (T)((ICloneable)item).Clone(), Current = item });
        }

        /// <summary>
        /// Обновить данные (не вызывает SubmitChanges!)
        /// </summary>
        public void Update()
        {
            var list = Data;

            foreach (var item in list)
            {
				if (item.Original == null && item.Current != null)
					DataContext.GetTable<T>().InsertOnSubmit(item.Current);
				else
				{
					//DataContext.GetTable<T>().Attach(item.Current, item.Original);
				}
            }


            var list1 = DeletedData;
            foreach (var item in list1)
            {
                //DataContext.GetTable<T>().Attach(item);
                DataContext.GetTable<T>().DeleteOnSubmit(item);
            }
        }

        public T GetItem(int index)
        {
            var list = Data;
            if (list == null)
                return null;
            return list[index].Current;
        }

		public T GetItemOriginal(int index)
		{
			var list = Data;
			if (list == null)
				return null;
			return list[index].Original;
		}

        public List<T> GetItems()
        {
            var list = Data;

            List<T> l = new List<T>();

            foreach (var item in list)
                l.Add(item.Current);

            return l;
        }
		
        public void Delete(int index)
        {
            var list = Data;

            if (index >= list.Count)
                return;

            if (list[index].Original != null)
                DeletedData.Add(list[index].Original);
            list.RemoveAt(index);
        }

        public void DeleteAll()
        {
            var list = Data;

            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (list[i].Original != null)
                    DeletedData.Add(list[i].Original);
                list.RemoveAt(i);
            }
        }

        public int ItemCount
        {
            get
            {
                var list = ViewState["data"] as List<ObjectListObject<T>>;
                if (list == null)
                    return 0;
                return list.Count;
            }
        }

        public void Add(T item)
        {
            Data.Add(new ObjectListObject<T> { Current = item, Original = null });
            if (item is IWithSeqNo)
            {
                if (Data.Count == 0)
                    ((IWithSeqNo)item).SeqNo = 1;
                else
                    ((IWithSeqNo)item).SeqNo = GetItems().Max(m => ((IWithSeqNo)m).SeqNo) + 1;
            }
        }

        public void MoveUp(int index)
        {
            if (index >= Data.Count || index == 0)
                return;

            var obj1 = Data[index].Current as IWithSeqNo;
            var obj2 = Data[index - 1].Current as IWithSeqNo;
            if (obj1 == null || obj2 == null)
                return;

            Data.Reverse(index - 1, 2);
            int seqno = obj1.SeqNo;
            obj1.SeqNo = obj2.SeqNo;
            obj2.SeqNo = seqno;
        }

        public void MoveDown(int index)
        {
            if (index >= Data.Count - 1)
                return;

            var obj1 = Data[index].Current as IWithSeqNo;
            var obj2 = Data[index + 1].Current as IWithSeqNo;
            if (obj1 == null || obj2 == null)
                return;

            Data.Reverse(index, 2);
            int seqno = obj1.SeqNo;
            obj1.SeqNo = obj2.SeqNo;
            obj2.SeqNo = seqno;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Controls.Add(action);
            Controls.Add(go);
            Controls.Add(editvisible);

            if (menutemplate != null)
            {
                menutemplateInstance = new Control();
                menutemplate.InstantiateIn(menutemplateInstance);
                Controls.Add(menutemplateInstance);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (action.Value.Length > 0)
            {
                switch (action.Value[0])
                {
                    case 'U':
                        MoveUp(action.Value.Substring(1).ToInt32(0));
                        break;
                    case 'D':
                        MoveDown(action.Value.Substring(1).ToInt32(0));
                        break;
                    case 'R':
                        Delete(action.Value.Substring(1).ToInt32(0));
                        break;
                }

                if (action.Value[0] != 'E')
                    action.Value = "";
            }
        }

        public void ClearItems()
        {
            Data.Clear();
        }

        protected override void Render(HtmlTextWriter writer)
        {
			if (EditorDialog == null)
				return;
            action.RenderControl(writer);
            go.RenderControl(writer);
            editvisible.RenderControl(writer);

            writer.Write("<div class='tabletitle'>");
            writer.Write(Title);
            if (!ReadOnly)
            {
                if (menutemplateInstance != null)
                    menutemplateInstance.RenderControl(writer);
                else
                    writer.Write(" <a href='#' onclick='" + EditorDialog.RenderRun() + @"'><img src='" + Settings.ImagesPath + "additem.png' class='middle' alt='" + AddingTooltip + "' /></a>");
            }
            writer.Write("</div>");

			writer.Write("<table class='ms-listviewtable' cellpadding='0' cellspacing='0' width='");

			if (String.IsNullOrEmpty(Width))
				writer.Write("100%");
			else
				writer.Write(Width);

			writer.Write("'><tr class='ms-viewheadertr'>");

            for (int i = 0; i < titles.Count; i++)
            {
                writer.Write("<th class='ms-vh2'><table class='ms-unselectedtitle'><tr><td class='ms-vb'>");
                writer.Write(titles[i]);
                writer.Write("</td></tr></table></th>");
            }

            writer.Write("<th class='ms-vh2'><table class='ms-unselectedtitle'><tr><td class='ms-vb'>");
            writer.Write("Действие");
            writer.Write("</td></tr></table></th>");
            writer.Write("</tr>");

            var list = Data;

			//foreach (var item in list)
			//{
			//	try
			//	{
			//		DataContext.GetTable<T>().Attach(item.Current);
			//	}
			//	catch { }
			//}
            for (int i = 0; i < list.Count; i++)
            {
                writer.Write("<tr" + ((i / 2) * 2 > 0 ? " class='ms-alternating'" : "") + ">");
                for (int t = 0; t < titles.Count; t++)
                {
                    writer.Write("<td class='ms-vb2'>");
					if (t == 0)
						writer.Write("<a href='#' onclick=\"" + EditorDialog.RenderRun(i.ToString()) + "\">");
                    if (encode[t])
                        writer.Write(HttpUtility.HtmlEncode((expressions[t](list[i].Current) ?? "").ToString()));
                    else
                        writer.Write((expressions[t](list[i].Current) ?? "").ToString());
					if (t == 0)
						writer.Write("</a>");
                    writer.Write("</td>");
                }
                writer.Write("<td align='center' width='90px'>");
                if (!ReadOnly)
                {
                    if (EditorDialog != null)
                        writer.Write("<a href='#' onclick=\"" + EditorDialog.RenderRun(i.ToString()) + "\"><img src='" + Settings.ImagesPath + "edit.png' alt='Редактировать' style='border-width:0px;'/></a>");
                    if (typeof(IWithSeqNo).IsAssignableFrom(typeof(T)))
                    {
                        writer.Write(" <a href='#' onclick=\"objectlist_" + ClientID + "_action('U'," + i.ToString() + ")\"><img src='" + Settings.ImagesPath + "arrow_up.png' alt='Переместить вверх' style='border-width:0px;'/></a>");
                        writer.Write(" <a href='#' onclick=\"objectlist_" + ClientID + "_action('D'," + i.ToString() + ")\"><img src='" + Settings.ImagesPath + "arrow_down.png' alt='Переместить вниз' style='border-width:0px;'/></a>");
                    }
                    writer.Write(" <a href='#' onclick=\"return objectlist_" + ClientID + "_action('R'," + i.ToString() + ")\"><img src='" + Settings.ImagesPath + "delete.gif' alt='Удалить' style='border-width:0px;'/></a>");
                }
                writer.Write("</td>");
                writer.Write("</tr>");
            }
            writer.Write("</table>");
            writer.Write(@"<script type='text/javascript'>
function objectlist_" + ClientID + @"_action(action, index)
{
	if (action == 'R')
		if (!confirm('Вы действительно хотите удалить элемент?'))
			return false;
    document.getElementById('" + action.ClientID + @"').value = action + index;
    " + OnClientPostBack + @"
	" + Page.ClientScript.GetPostBackEventReference(go, "") + @";
}
</script>");
        }

        public void ClearColumns()
        {
            titles.Clear();
            expressions.Clear();
            encode.Clear();
        }
    }

    [ControlBuilder(typeof(ObjectListExEdControlBuilder))]
    [ParseChildren(true)]
    [PersistChildren(false)]
    public class ObjectListExEd : Control, INamingContainer
    {
        public string Type { get; set; }
        public string OnClientPostBack { get; set; }
        public string Title { get; set; }
		public string AddingTooltip { get; set; }
		public string Width { get; set; }

        protected Control menutemplateInstance;
        protected ITemplate menutemplate = null;

        [TemplateInstance(TemplateInstance.Single)]
        [Browsable(false)]
        public virtual ITemplate MenuTemplate
        {
            get { return menutemplate; }
            set { menutemplate = value; }
        }
    }

    public class ObjectListExEdControlBuilder : ControlBuilder
    {
        public override void Init(TemplateParser parser, ControlBuilder parentBuilder, Type type, string tagName, string id, System.Collections.IDictionary attribs)
        {
            string typeName = (string)attribs["Type"];
            Type t = Type.GetType(typeName);
            Type genericType = typeof(ObjectListExEd<>);

            base.Init(parser, parentBuilder, genericType.MakeGenericType(t), tagName, id, attribs);
        }
    }

	[Serializable]
	public class ObjectListObject<T>
	{
		public T Current;
		public T Original;
	}

	/*public class ObjectListEventArgs<T> : EventArgs
	{
		public T Data;
	}

	public class ObjectListSaveEventArgs<T> : EventArgs
	{
		public T Data;
		public string Message;
		public bool Cancel = false;
	}*/
}
