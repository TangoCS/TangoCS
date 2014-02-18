using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nephrite.Web.Controls.Scripts
{
    public interface ISPM2Script
    {
        string FromLogin { get; }
        string FromSID { get; }
        string FromID { get; }
        string FromEmail { get; }
        string GetRolesAccessByIdQuery { get; }
        string GetRolesAccessByNameQuery { get; }
        string GetItemsIdsQuery { get; }
        string GetItemsNamesQuery { get; }
    }
}