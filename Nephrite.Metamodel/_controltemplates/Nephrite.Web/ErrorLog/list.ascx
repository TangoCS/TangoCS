 <%@ Control Language="C#" AutoEventWireup="true" Inherits="View_ErrorLog_list" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="Nephrite.Web" %>
<%@ Import Namespace="Nephrite.Web.ErrorLogging" %>
<%@ Import Namespace="Nephrite.Web.Controls" %>
<%@ Import Namespace="System.Linq.Expressions" %>
<%@ Import Namespace="System.Data.Linq.SqlClient" %>

<nw:Filter ID="filter" runat="server" Width="600px" />
<nw:QuickFilter ID="qfilter" runat="server" />

<asp:UpdatePanel runat="server" ID="up" UpdateMode="Conditional">
<ContentTemplate>
<%=Layout.ListTableBegin() %>
<%=Layout.ListHeaderBegin() %>
	<%=Layout.TH(AddSortColumn<ErrorLog, DateTime>("Дата ошибки", m => m.ErrorDate)) %>
	<%=Layout.TH("Текст") %>
	<%=Layout.TH("URL")%>
	<%=Layout.TH("Пользователь")%>
<%=Layout.ListHeaderEnd() %>

<% Html.Repeater(ApplyPaging(ApplyOrderBy(filter.ApplyFilter(qfilter.ApplyFilter(ViewData.OrderByDescending(l => l.ErrorDate), SearchExpression)))).
	   Select(o => new { o.ErrorLogID, o.ErrorText, o.ErrorDate, o.Url, o.UserHostAddress, o.UserHostName, o.UserName }),
	   "", "ms-alternating", (o, css) => {  %>
<%=Layout.ListRowBegin(css) %>
	<%=Layout.TD(Html.ActionLink<ErrorLogController>(c => c.View(o.ErrorLogID, Query.CreateReturnUrl()), o.ErrorDate.ToString("dd.MM.yyyy HH:mm:ss")))%>
	<%=Layout.TD(o.ErrorText.Length > 100 ? o.ErrorText.Substring(0,100) : o.ErrorText) %>
	<%=Layout.TD(o.Url != null ? (o.Url.Length > 100 ? o.Url.Substring(0, 100) : o.Url) : "")%>
	<%=Layout.TD(o.UserName + "<br/>" + o.UserHostAddress)%>
<%=Layout.ListRowEnd()%>
<%}); %>
<%=Layout.ListTableEnd() %>
<%=RenderPager(PageCount)%>
</ContentTemplate>
<Triggers>
	<asp:AsyncPostBackTrigger ControlID="qfilter" />
</Triggers>
</asp:UpdatePanel>

<script runat="server">
protected void Page_Load(object sender, EventArgs e)
{
	var ph = HttpContext.Current.Items["Toolbar"] as PlaceHolder;
	Toolbar toolbar = new Toolbar();
	toolbar.TableCssClass = "ms-menutoolbar ";
	ph.Controls.Add(toolbar);

	SetTitle("Журнал ошибок системы");
	
	filter.AddFieldDate<ErrorLog>("Дата ошибки", m => m.ErrorDate.Date, false);
	filter.AddFieldString<ErrorLog>("URL запроса", m => m.Url);
	filter.AddFieldString<ErrorLog>("URL, с которого перешли", m => m.UrlReferrer);
	filter.AddFieldString<ErrorLog>("Текст ошибки", m => m.ErrorText);
	filter.AddFieldString<ErrorLog>("Пользователь", m => m.UserName);
	filter.AddFieldString<ErrorLog>("Адрес хоста", m => m.UserHostAddress);
	filter.AddFieldString<ErrorLog>("Имя хоста", m => m.UserHostName);
	filter.AddFieldString<ErrorLog>("Клиент", m => m.UserAgent);
	filter.AddFieldString<ErrorLog>("Тип запроса", m => m.RequestType);
	filter.AddFieldString<ErrorLog>("Заголовки запроса", m => m.Headers);
	filter.AddFieldString<ErrorLog>("Протокол SQL", m => m.SqlLog);
	
	toolbar.AddItemFilter(filter);
	toolbar.AddItemSeparator();
	toolbar.AddItem<ErrorLogController>("delete.gif", "Удалить", c => c.Delete(Query.CreateReturnUrl()));
	toolbar.AddRightItemQuickFilter(qfilter);
	
	SearchExpression = s => (o => SqlMethods.Like(o.UserName, "%" + s + "%") || SqlMethods.Like(o.UserHostAddress, "%" + s + "%") || SqlMethods.Like(o.UserHostName, "%" + s + "%"));
}
protected Func<string, Expression<Func<ErrorLog, bool>>> SearchExpression { get; set; }
</script>