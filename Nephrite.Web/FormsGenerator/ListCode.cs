using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Nephrite.Meta;

namespace Nephrite.Web.FormsGenerator
{
	/*
	public partial class ViewCode
	{
		static string ClassName(MetaClass objectType)
		{
			if (objectType.IsMultiLingual)
			{
				if (objectType.HistoryTypeCode == HistoryType.IdentifiersMiss ||
					objectType.HistoryTypeCode == HistoryType.IdentifiersRetain)
					return "V_HST_" + objectType.SysName;
				else
					return "V_" + objectType.SysName;
			}
			else
			{
				if (objectType.HistoryTypeCode == HistoryType.IdentifiersMiss ||
					objectType.HistoryTypeCode == HistoryType.IdentifiersRetain)
					return "HST_" + objectType.SysName;
				else
					return objectType.SysName;
			}
		}

		public static string List(MM_ObjectType objectType)
		{
			MM_ObjectProperty idProp = objectType.MM_ObjectProperties.SingleOrDefault(o => o.IsPrimaryKey);

			StringBuilder res = new StringBuilder();

			//res.Append(@"<%@ Control Language=""C#"" AutoEventWireup=""true"" Inherits=""ViewControl<IQueryable<").Append(objectType.SysName).AppendLine(@">>"" %>");
			res.AppendLine(@"<nw:Filter ID=""filter"" runat=""server"" Width=""600px"" />");
			res.AppendLine(@"<nw:QuickFilter ID=""qfilter"" runat=""server"" />");

			res.AppendLine(@"<asp:UpdatePanel runat=""server"" ID=""up"" UpdateMode=""Conditional"">");
			res.AppendLine("<ContentTemplate>");
			res.AppendLine("<%=Layout.ListTableBegin() %>");
			res.AppendLine("<%=Layout.ListHeaderBegin() %>");

			var cols = from f in AppMM.DataContext.MM_FormFields
					   where f.MM_ObjectProperty.ObjectTypeID == objectType.ObjectTypeID
					   && f.ShowInList
					   orderby f.MM_ObjectProperty.SeqNo
					   select f.MM_ObjectProperty;

			if (cols.Count() == 0)
			{
				return "ERROR: Нет ни одного поля, отображаемого в списке";
			}

			foreach (MM_ObjectProperty p in cols)
			{
				if (!String.IsNullOrEmpty(p.Expression))
					if (String.IsNullOrEmpty(p.MM_FormField.SortExpression))
						res.Append("<%=Layout.TH(\"" + p.Title + "\")%>");
					else
						res.Append("<%=Layout.TH(AddSortColumn<").
						Append(ClassName(objectType)).
						Append(",").
						Append(CodeGenHelper.GetCSharpType(p.TypeCode, p.LowerBound)).
						Append(@">(""").
						Append(p.Title).
						Append(@""", " + p.MM_FormField.SortExpression + "))%>");
				else
					res.Append("<%=Layout.TH(AddSortColumn<").
						Append(ClassName(objectType)).
						Append(",").
						Append(CodeGenHelper.GetCSharpType(p.TypeCode, p.LowerBound)).
						Append(@">(""").
						Append(p.Title).
						Append(@""", o => o.").
						Append(p.TypeCode == ObjectPropertyType.Object ? p.SysName + ".Title" : p.SysName).
						AppendLine("))%>");
			}
			res.AppendLine(@"<%=Layout.TH(""Действия"")%>");
			res.AppendLine("<%=Layout.ListHeaderEnd() %>");
			res.AppendLine(@"<% Html.Repeater(ApplyPaging(ApplyOrderBy(filter.ApplyFilter(qfilter.ApplyFilter(ViewData, SearchExpression)))), """", HtmlHelperWSS.CSSClassAlternating, (o, css) => {  %>");
			res.AppendLine("<%=Layout.ListRowBegin(o.IsDeleted ? \"deletedItem\": css) %>");

			var linkColumn = cols.FirstOrDefault(o => o.TypeCode == ObjectPropertyType.String);
			string linkCol = linkColumn == null ? cols.First().SysName : linkColumn.SysName;
			foreach (MM_ObjectProperty p in cols)
			{
				res.Append("<%=Layout.TD(").Append(CodeGenHelper.GetCellValue(objectType, p, idProp, linkCol)).AppendLine(")%>");
			}

			res.AppendLine(@"<%=Layout.TDBegin(new { style = ""text-align:center""})%>");

			if (idProp != null)
			{
				res.AppendLine("<% if (!o.IsDeleted) { %>");
				res.AppendLine(@"<%=Html.ActionImage<" + objectType.SysName + "Controller>(oc => oc.Delete(o." + idProp.SysName + @", Query.CreateReturnUrl()), ""Удалить"", ""delete.gif"")%>");
				res.AppendLine("<% } else { %>");
				res.AppendLine(@"<%=Html.ActionImage<" + objectType.SysName + "Controller>(oc => oc.UnDelete(o." + idProp.SysName + @", Query.CreateReturnUrl()), ""Отменить удаление"", ""undelete.gif"")%>");
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
			res.AppendLine("\tSetTitle(\"" + objectType.TitlePlural + "\");");
			res.AppendLine("");

			res.AppendLine("\tvar ph = HttpContext.Current.Items[\"Toolbar\"] as PlaceHolder;");
			res.AppendLine("\tToolbar toolbar = new Toolbar();");
			res.AppendLine("\tph.Controls.Add(toolbar);");

			res.AppendLine("\ttoolbar.AddItemFilter(filter);");
			res.AppendLine("\ttoolbar.AddItemSeparator();");

			res.AppendLine("\ttoolbar.AddItem<" + objectType.SysName + @"Controller>(""add.png"", ""Создать"", c => c.CreateNew(Query.CreateReturnUrl()));");

			res.AppendLine("");
			foreach (MM_ObjectProperty p in cols)
			{
				res.AppendLine("\t" + CodeGenHelper.GetFilterCode(ClassName(objectType), p));
			}
			res.AppendLine("\t" + "filter.AddFieldBoolean<" + ClassName(objectType) + ">(\"Удален\", o => o.IsDeleted);");


			//res.AppendLine("\ttoolbar.AddRightItemText(search);");
			res.AppendLine("\ttoolbar.AddRightItemQuickFilter(qfilter);");

			if (objectType.IsEnableUserViews)
			{
				res.AppendLine("");
				res.AppendLine("\ttoolbar.AddRightItemSeparator();");
				res.AppendLine("\ttoolbar.EnableViews(filter);");
			}
			res.AppendLine("");
			res.Append("\tSearchExpression = s => (o =>");

			string[] s = new string[cols.Where(o => (o.Expression == null || o.Expression == "") && (o.TypeCode == ObjectPropertyType.String ||
				o.TypeCode == ObjectPropertyType.Number || o.TypeCode == ObjectPropertyType.Decimal ||
				(o.TypeCode == ObjectPropertyType.Object && o.UpperBound == 1))).Count()];
			int i = 0;
			foreach (MM_ObjectProperty p in cols.Where(o => (o.Expression == null || o.Expression == "") && (o.TypeCode == ObjectPropertyType.String ||
				o.TypeCode == ObjectPropertyType.Number || o.TypeCode == ObjectPropertyType.Decimal ||
				(o.TypeCode == ObjectPropertyType.Object && o.UpperBound == 1))))
			{
				if (p.TypeCode == ObjectPropertyType.Number || p.TypeCode == ObjectPropertyType.Decimal)
					s[i] = @"SqlMethods.Like(o." + p.SysName + @".ToString(), ""%"" + s + ""%"")";
				else if (p.TypeCode == ObjectPropertyType.Object)
					s[i] = @"SqlMethods.Like(o." + p.SysName + @".Title, ""%"" + s + ""%"")";
				else
					s[i] = @"SqlMethods.Like(o." + p.SysName + @", ""%"" + s + ""%"")";

				i++;
			}
			res.Append(s.Join(" || ")).AppendLine(");");

			res.AppendLine("}");
			res.AppendLine("protected Func<string, Expression<Func<" + ClassName(objectType) + ", bool>>> SearchExpression { get; set; }");
			res.AppendLine("</script>");

			return res.ToString();
		}
	}
	*/
}