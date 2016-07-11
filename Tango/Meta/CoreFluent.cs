using Tango.UI;
using System;
using System.Linq;

namespace Tango.Meta.Fluent
{
	public static class FluentExtensions
	{
		public static IMetaOperation Parm(this IMetaOperation op, IMetaParameterType type, string name)
		{
			var parm = op.Parameters.FirstOrDefault(o => o.Name.ToLower() == name.ToLower());
			if (parm == null)
			{
				parm = new MetaParameter { Name = name, Type = type };
				op.Parameters.Add(parm);
			}
			return op;
		}

		public static IMetaOperation ParmString(this IMetaOperation op, string name)
		{
			return op.Parm(TypeFactory.String, name);
		}
		public static IMetaOperation ParmInt(this IMetaOperation op, string name)
		{
			return op.Parm(TypeFactory.Int, name);
		}
		public static IMetaOperation ParmGuid(this IMetaOperation op, string name)
		{
			return op.Parm(TypeFactory.Guid, name);
		}
		public static IMetaOperation ParmGuidId(this IMetaOperation op)
		{
			return op.ParmGuid(Constants.Id);
		}
		public static IMetaOperation ParmIntId(this IMetaOperation op)
		{
			return op.ParmInt(Constants.Id);
		}
		public static IMetaOperation ParmReturnUrl(this IMetaOperation op)
		{
			return op.ParmString(Constants.ReturnUrl);
		}

		public static IMetaOperation WithImage(this IMetaOperation op, string name)
		{
			op.Image = name;
			return op;
		}


		public static IMetaReference<TClass, TRefClass> Aggregation<TClass, TRefClass>(this IMetaReference<TClass, TRefClass> r)
		{
			r.AssociationType = AssociationType.Aggregation;
			return r;
		}

		public static IMetaReference<TClass, TRefClass> Composition<TClass, TRefClass>(this IMetaReference<TClass, TRefClass> r)
		{
			r.AssociationType = AssociationType.Composition;
			return r;
		}

		public static ICanBeMultilingual Multilingual(this ICanBeMultilingual a)
		{
			a.IsMultilingual = true;
			return a;
		}
	}

	//public class FunctionBuilder
	//{
	//	IMetaClass _cl;

	//	public FunctionBuilder(IMetaClass cl)
	//	{
	//		_cl = cl;
	//	}

	//	public FunctionBuilder Parm(IMetaParameterType type, string name)
	//	{
	//		var parm = _cl.Parameters.FirstOrDefault(o => o.Name.ToLower() == name.ToLower());
	//		if (parm == null)
	//		{
	//			parm = new MetaParameter { Name = name, Type = type };
	//			_cl.Parameters.Add(parm);
	//		}
	//		return this;
	//	}

	//	public FunctionBuilder ParmString(string name)
	//	{
	//		return Parm(TypeFactory.String, name);
	//	}
	//	public FunctionBuilder ParmInt(string name)
	//	{
	//		return Parm(TypeFactory.Int, name);
	//	}
	//	public FunctionBuilder ParmGuid(string name)
	//	{
	//		return Parm(TypeFactory.Guid, name);
	//	}
	//	public FunctionBuilder ParmDateTime(string name)
	//	{
	//		return Parm(TypeFactory.DateTime, name);
	//	}
	//}

	public class SolutionBuilder
	{
		public MetaClassBuilder<TClass> Class<TClass>(IMetaClass cls)
		{
			return new MetaClassBuilder<TClass>(cls);
		}

		public MetaClassBuilder<TClass, TClassData> Class<TClass, TClassData>(IMetaClass cls)
		{
			return new MetaClassBuilder<TClass, TClassData>(cls);
		}
	}

	public abstract class AbstractMetaClassBuilder<T, TClass>
		where T : AbstractMetaClassBuilder<T, TClass>
	{
		protected T _this;

		public IMetaClass MetaClass { get; private set; }

		public AbstractMetaClassBuilder(IMetaClass cls)
		{
			MetaClass = cls;
			_this = this as T;
		}

		public T IntKey(bool isIdentity = true) => Key<int>(TypeFactory.Int, a => a.IsIdentity = isIdentity);
		public T IntKey(string name, bool isIdentity = true) => Key<int>(name, TypeFactory.Int, a => a.IsIdentity = isIdentity);
		public T LongKey(bool isIdentity = true) => Key<long>(TypeFactory.Long, a => a.IsIdentity = isIdentity);
		public T LongKey(string name, bool isIdentity = true) => Key<long>(name, TypeFactory.Long, a => a.IsIdentity = isIdentity);
		public T GuidKey() => Key<Guid>(TypeFactory.Guid);
		public T GuidKey(string name) => Key<Guid>(name, TypeFactory.Guid);
		public T StringKey() => Key<string>(TypeFactory.String);
		public T StringKey(string name) => Key<string>(name, TypeFactory.String);
		public T DateKey() => Key<DateTime>(TypeFactory.Date);
		public T DateKey(string name) => Key<DateTime>(name, TypeFactory.Date);

		public T Persistence(PersistenceType type)
		{
			MetaClass.Persistent = type;
			return _this;
		}

		public T Key<TKey>(IMetaIdentifierType type, Action<MetaAttribute<TClass, TKey>> attribute = null)
		{
			int i = MetaClass.Name.LastIndexOf('_'); if (i == -1) i = 0; else i++;
			string name = MetaClass.Name.Substring(i) + type.ColumnSuffix;
			return Key(name, type, attribute);
		}

		public T Key<TKey>(string name, IMetaIdentifierType type, Action<MetaAttribute<TClass, TKey>> attribute = null)
		{
			MetaAttribute<TClass, TKey> a = new MetaAttribute<TClass, TKey> {
				Name = name, IsMultilingual = false, IsRequired = true, Type = type
			};
			MetaClass.AddProperty(a);
			MetaClass.CompositeKey.Add(a);
			return _this;
		}

		public T ReferenceKey<TRefClass, TKey>(string name, Action<MetaReference<TClass, TRefClass, TKey>> attributes = null)
		{
			MetaReference<TClass, TRefClass, TKey> a = new MetaReference<TClass, TRefClass, TKey> {
				Name = name, IsRequired = true
			};
			a.SetRefClass(typeof(TRefClass).Name);
			if (attributes != null) attributes(a);
			MetaClass.AddProperty(a);
			MetaClass.CompositeKey.Add(a);
			return _this;
		}

		public T Attribute<TValue>(string name, IMetaPrimitiveType type, Action<MetaAttribute<TClass, TValue>> attributes = null)
		{
			if (type == null) type = TypeFactory.String;
			MetaAttribute<TClass, TValue> a = new MetaAttribute<TClass, TValue> {
				Name = name, Type = type, IsRequired = Nullable.GetUnderlyingType(typeof(TValue)) == null
			};
			if (attributes != null) attributes(a);
			MetaClass.AddProperty(a);
			return _this;
		}

		public T Reference<TRefClass>(string name, Action<MetaReference<TClass, TRefClass>> attributes = null)
		{
			return Reference(name, false, attributes);
		}

		public T Reference<TRefClass>(string name, bool isRequired, Action<MetaReference<TClass, TRefClass>> attributes = null)
		{
			MetaReference<TClass, TRefClass> a = new MetaReference<TClass, TRefClass> {
				Name = name, IsRequired = isRequired
			};
			a.SetRefClass(typeof(TRefClass).Name);
			if (attributes != null) attributes(a);
			MetaClass.AddProperty(a);
			return _this;
		}

		public T Reference<TRefClass, TKey>(string name, Action<MetaReference<TClass, TRefClass, TKey>> attributes = null)
		{
			return Reference(name, Nullable.GetUnderlyingType(typeof(TKey)) == null, attributes);
		}

		public T Reference<TRefClass, TKey>(string name, bool isRequired, Action<MetaReference<TClass, TRefClass, TKey>> attributes = null)
		{
			MetaReference<TClass, TRefClass, TKey> a = new MetaReference<TClass, TRefClass, TKey> {
				Name = name, IsRequired = isRequired
			};
			a.SetRefClass(typeof(TRefClass).Name);
			if (attributes != null) attributes(a);
			MetaClass.AddProperty(a);
			return _this;
		}

		public T ComputedAttribute<TValue>(string name, IMetaPrimitiveType type)
		{
			MetaComputedAttribute<TClass, TValue> a = new MetaComputedAttribute<TClass, TValue> {
				Name = name, Type = type, IsRequired = Nullable.GetUnderlyingType(typeof(TValue)) == null
			};
			MetaClass.AddProperty(a);
			return _this;
		}

		public T ComputedAttribute<TValue>(string name)
		{
			MetaComputedAttribute<TClass, TValue> a = new MetaComputedAttribute<TClass, TValue> {
				Name = name, Type = TypeFactory.FromCSharpType(typeof(TValue)),
				IsRequired = Nullable.GetUnderlyingType(typeof(TValue)) == null
			};
			MetaClass.AddProperty(a);
			return _this;
		}

		public T PersistentComputedAttribute<TValue>(string name, IMetaPrimitiveType type, Action<MetaPersistentComputedAttribute<TClass, TValue>> attributes = null)
		{
			MetaPersistentComputedAttribute<TClass, TValue> a = new MetaPersistentComputedAttribute<TClass, TValue> {
				Name = name, Type = type, IsMultilingual = false,
				IsRequired = Nullable.GetUnderlyingType(typeof(TValue)) == null
			};
			if (attributes != null) attributes(a);
			MetaClass.AddProperty(a);
			return _this;
		}

		public T PersistentComputedAttribute<TValue>(string name, Action<MetaPersistentComputedAttribute<TClass, TValue>> attributes = null)
		{
			MetaPersistentComputedAttribute<TClass, TValue> a = new MetaPersistentComputedAttribute<TClass, TValue> {
				Name = name, IsMultilingual = false,
				Type = TypeFactory.FromCSharpType(typeof(TValue)),
				IsRequired = Nullable.GetUnderlyingType(typeof(TValue)) == null
			};
			if (attributes != null) attributes(a);
			MetaClass.AddProperty(a);
			return _this;
		}

		public T PersistentComputedAttribute<TValue>(string name, bool isRequired, Action<MetaPersistentComputedAttribute<TClass, TValue>> attributes = null)
		{
			Action<MetaPersistentComputedAttribute<TClass, TValue>> a = x => { x.IsRequired = isRequired; if (attributes != null) attributes(x); };
			return PersistentComputedAttribute<TValue>(name, TypeFactory.FromCSharpType(typeof(TValue)), a);
		}

		public T Operation(string name, Action<IMetaOperation> attributes = null)
		{
			var op = new MetaOperation { Name = name };
			MetaClass.AddOperation(op);
			if (attributes != null) attributes(op);
			return _this;
		}

		public T Attribute<TValue>(string name, Action<MetaAttribute<TClass, TValue>> attributes = null)
		{
			return Attribute<TValue>(name, TypeFactory.FromCSharpType(typeof(TValue)), attributes);
		}

		public T Attribute<TValue>(string name, bool isRequired, Action<MetaAttribute<TClass, TValue>> attributes = null)
		{
			Action<MetaAttribute<TClass, TValue>> a = x => { x.IsRequired = isRequired; if (attributes != null) attributes(x); };
			return Attribute<TValue>(name, TypeFactory.FromCSharpType(typeof(TValue)), a);
		}

		public T Attribute<TValue>(string name, IMetaPrimitiveType type, bool isRequired, Action<MetaAttribute<TClass, TValue>> attributes = null)
		{
			Action<MetaAttribute<TClass, TValue>> a = x => { x.IsRequired = isRequired; if (attributes != null) attributes(x); };
			return Attribute<TValue>(name, type, a);
		}

		public T TimeStamp<TUser, TKey>()
		{
			Attribute<DateTime>("LastModifiedDate", x => x.DefaultDBValue = "(getdate())");
			Reference<TUser, TKey>("LastModifiedUser");
			MetaClass.Interfaces.Add(typeof(IWithTimeStamp));
			return _this;
		}

		public T Title(bool isRequired = true)
		{
			Attribute<string>("Title", x => x.IsRequired = isRequired);
			MetaClass.Interfaces.Add(typeof(IWithTitle));
			return _this;
		}

		public T IsDeleted()
		{
			Attribute<bool>("IsDeleted", x => x.DefaultDBValue = "(0)");
			MetaClass.Interfaces.Add(typeof(IWithTitle));
			return _this;
		}
	}

	public class MetaClassBuilder<TClass> : AbstractMetaClassBuilder<MetaClassBuilder<TClass>, TClass>
	{
		public MetaClassBuilder(IMetaClass cls) : base(cls) { }
	}

	public class MetaClassBuilder<TClass, TClassData> : AbstractMetaClassBuilder<MetaClassBuilder<TClass, TClassData>, TClass>
	{
		public MetaClassBuilder(IMetaClass cls) : base(cls) { }

		public MetaClassBuilder<TClass, TClassData> MultilingualAttribute<TValue>(string name, IMetaPrimitiveType type, Action<MetaAttribute<TClassData, TValue>> attributes = null)
		{
			if (type == null) type = TypeFactory.String;
			MetaAttribute<TClassData, TValue> a = new MetaAttribute<TClassData, TValue> {
				Name = name, Type = type, IsRequired = Nullable.GetUnderlyingType(typeof(TValue)) == null, IsMultilingual = true
			};
			if (attributes != null) attributes(a);
			MetaClass.AddProperty(a);
			return _this;
		}

		public MetaClassBuilder<TClass, TClassData> MultilingualAttribute<TValue>(string name, Action<MetaAttribute<TClassData, TValue>> attributes = null)
		{
			return MultilingualAttribute<TValue>(name, TypeFactory.FromCSharpType(typeof(TValue)), attributes);
		}

		public MetaClassBuilder<TClass, TClassData> MultilingualAttribute<TValue>(string name, bool isRequired, Action<MetaAttribute<TClassData, TValue>> attributes = null)
		{
			Action<MetaAttribute<TClassData, TValue>> a = x => { x.IsRequired = isRequired; if (attributes != null) attributes(x); };
			return MultilingualAttribute<TValue>(name, TypeFactory.FromCSharpType(typeof(TValue)), a);
		}
	}
}