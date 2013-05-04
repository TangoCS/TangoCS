<%@ Control Language="C#" AutoEventWireup="true" Inherits="View_ErrorLog_view" Codebehind="view.ascx.cs" %>
<%@ Import Namespace="System.Linq" %>

<%=HtmlHelperWSS.FormToolbarBegin("100%")%>
<%=HtmlHelperWSS.FormToolbarWhiteSpace() %>
<%=HtmlHelperWSS.FormToolbarItemBegin() %>
<nw:BackButton runat="server" ID="BackButton" />
<%=HtmlHelperWSS.FormToolbarItemEnd() %>
<%=HtmlHelperWSS.FormToolbarEnd() %>

<%=Layout.FormTableBegin(new { style = "table-layout:fixed;width:100%;overflow:hidden;word-wrap:break-word;" })%>
<%=Layout.FormRowBegin("Дата и время возниконовения ошибки")%>
<%=tv(ViewData.ErrorDate.ToString("dd.MM.yyyy HH:mm")) %>
<%=Layout.FormRowEnd()%>

<%=Layout.FormRowBegin("URL запроса")%>
<%=tv(ViewData.Url) %>
<%=Layout.FormRowEnd()%>

<%=Layout.FormRowBegin("URL, с которого перешли") %>
<%=tv(enc(ViewData.UrlReferrer))%>
<%=Layout.FormRowEnd()%>
<%=Layout.FormRowBegin("Пользователь") %>
<%=tv(enc(ViewData.UserName))%>
<%=Layout.FormRowEnd()%>

<%=Layout.FormRowBegin("Текст ошибки")%>
<%=tv(enc(ViewData.ErrorText).Replace("\n", "<br />"))%>
<%=Layout.FormRowEnd()%>

<%=Layout.FormRowBegin("Адрес хоста")%>
<%=tv(enc(ViewData.UserHostAddress))%>
<%=Layout.FormRowEnd()%>
<%=Layout.FormRowBegin("Имя хоста")%>
<%=tv(enc(ViewData.UserHostName))%>
<%=Layout.FormRowEnd()%>

<%=Layout.FormRowBegin("Клиент")%>
<%=tv(enc(ViewData.UserAgent))%>
<%=Layout.FormRowEnd()%>
<%=Layout.FormRowBegin("Тип запроса")%>
<%=tv(enc(ViewData.RequestType))%>
<%=Layout.FormRowEnd()%>
<%=Layout.FormRowBegin("Заголовки запроса")%>
<%=tv(enc(ViewData.Headers).Replace("\n", "<br />"))%>
<%=Layout.FormRowEnd()%>
<%=Layout.FormRowBegin("Протокол SQL")%>
<%=tv(enc(ViewData.SqlLog).Replace("\n", "<br />"))%>
<%=Layout.FormRowEnd()%>
<%=Layout.FormTableEnd()%>

<%=HtmlHelperWSS.FormToolbarBegin("100%") %>
<%=HtmlHelperWSS.FormToolbarWhiteSpace() %>
<%=HtmlHelperWSS.FormToolbarItemBegin() %>
<nw:BackButton runat="server" ID="BackButton1" />
<%=HtmlHelperWSS.FormToolbarItemEnd() %>
<%=HtmlHelperWSS.FormToolbarEnd() %>

<script runat="server">
protected void Page_Load(object sender, EventArgs e)
{
	SetTitle("Просмотр ошибки системы");
}
</script>