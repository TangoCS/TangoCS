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

			XElement x = Base.MetaXml;
			if (x == null) throw new Exception("Unable to load model from database");

			foreach (XElement xe in x.Nodes())
				if (xe.Name == "Package") LoadPackage(s, xe);

			return s;
		}

		static void LoadPackage(MetaSolution s, XElement xp)
		{
			MetaPackage np = new MetaPackage();
			np.ID = xp.GetAttributeValue("ID").ToGuid();
			np.Name = xp.GetAttributeValue("Name");
			np.Caption = xp.GetAttributeValue("Caption");
			np.Description = xp.GetAttributeValue("Description");

			string parent = xp.GetAttributeValue("ParentID");
			if (!parent.IsEmpty()) 
			{
				MetaPackage parentPck = s.GetPackage(parent.ToGuid());
				parentPck.AddPackage(np);
			}
			else
				s.AddPackage(np);

			foreach (XElement xe in xp.Element("Classes").Nodes())
			{
				LoadClass(np, xe);
			}
		}

		static void LoadClass(MetaPackage p, XElement xc)
		{
			MetaClass c = new MetaClass();
			c.ID = xc.GetAttributeValue("ID").ToGuid();
			c.Name = xc.GetAttributeValue("Name");
			c.Caption = xc.GetAttributeValue("Caption");
			c.Description = xc.GetAttributeValue("Description");

			p.AddClass(c);
		}

		static void LoadAttribute(MetaClass c, XElement xp)
		{
			MetaAttribute a = new MetaAttribute();
			a.ID = xp.GetAttributeValue("ID").ToGuid();
			a.Name = xp.GetAttributeValue("Name");
			a.Caption = xp.GetAttributeValue("Caption");
			a.Description = xp.GetAttributeValue("Description");
			//a.IsKey = xp.GetAttributeValue("IsKey") == "1";

			a.IsMultilingual = xp.GetAttributeValue("IsMultilingual") == "1";
			a.IsIdentity = xp.GetAttributeValue("IsIdentity") == "1";

			/*switch (xp.GetAttributeValue("DataType"))
			{
				case "S": a.Type = TypeFactory.String(xp.GetAttributeValue("Length").ToInt32(-1)); break;
				case "D": a.Type = TypeFactory.Date; break;
				case "T": a.Type = TypeFactory.DateTime; break;
				case "N": a.Type = TypeFactory.Int; break;
				case "O": break;
				case "U": a.Type = TypeFactory.Long; break;
				case "X": a.Type = TypeFactory.Data; break;
				case "B": a.Type = TypeFactory.Boolean; break;
				case "G": a.Type = TypeFactory.Guid; break;
				case "M": a.Type = TypeFactory.Decimal(xp.GetAttributeValue("Precision").ToInt32(14), xp.GetAttributeValue("Scale").ToInt32(6)); break;
				case "C": a.Type = TypeFactory.Char(xp.GetAttributeValue("Length").ToInt32(1)); break;
				case "F": a.Type = TypeFactory.FileID; break;
				case "E": a.Type = TypeFactory.FileGUID; break;
				case "Z": a.Type = TypeFactory.ZoneDateTime; break;
			}*/

			c.AddProperty(a);
		}

		static void LoadReference(MetaClass c, XElement xp)
		{
			MetaReference a = new MetaReference();
			a.ID = xp.GetAttributeValue("ID").ToGuid();
			a.Name = xp.GetAttributeValue("Name");
			a.Caption = xp.GetAttributeValue("Caption");
			a.Description = xp.GetAttributeValue("Description");
			//a.IsKey = xp.GetAttributeValue("IsKey") == "1";

			a.RefClassName = xp.GetAttributeValue("RefClass");
			a.InversePropertyName = xp.GetAttributeValue("InverseProperty");

			c.AddProperty(a);
		}

		static void LoadOperation(MetaClass c, XElement xo)
		{
			MetaOperation o = new MetaOperation();
			o.ID = xo.GetAttributeValue("ID").ToGuid();
			o.Name = xo.GetAttributeValue("Name");
			o.Caption = xo.GetAttributeValue("Caption");
			o.Description = xo.GetAttributeValue("Description");

			c.AddOperation(o);
		}

		
	}

	public class XsltExtension
	{
		public string getType(string typeCode, string isRequired)
		{
			switch (typeCode)
			{
				case "S": return "TypeFactory.String()";
				case "X": return "TypeFactory.ByteArray()";

				case "C": return String.Format("TypeFactory.Char({0})", isRequired);
				case "D": return String.Format("TypeFactory.Date({0})", isRequired);
				case "T": return String.Format("TypeFactory.DateTime({0})", isRequired);
				case "N": return String.Format("TypeFactory.Int({0})", isRequired);
				case "U": return String.Format("TypeFactory.Long({0})", isRequired);
				
				case "B": return String.Format("TypeFactory.Boolean({0})", isRequired);
				case "G": return String.Format("TypeFactory.Guid({0})", isRequired);
				case "M": return String.Format("TypeFactory.Decimal({0})", isRequired);
				
				case "F": return String.Format("TypeFactory.FileIntKey({0})", isRequired);
				case "E": return String.Format("TypeFactory.FileGuidKey({0})", isRequired);
			}
			return "new MetaPrimitiveType()";
		}
	}
}