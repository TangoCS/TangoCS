<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="generatedb.ascx.cs" Inherits="Nephrite.Metamodel.View.Utils.generatedb" %>
Классы:
<asp:Button runat="server" ID="bSelectAll" OnClick="bSelectAll_Click" Text="Выбрать все" />
<asp:CheckBoxList ID="cblObjectTypes" runat="server" DataTextField="FullSysName" DataValueField="ObjectTypeID" />
<br />
<br />
<asp:CheckBox runat="server" ID="scriptOnly" Text="Генерация только скриптов" />
<asp:CheckBox runat="server" ID="identity" Text="Identity" />
<br />
<asp:Button runat="server" ID="bGenerate" OnClick="bGenerate_Click" Text="Старт" />
<br />
<br />
<pre>
<asp:Label ID="lMsg" runat="server" />
</pre>
<br />
<asp:TextBox runat="server" ID="script" TextMode="MultiLine" Width="100%" Height="600px" />