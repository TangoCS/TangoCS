<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="upload.ascx.cs" Inherits="Nephrite.Web.View.TinyMCEFileManager_upload" %>


<%=Layout.FormTableBegin(new { style = "widthL:700px" })%>
<%=Layout.FormRowBegin("Файл 1")%>
<asp:FileUpload ID="fuFile1" runat="server" Width="100%" />
<%=Layout.FormRowEnd()%>
<%=Layout.FormRowBegin("Файл 2")%>
<asp:FileUpload ID="fuFile2" runat="server" Width="100%" />
<%=Layout.FormRowEnd()%>
<%=Layout.FormRowBegin("Файл 3")%>
<asp:FileUpload ID="fuFile3" runat="server" Width="100%" />
<%=Layout.FormRowEnd()%>
<%=Layout.FormRowBegin("Файл 4")%>
<asp:FileUpload ID="fuFile4" runat="server" Width="100%" />
<%=Layout.FormRowEnd()%>
<%=Layout.FormRowBegin("Файл 5")%>
<asp:FileUpload ID="fuFile5" runat="server" Width="100%" />
<%=Layout.FormRowEnd()%>
<%=Layout.FormRowBegin("Файл 6")%>
<asp:FileUpload ID="fuFile6" runat="server" Width="100%" />
<%=Layout.FormRowEnd()%>
<%=Layout.FormTableEnd()%>

<p>
<asp:Label ID="lInfo" runat="server" Text="" />
</p>
<p>
<asp:Label ID="lMsg" runat="server" Text="" ForeColor="Red" />
</p>

<%=HtmlHelperWSS.FormToolbarBegin() %>
<%=HtmlHelperWSS.FormToolbarWhiteSpace() %>
<%=HtmlHelperWSS.FormToolbarItemBegin() %>
<asp:Button ID="bOK" runat="server" Text="ОК" onclick="bOK_Click" CssClass="ms-ButtonHeightWidth" />
<%=HtmlHelperWSS.FormToolbarItemEnd() %>
<%=HtmlHelperWSS.FormToolbarItemBegin() %>
<nw:BackButton runat="server" ID="BackButton" />
<%=HtmlHelperWSS.FormToolbarItemEnd() %>
<%=HtmlHelperWSS.FormToolbarEnd() %>

