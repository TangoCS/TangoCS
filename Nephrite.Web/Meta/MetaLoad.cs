using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using Nephrite.Web;

namespace Nephrite.Meta
{
    public partial class MetaSolution : MetaElement
    {
        public static MetaSolution Load()
        {
            MetaSolution s = new MetaSolution();

            XElement x = A.MetaXml;
            if (x == null) throw new Exception("Unable to load model from database");

            foreach (XElement xe in x.Nodes())
                if (xe.Name == "Package") LoadPackage(s, xe);

            return s;
        }

        static void LoadPackage(MetaSolution s, XElement xp)
        {
            MetaPackage np = new MetaPackage();
            //np.ID = xp.GetAttributeValue("ID").ToGuid();
            np.Name = xp.GetAttributeValue("Name");
            np.Caption = xp.GetAttributeValue("Caption");
            np.Description = xp.GetAttributeValue("Description");

            string parent = xp.GetAttributeValue("ParentID");
            if (!parent.IsEmpty())
            {
                MetaPackage parentPck = s.GetPackage(parent);
                parentPck.AddPackage(np);
            }
            else
                s.AddPackage(np);

            var classes = xp.Elements("Classes");
            if (classes != null)
                foreach (XElement xe in classes.Nodes())
                {
                    LoadClass(np, xe);
                }
        }

        static void LoadClass(MetaPackage p, XElement xc)
        {
            MetaClass c = new MetaClass();
            //c.ID = xc.GetAttributeValue("ID").ToGuid();
            c.Name = xc.GetAttributeValue("Name");
            c.Caption = xc.GetAttributeValue("Caption");
            c.Description = xc.GetAttributeValue("Description");

            c.IsPersistent = xc.GetAttributeValue("IsSeparateTable") == "1"; 
            c.BaseClassName = xc.GetAttributeValue("BaseClass");
            if (c.Name == "MM_Package")
            {

            }

            var properties = xc.Elements("Properties");
            if (properties != null)
                foreach (XElement xe in properties.Nodes())
                {
                    if (xe.Name == "Attribute")
                    {
                        LoadAttribute(c, xe);
                    }
                    if (xe.Name == "Reference") LoadReference(c, xe);
                    if (xe.Name == "ComputedAttribute") LoadComputedAttribute(c, xe);
                    if (xe.Name == "PersistentComputedAttribute") LoadPersistentComputedAttribute(c, xe);
                }


            var operations = xc.Elements("Operations");
            if (operations != null)
                foreach (XElement xe in operations.Nodes())
                {
                    LoadOperation(c, xe);
                }

            var stereotypes = xc.Elements("Stereotypes");
            if (stereotypes != null)
                foreach (XElement xe in stereotypes.Nodes())
                {
                    if (xe.Name == "Versioning") // @Sad переделать потом на универсальный загрузчик
                    {
                        SVersioning s = new SVersioning(xe.GetAttributeValue("VersioningType"));
                        c.AddStereotype(s);
                    }
                }

            p.AddClass(c);
        }

        static void LoadAttribute(MetaClass c, XElement xp)
        {
            if (c.Name == "Appendix")
            {

            }
            MetaAttribute a = new MetaAttribute();
            //a.ID = xp.GetAttributeValue("ID").ToGuid();
            a.Name = xp.GetAttributeValue("Name");
            a.Caption = xp.GetAttributeValue("Caption");
            a.Description = xp.GetAttributeValue("Description");

            a.IsRequired = xp.GetAttributeValue("IsRequired").ToLower() == "true";
            a.UpperBound = xp.GetAttributeValue("UpperBound").ToInt32(0);
            a.DefaultDBValue = xp.GetAttributeValue("DefaultDBValue");

            a.IsMultilingual = xp.GetAttributeValue("IsMultilingual").ToLower() == "true";
            a.IsIdentity = xp.GetAttributeValue("IsIdentity").ToLower() == "true";
            //a.IsKey = xp.GetAttributeValue("IsKey");
            switch (xp.GetAttributeValue("DataType"))
            {
				case "S": a.Type = TypeFactory.String(xp.GetAttributeValue("Length").ToInt32(-1), a.IsRequired); break;
                case "D": a.Type = TypeFactory.Date(a.IsRequired); break;
                case "T": a.Type = TypeFactory.DateTime(a.IsRequired); break;
                case "N": a.Type = TypeFactory.Int(a.IsRequired); break;
                case "O": break;
                case "U": a.Type = TypeFactory.Long(a.IsRequired); break;
				case "X": a.Type = TypeFactory.ByteArray(xp.GetAttributeValue("Length").ToInt32(-1), a.IsRequired); break;
                case "B": a.Type = TypeFactory.Boolean(a.IsRequired); break;
                case "G": a.Type = TypeFactory.Guid(a.IsRequired); break;
                case "M": a.Type = TypeFactory.Decimal(xp.GetAttributeValue("Precision").ToInt32(14), xp.GetAttributeValue("Scale").ToInt32(6), a.IsRequired); break;
                case "C": a.Type = new MetaEnum { Name = xp.GetAttributeValue("EnumName") ??"", NotNullable = a.IsRequired }; break;
                case "F": a.Type = TypeFactory.FileIntKey(a.IsRequired); break;
                case "E": a.Type = TypeFactory.FileGuidKey(a.IsRequired); break;
                case "Z": a.Type = TypeFactory.ZoneDateTime(a.IsRequired); break;
            }

            if (xp.GetAttributeValue("IsKey").ToLower() == "true") c.CompositeKey.Add(a);
            c.AddProperty(a);
        }

        static void LoadComputedAttribute(MetaClass c, XElement xp)
        {
            MetaComputedAttribute a = new MetaComputedAttribute();
            //a.ID = xp.GetAttributeValue("ID").ToGuid();
            a.Name = xp.GetAttributeValue("Name");
            a.Caption = xp.GetAttributeValue("Caption");
            a.Description = xp.GetAttributeValue("Description");
            //a.GetExpression = xp.GetAttributeValue("GetExpression");
            switch (xp.GetAttributeValue("DataType"))
            {
				case "S": a.Type = TypeFactory.String(xp.GetAttributeValue("Length").ToInt32(-1), a.IsRequired); break;
                case "D": a.Type = TypeFactory.Date(a.IsRequired); break;
                case "T": a.Type = TypeFactory.DateTime(a.IsRequired); break;
                case "N": a.Type = TypeFactory.Int(a.IsRequired); break;
                case "O": break;
                case "U": a.Type = TypeFactory.Long(a.IsRequired); break;
				case "X": a.Type = TypeFactory.ByteArray(a.IsRequired); break;
                case "B": a.Type = TypeFactory.Boolean(a.IsRequired); break;
                case "G": a.Type = TypeFactory.Guid(a.IsRequired); break;
                case "M": a.Type = TypeFactory.Decimal(xp.GetAttributeValue("Precision").ToInt32(14), xp.GetAttributeValue("Scale").ToInt32(6), a.IsRequired); break;
                case "C": a.Type = new MetaEnum { Name = xp.GetAttributeValue("EnumName") ?? "", NotNullable = a.IsRequired }; break;
                case "F": a.Type = TypeFactory.FileIntKey(a.IsRequired); break;
                case "E": a.Type = TypeFactory.FileGuidKey(a.IsRequired); break;
                case "Z": a.Type = TypeFactory.ZoneDateTime(a.IsRequired); break;
            }
            c.AddProperty(a);
        }

        static void LoadPersistentComputedAttribute(MetaClass c, XElement xp)
        {
            MetaPersistentComputedAttribute a = new MetaPersistentComputedAttribute();
            //a.ID = xp.GetAttributeValue("ID").ToGuid();
            a.Name = xp.GetAttributeValue("Name");
            a.Caption = xp.GetAttributeValue("Caption");
            a.Description = xp.GetAttributeValue("Description");
            a.Expression = xp.GetAttributeValue("Expression");
            //a.DataType = xp.GetAttributeValue("DataType");
            switch (xp.GetAttributeValue("DataType"))
            {
				case "S": a.Type = TypeFactory.String(xp.GetAttributeValue("Length").ToInt32(-1), a.IsRequired); break;
                case "D": a.Type = TypeFactory.Date(a.IsRequired); break;
                case "T": a.Type = TypeFactory.DateTime(a.IsRequired); break;
                case "N": a.Type = TypeFactory.Int(a.IsRequired); break;
                case "O": break;
                case "U": a.Type = TypeFactory.Long(a.IsRequired); break;
				case "X": a.Type = TypeFactory.ByteArray(a.IsRequired); break;
                case "B": a.Type = TypeFactory.Boolean(a.IsRequired); break;
                case "G": a.Type = TypeFactory.Guid(a.IsRequired); break;
                case "M": a.Type = TypeFactory.Decimal(xp.GetAttributeValue("Precision").ToInt32(14), xp.GetAttributeValue("Scale").ToInt32(6), a.IsRequired); break;
                case "C": a.Type = new MetaEnum { Name = xp.GetAttributeValue("EnumName") ?? "", NotNullable = a.IsRequired }; break;
                case "F": a.Type = TypeFactory.FileIntKey(a.IsRequired); break;
                case "E": a.Type = TypeFactory.FileGuidKey(a.IsRequired); break;
                case "Z": a.Type = TypeFactory.ZoneDateTime(a.IsRequired); break;
            }

            c.AddProperty(a);
        }

        static void LoadReference(MetaClass c, XElement xp)
        {
            MetaReference a = null;
            if (xp.GetAttributeValue("IsReferenceToVersion") != null && xp.GetAttributeValue("IsReferenceToVersion").ToLower() == "true")
                a = new MetaReferenceToVersion();
            else
                a = new MetaReference();
            //a.ID = xp.GetAttributeValue("ID").ToGuid();
            a.Name = xp.GetAttributeValue("Name");
            a.Caption = xp.GetAttributeValue("Caption");
            a.Description = xp.GetAttributeValue("Description");

            a.IsRequired = xp.GetAttributeValue("IsRequired").ToLower() == "true";
            a.UpperBound = xp.GetAttributeValue("UpperBound").ToInt32(0);

            a.RefClassName = xp.GetAttributeValue("RefClass");
            a.InversePropertyName = xp.GetAttributeValue("InverseProperty");
            a.AssociationType = (AssociationType)xp.GetAttributeValue("AssociationType").ToInt32(0);
            //a.IsKey = xp.GetAttributeValue("IsKey");
            if (xp.GetAttributeValue("IsKey").ToLower() == "true") c.CompositeKey.Add(a);
            c.AddProperty(a);
        }

        static void LoadOperation(MetaClass c, XElement xo)
        {
            MetaOperation o = new MetaOperation();
            //o.ID = xo.GetAttributeValue("ID").ToGuid();
            o.Name = xo.GetAttributeValue("Name");
            o.Caption = xo.GetAttributeValue("Caption");
            o.Description = xo.GetAttributeValue("Description");

			if (xo.GetAttributeValue("IsDefault").ToLower() == "true")
				c.DefaultOperation = o;
            c.AddOperation(o);
        }
    }
}