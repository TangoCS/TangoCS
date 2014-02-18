using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Nephrite.Web.Controls.Scripts
{

    public class SPM2Scripts
    {
        private ISPM2Script _ispm2Script;
        public SPM2Scripts()
        {
            _ispm2Script = ConfigurationManager.AppSettings["DBType"].ToUpper() == "DB2"
                                ? (ISPM2Script)new SPM2ScriptDB2()
                                : new SPM2ScriptMSSQL();
        }

        private ISPM2Script Script
        {
            get { return _ispm2Script; }
        }
        public string GetFromLoginScript
        {
            get { return Script.FromLogin; }
        }

        public string GetFromSIDScript
        {
            get { return Script.FromSID; }
        }
        public string GetFromIDScript
        {
            get { return Script.FromID; }
        }
        public string GetFromEmailScript
        {
            get { return Script.FromEmail; }
        }

        public string GetRolesAccessByIdQueryScript
        {
            get { return Script.GetRolesAccessByIdQuery; }
        }
        public string GetRolesAccessByNameQueryScript
        {
            get { return Script.GetRolesAccessByNameQuery; }
        }
        public string GetItemsIdsQueryScript
        {
            get { return Script.GetItemsIdsQuery; }
        }
        public string GetItemsNamesQuery
        {
            get { return Script.GetItemsNamesQuery; }
        }
 
    }
}