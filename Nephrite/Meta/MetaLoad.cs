using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Nephrite.Meta
{
	public partial class MetaSolution : MetaElement
	{
		public static MetaSolution Load(XElement metaData)
		{
			MetaSolution s = new MetaSolution();

			foreach (XElement xe in metaData.Nodes())
				if (xe.Name == "Package") LoadPackage(s, xe);

			return s;
		}

		static void LoadPackage(MetaSolution s, XElement xp)
		{
			var operations = xp.Elements("Operations");
			if (operations != null && operations.Nodes().Count() > 0)
			{
				MetaClass np = new MetaClass();
				//np.ID = xp.GetAttributeValue("ID").ToGuid();
				np.Name = xp.GetAttributeValue("Name");
				np.Caption = xp.GetAttributeValue("Caption");
				np.Description = xp.GetAttributeValue("Description");
				s.AddClass(np);

				foreach (XElement xe in operations.Nodes())
				{
					LoadOperation(np, xe);
				}
			}

			var enums = xp.Elements("Enums");
			if (enums != null)
				foreach (XElement xe in enums.Nodes())
				{
					LoadEnum(s, xe);
				}

			var classes = xp.Elements("Classes");
			if (classes != null)
				foreach (XElement xe in classes.Nodes())
				{
					LoadClass(s, xe);
				}
		}

		static void LoadEnum(MetaSolution p, XElement xc)
		{
			MetaEnum e = new MetaEnum();
			e.Name = xc.GetAttributeValue("Name");
			e.Caption = xc.GetAttributeValue("Caption");

			var values = xc.Elements("Value");
			if (values != null)
			{
				foreach (XElement xe in values)
				{
					MetaEnumValue ev = new MetaEnumValue(xe.GetAttributeValue("ID"), xe.GetAttributeValue("Name"), xe.GetAttributeValue("Caption"));
					e.Values.Add(ev);
				}
			}

			p.AddEnum(e);
			
		}

		static void LoadClass(MetaSolution p, XElement xc)
		{
			MetaClass c = new MetaClass();
			//c.ID = xc.GetAttributeValue("ID").ToGuid();
			c.Name = xc.GetAttributeValue("Name");
			c.Caption = xc.GetAttributeValue("Caption");
			c.Description = xc.GetAttributeValue("Description");

			if (xc.GetAttributeValue("IsSeparateTable") == "1") c.Persistent = PersistenceType.Table;
			c.BaseClassName = xc.GetAttributeValue("BaseClass");
			c.LogicalDeleteExpressionString = xc.GetAttributeValue("LogicalDeleteExpression");
			c.DefaultOrderByExpressionString = xc.GetAttributeValue("DefaultOrderBy");

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
						SVersioning s = new SVersioning(xe.GetAttributeValue("Type"));
						c.AssignStereotype(s);
					}
				}

			p.AddClass(c);
		}

		static void LoadAttribute(MetaClass c, XElement xp)
		{
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
				case "C":
					a.Type = new MetaEnumType { Name = xp.GetAttributeValue("EnumName") ?? "", NotNullable = a.IsRequired }; break;
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
			a.GetExpressionString = xp.GetAttributeValue("GetExpression");
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
				case "C": a.Type = new MetaEnumType { Name = xp.GetAttributeValue("EnumName") ?? "", NotNullable = a.IsRequired }; break;
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
			a.IsMultilingual = xp.GetAttributeValue("IsMultilingual").ToLower() == "true";
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
				case "C": a.Type = new MetaEnumType { Name = xp.GetAttributeValue("EnumName") ?? "", NotNullable = a.IsRequired }; break;
				case "F": a.Type = TypeFactory.FileIntKey(a.IsRequired); break;
				case "E": a.Type = TypeFactory.FileGuidKey(a.IsRequired); break;
				case "Z": a.Type = TypeFactory.ZoneDateTime(a.IsRequired); break;
			}

			c.AddProperty(a);
		}

		static void LoadReference(MetaClass c, XElement xp)
		{
			MetaReference a = null;
			//if (xp.GetAttributeValue("IsReferenceToVersion") != null && xp.GetAttributeValue("IsReferenceToVersion").ToLower() == "true")
			//	a = new MetaReferenceToVersion();
			//else
			a = new MetaReference(xp.GetAttributeValue("Name"), xp.GetAttributeValue("Caption"), xp.GetAttributeValue("RefClass"),
				xp.GetAttributeValue("IsRequired").ToLower() == "true", xp.GetAttributeValue("UpperBound").ToInt32(1),
 				(AssociationType)xp.GetAttributeValue("AssociationType").ToInt32(0),
				xp.GetAttributeValue("InverseProperty"), xp.GetAttributeValue("Description"));
			//a.ID = xp.GetAttributeValue("ID").ToGuid(); 
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

			o.ActionString = xo.GetAttributeValue("ActionString");
			o.PredicateString = xo.GetAttributeValue("PredicateString");

			//o.ViewName = xo.GetAttributeValue("ViewName");
			//o.ViewClass = xo.GetAttributeValue("ViewClass");

			if (xo.GetAttributeValue("IsDefault").ToLower() == "true" && c is MetaClass)
				(c as MetaClass).DefaultOperation = o;

			var parms = xo.Elements("Parameter");
			if (parms != null)
				foreach (XElement xe in parms)
				{
					MetaParameter parm = new MetaParameter();
					parm.Name = xe.GetAttributeValue("Name");
					switch (xe.GetAttributeValue("Type").ToLower())
					{
						case "string": parm.Type = MetaStringType.NotNull(); break;
						case "int": parm.Type = MetaIntType.NotNull(); break;
						case "guid": parm.Type = MetaGuidType.NotNull(); break;
					}
					o.Parameters.Add(parm);
				}

			c.AddOperation(o);
		}
	}
}