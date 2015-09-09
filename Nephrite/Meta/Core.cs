using System;
using System.Collections.Generic;
using System.Linq;
using Nephrite.Multilanguage;

namespace Nephrite.Meta
{
	/// <summary>
	/// Абстрактный класс для всех сущностей модели
	/// </summary>
	public abstract class MetaElement : IMetaElement
	{
		public MetaElement() { }
		public MetaElement(string name = "", string caption = "", string description = "")
		{
			Name = name;
			Caption = caption;
			Description = description;
		}

		public string Namespace { get; set; } = "Nephrite";

		/// <summary>
		/// Уникальный идентификатор
		/// </summary>
		public virtual string ID { get { return Namespace + "." + Name; } }
		/// <summary>
		/// Системное имя
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// Название на локальном языке
		/// </summary>
		public virtual string Caption { get; set; }

		/// <summary>
		/// Описание
		/// </summary>
		public string Description { get; set; }
		//public Dictionary<string, MetaTaggedValue> TaggedValues { get; set; }

		protected Dictionary<Type, IMetaStereotype> _stereotypes = new Dictionary<Type, IMetaStereotype>();

		public T Stereotype<T>() where T : IMetaStereotype
		{
			Type t = typeof(T);
			if (!_stereotypes.ContainsKey(t)) return default(T);
			return (T)_stereotypes[t];
		}

		public void AssignStereotype(IMetaStereotype stereotype)
		{
			Type t = stereotype.GetType();
			if (!_stereotypes.ContainsKey(t))
				_stereotypes.Add(t, stereotype);
			stereotype.Parent = this;
		}

		List<string> _tags = new List<string>();
		public List<string> Tags
		{
			get
			{
				return _tags;
			}
		}
	}

	/// <summary>
	/// Модель предметной области
	/// </summary>
	public partial class MetaSolution : MetaElement, IMetaSolution
	{
		public ITextResource TextResource { get; }

		public MetaSolution(ITextResource textResource = null)
		{
			TextResource = textResource;
        }

		Dictionary<string, IMetaClass> _classesbyname = new Dictionary<string, IMetaClass>(255);
		//Dictionary<string, MetaPackage> _packagesbyname = new Dictionary<string, MetaPackage>(32);
		Dictionary<string, IMetaEnum> _enumsbyname = new Dictionary<string, IMetaEnum>(32);

		/// <summary>
		/// Пакеты модели
		/// </summary>
		//public Dictionary<string, MetaPackage>.ValueCollection Packages { get { return _packagesbyname.Values; } }
		/// <summary>
		/// Классы модели
		/// </summary>
		public Dictionary<string, IMetaClass>.ValueCollection Classes { get { return _classesbyname.Values; } }
		/// <summary>
		/// Enums модели
		/// </summary>
		public Dictionary<string, IMetaEnum>.ValueCollection Enums { get { return _enumsbyname.Values; } }

		public void AddClass(IMetaClass metaClass)
		{
			metaClass.Parent = this;
			var key = metaClass.Name.ToLower();
			if (_classesbyname.ContainsKey(key))
				throw new Exception(String.Format("Class {0} already exists in the model", metaClass.Name));
			_classesbyname.Add(key, metaClass);
		}

		public IMetaClass AddClass<T>(string caption = "", string description = "")
		{
			IMetaClass c = new MetaClass { Name = typeof(T).Name, Caption = caption, Description = description, Persistent = PersistenceType.Table };
			AddClass(c);
			return c;
		}

		//public MetaPackage AddPackage(MetaPackage metaPackage)
		//{
		//	metaPackage.Solution = this;
		//	_packagesbyname.Add(metaPackage.Name.ToLower(), metaPackage);

		//	foreach (var c in metaPackage.Classes)
		//		AddClass(c);

		//	foreach (var e in metaPackage.Enums)
		//		AddEnum(e);

		//	return metaPackage;
		//}

		internal void AddEnum(IMetaEnum metaEnum)
		{
			var key = metaEnum.Name.ToLower();
			if (_enumsbyname.ContainsKey(key))
				throw new Exception(String.Format("Enum {0} already exists in the model", metaEnum.Name));
			_enumsbyname.Add(key, metaEnum);
		}

		public IMetaEnum AddEnum(string name, string caption = "", string description = "")
		{
			IMetaEnum c = new MetaEnum { Name = name, Caption = caption, Description = description };
			AddEnum(c);
			return c;
		}

		//public MetaPackage AddPackage(string name, string caption = "", string description = "")
		//{
		//	MetaPackage p = new MetaPackage { Name = name, Caption = caption, Description = description };
		//	AddPackage(p);
		//	return p;
		//}

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

		//public MetaPackage GetPackage(string name)
		//{
		//	string s = name.ToLower();
		//	return _packagesbyname.ContainsKey(s) ? _packagesbyname[s] : null;
		//}

		public IMetaEnum GetEnum(string name)
		{
			string s = name.ToLower();
			return _enumsbyname.ContainsKey(s) ? _enumsbyname[s] : null;
		}
	}

	//public class MetaPackage : MetaElement, IMetaOperationContainer
	//{
	//	public MetaPackage(string name = "", string caption = "", string description = "") : base(name, caption, description) { }

	//	public string ParentID { get; set; }
	//	MetaPackage _parent = null;

	//	/// <summary>
	//	/// Родительский пакет
	//	/// </summary>
	//	public MetaPackage Parent
	//	{
	//		get
	//		{
	//			if (_parent == null && !String.IsNullOrEmpty(ParentID)) _parent = Solution.GetPackage(ParentID);
	//			return _parent;
	//		}
	//	}

	//	/// <summary>
	//	/// Модель, которой принадлежит пакет
	//	/// </summary>
	//	public MetaSolution Solution { get; internal set; }
	//	Dictionary<string, MetaPackage> _packages = new Dictionary<string, MetaPackage>(16);
	//	Dictionary<string, MetaClass> _classes = new Dictionary<string, MetaClass>(64);
	//	Dictionary<string, MetaOperation> _operations = new Dictionary<string, MetaOperation>(16);
	//	Dictionary<string, MetaEnum> _enums = new Dictionary<string, MetaEnum>(32);

	//	/// <summary>
	//	/// Вложенные пакеты
	//	/// </summary>
	//	public Dictionary<string, MetaPackage>.ValueCollection Packages { get { return _packages.Values; } }
	//	/// <summary>
	//	/// Классы пакета
	//	/// </summary>
	//	public Dictionary<string, MetaClass>.ValueCollection Classes { get { return _classes.Values; } }
	//	/// <summary>
	//	/// Операции пакета
	//	/// </summary>
	//	public Dictionary<string, MetaOperation>.ValueCollection Operations { get { return _operations.Values; } }

	//	public Dictionary<string, MetaEnum>.ValueCollection Enums { get { return _enums.Values; } }

	//	public void AddClass(MetaClass metaClass)
	//	{
	//		if (Solution != null) Solution.AddClass(metaClass);
	//		metaClass.Parent = this;
	//		_classes.Add(metaClass.Name.ToLower(), metaClass);
	//	}

	//	public MetaClass AddClass(string name, string caption = "", string description = "")
	//	{
	//		MetaClass c = new MetaClass { Name = name, Caption = caption, Description = description, IsPersistent = true };
	//		AddClass(c);
	//		return c;
	//	}

	//	public MetaClass AddClass<T>(string caption = "", string description = "")
	//	{
	//		MetaClass c = new MetaClass { Name = typeof(T).Name, Caption = caption, Description = description, IsPersistent = true };
	//		AddClass(c);
	//		return c;
	//	}

	//	public MetaPackage AddPackage(MetaPackage metaPackage)
	//	{
	//		metaPackage.ParentID = this.ID;
	//		//metaPackage.Solution = this.Solution;

	//		//foreach (var c in metaPackage.Classes)
	//		//	Solution.AddClass(c);

	//		//foreach (var e in metaPackage.Enums)
	//		//	Solution.AddEnum(e);

	//		Solution.AddPackage(metaPackage);
	//		_packages.Add(metaPackage.Name.ToLower(), metaPackage);
	//		return metaPackage;
	//	}

	//	public void AddEnum(MetaEnum metaEnum)
	//	{
	//		if (Solution != null) Solution.AddEnum(metaEnum);
	//		_enums.Add(metaEnum.Name.ToLower(), metaEnum);
	//	}

	//	public MetaEnum AddEnum(string name, string caption = "", string description = "")
	//	{
	//		MetaEnum c = new MetaEnum { Name = name, Caption = caption, Description = description };
	//		AddEnum(c);
	//		return c;
	//	}


	//	public MetaPackage AddPackage(string name, string caption = "", string description = "")
	//	{
	//		MetaPackage p = new MetaPackage { Name = name, Caption = caption, Description = description };
	//		AddPackage(p);
	//		return p;
	//	}

	//	public void AddOperation(MetaOperation metaOperation)
	//	{
	//		metaOperation.Parent = this;
	//		_operations.Add(metaOperation.Name.ToLower(), metaOperation);
	//	}

	//	public MetaClass GetClass(string name)
	//	{
	//		string s = name.ToLower();
	//		return _classes.ContainsKey(s) ? _classes[s] : null;
	//	}

	//	public MetaPackage GetPackage(string name)
	//	{
	//		string s = name.ToLower();
	//		return _packages.ContainsKey(s) ? _packages[s] : null;
	//	}

	//	public MetaOperation GetOperation(string name)
	//	{
	//		string s = name.ToLower();
	//		return _operations.ContainsKey(s) ? _operations[s] : null;
	//	}

	//	public override string ID
	//	{
	//		get
	//		{
	//			return Name;
	//		}
	//	}
	//}

	public abstract partial class MetaClassifier : MetaElement, IMetaClassifier
	{
		public abstract string CLRType { get; }

		public virtual string ColumnName(string propName)
		{
			return propName;
		}
	}

	public partial class MetaClass : MetaClassifier, IMetaClass
	{
		public override string Caption
		{
			get
			{
				if (Parent.TextResource == null) return base.Caption;
                return Parent.TextResource.Get(ID, base.Caption);
			}
			set
			{
				base.Caption = value;
			}
		}

		string _captionPlural = "";
		public string CaptionPlural
		{
			get
			{
				if (Parent.TextResource == null) return _captionPlural;
				return Parent.TextResource.Get(ID + "-pl", _captionPlural);
			}
			set
			{
				_captionPlural = value;
			}
		}

		public override string CLRType
		{
			get
			{
				return IsMultilingual ? "V_" + Name : Name;
			}
		}

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

		bool _isMultilingual = false;

		/// <summary>
		/// Есть ли у класса мультиязычные свойства
		/// </summary>
		public bool IsMultilingual { get { return _isMultilingual; } }

		List<IMetaProperty> _compositeKey = new List<IMetaProperty>();
		/// <summary>
		/// Свойство класса, являющееся первичным ключом (если оно одно)
		/// </summary>
		public IMetaProperty Key
		{
			get
			{
				if (_compositeKey.Count != 1)
					throw new Exception(String.Format("Error while getting single key property for class {0}. Length of the key properties array: {1}", Name, _compositeKey.Count()));
				else
					return _compositeKey.First();
			}
		}

		/// <summary>
		/// Все свойства класса, входящие в первичный ключ
		/// </summary>
		public List<IMetaProperty> CompositeKey
		{
			get { return _compositeKey; }
		}

		/// <summary>
		/// Свойства класса
		/// </summary>
		public Dictionary<string, IMetaProperty>.ValueCollection Properties { get { return _properties.Values; } }
		public Dictionary<string, IMetaProperty>.KeyCollection PropertyNames { get { return _properties.Keys; } }
		public Dictionary<string, IMetaProperty>.ValueCollection AllProperties
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
		public Dictionary<string, IMetaOperation>.ValueCollection Operations { get { return _operations.Values; } }
		public Dictionary<string, IMetaOperation>.KeyCollection OperationNames { get { return _operations.Keys; } }

		/// <summary>
		/// Метод класса по умолчанию
		/// </summary>
		public IMetaOperation DefaultOperation { get; set; }

		public void AddProperty(IMetaProperty metaProperty)
		{
			metaProperty.Parent = this;
			_properties.Add(metaProperty.Name.ToLower(), metaProperty);
			if (metaProperty is MetaAttribute && (metaProperty as MetaAttribute).IsMultilingual) _isMultilingual = true;
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

		/// <summary>
		/// Имя столбца для свойств, которые ссылаются на данный класс
		/// </summary>
		public override string ColumnName(string propName)
		{
			return propName + (Key.Type as IMetaIdentifierType).ColumnSuffix;
		}

		public IQueryable AllObjects { get; set; }

		List<Type> _interfaces = new List<Type>();
		public List<Type> Interfaces
		{
			get
			{
				return _interfaces;
			}
		}
	}

	public abstract partial class MetaProperty : MetaElement, IMetaProperty
	{
		public override string Caption
		{
			get
			{
				if (Parent.Parent.TextResource == null) return base.Caption;
				return Parent.Parent.TextResource.Get(ID, base.Caption);
			}
			set
			{
				base.Caption = value;
			}
		}

		string _captionShort = "";
		public string CaptionShort
		{
			get
			{
				string res = _captionShort;
                if (Parent.Parent.TextResource != null)
					res = Parent.Parent.TextResource.Get(ID + "-s", res);

				return String.IsNullOrEmpty(res) ? Caption : res;
			}
			set { _captionShort = value; }
		}

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
		public int UpperBound { get { return _upperBound; } set { _upperBound = value; } }
		int _upperBound = 1;

		public override string ID
		{
			get
			{
				return Parent.ID + "." + Name;
			}
		}

		/// <summary>
		/// Имя столбца в базе данных
		/// </summary>
		public virtual string ColumnName
		{
			get { return Type == null ? Name : Type.ColumnName(Name); }
		}

		// Func<TClass, TValue>
		public object GetValue { get; set; }
		// Action<TClass, TValue>
		public object SetValue { get; set; }
		// Expression<Func<TClass, TValue>>
		public object GetValueExpression { get; set; }

		public abstract string GetStringValue<TClass>(TClass obj, string format = "", IFormatProvider provider = null);

	}

	public partial class MetaValueProperty : MetaProperty, IMetaValueProperty
	{
		/// <summary>
		/// Является мультиязычным
		/// </summary>
		public bool IsMultilingual { get; set; }

		public override string GetStringValue<TClass>(TClass obj, string format = "", IFormatProvider provider = null)
		{
			var pt = Type;
			if (pt.GetStringValue == null) return "";
			dynamic valueGetter = GetValue;
			dynamic val = valueGetter(obj);
			if (val == null) return "";
			dynamic getter = pt.GetStringValue;
			return getter(val, null, null).ToString();
		}
	}

	/// <summary>
	/// Атрибут класса
	/// </summary>
	public partial class MetaAttribute : MetaValueProperty
	{		
		/// <summary>
		/// Является автоинкрементным
		/// </summary>
		public bool IsIdentity { get; set; }
	}

	/// <summary>
	/// Вычислимое свойство класса
	/// </summary>
	public partial class MetaComputedAttribute : MetaValueProperty, IMetaComputedAttribute
	{
		public string GetExpressionString { get; set; }
		public string SetExpressionString { get; set; }
	}

	/// <summary>
	/// Вычислимое на уровне базы данных свойство класса
	/// </summary>
	public partial class MetaPersistentComputedAttribute : MetaValueProperty, IMetaPersistentComputedAttribute
	{
		/// <summary>
		/// Выражение
		/// </summary>
		public string Expression { get; set; }

	}



	/// <summary>
	/// Свойство класса - ссылка 
	/// </summary>
	public partial class MetaReference : MetaProperty, IMetaReference
	{
		public MetaReference(string name, string caption, string refClassName, bool isRequired = false,
			int upperBound = 1, AssociationType associationType = AssociationType.Default,
			string inversePropertyName = "", string description = "")
		{
			Name = name;
			Caption = caption;
			_refClassName = refClassName;
			_inversePropertyName = inversePropertyName;
			Description = description;
			UpperBound = upperBound;
			IsRequired = isRequired;
			AssociationType = associationType;
		}

		/// <summary>
		/// Тип ассоциации
		/// </summary>
		public AssociationType AssociationType { get; set; }

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
					if (_refClass == null) _refClass = Parent.Parent.GetClass(_refClassName);
					_type = _refClass.Key.Type.Clone(IsRequired);
				}
				return _type;
			}
			set
			{
				throw new Exception("Changing reference type is unsupported");
			}
		}

		string _inversePropertyName = null;
		IMetaReference _refInverseProperty = null;
		/// <summary>
		/// Является ли ссылка обратной (т.е. у класса, на который ссылается свойство есть тоже ссылка на данный класс)
		/// </summary>
		public IMetaReference InverseProperty
		{
			get
			{
				if (String.IsNullOrEmpty(_inversePropertyName)) return null;
				if (_refInverseProperty == null && RefClass != null) _refInverseProperty = RefClass.GetProperty(_inversePropertyName) as MetaReference;
				return _refInverseProperty;
			}
		}
		public void SetInverseProperty(string inversePropertyName)
		{
			_refInverseProperty = null;
			_inversePropertyName = inversePropertyName;
        }

		/// <summary>
		/// Имя столбца в базе данных
		/// </summary>
		public override string ColumnName
		{
			get
			{
				return RefClass.ColumnName(Name);
			}
		}


		public IQueryable AllObjects { get; set; }
		public string DataTextField { get; set; }

		// Func<TClass, TValue>
		public object GetValueID { get; set; }
		// Action<TClass, TValue>
		public object SetValueID { get; set; }
		// Expression<Func<TClass, TValue>>
		public object GetValueIDExpression { get; set; }


		public override string GetStringValue<TClass>(TClass obj, string format = "", IFormatProvider provider = null)
		{
			dynamic valueGetter = GetValue;
			dynamic refObj = valueGetter(obj);
			if (refObj != null && refObj is IWithTitle)
				return (refObj as IWithTitle).Title;
			else
				return "";
		}
	}

	/// <summary>
	/// Параметр метода
	/// </summary>
	public class MetaParameter : MetaElement, IMetaParameter
	{
		/// <summary>
		/// Тип данных
		/// </summary>
		public IMetaParameterType Type { get; set; }
	}

	/// <summary>
	/// Метод класса
	/// </summary>
	public class MetaOperation : MetaElement, IMetaOperation
	{
		List<IMetaParameter> _parameters = new List<IMetaParameter>();

		public override string Caption
		{
			get
			{
				if (Parent.Parent.TextResource == null) return base.Caption;
				return Parent.Parent.TextResource.Get(ID, base.Caption);
			}
			set
			{
				base.Caption = value;
			}
		}

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

		//public Action Invoke { get; set; }

		public string ActionString { get; set; }
		public string PredicateString { get; set; }

		//public ViewEngineType ViewEngine { get; set; }
		//public string ViewName { get; set; }
		//public string ViewClass { get; set; }
		//public string DTOClass { get; set; }

		//InteractionType _interactionType = InteractionType.OneWayView;
  //      public InteractionType InteractionType { get { return _interactionType; } set { _interactionType = value; } }
		//public DTOClassKind DTOClassKind { get; set; }

		public string ParametersString
		{
			get
			{
				return string.Join(", ", Parameters.Select(o => o.Type.CLRType + " " + o.Name));
			}
		}
	}

	

	public class MetaEnum : MetaElement, IMetaEnum
	{
		public IMetaSolution Parent { get; set; }

		List<IMetaEnumValue> _values = new List<IMetaEnumValue>();
		public List<IMetaEnumValue> Values { get { return _values; } }
	}

	public class MetaEnumValue : MetaElement, IMetaEnumValue
	{
		string _value = "";

		public MetaEnumValue(string id, string name, string caption)
			: base(name, caption)
		{
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

	public abstract class MetaStereotype : MetaElement, IMetaStereotype
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

	
}