using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Tango.Meta
{
	public abstract class MetaElement : IMetaElement
	{
		/// <summary>
		/// Уникальный идентификатор
		/// </summary>
		public virtual string ID { get; protected set; }

		protected Dictionary<Type, IMetaStereotype> _stereotypes = new Dictionary<Type, IMetaStereotype>();

		public T Stereotype<T>() where T : IMetaStereotype
		{
			Type t = typeof(T);
			if (!_stereotypes.ContainsKey(t)) return default;
			return (T)_stereotypes[t];
		}

		public void AssignStereotype(IMetaStereotype stereotype)
		{
			Type t = stereotype.GetType();
			if (!_stereotypes.ContainsKey(t))
				_stereotypes.Add(t, stereotype);
			stereotype.Parent = this;
		}
	}


	/// <summary>
	/// Абстрактный класс для всех сущностей модели
	/// </summary>
	public abstract class MetaNamedElement : MetaElement, IMetaNamedElement
	{
		public string Namespace { get; set; } = "Tango";
		public override string ID { get { return Namespace + "." + Name; } }

		/// <summary>
		/// Системное имя
		/// </summary>
		public string Name { get; set; }
	}

	/// <summary>
	/// Модель предметной области
	/// </summary>
	public class MetaSolution : MetaNamedElement, IMetaSolution
	{
		Dictionary<string, IMetaClass> _classesbyname = new Dictionary<string, IMetaClass>(255);
		Dictionary<string, IMetaEnum> _enumsbyname = new Dictionary<string, IMetaEnum>(32);

		/// <summary>
		/// Классы модели
		/// </summary>
		public IEnumerable<IMetaClass> Classes { get { return _classesbyname.Values; } }
		/// <summary>
		/// Enums модели
		/// </summary>
		public IEnumerable<IMetaEnum> Enums { get { return _enumsbyname.Values; } }

		public void AddClass(IMetaClass metaClass)
		{
			metaClass.Parent = this;
			var key = metaClass.Name.ToLower();
			if (_classesbyname.ContainsKey(key))
				throw new Exception(String.Format("Class {0} already exists in the model", metaClass.Name));
			_classesbyname.Add(key, metaClass);
		}

		public IMetaClass AddClass<T>()
		{
			var c = new MetaClass(typeof(T));
			AddClass(c);
			return c;
		}

		internal void AddEnum(IMetaEnum metaEnum)
		{
			var key = metaEnum.Name.ToLower();
			if (_enumsbyname.ContainsKey(key))
				throw new Exception(String.Format("Enum {0} already exists in the model", metaEnum.Name));
			_enumsbyname.Add(key, metaEnum);
		}

		public IMetaEnum AddEnum(string name)
		{
			IMetaEnum c = new MetaEnum { Name = name };
			AddEnum(c);
			return c;
		}

		public IMetaClass GetClass(string name)
		{
			string s = name.ToLower();
			return _classesbyname.ContainsKey(s) ? _classesbyname[s] : null;
		}

		public IMetaOperation GetOperation(string className, string operationName)
		{
			IMetaClass c = GetClass(className);
			if (c == null) return null;
			return c.GetOperation(operationName);
		}

		public IMetaEnum GetEnum(string name)
		{
			string s = name.ToLower();
			return _enumsbyname.ContainsKey(s) ? _enumsbyname[s] : null;
		}
	}

	public class MetaClass : MetaNamedElement, IMetaClass
	{
		public MetaClass(Type t)
		{
			Name = t.Name;
			Namespace = t.Namespace;
			Persistent = PersistenceType.Table;
			if (t.BaseType != null)
				BaseClassName = t.BaseType.Name;
		}

		public string CaptionPlural { get; set; }

		/// <summary>
		/// Модель, в которой располагается класс
		/// </summary>
		public IMetaSolution Parent { get; set; }

		Dictionary<string, IMetaProperty> _properties = new Dictionary<string, IMetaProperty>();
		Dictionary<string, IMetaProperty> _allproperties = null;
		Dictionary<string, IMetaOperation> _operations = new Dictionary<string, IMetaOperation>();
		List<IMetaParameter> _parameters = null;

		internal string BaseClassName { get; set; }
		IMetaClass _baseClass = null;

		/// <summary>
		/// От какого класса унаследован
		/// </summary>
		public IMetaClass BaseClass
		{
			get
			{
				if (_baseClass == null && !String.IsNullOrEmpty(BaseClassName)) _baseClass = Parent.GetClass(BaseClassName);
				return _baseClass;
			}
		}

		/// <summary>
		/// Сохраняются ли объекты класса в базе данных
		/// </summary>
		public PersistenceType Persistent { get; set; }

		/// <summary>
		/// Есть ли у класса мультиязычные свойства
		/// </summary>
		public bool IsMultilingual { get; private set; } = false;

		/// <summary>
		/// Свойство класса, являющееся первичным ключом (если оно одно)
		/// </summary>
		public IMetaProperty Key
		{
			get
			{
				if (CompositeKey.Count != 1)
					if (BaseClass != null)
						return BaseClass.Key;
					else
						throw new Exception(String.Format("Error while getting single key property for class {0}. Length of the key properties array: {1}", Name, CompositeKey.Count()));
				else
					return CompositeKey.First();
			}
		}

		/// <summary>
		/// Все свойства класса, входящие в первичный ключ
		/// </summary>
		public List<IMetaProperty> CompositeKey { get; } = new List<IMetaProperty>();

		/// <summary>
		/// Свойства класса
		/// </summary>
		public IEnumerable<IMetaProperty> Properties { get { return _properties.Values; } }
		public IEnumerable<string> PropertyNames { get { return _properties.Keys; } }
		public IEnumerable<IMetaProperty> AllProperties
		{
			get
			{
				if (_allproperties == null)
				{
					Dictionary<string, IMetaProperty> p = new Dictionary<string, IMetaProperty>(_properties);
					if (BaseClass != null)
						foreach (var kv in BaseClass.AllProperties)
							p.Add(kv.Name, kv);
					_allproperties = p;
				}
				return _allproperties.Values;
			}
		}
		public List<IMetaParameter> Parameters 
		{
			get 
			{
				if (_parameters == null)
					_parameters = new List<IMetaParameter>() ;
				return _parameters; 
			} 
		}

		/// <summary>
		/// Методы класса
		/// </summary>
		public IEnumerable<IMetaOperation> Operations { get { return _operations.Values; } }
		public IEnumerable<string> OperationNames { get { return _operations.Keys; } }

		/// <summary>
		/// Метод класса по умолчанию
		/// </summary>
		public IMetaOperation DefaultOperation { get; set; }

		public void AddProperty(IMetaProperty metaProperty)
		{
			metaProperty.Parent = this;
			if (_properties.ContainsKey(metaProperty.Name.ToLower()))
				throw new Exception(String.Format("Property {0} already exists in the class {1}", metaProperty.Name, Name));
			_properties.Add(metaProperty.Name.ToLower(), metaProperty);
			if (metaProperty is ICanBeMultilingual && (metaProperty as ICanBeMultilingual).IsMultilingual) IsMultilingual = true;
		}
		public void AddOperation(IMetaOperation metaOperation)
		{
			metaOperation.Parent = this;
			_operations.Add(metaOperation.Name.ToLower(), metaOperation);
		}

		public IMetaProperty GetProperty(string name)
		{
			string s = name.ToLower();
			return _properties.ContainsKey(s) ? _properties[s] : null;
		}
		public IMetaOperation GetOperation(string name)
		{
			string s = name.ToLower();
			return _operations.ContainsKey(s) ? _operations[s] : null;
		}

		public string LogicalDeleteExpressionString { get; set; }
		public string DefaultOrderByExpressionString { get; set; }
		public List<Type> Interfaces { get; } = new List<Type>();
	}

	public abstract class MetaProperty : MetaNamedElement, IMetaProperty
	{
		/// <summary>
		/// Класс, к которому принадлежит свойство
		/// </summary>
		public IMetaClass Parent { get; set; }

		public virtual string DefaultDBValue { get; set; }

		/// <summary>
		/// Тип данных
		/// </summary>
		public virtual IMetaPrimitiveType Type { get; set; }
		/// <summary>
		/// Является ли свойство обязательным для заполнения
		/// </summary>
		public bool IsRequired { get; set; }
		/// <summary>
		/// Верхняя граница: 1 - максимум одно значение, -1 - значений может быть сколько угодно
		/// </summary>
		public int UpperBound { get; set; } = 1;

		public override string ID
		{
			get
			{
				if (Parent == null) throw new Exception(Name + " has no parent");
				return Parent.ID + "." + Name;
			}
		}

		/// <summary>
		/// Имя столбца в базе данных
		/// </summary>
		public virtual string ColumnName
		{
			get { return Name; }
		}
	}

	public abstract class MetaProperty<TClass, TValue> : MetaProperty, IMetaProperty<TClass, TValue>
	{
		public Func<TClass, TValue> GetValue { get; set; }
		public Action<TClass, TValue> SetValue { get; set; }
		public Expression<Func<TClass, TValue>> GetValueExpression { get; set; }
		
		public abstract string GetStringValue(TClass obj, string format = "", IFormatProvider provider = null);
	}

	public abstract class MetaValueProperty<TClass, TValue> : MetaProperty<TClass, TValue>, IMetaValueProperty, ICanBeMultilingual
	{
		/// <summary>
		/// Является мультиязычным
		/// </summary>
		public bool IsMultilingual { get; set; }

		public Func<TValue, string, IFormatProvider, string> StringConverter { get; set; }
		public override string GetStringValue(TClass obj, string format = "", IFormatProvider provider = null)
		{
			if (StringConverter == null) return "";
			var val = GetValue(obj);
			if (val == null) return "";
			return StringConverter(val, format, provider);
		}
	}

	/// <summary>
	/// Атрибут класса
	/// </summary>
	public class MetaAttribute<TClass, TValue> : MetaValueProperty<TClass, TValue>, ICanBeMultilingual, ICanBeIdentity
	{
		/// <summary>
		/// Является автоинкрементным
		/// </summary>
		public bool IsIdentity { get; set; }
	}

	/// <summary>
	/// Вычислимое свойство класса
	/// </summary>
	public class MetaComputedAttribute<TClass, TValue> : MetaValueProperty<TClass, TValue>, IMetaComputedAttribute
	{

	}

	/// <summary>
	/// Вычислимое на уровне базы данных свойство класса
	/// </summary>
	public class MetaPersistentComputedAttribute<TClass, TValue> : MetaValueProperty<TClass, TValue>, IMetaPersistentComputedAttribute
	{
		/// <summary>
		/// Выражение
		/// </summary>
		public string Expression { get; set; }

	}



	/// <summary>
	/// Свойство класса - ссылка 
	/// </summary>
	public abstract class MetaReference : MetaProperty, IMetaReference
	{
		/// <summary>
		/// Тип ассоциации
		/// </summary>
		public AssociationType AssociationType { get; set; } = AssociationType.Default;

		string _refClassName = null;
		IMetaClass _refClass = null;
		/// <summary>
		/// На какой класс ссылается
		/// </summary>
		public IMetaClass RefClass
		{
			get
			{
				if (_refClass == null) _refClass = Parent.Parent.GetClass(_refClassName);
				return _refClass;
			}
		}
		public void SetRefClass(string refClassName)
		{
			_refClass = null;
			_refClassName = refClassName;
		}


		IMetaPrimitiveType _type = null;
		/// <summary>
		/// Тип данных
		/// </summary>
		public override IMetaPrimitiveType Type
		{
			get
			{
				if (_type == null)
				{
					if (_refClassName == null) throw new Exception(ID + " doesn't have a name of RefClass");
					if (_refClass == null) _refClass = Parent.Parent.GetClass(_refClassName);
					if (_refClass == null) throw new Exception(ID + ": can't get class for " + _refClassName);
					if (_refClass.Key == null) throw new Exception(_refClass.ID + " doesn't have a key");
					_type = _refClass.Key.Type;
				}
				return _type;
			}
			set
			{
				throw new Exception("Changing reference type is unsupported");
			}
		}

		string _inversePropertyName = null;
		IMetaProperty _refInverseProperty = null;
		/// <summary>
		/// Является ли ссылка обратной (т.е. у класса, на который ссылается свойство есть тоже ссылка на данный класс)
		/// </summary>
		public IMetaProperty InverseProperty
		{
			get
			{
				if (String.IsNullOrEmpty(_inversePropertyName)) return null;
				if (_refInverseProperty == null && RefClass != null) _refInverseProperty = RefClass.GetProperty(_inversePropertyName);
				return _refInverseProperty;
			}
		}
		public void SetInverseProperty(string inversePropertyName)
		{
			_refInverseProperty = null;
			_inversePropertyName = inversePropertyName;
        }

		ColumnNameFuncDelegate _columnNameFunc = (name, suffix) => name + suffix;
		public void SetColumnNameFunc(ColumnNameFuncDelegate func)
		{
			_columnNameFunc = func;
		}

		/// <summary>
		/// Имя столбца в базе данных
		/// </summary>
		public override string ColumnName
		{
			get
			{
				if (RefClass == null) throw new Exception(ID + " doesn't have a RefClass");
				if (RefClass.Key == null) throw new Exception(RefClass.ID + " doesn't have a key.");
				if (RefClass.Key.Type as IMetaIdentifierType == null) throw new Exception(RefClass.ID + " has a non IMetaIdentifierType key.");
				return _columnNameFunc(Name, (RefClass.Key.Type as IMetaIdentifierType).ColumnSuffix);
			}
		}
	}

	public delegate string ColumnNameFuncDelegate(string name, string columnSuffix);

	public class MetaReference<TClass, TRefClass> : MetaReference, IMetaReference<TClass, TRefClass>
	{
		public MetaReference()
		{
			UpperBound = -1;
		}

		public Func<TClass, IQueryable<TRefClass>> GetValue { get; set; }
		public Action<TClass, IQueryable<TRefClass>> SetValue { get; set; }
		public Expression<Func<TClass, IQueryable<TRefClass>>> GetValueExpression { get; set; }

		public string GetStringValue(TClass obj, string format = "", IFormatProvider provider = null)
		{
			var objs = GetValue(obj);
			if (typeof(TRefClass).IsAssignableFrom(typeof(IWithTitle)))
				return objs.Cast<IWithTitle>().Select(o => o.Title).Join(", ");
			else if (typeof(TRefClass).IsAssignableFrom(typeof(IWithKey<int>)))
				return objs.Cast<IWithKey<int>>().Select(o => o.ID.ToString()).Join(", ");
			else if (typeof(TRefClass).IsAssignableFrom(typeof(IWithKey<Guid>)))
				return objs.Cast<IWithKey<Guid>>().Select(o => o.ID.ToString()).Join(", ");
			else
				return "";
		}
	}

	public class MetaReference<TClass, TRefClass, TKey> : MetaReference, IMetaReference<TClass, TRefClass, TKey>
	{
		public Func<TClass, TRefClass> GetValue { get; set; }
		public Action<TClass, TRefClass> SetValue { get; set; }
		public Expression<Func<TClass, TRefClass>> GetValueExpression { get; set; }

		public Func<TClass, TKey> GetValueID { get; set; }
		public Action<TClass, TKey> SetValueID { get; set; }
		public Expression<Func<TClass, TKey>> GetValueIDExpression { get; set; }

		public string GetStringValue(TClass obj, string format = "", IFormatProvider provider = null)
		{
			var refObj = GetValue(obj);
			if (refObj != null && refObj is IWithTitle)
				return (refObj as IWithTitle).Title;
			else
				return "";
		}
	}

	/// <summary>
	/// Параметр метода
	/// </summary>
	public class MetaParameter : MetaNamedElement, IMetaParameter
	{
		/// <summary>
		/// Тип данных
		/// </summary>
		public IMetaParameterType Type { get; set; }
	}

	/// <summary>
	/// Метод класса
	/// </summary>
	public class MetaOperation : MetaNamedElement, IMetaOperation
	{
		List<IMetaParameter> _parameters = new List<IMetaParameter>();

		/// <summary>
		/// Класс, которому принадлежит метод
		/// </summary>
		public IMetaClass Parent { get; set; }
		/// <summary>
		/// Параметры метода
		/// </summary>
		public List<IMetaParameter> Parameters { get { return _parameters; } set { _parameters = value; } }
		/// <summary>
		/// Иконка
		/// </summary>
		public string Image { get; set; }

		public override string ID
		{
			get
			{
				return Parent.ID + "." + Name;
			}
		}

		public string ActionString { get; set; }
		public string PredicateString { get; set; }
		public string ParametersString
		{
			get
			{
				return string.Join(", ", Parameters.Select(o => o.Type.ToCSharpType(false) + " " + o.Name));
			}
		}
	}

	public class MetaEnum : MetaNamedElement, IMetaEnum
	{
		public IMetaSolution Parent { get; set; }

		List<IMetaEnumValue> _values = new List<IMetaEnumValue>();
		public List<IMetaEnumValue> Values { get { return _values; } }
	}

	public class MetaEnumValue : MetaNamedElement, IMetaEnumValue
	{
		string _value = "";

		public MetaEnumValue(string id, string name)
		{
			Name = name;
			_value = id;
		}

		public override string ID
		{
			get
			{
				return _value;
			}
		}
	}	

	public abstract class MetaStereotype : MetaNamedElement, IMetaStereotype
	{
		public IMetaElement Parent { get; set; }

		public override string ID
		{
			get
			{
				return Namespace + ".Stereotype." + Name;
			}
		}
	}

	public interface IMetaClassDescription
	{
		IMetaClass GetInfo();
	}
}