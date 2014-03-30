using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using IBM.Data.DB2;
using Nephrite.Meta;
using Nephrite.Web.Hibernate;

namespace Nephrite.Web
{
    public class A
    {
        [ThreadStatic]
        static IDataContext model = null;
        public static IDataContext Model
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    return HttpContext.Current.Items["Base.Model"] as IDataContext;
                }
                else
                {
                    return model;
                }
            }
            set
            {
                if (HttpContext.Current != null)
                {
                    HttpContext.Current.Items["Base.Model"] = value;
                }
                else
                {
                    model = value;
                }
            }
        }

        public static MetaSolution Meta { get; set; }

        static XElement _metaXml = null;
        public static XElement MetaXml
        {
            get
            {
                if (HDataContext.DBType == "DB2")
                {
                    DB2Connection con = new DB2Connection(ConnectionManager.ConnectionString);
                    using (DB2Command cmd = new DB2Command("CALL DBO.USP_MODEL()", con))
                    {
                        con.Open();

                        using (XmlReader reader = cmd.ExecuteXmlReader())
                        {
                            while (reader.Read())
                            {
                                string s = reader.ReadOuterXml();
                                return XElement.Parse(s);
                            }
                        }
                    }
                }
                if (_metaXml == null) _metaXml = Model.ExecuteQuery<XElement>("EXEC [dbo].[usp_model]").FirstOrDefault();
                return _metaXml;
            }
        }

        static MetaSolution _meta = null;
        public static MetaSolution DynamicMeta
        {
            get
            {
                if (_meta == null) _meta = MetaSolution.Load();
                return _meta;
            }
        }
    }
}