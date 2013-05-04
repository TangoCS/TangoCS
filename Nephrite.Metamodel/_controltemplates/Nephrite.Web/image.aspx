<%@ Page Language="C#" AutoEventWireup="True" CodeBehind="image.aspx.cs" Inherits="Nephrite.Web.Controls.image" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="Nephrite.Web" %>
<%@ Import Namespace="Nephrite.Web.Controls" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
	<title>{#advanced_dlg.image_title}</title>
	<script type="text/javascript" src="js/tiny_mce/themes/advanced/../../tiny_mce_popup.js"></script>
	<script type="text/javascript" src="js/tiny_mce/themes/advanced/../../utils/mctabs.js"></script>
	<script type="text/javascript" src="js/tiny_mce/themes/advanced/../../utils/form_utils.js"></script>
	<script type="text/javascript" src="js/tiny_mce/themes/advanced/js/image.js"></script>
	<base target="_self" />
</head>
<body id="image" style="display: none">
    <form id="form1" runat="server">
        <asp:ScriptManager runat="server" ID="sm" EnablePartialRendering="true" LoadScriptsBeforeUI="true" />
        <div class="tabs">
		    <ul>
			    <li id="tab2" class="current">
			        <span>
			            <a href="javascript:mcTabs.displayTab('tab2','panel2');" onmousedown="return false;">
			                Выбор изображения
			            </a>
			        </span>
			    </li>
			    <li id="general_tab">
			        <span>
			            <a href="javascript:mcTabs.displayTab('general_tab','general_panel');" onmousedown="return false;">
			                {#advanced_dlg.image_title}
			            </a>
			        </span>
			    </li>
		    </ul>
	    </div>
        <div class="panel_wrapper" style="height:500px;">
		    <div id="general_panel" class="panel">
                <input id="src" type="hidden" value="<%=srcfortext %>" />
                <table border="0" cellpadding="4" cellspacing="0" width="100%">
                  <tr>
                    <td nowrap="nowrap"><label for="src">Изображение</label></td>
                    <td>
                        <img id="imagePreview" src="<%=src %>" style="max-width:300px; max-height:200px;"/>
                    </td>
                  </tr>
		          <tr>
			        <td><label for="image_list">{#advanced_dlg.image_list}</label></td>
			        <td><select id="image_list" name="image_list" onchange="document.getElementById('src').value=this.options[this.selectedIndex].value;document.getElementById('alt').value=this.options[this.selectedIndex].text;"></select></td>
		          </tr>
                  <tr>
                    <td nowrap="nowrap"><label for="alt">{#advanced_dlg.image_alt}</label></td>
                    <td><input id="alt" name="alt" type="text" value="" style="width: 100%" /></td>
                  </tr>
                  <tr>
                    <td nowrap="nowrap"><label for="align">{#advanced_dlg.image_align}</label></td>
                    <td><select id="align" name="align" onchange="ImageDialog.updateStyle();">
                        <option value="">{#not_set}</option>
                        <option value="baseline">{#advanced_dlg.image_align_baseline}</option>
                        <option value="top">{#advanced_dlg.image_align_top}</option>
                        <option value="middle">{#advanced_dlg.image_align_middle}</option>
                        <option value="bottom">{#advanced_dlg.image_align_bottom}</option>
                        <option value="text-top">{#advanced_dlg.image_align_texttop}</option>
                        <option value="text-bottom">{#advanced_dlg.image_align_textbottom}</option>
                        <option value="left">{#advanced_dlg.image_align_left}</option>
                        <option value="right">{#advanced_dlg.image_align_right}</option>
                      </select></td>
                  </tr>
                  <tr>
                    <td nowrap="nowrap"><label for="width">{#advanced_dlg.image_dimensions}</label></td>
                    <td>
                      <input id="width" name="width" type="text" value="" size="3" maxlength="5" />
                      x
                      <input id="height" name="height" type="text" value="" size="3" maxlength="5" />
                    </td>
                  </tr>
                  <tr>
                    <td nowrap="nowrap"><label for="border">{#advanced_dlg.image_border}</label></td>
                    <td><input id="border" name="border" type="text" value="" size="3" maxlength="3" onchange="ImageDialog.updateStyle();" /></td>
                  </tr>
                  <tr>
                    <td nowrap="nowrap"><label for="lspace">Отступ слева</label></td>
                    <td><input id="lspace" name="lvspace" type="text" value="" size="3" maxlength="3" onchange="ImageDialog.updateStyle();" /></td>
                  </tr>
                  <tr>
                    <td nowrap="nowrap"><label for="rspace">Отступ справа</label></td>
                    <td><input id="rspace" name="rspace" type="text" value="" size="3" maxlength="3" onchange="ImageDialog.updateStyle();" /></td>
                  </tr>
                  <tr>
                    <td nowrap="nowrap"><label for="tspace">Отступ сверху</label></td>
                    <td><input id="tspace" name="tspace" type="text" value="" size="3" maxlength="3" onchange="ImageDialog.updateStyle();" /></td>
                  </tr>
                  <tr>
                    <td nowrap="nowrap"><label for="bspace">Отступ снизу</label></td>
                    <td><input id="bspace" name="bspace" type="text" value="" size="3" maxlength="3" onchange="ImageDialog.updateStyle();" /></td>
                  </tr>
                </table>

                <table border="0" class="mceActionPanel" width="100%">
                    <tr>
                        <td align="left">
                            <input type="submit" id="insert" name="insert" value="{#insert}" onclick="ImageDialog.update();" />
                        </td>
                        <td align="right">
                            <input type="button" id="cancel" name="cancel" value="{#cancel}" onclick="tinyMCEPopup.close();" />
                        </td>
                    </tr>
                </table>
            </div>
	        <div id="panel2" class="panel current" style="height:100%;">
	            <asp:UpdatePanel runat="server" ID="up">
	            <ContentTemplate>
	                <table border="0" width="100%" cellspacing="2" cellpadding="2">
	                    <tr>
	                        <td align="right">Файл:</td>
	                        <td style="width:99%">
	                            <asp:FileUpload ID="FileUpload1" runat="server" Width="100%" />
	                        </td>
	                    </tr>
	                    <tr>
	                        <td align="right">Название:</td>
	                        <td>
	                            <asp:TextBox ID="tbTitle" runat="server" Width="100%" />
	                        </td>
	                    </tr>
	                    <tr>
	                        <td align="right">Внешний Url:</td>
	                        <td>
	                            <input id="externalurl" name="externalurl" type="text" value="" style="width: 100%" />
	                            <input type="button" value="Выбрать" onclick="ChooseExternal();" />
	                        </td>
	                    </tr>
	                </table>
                    <asp:Button ID="bUpload" runat="server" Text="Загрузить" OnClick="bUpload_Click" CausesValidation="true" />
	                <hr />
	                <div style="width:100%; height:380px; overflow-y:scroll;">
	                    <table>
	                    <%  using (ImageDataContext ctx = new ImageDataContext())
                         {
                             HtmlHelperBase.Instance.Repeater<Nephrite.Web.Controls.Image>(ctx.Images.Where(img => img.InfoObjectID == Query.GetInt("infoobjectid", 0)).OrderBy(img => img.Title), "", "", (o, css) =>
                             { %>
                             <tr>
                                 <td valign="top"><%=o.RenderIcon(200,100) %></td>
                                 <td valign="top"><%=o.FileName %><br /><%=o.Title %><br /><br />
                                     <input type="button" value="Выбрать" onclick="ChoosePic('<%=o.Url %>')" />
                                 </td>
                             </tr>
                        <%});
                         } %>
	                    </table>
	                </div>
	            </ContentTemplate>
	            </asp:UpdatePanel>
	        </div>
	    </div>
    </form>
    <script type="text/javascript">
        function ChoosePic(url) {
            document.getElementById('imagePreview').setAttribute('src', tinyMCEPopup.editor.documentBaseURI.toAbsolute(url) + '&maxwidth=200&maxheight=200');
            document.forms[0].src.value = url;
            document.forms[0].externalurl.value = '';
            mcTabs.displayTab('general_tab', 'general_panel');
        }
        function ChooseExternal() {
            document.getElementById('imagePreview').setAttribute('src', document.forms[0].externalurl.value);
            document.forms[0].src.value = document.forms[0].externalurl.value;
            mcTabs.displayTab('general_tab', 'general_panel');
        }
    </script>
</body>
</html>
