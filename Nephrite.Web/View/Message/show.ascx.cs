using System;
using Nephrite.Web;
using System.Linq;
using Nephrite.Web.Controllers;

public partial class Message_show : ViewControl<MessageViewData>
{
    protected void Page_Load(object sender, EventArgs e)
    {
        SetTitle(ViewData.Title);
    }
}
