using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Nephrite.Meta;

namespace Nephrite.Web.FormsGenerator
{
	public partial class ViewCode
	{
		public static string View(FormElement form)
		{
			string idCol = (form.Type as MetaClass).Key.Name;

			var cols = form.Content.Where(o => o.Content == null);
			var groups = form.Content.Where(o => o.Content != null);

			StringBuilder res = new StringBuilder();
			res.AppendLine(@"<nw:Toolbar ID=""toolbar"" runat=""server"" />");

			if (cols.Count() > 0)
			{
				res.AppendLine(@"<%=Layout.FormTableBegin(new {style=""width:700px""})%>");
				foreach (var p in cols)
				{
					res.AppendLine(@"<%=Layout.FormRowBegin(""" + p.Caption + @""")%>");
					res.AppendLine(GetViewFieldValue(p));
					res.AppendLine("<%=Layout.FormRowEnd()%>");
				}
				res.AppendLine("<%=Layout.FormTableEnd() %>");
			}

			foreach (var g in groups)
			{
				res.AppendLine(@"<%=Layout.GroupTitle(""" + g.Caption + @""")%>");
				res.AppendLine(@"<%=Layout.FormTableBegin(new {style=""width:700px""})%>");
				foreach (var p in g.Content)
				{
					res.AppendLine(@"<%=Layout.FormRowBegin(""" + p.Caption + @""")%>");
					res.AppendLine(GetViewFieldValue(p));
					res.AppendLine("<%=Layout.FormRowEnd()%>");
				}
				res.AppendLine("<%=Layout.FormTableEnd() %>");
			}

			res.AppendLine("<%=HtmlHelperWSS.FormToolbarBegin() %>");
			res.AppendLine("<%=HtmlHelperWSS.FormToolbarWhiteSpace() %>");
			res.AppendLine("<%=HtmlHelperWSS.FormToolbarItemBegin() %>");
			res.AppendLine(@"<nw:BackButton runat=""server"" ID=""BackButton"" />");
			res.AppendLine("<%=HtmlHelperWSS.FormToolbarItemEnd() %>");
			res.AppendLine("<%=HtmlHelperWSS.FormToolbarEnd() %>");



			res.AppendLine(@"<br />
<br />
<span style=""font-size:xx-small""><%=""Последнее изменение: "" + ViewData.LastModifiedDate.DateToString() + "" "" + ViewData.LastModifiedDate.TimeToString() + "" "" + ViewData.LastModifiedUser.Title %></span>
");


			// скрипт
			res.AppendLine(@"<script runat=""server"">");
			res.AppendLine("");

			res.AppendLine("protected void Page_Load(object sender, EventArgs e)");
			res.AppendLine("{");
			res.AppendLine("\tSetTitle(\"" + form.Caption + "\");");

			res.AppendLine("\tif (!ViewData.IsDeleted) {");
			res.AppendLine("\t\ttoolbar.AddItem<" + form.Type.Name + @"Controller>(""edititem.gif"", ""Редактировать"", c => c.Edit(ViewData." + idCol + ", Query.CreateReturnUrl()));");
			res.AppendLine("\t\ttoolbar.AddItemSeparator();");
			res.AppendLine("\t\ttoolbar.AddItem<" + form.Type.Name + @"Controller>(""delete.gif"", ""Удалить"", c => c.Delete(ViewData." + idCol + ", Query.CreateReturnUrl()));");
			res.AppendLine("\t}");
			res.AppendLine("\telse");
			res.AppendLine("\t\ttoolbar.Visible = false;");
			res.AppendLine("}");

			res.AppendLine("</script>");
			return res.ToString();
		}

		public static string GetViewFieldValue(FormElement p)
		{
			if (p.Type is MetaGuidType || p.Type is MetaStringType)
			{
				return "<%=enc(ViewData." + p.Name + ") %>";
			}
			else if (p.Type is MetaDateTimeType)
			{
				return "<%=ViewData." + p.Name + ".DateTimeToString() %>";
			}
			else if (p.Type is MetaDateType)
			{
				return "<%=ViewData." + p.Name + ".DateToString() %>";
			}
			else if (p.Type is IMetaNumericType)
			{
				return "<%=ViewData." + p.Name + ".ToString() %>";
			}
			else if (p.Type is MetaBooleanType)
			{
				return "<%=ViewData." + p.Name + ".Icon() %>";
			}
			else if (p.Type is MetaClass)
			{
				if (p.IsRequired)
				{
					return "<%=enc(ViewData." + p.Name + ".Title) %>";
				}
				else
					return "<%=ViewData." + p.Name + " == null ? \"\" : enc(ViewData." + p.Name + ".Title) %>";
			}
			else
				return "";
		}

		public static string Delete(FormElement form)
		{
			return @"<%if(lo .GetLinkedObjects().Count > 0){%>
Нельзя удалить <%=ViewData.MetaClass.Caption.ToLower()%>, так как имеются связанные объекты:
<br /><br />
<nw:LinkedObjects runat=""server"" ID=""lo"" />
<br />
<%}%>
<%if(lo.GetLinkedObjects().Count == 0){%>
Вы уверены, что хотите удалить <%=ViewData.MetaClass.Caption%> ""<%=ViewData.Title%>""?
<br /><br />
<asp:Button CssClass=""ms-ButtonHeightWidth"" Text=""Удалить"" ID=""bDelete"" runat=""server"" OnClick=""bDelete_Click"" />
<%}%>
<nw:BackButton runat=""server"" ID=""bBackButton"" />
<script runat=""server"">
protected void Page_Load(object sender, EventArgs e)
{
    SetTitle(ViewData.Title + "" - удаление"");
    if (!ViewData.IsLogicalDelete) lo.SetObject(ViewData);
}

protected void bDelete_Click(object sender, EventArgs e)
{
	var r = new Repository();
	if (ViewData.IsLogicalDelete)
		ViewData.IsDeleted = true;
	else
		r.Delete(ViewData);
	r.SubmitChanges();

	Query.RedirectBack();
}
</script>";
		}

		public static string UnDelete(FormElement form)
		{
			return @"Вы уверены, что хотите отменить удаление ""<%=ViewData.Title%>""?
<br /><br />
<asp:Button CssClass=""ms-ButtonHeightWidth"" Text=""Продолжить"" ID=""bUndelete"" runat=""server"" OnClick=""bUndelete_Click"" />
<nw:BackButton runat=""server"" ID=""bBackButton"" />
<script runat=""server"">
protected void Page_Load(object sender, EventArgs e)
{
	SetTitle(""Отмена удаления"");
}

protected void bUndelete_Click(object sender, EventArgs e)
{
	ViewData.IsDeleted = false;
	App.DataContext.SubmitChanges();
	Query.RedirectBack();
}
</script>";
		}

	}
}