using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nephrite.Metamodel.Model;

namespace Nephrite.Metamodel
{
	public partial class ViewCode
	{
		public static string Delete(MM_ObjectType objectType)
		{
			return @"<%if(lo.GetLinkedObjects().Count > 0){%>
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
    SetTitle(ViewData.MetaClass.Caption + "" - удаление"");
    if (!ViewData.IsLogicalDelete) lo.SetObject(ViewData);
}

protected void bDelete_Click(object sender, EventArgs e)
{
	var r = new Nephrite.Metamodel.Repository();
	if (ViewData.IsLogicalDelete)
		ViewData.IsDeleted = true;
	else
		r.Delete(ViewData);
	r.SubmitChanges();

	Query.RedirectBack();
}
</script>";
		}

		public static string UnDelete(MM_ObjectType objectType)
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
	(new " + objectType.SysName + @"Controller()).Update(ViewData);
	Query.RedirectBack();
}
</script>";
		}
	}
}
