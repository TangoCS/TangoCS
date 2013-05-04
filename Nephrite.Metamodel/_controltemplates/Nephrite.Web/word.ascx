<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="word.ascx.cs" Inherits="Nephrite.Web.Controls.word" %>
<html xmlns:o='urn:schemas-microsoft-com:office:office' xmlns:w='urn:schemas-microsoft-com:office:word' xmlns='http://www.w3.org/TR/REC-html40'>
<head>
<meta http-equiv="content-type" content="text/html; charset=UTF-8">
<title><asp:PlaceHolder ID="Title" runat="server" /></title>
<!--[if gte mso 9]><xml>
 <w:WordDocument>
  <w:View>Print</w:View>
  <w:Zoom>100</w:Zoom>
 </w:WordDocument>
</xml><![endif]-->
<style>
<!--
@page Section1
	{<asp:PlaceHolder ID="Section1" runat="server" />}
div.Section1
	{page:Section1;}
-->
table
{
	font-size:14pt;
}
.bordered
{
	 border:solid 1px black;
	 padding:0.1cm;
}
</style>

<body lang="RU" style='tab-interval:.5in'>
<div class="Section1" style='font-size:14.0pt;mso-fareast-font-family:"Times New Roman"'>
<asp:PlaceHolder ID="phMain" runat="server" />
</div>
</body>
</html>