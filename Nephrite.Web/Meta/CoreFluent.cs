using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Nephrite.Meta.Fluent
{
	public class ReferenceBuilder
	{
		MetaReference _ref;
		MetaClass _cls;

		public ReferenceBuilder(MetaClass cls, MetaReference reference)
		{
			_cls = cls;
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

		public ReferenceBuilder DefaultDBValue(string value)
		{
			_ref.DefaultDBValue = value;
			return this;
		}
	}

	public class ValuePropertyBuilder
	{
		MetaValueProperty _prop;

		public ValuePropertyBuilder(MetaValueProperty property)
		{
			_prop = property;
		}

		public ValuePropertyBuilder Multilingual()
		{
			_prop.IsMultilingual = true;
			return this;
		}

		public ValuePropertyBuilder DefaultDBValue(string value)
		{
			_prop.DefaultDBValue = value;
			return this;
		}
	}

	public class OperationBuilder
	{
		MetaOperation _op;

		public OperationBuilder(MetaOperation op)
		{
			_op = op;
		}

		public OperationBuilder Parm(IMetaParameterType type, string name)
		{
			var parm = _op.Parameters.FirstOrDefault(o => o.Name.ToLower() == name.ToLower());
			if (parm == null)
			{
				parm = new MetaOperationParameter { Name = name, Type = type };
				_op.Parameters.Add(parm);
			}
			return this;
		}

		public OperationBuilder ParmString(string name)
		{
			return Parm(MetaStringType.NotNull(), name);
		}
		public OperationBuilder ParmInt(string name)
		{
			return Parm(MetaIntType.NotNull(), name);
		}
		public OperationBuilder ParmGuid(string name)
		{
			return Parm(MetaGuidType.NotNull(), name);
		}

		public OperationBuilder Image(string name)
		{
			_op.Image = name;
			return this;
		}

		public OperationBuilder InvokesView(string viewClass, string viewName)
		{
			_op.ViewClass = viewClass;
			_op.ViewName = viewName;
			_op.DTOClassKind = DTOClassKind.None;
			return this;
		}

		public OperationBuilder InvokesSingleObjectView(string viewName)
		{
			_op.ViewClass = "ViewControl";
			_op.ViewName = viewName;
			WithDTOClass(DTOClassKind.Single);
			return this;
		}

		public OperationBuilder InvokesObjectListView(string viewName)
		{
			_op.ViewClass = "ViewControl";
			_op.ViewName = viewName;
			WithDTOClass(DTOClassKind.Queryable);
			return this;
		}

		public OperationBuilder WithDTOClass(string className, DTOClassKind kind = DTOClassKind.Single)
		{
			_op.DTOClass = className;
			_op.DTOClassKind = kind;
			return this;
		}

		public OperationBuilder WithDTOClass(DTOClassKind kind)
		{
			_op.DTOClass = _op.Parent.Name;
			_op.DTOClassKind = kind;
			return this;
		}


	}

	public static class CoreFluent
	{
		public static MetaEnum Value(this MetaEnum cdf, string id, string name, string caption)
		{
			cdf.Values.Add(new MetaEnumValue(id, name, caption));
			return cdf;
		}

		public static MetaClass IntKey(this MetaClass cls, string name = "", bool isIdentity = true)
		{
			var t = MetaIntType.NotNull();
			int i = cls.Name.IndexOf('_'); if (i == -1) i = 0; else i++;
			return cls.AttributeKey(name.IsEmpty() ? cls.Name.Substring(i) + t.ColumnSuffix : name, "Ид", t, isIdentity);
		}

		public static MetaClass GuidKey(this MetaClass cls, string name = "")
		{
			var t = MetaGuidType.NotNull();
			int i = cls.Name.IndexOf('_'); if (i == -1) i = 0; else i++;
			return cls.AttributeKey(name.IsEmpty() ? cls.Name.Substring(i) + t.ColumnSuffix : name, "Ид", t);
		}

		public static MetaClass NonPersistent(this MetaClass cls)
		{
			cls.IsPersistent = false;
			return cls;
		}

		public static MetaClass AttributeKey(this MetaClass cls, string name, string caption, IMetaIdentifierType type, bool isIdentity = false)
		{
			MetaAttribute a = new MetaAttribute { Name = name, Caption = caption, Type = type, IsMultilingual = false, IsRequired = true, IsIdentity = isIdentity };
			cls.AddProperty(a);
			cls.CompositeKey.Add(a);
			return cls;
		}

		public static MetaClass Attribute(this MetaClass cls, string name, string caption, IMetaPrimitiveType type, Action<ValuePropertyBuilder> attributes = null)
		{
			MetaAttribute a = new MetaAttribute { Name = name, Caption = caption, Type = type, IsMultilingual = false };
			if (type.NotNullable) a.IsRequired = true;
			if (attributes != null) attributes(new ValuePropertyBuilder(a));
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

		public static MetaClass PersistentComputedAttribute(this MetaClass cls, string name, string caption, IMetaPrimitiveType type, Action<ValuePropertyBuilder> attributes = null)
		{
			MetaPersistentComputedAttribute a = new MetaPersistentComputedAttribute { Name = name, Caption = caption, Type = type, IsMultilingual = false };
			if (type.NotNullable) a.IsRequired = true;
			if (attributes != null) attributes(new ValuePropertyBuilder(a));
			cls.AddProperty(a);
			return cls;
		}

		public static MetaClass Reference<T>(this MetaClass cls, string name, string caption, Action<ReferenceBuilder> attributes = null)
		{
			MetaReference a = new MetaReference(name, caption, typeof(T).Name);
			if (attributes != null) attributes(new ReferenceBuilder(cls, a));
			cls.AddProperty(a);
			return cls;
		}
		public static MetaClass ReferenceKey<T>(this MetaClass cls, string name, string caption, Action<ReferenceBuilder> attributes = null)
		{
			MetaReference a = new MetaReference(name, caption, typeof(T).Name);
			if (attributes != null) attributes(new ReferenceBuilder(cls, a).Required());
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
			cls.Reference<T>("LastModifiedUser", "Последний редактировавший пользователь", x => x.Required());
			cls.Interfaces.Add(typeof(IWithTimeStamp));
			return cls;
		}



		public static MetaClass OperationCreateNew(this MetaClass cls, Action<OperationBuilder> attributes = null)
		{
			var o = new MetaOperation { Name = "CreateNew", Caption = "Создать" };
			cls.AddOperation(o);

			var ob = new OperationBuilder(o);
			ob.Image("create").InvokesSingleObjectView("edit");
			if (attributes != null) attributes(ob);

			if (o.Parameters.Count == 0)
				ob.ParmString("returnurl");

			return cls;
		}

		public static MetaClass OperationEdit(this MetaClass cls, Action<OperationBuilder> attributes = null)
		{
			var o = new MetaOperation { Name = "Edit", Caption = "Редактировать" };
			cls.AddOperation(o);

			var ob = new OperationBuilder(o);
			ob.Image("edit").InvokesSingleObjectView("edit");
			if (attributes != null) attributes(ob);

			if (o.Parameters.Count == 0)
				ob.Parm(cls.Key.Type as IMetaParameterType, "id").ParmString("returnurl");

			return cls;
		}

		public static MetaClass OperationList(this MetaClass cls, Action<OperationBuilder> attributes = null)
		{
			var o = new MetaOperation { Name = "ViewList", Caption = "Список" };
			cls.AddOperation(o);

			var ob = new OperationBuilder(o);
			ob.Image("list").InvokesObjectListView("list");
			if (attributes != null) attributes(ob);

			return cls;
		}

		public static MetaClass OperationView(this MetaClass cls, Action<OperationBuilder> attributes = null)
		{
			var o = new MetaOperation { Name = "View", Caption = "Свойства" };
			cls.AddOperation(o);

			var ob = new OperationBuilder(o);
			ob.Image("view").InvokesSingleObjectView("view");
			if (attributes != null) attributes(ob);

			if (o.Parameters.Count == 0)
				ob.Parm(cls.Key.Type as IMetaParameterType, "id").ParmString("returnurl");

			return cls;
		}

		public static MetaClass OperationDelete(this MetaClass cls, Action<OperationBuilder> attributes = null)
		{
			var o = new MetaOperation { Name = "Delete", Caption = "Удалить" };
			cls.AddOperation(o);

			var ob = new OperationBuilder(o);
			ob.Image("delete").InvokesSingleObjectView("delete");
			if (attributes != null) attributes(ob);

			if (o.Parameters.Count == 0)
				ob.Parm(cls.Key.Type as IMetaParameterType, "id").ParmString("returnurl");

			return cls;
		}

		public static MetaClass OperationUnDelete(this MetaClass cls, Action<OperationBuilder> attributes = null)
		{
			var o = new MetaOperation { Name = "UnDelete", Caption = "Отменить удаление" };
			cls.AddOperation(o);

			var ob = new OperationBuilder(o);
			ob.Image("undelete").InvokesSingleObjectView("undelete");
			if (attributes != null) attributes(ob);

			if (o.Parameters.Count == 0)
				ob.Parm(cls.Key.Type as IMetaParameterType, "id").ParmString("returnurl");

			return cls;
		}

		public static MetaClass Operation(this MetaClass cls, string name, string caption, Action<OperationBuilder> attributes = null)
		{
			var op = new MetaOperation { Name = name, Caption = caption };
			cls.AddOperation(op);
			if (attributes != null) attributes(new OperationBuilder(op));
			return cls;
		}

		//public static MetaPackage Operation(this MetaPackage pck, string name, string caption, Action<OperationBuilder> attributes = null)
		//{
		//	var op = new MetaOperation { Name = name, Caption = caption };
		//	pck.AddOperation(op);
		//	if (attributes != null) attributes(new OperationBuilder(op));
		//	return pck;
		//}

		public static MetaClass Workflow(this MetaClass cls)
		{
			//cls.Reference("Activity", "Статус", x => x.To("WF_Activity"));
			return cls;
		}

		public static MetaClass TCLED(this MetaClass cls)
		{
			cls.Title().
				OperationCreateNew().OperationList().OperationEdit().OperationDelete().OperationUnDelete();
			return cls;
		}
		public static MetaClass TCLEVD(this MetaClass cls)
		{
			cls.Title().
				OperationCreateNew().OperationList().OperationEdit().OperationView().OperationDelete().OperationUnDelete();
			return cls;
		}


	}

}