<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ModalDialog.ascx.cs"
	Inherits="Nephrite.Web.Controls.ModalDialog" %>
<%@ Import Namespace="Nephrite.Web" %>

<script type="text/javascript">
function hide<%=ClientID %>()
{
    <%=OnClientHide %>
    document.getElementById('<%=hfVisible.ClientID %>').value = '0';
    document.getElementById('<%=ClientID %>').style.visibility='hidden';
	mdm_hidemodalpopup();
	document.getElementById('<%=btnOK.ClientID %>').disabled = true;
    document.getElementById('<%=btnCancel.ClientID %>').disabled = true;
	defaultButtonId = '';
}

function loaded<%=ClientID %>()
{
    document.getElementById('<%=ClientID %>').style.left = (document.body.offsetWidth - parseInt(document.getElementById('<%=ClientID %>').style.width)) / 2 + 'px';
    var i = document.getElementById('loadingImage');
    if (i != null) i.style.visibility = 'hidden';
    document.getElementById('<%=ClientID %>').style.zIndex = mdm_getzindex();
    mdm_enable();
    document.getElementById('<%=btnOK.ClientID %>').disabled = false;
    document.getElementById('<%=btnCancel.ClientID %>').disabled = false;
	if (document.getElementById('<%=ClientID %>').style.visibility != 'visible')
	{
		document.getElementById('<%=ClientID %>').style.visibility = 'visible';
		document.getElementById('<%=btnOK.ClientID %>').focus();
	}
}

function disable<%=ClientID %>()
{
    if (document.getElementById('<%=hfDisableOnSubmit.ClientID %>').value == '1')
    {
        document.getElementById('<%=ClientID %>').style.zIndex = mdm_getzindex()-2;
        document.getElementById('loadingImage').style.visibility = 'visible';
        mdm_disable();
    }
}

function refresh<%=ClientID %>()
{
    <%=postBackFunc() %>;
}

function show<%=ClientID %>Arg(arg, mode)
{
    mdm_showmodalpopup();
    document.getElementById('<%=hfPageIndex.ClientID%>').value = '1';
    document.getElementById('<%=hfVisible.ClientID%>').value = '1';
    document.getElementById('<%=hfFirstPopulate.ClientID%>').value = '1';
    document.getElementById('<%=hfArgument.ClientID%>').value = arg;
    document.getElementById('<%=hfMode.ClientID%>').value = mode;
    refresh<%=ClientID %>();
}

function reopen<%=ClientID %>()
{
	mdm_showmodalpopup();
	document.getElementById('<%=hfFirstPopulate.ClientID%>').value = '0';
	document.getElementById('<%=hfVisible.ClientID%>').value = '1';
	refresh<%=ClientID %>();
}

function show<%=ClientID %>(arg)
{
    show<%=ClientID %>Arg(arg, '');
}

function show<%=ClientID %>()
{
    show<%=ClientID %>Arg('', '');
}
function setPageIndex<%=ClientID %>(pageIndex)
{
    document.getElementById('<%=hfPageIndex.ClientID%>').value = pageIndex;
}
function skipDisable<%=ClientID %>()
{
    document.getElementById('<%=hfDisableOnSubmit.ClientID %>').value = '0';
}
</script>

<%=Layout.ModalBegin(ClientID, Width, Top) %>

<%=Layout.ModalHeaderBegin() %>
<%=Title %>
<%=Layout.ModalHeaderEnd() %>

<%=Layout.ModalBodyBegin() %>
	<asp:PlaceHolder ID="phnonajaxContent" runat="server" />
	<asp:UpdatePanel ID="up" runat="server" UpdateMode="Conditional" RenderMode="Inline">
	<ContentTemplate>
		<asp:HiddenField ID="hfPageIndex" runat="server" />
		<asp:HiddenField ID="hfVisible" runat="server" />
		<asp:HiddenField ID="hfFirstPopulate" runat="server" />
		<asp:HiddenField ID="hfArgument" runat="server" />
		<asp:HiddenField ID="hfMode" runat="server" />
		<asp:HiddenField ID="hfOKClick" runat="server" />
		<asp:HiddenField ID="hfDisableOnSubmit" runat="server" Value="1" />
		<asp:PlaceHolder ID="phContent" runat="server" />
	</ContentTemplate>
	<Triggers>
		<asp:AsyncPostBackTrigger ControlID="lbRun" />
	</Triggers>
	</asp:UpdatePanel>
<%=Layout.ModalBodyEnd() %>

<%=Layout.ModalFooterBegin() %>
<%=Layout.ModalFooterLeftBegin() %>
    <asp:PlaceHolder ID="phBottomLeft" runat="server" />
<%=Layout.ModalFooterLeftEnd() %>
<%=Layout.ModalFooterRightBegin() %>
    <asp:Button ID="btnOK" runat="server" CssClass="ms-ButtonHeightWidth" />
    <asp:Button ID="btnCancel" runat="server" CssClass="ms-ButtonHeightWidth" />
<%=Layout.ModalFooterRightEnd() %>
<%=Layout.ModalFooterEnd() %>

<%=Layout.ModalEnd() %>

<asp:LinkButton ID="lbRun" runat="server" />
