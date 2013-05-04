using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web;
using System.Web.UI.WebControls;
using System.Runtime.Serialization;
using System.ComponentModel;

using System.Data.Linq;
using Nephrite.Web.Controllers;
using Nephrite.Web.App;
using System.Configuration;


namespace Nephrite.Web.Controls
{
	[ParseChildren(true)]
	[PersistChildren(false)]
	public class ObjectList2<T> : ObjectList2 where T : class, IModelObject, IMovableObject, new()
	{
		List<Func<T, object>> expressions = new List<Func<T, object>>();
		List<string> titles = new List<string>();
		List<bool> encode = new List<bool>();

		HiddenField action = new HiddenField { ID = "Action" };
		HiddenField editvisible = new HiddenField { ID = "EditVisible", Value = "0" };
		LinkButton go = new LinkButton { ID = "Go" };

		public DataContext DataContext { get; set; }
		public bool ReadOnly { get; set; }

		IQueryable<T> _data;
		public IQueryable<T> Data 
		{
			get
			{
				return _data;
			}
			set
			{
				_data = value;
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

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			Controls.Add(action);
			Controls.Add(go);
			Controls.Add(editvisible);

			if (edittemplate != null)
			{
				edittemplateInstance = new Control();
				edittemplate.InstantiateIn(edittemplateInstance);
				Controls.Add(edittemplateInstance);
			}

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
			T obj = new T();

			if (action.Value.Length > 0)
			{
				switch (action.Value[0])
				{
					case 'A':
						OnAddItem(EventArgs.Empty);
						break;
					case 'U':
						SimpleClassMover<T>.Up(_data, action.Value.Substring(1).ToInt32(0), true);
						break;
					case 'D':
						SimpleClassMover<T>.Down(_data, action.Value.Substring(1).ToInt32(0), true);
						break;
					case 'R':
						obj = DataContext.GetTable<T>().SingleOrDefault(obj.FindByID<T>(action.Value.Substring(1).ToInt32(0)));
						OnDeleteItem(new ObjectListEventArgs<T> { Data = obj });
						DataContext.GetTable<T>().DeleteOnSubmit(obj);
						DataContext.SubmitChanges();
						break;
					case 'S':
						if (action.Value.Length > 2 && !char.IsDigit(action.Value[1]))
						{
							obj = DataContext.GetTable<T>().SingleOrDefault(obj.FindByID<T>(action.Value.Substring(2).ToInt32(0)));
							ObjectListEventArgs<T> args = new ObjectListEventArgs<T> { Data = obj };
							OnSaveItem(args);
							if (args.Data != null)
							{
								DataContext.SubmitChanges();
								editvisible.Value = "0";
							}
						}
						else
						{
							ObjectListEventArgs<T> args = new ObjectListEventArgs<T> { Data = new T() };
							

							OnSaveItem(args);
							if (args.Data != null)
							{
								args.Data.SeqNo = _data.Count() > 0 ? _data.Max(o => o.SeqNo) + 1 : 1;

								DataContext.GetTable<T>().InsertOnSubmit(args.Data);
								DataContext.SubmitChanges();
								editvisible.Value = "0";
							}
						}
						break;
					case 'E':
						if (editvisible.Value != "1" /* вызывать только один раз вначале */)
						{
							obj = DataContext.GetTable<T>().SingleOrDefault(obj.FindByID<T>(action.Value.Substring(1).ToInt32(0)));
							OnEditItem(new ObjectListEventArgs<T> { Data = obj });
							editvisible.Value = "1";
						}
						break;
				}

				if (action.Value[0] != 'E')
					action.Value = "";
			}
		}


		protected override void Render(HtmlTextWriter writer)
		{
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
					writer.Write(" <a href='#' onclick='objectlist_" + ClientID + @"_showedit()'><img src='" + Settings.ImagesPath + "additem.png' class='middle' alt='" + AddingTooltip + "' /></a>");
			}
			writer.Write("</div>");
			if (editvisible.Value == "1")
			{
				writer.Write("<div id='" + ClientID + "_edit'>");
				edittemplateInstance.RenderControl(writer);
				writer.Write("<div style='text-align:right' class='tabletitle'>");
				writer.Write("<input type='button' value='Добавить' onclick='objectlist_" + ClientID + @"_save()' class='ms-ButtonHeightWidth' />");
				writer.Write(" <input type='button' value='Отмена' onclick='objectlist_" + ClientID + @"_hideedit()' class='ms-ButtonHeightWidth' />");
				writer.Write("</div></div>");
			}

			writer.Write("<table class='ms-listviewtable' cellpadding='0' cellspacing='0' width='100%'>");
			writer.Write("<tr class='ms-viewheadertr'>");

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

			if (_data != null)
			{
				bool alt = false;
				foreach (T obj in _data)
				{
					writer.Write("<tr" + (alt ? " class='ms-alternating'" : "") + ">");
					alt = !alt;
					for (int t = 0; t < titles.Count; t++)
					{
						writer.Write("<td class='ms-vb2'>");
						if (encode[t])
							writer.Write(HttpUtility.HtmlEncode((expressions[t](obj) ?? "").ToString()));
						else
							writer.Write((expressions[t](obj) ?? "").ToString());
						writer.Write("</td>");
					}
					writer.Write("<td align='center' width='90px'>");
					if (!ReadOnly)
					{
						if (EditItem != null)
							writer.Write("<a href='#' onclick=\"objectlist_" + ClientID + "_action('E'," + obj.ObjectID.ToString() + ")\"><img src='" + Settings.ImagesPath + "edit.png' alt='Редактировать' style='border-width:0px;'/></a>");
						writer.Write(" <a href='#' onclick=\"objectlist_" + ClientID + "_action('U'," + obj.ObjectID.ToString() + ")\"><img src='" + Settings.ImagesPath + "arrow_up.png' alt='Переместить вверх' style='border-width:0px;'/></a>");
						writer.Write(" <a href='#' onclick=\"objectlist_" + ClientID + "_action('D'," + obj.ObjectID.ToString() + ")\"><img src='" + Settings.ImagesPath + "arrow_down.png' alt='Переместить вниз' style='border-width:0px;'/></a>");
						writer.Write(" <a href='#' onclick=\"return objectlist_" + ClientID + "_action('R'," + obj.ObjectID.ToString() + ")\"><img src='" + Settings.ImagesPath + "delete.gif' alt='Удалить' style='border-width:0px;'/></a>");
					}
					writer.Write("</td>");
					writer.Write("</tr>");
				}
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
function objectlist_" + ClientID + @"_showedit()
{
    document.getElementById('" + editvisible.ClientID + @"').value = '1';
    document.getElementById('" + action.ClientID + @"').value = 'A';
    " + OnClientPostBack + @"
	" + Page.ClientScript.GetPostBackEventReference(go, "") + @";
}
function objectlist_" + ClientID + @"_save()
{
    document.getElementById('" + action.ClientID + @"').value = 'S' + document.getElementById('" + action.ClientID + @"').value;
    " + OnClientPostBack + @"
	" + Page.ClientScript.GetPostBackEventReference(go, "") + @";
}
function objectlist_" + ClientID + @"_hideedit()
{
	document.getElementById('" + editvisible.ClientID + @"').value = '0';
    document.getElementById('" + ClientID + @"_edit').style.display='none'; 
}
</script>");
		}

		public event EventHandler<ObjectListEventArgs<T>> EditItem;
		public event EventHandler<ObjectListEventArgs<T>> SaveItem;
		public event EventHandler<ObjectListEventArgs<T>> DeleteItem;
		public event EventHandler AddItem;

		protected internal virtual void OnEditItem(ObjectListEventArgs<T> e)
		{
			if (EditItem != null)
				EditItem(this, e);
		}
		
		protected internal virtual void OnDeleteItem(ObjectListEventArgs<T> e)
		{
			if (DeleteItem != null)
				DeleteItem(this, e);
		}

		protected internal virtual void OnSaveItem(ObjectListEventArgs<T> e)
		{
			if (SaveItem != null)
				SaveItem(this, e);
		}

		protected internal virtual void OnAddItem(EventArgs e)
		{
			if (AddItem != null)
			{
				AddItem(this, e);
			}
		}

		public void ClearColumns()
		{
			titles.Clear();
			expressions.Clear();
			encode.Clear();
		}
	}

	[ControlBuilder(typeof(ObjectList2ControlBuilder))]
	[ParseChildren(true)]
	[PersistChildren(false)]
	public class ObjectList2 : Control, INamingContainer
	{
		public string Type { get; set; }
		public string OnClientPostBack { get; set; }
		public string Title { get; set; }
		public string AddingTooltip { get; set; }

		protected Control edittemplateInstance;
		protected ITemplate edittemplate = null;

		protected Control menutemplateInstance;
		protected ITemplate menutemplate = null;

		[TemplateInstance(TemplateInstance.Single)]
		[Browsable(false)]
		public virtual ITemplate EditTemplate
		{
			get { return edittemplate; }
			set { edittemplate = value; }
		}

		[TemplateInstance(TemplateInstance.Single)]
		[Browsable(false)]
		public virtual ITemplate MenuTemplate
		{
			get { return menutemplate; }
			set { menutemplate = value; }
		}
	}


	public class ObjectList2ControlBuilder : ControlBuilder
	{
		public override void Init(TemplateParser parser, ControlBuilder parentBuilder, Type type, string tagName, string id, System.Collections.IDictionary attribs)
		{
			string typeName = (string)attribs["Type"];
            Type t = Type.GetType(typeName);
			Type genericType = typeof(ObjectList2<>);

			base.Init(parser, parentBuilder, genericType.MakeGenericType(t), tagName, id, attribs);
		}
	}


}
