using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.HtmlControls;
using Nephrite.Web;
using Nephrite.Web.TextResources;


namespace Nephrite.Meta
{
	/// <summary>
	/// Абстрактный класс для всех сущностей модели
	/// </summary>
	public abstract class MetaElement
	{
		public MetaElement()
		{
		}
		public MetaElement(Guid id, string name)
		{
			ID = id;
			Name = name;
		}
		public MetaElement(Guid id, string name, string caption)
		{
			ID = id;
			Name = name;
			Caption = caption;
		}
		public MetaElement(Guid id, string name, string caption, string description)
		{
			ID = id;
			Name = name;
			Caption = caption;
			Description = description;
		}

		/// <summary>
		/// Уникальный идентификатор
		/// </summary>
		public Guid ID { get; set; }
		/// <summary>
		/// Системное имя
		/// </summary>
		public string Name { get; set; }

		#region Caption - работа с многоязычностью
		public virtual string GetCaptionResourceKey()
		{
			throw new InvalidOperationException("Элемент " + Name + " не имеет ключа ресурса");
		}
		string _caption;
		/// <summary>
		/// Название на локальном языке
		/// </summary>
		public string Caption
		{
			get
			{
				return TextResource.Get(GetCaptionResourceKey(), _caption);
			}
			set
			{
				_caption = value;
			}
		}
		#endregion

		/// <summary>
		/// Описание
		/// </summary>
		public string Description { get; set; }
		//public Dictionary<string, MetaTaggedValue> TaggedValues { get; set; }

		Dictionary<Type, MetaStereotype> _stereotypes = new Dictionary<Type, MetaStereotype>();

		public void AddStereotype(MetaStereotype stereotype)
		{
			Type t = stereotype.GetType();
			_stereotypes.Add(t, stereotype);
			Stereotypes.AssignStereotype(t, this);
			stereotype.Parent = this;
		}

		public T S<T>() where T : MetaStereotype
		{

			return (T)_stereotypes[typeof(T)];
		}
	}

	/// <summary>
	/// Модель предметной области
	/// </summary>
	public partial class MetaSolution : MetaElement
	{
		Dictionary<string, MetaClass> _classesbyname = new Dictionary<string, MetaClass>(255);
		Dictionary<Guid, MetaClass> _classesbyid = new Dictionary<Guid, MetaClass>(255);

		Dictionary<string, MetaPackage> _packagesbyname = new Dictionary<string, MetaPackage>(32);
		Dictionary<Guid, MetaPackage> _packagesbyid = new Dictionary<Guid, MetaPackage>(32);

		/// <summary>
		/// Пакеты модели
		/// </summary>
		public Dictionary<Guid, MetaPackage>.ValueCollection Packages { get { return _packagesbyid.Values; } }
		/// <summary>
		/// Классы модели
		/// </summary>
		public Dictionary<Guid, MetaClass>.ValueCollection Classes { get { return _classesbyid.Values; } }

		//public string EntryPoint { get; }
		public void AddClass(MetaClass metaClass)
		{
			metaClass.Solution = this;
			_classesbyname.Add(metaClass.Name.ToLower(), metaClass);
			_classesbyid.Add(metaClass.ID, metaClass);
		}

		public void AddPackage(MetaPackage metaPackage)
		{
			metaPackage.Solution = this;
			_packagesbyname.Add(metaPackage.Name.ToLower(), metaPackage);
			_packagesbyid.Add(metaPackage.ID, metaPackage);
		}

		public MetaClass GetClass(string name)
		{
			string s = name.ToLower();
			return _classesbyname.ContainsKey(s) ? _classesbyname[s] : null;
		}

		public MetaClass GetClass(Guid id)
		{
			return _classesbyid.ContainsKey(id) ? _classesbyid[id] : null;
		}

		public MetaOperation GetOperation(string className, string operationName)
		{
			MetaClass c = GetClass(className);
			if (c == null) return null;
			return c.GetOperation(operationName);
		}

		public MetaPackage GetPackage(string name)
		{
			string s = name.ToLower();
			return _packagesbyname.ContainsKey(s) ? _packagesbyname[s] : null;
		}

		public MetaPackage GetPackage(Guid id)
		{
			return _packagesbyid.ContainsKey(id) ? _packagesbyid[id] : null;
		}
	}

	public class MetaPackage : MetaElement
	{
		protected Guid? ParentID { get; set; }
		MetaPackage _parent = null;

		/// <summary>
		/// Родительский пакет
		/// </summary>
		public MetaPackage Parent
		{
			get
			{
				if (_parent == null && ParentID != null) _parent = Solution.GetPackage(ParentID.Value);
				return _parent;
			}
		}

		/// <summary>
		/// Модель, которой принадлежит пакет
		/// </summary>
		public MetaSolution Solution { get; internal set; }
		Dictionary<string, MetaPackage> _packages = new Dictionary<string, MetaPackage>(16);
		Dictionary<string, MetaClass> _classes = new Dictionary<string, MetaClass>(64);

		/// <summary>
		/// Вложенные пакеты
		/// </summary>
		public Dictionary<string, MetaPackage>.ValueCollection Packages { get { return _packages.Values; } }
		/// <summary>
		/// Классы пакета
		/// </summary>
		public Dictionary<string, MetaClass>.ValueCollection Classes { get { return _classes.Values; } }

		public void AddClass(MetaClass metaClass)
		{
			Solution.AddClass(metaClass);
			metaClass.Parent = this;
			_classes.Add(metaClass.Name.ToLower(), metaClass);
		}

		public void AddPackage(MetaPackage metaPackage)
		{
			Solution.AddPackage(metaPackage);
			metaPackage.ParentID = this.ID;
			_packages.Add(metaPackage.Name.ToLower(), metaPackage);
		}

		public MetaClass GetClass(string name)
		{
			string s = name.ToLower();
			return _classes.ContainsKey(s) ? _classes[s] : null;
		}

		public MetaPackage GetPackage(string name)
		{
			string s = name.ToLower();
			return _packages.ContainsKey(s) ? _packages[s] : null;
		}

		public override string GetCaptionResourceKey()
		{
			return Name;
		}
	}

	public abstract partial class MetaClassifier : MetaElement
	{
		//public virtual Type CLRType { get; set; }
		public virtual string ColumnName(string propName)
		{
			return propName;
		}
	
	}

	public class MetaClass : MetaClassifier
	{
		/*public override Type CLRType
		{
			get
			{
				return Type.GetType("Solution." + Name);
			}
			set
			{
				throw new Exception("Changing class CLR type is unsupported");
			}
		}*/

		/// <summary>
		/// Модель, которой принадлежит класс
		/// </summary>
		public MetaSolution Solution { get; internal set; }
		/// <summary>
		/// Пакет, в котором располагается класс
		/// </summary>
		public MetaPackage Parent { get; set; }

		Dictionary<string, MetaProperty> _properties = new Dictionary<string, MetaProperty>();
		Dictionary<string, MetaOperation> _operations = new Dictionary<string, MetaOperation>();

		internal string BaseClassName { get; set; }
		MetaClass _baseClass = null;

		/// <summary>
		/// От какого класса унаследован
		/// </summary>
		public MetaClass BaseClass
		{
			get
			{
				if (_baseClass == null && !BaseClassName.IsEmpty()) _baseClass = Solution.GetClass(BaseClassName);
				return _baseClass;
			}
		}

		/// <summary>
		/// Сохраняются ли объекты класса в базе данных
		/// </summary>
		public bool IsPersistent { get; set; }

		bool _isMultilingual = false;

		/// <summary>
		/// Есть ли у класса мультиязычные свойства
		/// </summary>
		public bool IsMultilingual { get { return _isMultilingual; } }

		List<MetaProperty> _compositeKey = new List<MetaProperty>();
		/// <summary>
		/// Свойство класса, являющееся первичным ключом (если оно одно)
		/// </summary>
		public MetaProperty Key
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
		public List<MetaProperty> CompositeKey
		{
			get { return _compositeKey; }
		}

		/// <summary>
		/// Свойства класса
		/// </summary>
		public Dictionary<string, MetaProperty>.ValueCollection Properties { get { return _properties.Values; } }
		//public IEnumerable<T> Properties<T>() where T : MetaProperty { return _properties.Where(o => o is T).Select(o => o as T);  }
		/// <summary>
		/// Методы класса
		/// </summary>
		public Dictionary<string, MetaOperation>.ValueCollection Operations { get { return _operations.Values; } }

		/// <summary>
		/// Метод класса по умолчанию
		/// </summary>
		public MetaOperation DefaultOperation { get; set; }

		public void AddProperty(MetaProperty metaProperty)
		{
			metaProperty.Parent = this;
			_properties.Add(metaProperty.Name.ToLower(), metaProperty);
			if (metaProperty is MetaAttribute && (metaProperty as MetaAttribute).IsMultilingual) _isMultilingual = true;
		}
		public void AddOperation(MetaOperation metaOperation)
		{
			metaOperation.Parent = this;
			_operations.Add(metaOperation.Name.ToLower(), metaOperation);
		}

		public MetaProperty GetProperty(string name)
		{
			string s = name.ToLower();
			return _properties.ContainsKey(s) ? _properties[s] : null;
		}
		public MetaOperation GetOperation(string name)
		{
			string s = name.ToLower();
			return _operations.ContainsKey(s) ? _operations[s] : null;
		}

		public override string GetCaptionResourceKey()
		{
			return Name;
		}

		/// <summary>
		/// Имя столбца для свойств, которые ссылаются на данный класс
		/// </summary>
		public override string ColumnName(string propName)
		{
			return propName + (Key.Type as IMetaIdentifierType).ColumnSuffix;
		}
	}

	public abstract class MetaProperty : MetaElement
	{
		/// <summary>
		/// Класс, к которому принадлежит свойство
		/// </summary>
		public MetaClass Parent { get; set; }
		/// <summary>
		/// Тип данных
		/// </summary>
		public virtual string DataType { get; set; }
		/// <summary>
		/// Тип данных
		/// </summary>
		public virtual MetaClassifier Type { get; set; }
		/// <summary>
		/// Является ли свойство обязательным для заполнения
		/// </summary>
		public bool IsRequired { get; set; }
		/// <summary>
		/// Верхняя граница: 1 - максимум одно значение, -1 - значений может быть сколько угодно
		/// </summary>
		public int UpperBound { get; set; }

		public override string GetCaptionResourceKey()
		{
			return Parent.Name + ".P." + Name;
		}

		/// <summary>
		/// Имя столбца в базе данных
		/// </summary>
		public virtual string ColumnName
		{
			get { return Type == null ? Name : Type.ColumnName(Name); }
		}

	}

	/// <summary>
	/// Атрибут класса
	/// </summary>
	public class MetaAttribute : MetaProperty
	{
		/// <summary>
		/// Является мультиязычным
		/// </summary>
		public bool IsMultilingual { get; set; }
		/// <summary>
		/// Является автоинкрементным
		/// </summary>
		public bool IsIdentity { get; set; }
		/// <summary>
		/// Является первичным
		/// </summary>
		public string IsKey { get; set; }
	}

	/// <summary>
	/// Вычислимое свойство класса
	/// </summary>
	public class MetaComputedAttribute : MetaProperty
	{
		/// <summary>
		/// Код для get
		/// </summary>
		public string GetExpression { get; set; }
		/// <summary>
		/// Код для set
		/// </summary>
		public string SetExpression { get; set; }
	}

	/// <summary>
	/// Вычислимое на уровне базы данных свойство класса
	/// </summary>
	public class MetaPersistentComputedAttribute : MetaProperty
	{
		/// <summary>
		/// Выражение
		/// </summary>
		public string Expression { get; set; }

	}

	/// <summary>
	/// Тип ассоциации
	/// </summary>
	public enum AssociationType
	{
		/// <summary>
		/// Агрегация
		/// </summary>
		Aggregation = 1,
		/// <summary>
		/// Композиция
		/// </summary>
		Composition = 2,
		/// <summary>
		/// Нет ни агрегации, ни композиции
		/// </summary>
		Default = 0
	}

	/// <summary>
	/// Свойство класса - ссылка 
	/// </summary>
	public class MetaReference : MetaProperty
	{
		/// <summary>
		/// Тип ассоциации
		/// </summary>
		public AssociationType AssociationType { get; set; }

		internal string RefClassName { get; set; }
		MetaClass _refClass = null;
		/// <summary>
		/// На какой класс ссылается
		/// </summary>
		public MetaClass RefClass
		{
			get
			{
				if (_refClass == null) _refClass = Parent.Solution.GetClass(RefClassName);
				return _refClass;
			}
		}

		/// <summary>
		/// Тип данных
		/// </summary>
		public override MetaClassifier Type
		{
			get
			{
				if (_refClass == null) _refClass = Parent.Solution.GetClass(RefClassName);
				return _refClass.Key.Type;
			}
			set
			{
				throw new Exception("Changing reference type is unsupported");
			}
		}

		internal string InversePropertyName { get; set; }
		MetaReference _refInverseProperty = null;
		/// <summary>
		/// Является ли ссылка обратной (т.е. у класса, на который ссылается свойство есть тоже ссылка на данный класс)
		/// </summary>
		public MetaReference InverseProperty
		{
			get
			{
				if (InversePropertyName.IsEmpty()) return null;
				if (_refInverseProperty == null && RefClass != null) _refInverseProperty = RefClass.GetProperty(InversePropertyName) as MetaReference;
				return _refInverseProperty;
			}
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
		/// <summary>
		/// Является первичным
		/// </summary>
		public string IsKey { get; set; }
	}

	/// <summary>
	/// Параметр метода
	/// </summary>
	public class MetaOperationParameter : MetaElement
	{
		/// <summary>
		/// Тип данных
		/// </summary>
		public MetaClassifier Type { get; set; }
	}

	/// <summary>
	/// Метод класса
	/// </summary>
	public class MetaOperation : MetaElement
	{
		Dictionary<string, MetaOperationParameter> _parameters = new Dictionary<string, MetaOperationParameter>();

		/// <summary>
		/// Класс, которому принадлежит метод
		/// </summary>
		public MetaClass Parent { get; set; }
		/// <summary>
		/// Параметры метода
		/// </summary>
		public Dictionary<string, MetaOperationParameter> Parameters { get { return _parameters; } }
		/// <summary>
		/// Иконка
		/// </summary>
		public string Image { get; set; }
		//public bool IsDefault { get; set; }

		public override string GetCaptionResourceKey()
		{
			return Parent.Name + ".O." + Name;
		}
		/// <summary>
		/// По умолчанию
		/// </summary>
		public string IsDefault { get; set; }
	}

	public class MetaStereotype : MetaElement
	{
		public MetaElement Parent { get; set; }

		public override string GetCaptionResourceKey()
		{
			return Parent.Name + ".S." + Name;
		}
	}

	public static class Stereotypes
	{
		static Dictionary<Type, Dictionary<string, MetaElement>> _selements = new Dictionary<Type, Dictionary<string, MetaElement>>();
		internal static void AssignStereotype(Type stereotype, MetaElement element)
		{
			Dictionary<string, MetaElement> elements = null;
			if (!_selements.ContainsKey(stereotype))
			{
				elements = new Dictionary<string, MetaElement>();
				_selements.Add(stereotype, elements);
			}
			else
				elements = _selements[stereotype];

			elements.Add(element.Name, element);
		}

		public static Dictionary<string, MetaElement> GetElements<T>()
			where T : MetaStereotype
		{
			Type t = typeof(T);
			if (_selements.ContainsKey(t))
				return _selements[t];
			else
				return null;
		}
	}
}