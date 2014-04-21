using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using Nephrite.Web;

namespace Nephrite.Meta.Fluent
{
	public class ReferenceBuilder
	{
		MetaReference _ref;

		public ReferenceBuilder(MetaReference reference)
		{
			_ref = reference;
		}

		public ReferenceBuilder To(string refClassName)
		{
			_ref.RefClassName = refClassName;
			return this;
		}

		public ReferenceBuilder Required()
		{
			_ref.IsRequired = true;
			return this;
		}

		public ReferenceBuilder Multiple()
		{
			_ref.UpperBound = -1;
			return this;
		}

		public ReferenceBuilder Aggregation()
		{
			_ref.AssociationType = AssociationType.Aggregation;
			return this;
		}

		public ReferenceBuilder Composition()
		{
			_ref.AssociationType = AssociationType.Composition;
			return this;
		}

		public ReferenceBuilder InverseProperty(string inverseProperty)
		{
			_ref.InversePropertyName = inverseProperty;
			return this;
		}
	}

	public static class CoreFluent
	{
		public static MetaClass IntKey(this MetaClass cls, bool isIdentity = true)
		{
			var t = MetaIntType.NotNull();
			return cls.AttributeKey(cls.Name + t.ColumnSuffix, "Ид", t, isIdentity);
		}

		public static MetaClass GuidKey(this MetaClass cls)
		{
			var t = MetaGuidType.NotNull();
			return cls.AttributeKey(cls.Name + t.ColumnSuffix, "Ид", t);
		}

		public static MetaClass AttributeKey(this MetaClass cls, string name, string caption, IMetaIdentifierType type, bool isIdentity = false)
		{
			MetaAttribute a = new MetaAttribute { Name = name, Caption = caption, Type = type, IsMultilingual = false, IsRequired = true, IsIdentity = isIdentity };
			cls.AddProperty(a);
			cls.CompositeKey.Add(a);
			return cls;
		}

		public static MetaClass Attribute(this MetaClass cls, string name, string caption, IMetaPrimitiveType type, bool isMultilingual = false)
		{
			MetaAttribute a = new MetaAttribute { Name = name, Caption = caption, Type = type, IsMultilingual = isMultilingual };
			if (type.NotNullable) a.IsRequired = true;
			cls.AddProperty(a);
			return cls;
		}

		public static MetaClass ComputedAttribute(this MetaClass cls, string name, string caption, IMetaPrimitiveType type)
		{
			MetaComputedAttribute a = new MetaComputedAttribute { Name = name, Caption = caption, Type = type};
			if (type.NotNullable) a.IsRequired = true;
			cls.AddProperty(a);
			return cls;
		}

		public static MetaClass PersistentComputedAttribute(this MetaClass cls, string name, string caption, IMetaPrimitiveType type)
		{
			MetaPersistentComputedAttribute a = new MetaPersistentComputedAttribute { Name = name, Caption = caption, Type = type};
			if (type.NotNullable) a.IsRequired = true;
			cls.AddProperty(a);
			return cls;
		}

		public static MetaClass Reference<T>(this MetaClass cls, string name, string caption, Action<ReferenceBuilder> attributes = null)
		{
			MetaReference a = new MetaReference { Name = name, Caption = caption, UpperBound = 1, RefClassName = typeof(T).Name };
			if (attributes != null) attributes(new ReferenceBuilder(a));
			cls.AddProperty(a);
			return cls;
		}
		public static MetaClass ReferenceKey<T>(this MetaClass cls, string name, string caption, Action<ReferenceBuilder> attributes = null)
		{
			MetaReference a = new MetaReference { Name = name, Caption = caption, UpperBound = 1, RefClassName = typeof(T).Name };
			if (attributes != null) attributes(new ReferenceBuilder(a).Required());
			cls.AddProperty(a);
			cls.CompositeKey.Add(a);
			return cls;
		}

		public static MetaClass Title(this MetaClass cls, string caption = "Наименование")
		{
			cls.AddProperty(new MetaAttribute { Name = "Title", Caption = caption, IsRequired = true, Type = MetaStringType.NotNull() });
			cls.Interfaces.Add(typeof(IWithTitle));
			return cls;
		}

		public static MetaClass TimeStamp<T>(this MetaClass cls)
		{
			cls.Attribute("LastModifiedDate", "Дата последней модификации", MetaDateTimeType.NotNull());
			cls.Reference<T>("LastModifiedUser", "Последний редактировавший пользователь");
			cls.Interfaces.Add(typeof(IWithTimeStamp));
			return cls;
		}

		

		public static MetaClass OperationCreateNew(this MetaClass cls)
		{
			cls.AddOperation(new MetaOperation { Name = "CreateNew", Caption = "Создать", Image = "create" }); 
			return cls;
		}

		public static MetaClass OperationEdit(this MetaClass cls)
		{
			var op = new MetaOperation { Name = "Edit", Caption = "Редактировать", Image = "edit" };
			cls.AddOperation(op);
			cls.DefaultOperation = op; 
			return cls;
		}

		public static MetaClass OperationList(this MetaClass cls)
		{
			cls.AddOperation(new MetaOperation { Name = "ViewList", Caption = "Список", Image = "list" });
			return cls;
		}

		public static MetaClass OperationView(this MetaClass cls)
		{
			var op = new MetaOperation { Name = "View", Caption = "Свойства", Image = "properties" };
			cls.AddOperation(op);
			cls.DefaultOperation = op; 
			return cls;
		}

		public static MetaClass OperationDelete(this MetaClass cls)
		{
			cls.AddOperation(new MetaOperation { Name = "Delete", Caption = "Удалить", Image = "delete" });
			return cls;
		}

		public static MetaClass LogicalDelete(this MetaClass cls)
		{
			cls.AddProperty(new MetaAttribute { Name = "IsDeleted", Caption = "Удален", IsRequired = true, Type = TypeFactory.Boolean(true) });
			cls.AddOperation(new MetaOperation { Name = "UnDelete", Caption = "Отменить удаление", Image = "undelete" });
			cls.Interfaces.Add(typeof(IWithLogicalDelete));
			return cls;
		}

		public static MetaClass Operation(this MetaClass cls, string name, string caption, string image = "")
		{
			cls.AddOperation(new MetaOperation { Name = name, Caption = caption, Image = image });
			return cls;
		}

		public static MetaClass Workflow(this MetaClass cls)
		{
			//cls.Reference("Activity", "Статус", x => x.To("WF_Activity"));
			return cls;
		}

		public static MetaClass TCLED(this MetaClass cls)
		{
			cls.Title().OperationCreateNew().OperationList().OperationEdit().OperationDelete();
			return cls;
		}
		public static MetaClass TCLEVD(this MetaClass cls)
		{
			cls.Title().OperationCreateNew().OperationList().OperationEdit().OperationView().OperationDelete();
			return cls;
		}


	}


}