using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Nephrite.Meta.Fluent
{
	public class ReferenceBuilder
	{
		IMetaReference _ref;
		IMetaClass _cls;

		public ReferenceBuilder(IMetaClass cls, IMetaReference reference)
		{
			_cls = cls;
			_ref = reference;
		}

		public ReferenceBuilder To(string refClassName)
		{
			_ref.SetRefClass(refClassName);
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
			_ref.SetInverseProperty(inverseProperty);
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
		IMetaValueProperty _prop;

		public ValuePropertyBuilder(IMetaValueProperty property)
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
		IMetaOperation _op;

		public OperationBuilder(IMetaOperation op)
		{
			_op = op;
		}

		public OperationBuilder Parm(IMetaParameterType type, string name)
		{
			var parm = _op.Parameters.FirstOrDefault(o => o.Name.ToLower() == name.ToLower());
			if (parm == null)
			{
				parm = new MetaParameter { Name = name, Type = type };
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

		public OperationBuilder InvokesCode()
		{
			_op.ViewClass = "";
			_op.ViewName = "";
			_op.DTOClassKind = DTOClassKind.None;
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
			WithDTOClass(_op.Parent.IsMultilingual ? ("V_" + _op.Parent.Name) : _op.Parent.Name, DTOClassKind.Queryable);
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

	public class FunctionBuilder
	{
		IMetaClass _cl;

		public FunctionBuilder(IMetaClass cl)
		{
			_cl = cl;
		}

		public FunctionBuilder Parm(IMetaParameterType type, string name)
		{
			var parm = _cl.Parameters.FirstOrDefault(o => o.Name.ToLower() == name.ToLower());
			if (parm == null)
			{
				parm = new MetaParameter { Name = name, Type = type };
				_cl.Parameters.Add(parm);
			}
			return this;
		}

		public FunctionBuilder ParmString(string name)
		{
			return Parm(MetaStringType.NotNull(), name);
		}
		public FunctionBuilder ParmInt(string name)
		{
			return Parm(MetaIntType.NotNull(), name);
		}
		public FunctionBuilder ParmGuid(string name)
		{
			return Parm(MetaGuidType.NotNull(), name);
		}
		public FunctionBuilder ParmDateTime(string name)
		{
			return Parm(MetaDateTimeType.NotNull(), name);
		}
	}

	public static class CoreFluent
	{
		public static IMetaEnum Value(this IMetaEnum cdf, string id, string name, string caption)
		{
			cdf.Values.Add(new MetaEnumValue(id, name, caption));
			return cdf;
		}

		public static IMetaClass IntKey(this IMetaClass cls, string name = "", string caption = "Ид", bool isIdentity = true)
		{
			var t = MetaIntType.NotNull();
			int i = cls.Name.IndexOf('_'); if (i == -1) i = 0; else i++;
			return cls.AttributeKey(name.IsEmpty() ? cls.Name.Substring(i) + t.ColumnSuffix : name, caption, t, isIdentity);
		}

		public static IMetaClass GuidKey(this IMetaClass cls, string name = "", string caption = "Ид")
		{
			var t = MetaGuidType.NotNull();
			int i = cls.Name.IndexOf('_'); if (i == -1) i = 0; else i++;
			return cls.AttributeKey(name.IsEmpty() ? cls.Name.Substring(i) + t.ColumnSuffix : name, caption, t);
		}

		public static IMetaClass NonPersistent(this IMetaClass cls)
		{
			cls.Persistent = PersistenceType.None;
			return cls;
		}
		public static IMetaClass Persistent(this IMetaClass cls, PersistenceType type, Action<FunctionBuilder> parameters = null)
		{
			cls.Persistent = type;
			if (parameters != null)
			{
				parameters(new FunctionBuilder(cls));
			}
			return cls;
		}

		public static IMetaClass AttributeKey(this IMetaClass cls, string name, string caption, IMetaIdentifierType type, bool isIdentity = false)
		{
			MetaAttribute a = new MetaAttribute { Name = name, Caption = caption, Type = type, IsMultilingual = false, IsRequired = true, IsIdentity = isIdentity };
			cls.AddProperty(a);
			cls.CompositeKey.Add(a);
			return cls;
		}

		public static IMetaClass Attribute(this IMetaClass cls, string name, string caption, IMetaPrimitiveType type, Action<ValuePropertyBuilder> attributes = null)
		{
			MetaAttribute a = new MetaAttribute { Name = name, Caption = caption, Type = type, IsMultilingual = false };
			if (type.NotNullable) a.IsRequired = true;
			if (attributes != null) attributes(new ValuePropertyBuilder(a));
			cls.AddProperty(a);
			return cls;
		}

		public static IMetaClass ComputedAttribute(this IMetaClass cls, string name, string caption, IMetaPrimitiveType type)
		{
			MetaComputedAttribute a = new MetaComputedAttribute { Name = name, Caption = caption, Type = type};
			if (type.NotNullable) a.IsRequired = true;
			cls.AddProperty(a);
			return cls;
		}

		public static IMetaClass PersistentComputedAttribute(this IMetaClass cls, string name, string caption, IMetaPrimitiveType type, Action<ValuePropertyBuilder> attributes = null)
		{
			MetaPersistentComputedAttribute a = new MetaPersistentComputedAttribute { Name = name, Caption = caption, Type = type, IsMultilingual = false };
			if (type.NotNullable) a.IsRequired = true;
			if (attributes != null) attributes(new ValuePropertyBuilder(a));
			cls.AddProperty(a);
			return cls;
		}

		public static IMetaClass Reference<T>(this IMetaClass cls, string name, string caption, Action<ReferenceBuilder> attributes = null)
		{
			MetaReference a = new MetaReference(name, caption, typeof(T).Name);
			if (attributes != null) attributes(new ReferenceBuilder(cls, a));
			cls.AddProperty(a);
			return cls;
		}
		public static IMetaClass ReferenceKey<T>(this IMetaClass cls, string name, string caption, Action<ReferenceBuilder> attributes = null)
		{
			MetaReference a = new MetaReference(name, caption, typeof(T).Name);
			if (attributes != null) attributes(new ReferenceBuilder(cls, a).Required());
			cls.AddProperty(a);
			cls.CompositeKey.Add(a);
			return cls;
		}

		public static IMetaClass Title(this IMetaClass cls, string caption = "Наименование")
		{
			cls.AddProperty(new MetaAttribute { Name = "Title", Caption = caption, IsRequired = true, Type = MetaStringType.NotNull() });
			cls.Interfaces.Add(typeof(IWithTitle));
			return cls;
		}

		public static IMetaClass TimeStamp<T>(this IMetaClass cls)
		{
			cls.Attribute("LastModifiedDate", "Дата последней модификации", MetaDateTimeType.NotNull(), x => x.DefaultDBValue("(getdate())"));
			cls.Reference<T>("LastModifiedUser", "Последний редактировавший пользователь", x => x.Required());
			cls.Interfaces.Add(typeof(IWithTimeStamp));
			return cls;
		}



		public static IMetaClass OperationCreateNew(this IMetaClass cls, Action<OperationBuilder> attributes = null)
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

		public static IMetaClass OperationEdit(this IMetaClass cls, Action<OperationBuilder> attributes = null)
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

		public static IMetaClass OperationList(this IMetaClass cls, Action<OperationBuilder> attributes = null)
		{
			var o = new MetaOperation { Name = "ViewList", Caption = "Список" };
			cls.AddOperation(o);

			var ob = new OperationBuilder(o);
			ob.Image("list").InvokesObjectListView("list");
			if (attributes != null) attributes(ob);

			return cls;
		}

		public static IMetaClass OperationView(this IMetaClass cls, Action<OperationBuilder> attributes = null)
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

		public static IMetaClass OperationDelete(this IMetaClass cls, Action<OperationBuilder> attributes = null)
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

		public static IMetaClass OperationUnDelete(this IMetaClass cls, Action<OperationBuilder> attributes = null)
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

		public static IMetaClass Operation(this IMetaClass cls, string name, string caption, Action<OperationBuilder> attributes = null)
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

		public static IMetaClass Workflow(this IMetaClass cls)
		{
			//cls.Reference("Activity", "Статус", x => x.To("WF_Activity"));
			return cls;
		}

		public static IMetaClass TCLED(this IMetaClass cls)
		{
			cls.Title().
				OperationCreateNew().OperationList().OperationEdit().OperationDelete().OperationUnDelete();
			return cls;
		}
		public static IMetaClass TCLEVD(this IMetaClass cls)
		{
			cls.Title().
				OperationCreateNew().OperationList().OperationEdit().OperationView().OperationDelete().OperationUnDelete();
			return cls;
		}



	}
}