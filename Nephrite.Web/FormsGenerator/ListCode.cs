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
		public static string List(FormElement form)
		{
			StringBuilder res = new StringBuilder();

			//res.Append(@"<%@ Control Language=""C#"" AutoEventWireup=""true"" Inherits=""ViewControl<IQueryable<").Append(objectType.SysName).AppendLine(@">>"" %>");
			res.AppendLine(@"<nw:Filter ID=""filter"" runat=""server"" Width=""600px"" />");
			res.AppendLine(@"<nw:QuickFilter ID=""qfilter"" runat=""server"" />");

			res.AppendLine(@"<asp:UpdatePanel runat=""server"" ID=""up"" UpdateMode=""Conditional"">");
			res.AppendLine("<ContentTemplate>");
			res.AppendLine("<%=Layout.ListTableBegin() %>");
			res.AppendLine("<%=Layout.ListHeaderBegin() %>");

			var cols = form.Content;

			if (cols.Count() == 0)
			{
				return "ERROR: Нет ни одного поля, отображаемого в списке";
			}

			foreach (var p in cols)
			{
				res.Append("<%=Layout.TH(AddSortColumn<").
					Append(form.ViewDataClass).
					Append(",").
					Append(p.Type.CLRType).
					Append(@">(""").
					Append(p.Caption).
					Append(@""", o => o.").
					Append(p.Type is MetaClass ? p.Name + ".Title" : p.Name).
					AppendLine("))%>");
			}
			res.AppendLine(@"<%=Layout.TH(""Действия"")%>");
			res.AppendLine("<%=Layout.ListHeaderEnd() %>");
			res.AppendLine(@"<% Html.Repeater(ApplyPaging(ApplyOrderBy(filter.ApplyFilter(qfilter.ApplyFilter(ViewData, SearchExpression)))), """", HtmlHelperWSS.CSSClassAlternating, (o, css) => {  %>");
			res.AppendLine("<%=Layout.ListRowBegin(o.IsDeleted ? \"deletedItem\": css) %>");

			var idProp = (form.Type as MetaClass).Key;

			var linkColumn = cols.FirstOrDefault(o => o.Type is MetaStringType);
			string linkCol = linkColumn == null ? cols.First().Name : linkColumn.Name;
			foreach (var p in cols)
			{
				res.Append("<%=Layout.TD(").Append(GetCellValue(form.Type as MetaClass, p, idProp, linkCol)).AppendLine(")%>");
			}

			res.AppendLine(@"<%=Layout.TDBegin(new { style = ""text-align:center""})%>");

			if (idProp != null)
			{
				res.AppendLine("<% if (!o.IsDeleted) { %>");
				res.AppendLine(@"<%=Html.ActionImage<" + form.Type.Name + "Controller>(oc => oc.Delete(o." + idProp.Name + @", Query.CreateReturnUrl()), ""Удалить"", ""delete.gif"")%>");
				res.AppendLine("<% } else { %>");
				res.AppendLine(@"<%=Html.ActionImage<" + form.Type.Name + "Controller>(oc => oc.UnDelete(o." + idProp.Name + @", Query.CreateReturnUrl()), ""Отменить удаление"", ""undelete.gif"")%>");
				res.AppendLine("<% }%>");
			}
			res.AppendLine("<%=Layout.TDEnd()%>");

			res.AppendLine("<%=Layout.ListRowEnd() %>");
			res.AppendLine("<%}); %>");
			res.AppendLine("<%=Layout.ListTableEnd() %>");
			res.AppendLine("<%=RenderPager(PageCount) %>");
			res.AppendLine("");
			res.AppendLine("</ContentTemplate>");
			res.AppendLine("<Triggers>");
			res.AppendLine(@"	<asp:AsyncPostBackTrigger ControlID=""qfilter"" />");
			res.AppendLine("</Triggers>");
			res.AppendLine("</asp:UpdatePanel>");

			res.AppendLine("");
			res.AppendLine(@"<script runat=""server"">");
			res.AppendLine("protected void Page_Load(object sender, EventArgs e)");
			res.AppendLine("{");
			res.AppendLine("\tSetTitle(\"" + form.Caption + "\");");
			res.AppendLine("");

			res.AppendLine("\tvar ph = HttpContext.Current.Items[\"Toolbar\"] as PlaceHolder;");
			res.AppendLine("\tToolbar toolbar = new Toolbar();");
			res.AppendLine("\tph.Controls.Add(toolbar);");

			res.AppendLine("\ttoolbar.AddItemFilter(filter);");
			res.AppendLine("\ttoolbar.AddItemSeparator();");

			res.AppendLine("\ttoolbar.AddItem<" + form.Type.Name + @"Controller>(""add.png"", ""Создать"", c => c.CreateNew(Query.CreateReturnUrl()));");

			res.AppendLine("");
			foreach (var p in cols)
			{
				res.AppendLine("\t" + GetFilterCode(form.ViewDataClass, p));
			}
			res.AppendLine("\t" + "filter.AddFieldBoolean<" + form.ViewDataClass + ">(\"Удален\", o => o.IsDeleted);");


			res.AppendLine("\ttoolbar.AddRightItemQuickFilter(qfilter);");

			res.AppendLine("");
			res.AppendLine("\ttoolbar.AddRightItemSeparator();");
			res.AppendLine("\ttoolbar.EnableViews(filter);");
			
			res.AppendLine("");
			res.Append("\tSearchExpression = s => (o =>");

			List<string> s = new List<string>();
			foreach (var p in cols)
			{
				if (p.Type is IMetaNumericType)
					s.Add(@"SqlMethods.Like(o." + p.Name + @".ToString(), ""%"" + s + ""%"")");
				else if (p.Type is MetaClass)
					s.Add(@"SqlMethods.Like(o." + p.Name + @".Title, ""%"" + s + ""%"")");
				else
					s.Add(@"SqlMethods.Like(o." + p.Name + @", ""%"" + s + ""%"")");
			}
			res.Append(s.Join(" || ")).AppendLine(");");

			res.AppendLine("}");
			res.AppendLine("protected Func<string, Expression<Func<" + form.ViewDataClass + ", bool>>> SearchExpression { get; set; }");
			res.AppendLine("</script>");

			return res.ToString();
		}

		public static string GetCellValue(MetaClass t, FormElement p, MetaProperty idProp, string linkCol)
		{
			return GetCellValue(t, p, idProp, linkCol, false, "");
		}
		public static string GetCellValue(MetaClass t, FormElement p, MetaProperty idProp, string linkCol, bool internalLink, string modal)
		{
			if (p.Type is MetaGuidType || p.Type is MetaStringType)
			{
				if (p.Name == linkCol && idProp != null)
				{
					if (internalLink)
					{
						return "Html.InternalLink(" + modal + ".RenderRun(o." + idProp.Name + "), enc(o." + p.Name + "), true)";
					}
					else
					{
						var m = t.DefaultOperation;
						return "Html.ActionLink<" + t.Name + "Controller>(c => c." + (m != null ? m.Name : "") + "(o." + idProp.Name + ", Query.CreateReturnUrl()), enc(o." + p.Name + "))";
					}
				}
				else
					return "enc(o." + p.Name + ")";
			}
			else if (p.Type is MetaDateTimeType)
			{
				return "o." + p.Name + ".DateTimeToString()";
			}
			else if (p.Type is MetaDateType)
			{
				return "o." + p.Name + ".DateToString()";
			}
			else if (p.Type is IMetaNumericType)
			{
				if (p.Name == linkCol && idProp != null)
				{
					if (internalLink)
					{
						return "Html.InternalLink(" + modal + ".RenderRun(o." + idProp.Name + "), enc(o." + p.Name + ".ToString()), true)";
					}
					else
					{
						var m = t.DefaultOperation;
						return "Html.ActionLink<" + t.Name + "Controller>(c => c." + (m != null ? m.Name : "") +
							"(o." + idProp.Name + ", Query.CreateReturnUrl()), enc(o." + p.Name + ".ToString()))";
					}
				}
				else
				{
					if (!p.IsRequired)
						return "o." + p.Name + ".HasValue ? o." + p.Name + ".Value.ToString() : \"\"";
					else
						return "o." + p.Name + ".ToString()";
				}
			}
			else if (p.Type is MetaBooleanType)
			{
				return "o." + p.Name + ".Icon()";
			}
			else if (p.Type is MetaFileType)
			{
				return @"String.Format(""<a href='/file.ashx?oid={0}'>{1}</a>"", o." + p.Name + ".FileID.ToString(), o." + p.Name + ".Title)";
			}
			else if (p.Type is MetaClass)
			{
				if (p.IsRequired)
				{
					if (p.Name == linkCol && idProp != null)
					{
						if (internalLink)
						{
							return "Html.InternalLink(" + modal + ".RenderRun(o." + idProp.Name + "), enc(o." + p.Name + ".Title), true)";
						}
						else
						{
							var m = t.DefaultOperation;
							return "Html.ActionLink<" + t.Name + "Controller>(c => c." + (m != null ? m.Name : "") + "(o." + idProp.Name + ", Query.CreateReturnUrl()), enc(o." + p.Name + ".Title))";
						}
					}
					else
						return "enc(o." + p.Name + ".Title)";
				}
				else
					return "enc(o." + p.Name + @" == null ? """" : o." + p.Name + ".Title)";
			}
			else
				return "";
		}

		public static string GetFilterCode(string t, FormElement p)
		{
			string fname = p.Caption;
			if (p.Type is MetaGuidType || p.Type is MetaStringType)
				return "filter.AddFieldString<" + t + ">(\"" + fname + "\", o => o." + p.Name + ");";
			else if (p.Type is MetaDateTimeType)
				return "filter.AddFieldDate<" + t + ">(\"" + fname + "\", o => o." + p.Name + ", false);";
			else if (p.Type is IMetaNumericType)
				return "filter.AddFieldNumber<" + t + ">(\"" + fname + "\", o => o." + p.Name + ");";
			else if (p.Type is MetaBooleanType)
				return "filter.AddFieldBoolean<" + t + ">(\"" + fname + "\", o => o." + p.Name + ");";
			else if (p.Type is MetaClass)
				return "filter.AddFieldString<" + t + ">(\"" + fname + "\", o => o." + p.Name + ".Title);";
			else
				return "";
			}

	}
	
	
}