using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.HtmlControls;

namespace Nephrite.Meta
{
	/// <summary>
	/// ����������� ����� ��� ���� ��������� ������
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

		/// <summary>
		/// ���������� �������������
		/// </summary>
		public virtual string ID { get { return "MetaElement." + Name; } }
		/// <summary>
		/// ��������� ���
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// �������� �� ��������� �����
		/// </summary>
		public string Caption { get; set; }

		/// <summary>
		/// ��������
		/// </summary>
		public string Description { get; set; }
		//public Dictionary<string, MetaTaggedValue> TaggedValues { get; set; }

		protected Dictionary<Type, MetaStereotype> _stereotypes = new Dictionary<Type, MetaStereotype>();

		public T S<T>() where T : MetaStereotype
		{
			Type t = typeof(T);
			if (!_stereotypes.ContainsKey(t)) return null;
			return (T)_stereotypes[t];
		}

		public void AssignStereotype(MetaStereotype stereotype)
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
	/// ������ ���������� �������
	/// </summary>
	public partial class MetaSolution : MetaElement
	{
		public override string ID
		{
			get
			{
				return Name;
			}
		}

		Dictionary<string, MetaClass> _classesbyname = new Dictionary<string, MetaClass>(255);
		Dictionary<string, MetaPackage> _packagesbyname = new Dictionary<string, MetaPackage>(32);
		Dictionary<string, MetaEnum> _enumsbyname = new Dictionary<string, MetaEnum>(32);

		/// <summary>
		/// ������ ������
		/// </summary>
		public Dictionary<string, MetaPackage>.ValueCollection Packages { get { return _packagesbyname.Values; } }
		/// <summary>
		/// ������ ������
		/// </summary>
		public Dictionary<string, MetaClass>.ValueCollection Classes { get { return _classesbyname.Values; } }
		/// <summary>
		/// Enums ������
		/// </summary>
		public Dictionary<string, MetaEnum>.ValueCollection Enums { get { return _enumsbyname.Values; } }

		internal void AddClass(MetaClass metaClass)
		{
			metaClass.Solution = this;
			_classesbyname.Add(metaClass.Name.ToLower(), metaClass);
		}

		public MetaPackage AddPackage(MetaPackage metaPackage)
		{
			metaPackage.Solution = this;
			_packagesbyname.Add(metaPackage.Name.ToLower(), metaPackage);

			foreach (var c in metaPackage.Classes)
				AddClass(c);

			foreach (var e in metaPackage.Enums)
				AddEnum(e);

			return metaPackage;
		}

		internal void AddEnum(MetaEnum metaEnum)
		{
			_enumsbyname.Add(metaEnum.Name.ToLower(), metaEnum);
		}

		public MetaPackage AddPackage(string name, string caption = "", string description = "")
		{
			MetaPackage p = new MetaPackage { Name = name, Caption = caption, Description = description };
			AddPackage(p);
			return p;
		}

		public MetaClass GetClass(string name)
		{
			string s = name.ToLower();
			return _classesbyname.ContainsKey(s) ? _classesbyname[s] : null;
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

		public MetaEnum GetEnum(string name)
		{
			string s = name.ToLower();
			return _enumsbyname.ContainsKey(s) ? _enumsbyname[s] : null;
		}
	}

	public class MetaPackage : MetaElement, IMetaOperationContainer
	{
		public MetaPackage(string name = "", string caption = "", string description = "") : base(name, caption, description) { }

		public string ParentID { get; set; }
		MetaPackage _parent = null;

		/// <summary>
		/// ������������ �����
		/// </summary>
		public MetaPackage Parent
		{
			get
			{
				if (_parent == null && !String.IsNullOrEmpty(ParentID)) _parent = Solution.GetPackage(ParentID);
				return _parent;
			}
		}

		/// <summary>
		/// ������, ������� ����������� �����
		/// </summary>
		public MetaSolution Solution { get; internal set; }
		Dictionary<string, MetaPackage> _packages = new Dictionary<string, MetaPackage>(16);
		Dictionary<string, MetaClass> _classes = new Dictionary<string, MetaClass>(64);
		Dictionary<string, MetaOperation> _operations = new Dictionary<string, MetaOperation>(16);
		Dictionary<string, MetaEnum> _enums = new Dictionary<string, MetaEnum>(32);

		/// <summary>
		/// ��������� ������
		/// </summary>
		public Dictionary<string, MetaPackage>.ValueCollection Packages { get { return _packages.Values; } }
		/// <summary>
		/// ������ ������
		/// </summary>
		public Dictionary<string, MetaClass>.ValueCollection Classes { get { return _classes.Values; } }
		/// <summary>
		/// �������� ������
		/// </summary>
		public Dictionary<string, MetaOperation>.ValueCollection Operations { get { return _operations.Values; } }

		public Dictionary<string, MetaEnum>.ValueCollection Enums { get { return _enums.Values; } }

		public void AddClass(MetaClass metaClass)
		{
			if (Solution != null) Solution.AddClass(metaClass);
			metaClass.Parent = this;
			_classes.Add(metaClass.Name.ToLower(), metaClass);
		}

		public MetaClass AddClass(string name, string caption = "", string description = "")
		{
			MetaClass c = new MetaClass { Name = name, Caption = caption, Description = description, IsPersistent = true };
			AddClass(c);
			return c;
		}

		public MetaClass AddClass<T>(string caption = "", string description = "")
		{
			MetaClass c = new MetaClass { Name = typeof(T).Name, Caption = caption, Description = description, IsPersistent = true };
			AddClass(c);
			return c;
		}

		public MetaPackage AddPackage(MetaPackage metaPackage)
		{
			metaPackage.ParentID = this.ID;
			//metaPackage.Solution = this.Solution;

			//foreach (var c in metaPackage.Classes)
			//	Solution.AddClass(c);

			//foreach (var e in metaPackage.Enums)
			//	Solution.AddEnum(e);

			Solution.AddPackage(metaPackage);
			_packages.Add(metaPackage.Name.ToLower(), metaPackage);
			return metaPackage;
		}

		public void AddEnum(MetaEnum metaEnum)
		{
			if (Solution != null) Solution.AddEnum(metaEnum);
			_enums.Add(metaEnum.Name.ToLower(), metaEnum);
		}

		public MetaEnum AddEnum(string name, string caption = "", string description = "")
		{
			MetaEnum c = new MetaEnum { Name = name, Caption = caption, Description = description };
			AddEnum(c);
			return c;
		}


		public MetaPackage AddPackage(string name, string caption = "", string description = "")
		{
			MetaPackage p = new MetaPackage { Name = name, Caption = caption, Description = description };
			AddPackage(p);
			return p;
		}

		public void AddOperation(MetaOperation metaOperation)
		{
			metaOperation.Parent = this;
			_operations.Add(metaOperation.Name.ToLower(), metaOperation);
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

		public MetaOperation GetOperation(string name)
		{
			string s = name.ToLower();
			return _operations.ContainsKey(s) ? _operations[s] : null;
		}

		public override string ID
		{
			get
			{
				return Name;
			}
		}
	}

	public interface IMetaElement
	{
		string ID { get; }
		string Name { get; set; }
	}

	public interface IMetaOperationContainer : IMetaElement
	{
		Dictionary<string, MetaOperation>.ValueCollection Operations { get; }
		void AddOperation(MetaOperation metaOperation);
	}

	public partial interface IMetaClassifier : IMetaElement
	{
		string CLRType { get; }
		string ColumnName(string propName);
	}

	public abstract partial class MetaClassifier : MetaElement, IMetaClassifier
	{
		public abstract string CLRType { get; }

		public virtual string ColumnName(string propName)
		{
			return propName;
		}
	}

	public partial class MetaClass : MetaClassifier, IMetaOperationContainer
	{
		public string CaptionPlural { get; set; }

		public override string CLRType
		{
			get
			{
				return IsMultilingual ? "V_" + Name : Name;
			}
		}

		/// <summary>
		/// ������, ������� ����������� �����
		/// </summary>
		public MetaSolution Solution { get; internal set; }
		/// <summary>
		/// �����, � ������� ������������� �����
		/// </summary>
		public MetaPackage Parent { get; set; }

		Dictionary<string, MetaProperty> _properties = new Dictionary<string, MetaProperty>();
		Dictionary<string, MetaProperty> _allproperties = null;
		Dictionary<string, MetaOperation> _operations = new Dictionary<string, MetaOperation>();

		internal string BaseClassName { get; set; }
		MetaClass _baseClass = null;

		/// <summary>
		/// �� ������ ������ �����������
		/// </summary>
		public MetaClass BaseClass
		{
			get
			{
				if (_baseClass == null && !String.IsNullOrEmpty(BaseClassName)) _baseClass = Solution.GetClass(BaseClassName);
				return _baseClass;
			}
		}

		/// <summary>
		/// ����������� �� ������� ������ � ���� ������
		/// </summary>
		public bool IsPersistent { get; set; }

		bool _isMultilingual = false;

		/// <summary>
		/// ���� �� � ������ ������������� ��������
		/// </summary>
		public bool IsMultilingual { get { return _isMultilingual; } }

		List<MetaProperty> _compositeKey = new List<MetaProperty>();
		/// <summary>
		/// �������� ������, ���������� ��������� ������ (���� ��� ����)
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
		/// ��� �������� ������, �������� � ��������� ����
		/// </summary>
		public List<MetaProperty> CompositeKey
		{
			get { return _compositeKey; }
		}

		/// <summary>
		/// �������� ������
		/// </summary>
		public Dictionary<string, MetaProperty>.ValueCollection Properties { get { return _properties.Values; } }
		public Dictionary<string, MetaProperty>.KeyCollection PropertyNames { get { return _properties.Keys; } }
		public Dictionary<string, MetaProperty>.ValueCollection AllProperties
		{
			get
			{
				if (_allproperties == null)
				{
					Dictionary<string, MetaProperty> p = new Dictionary<string, MetaProperty>(_properties);
					if (BaseClass != null)
						foreach (var kv in BaseClass.AllProperties)
							p.Add(kv.Name, kv);
					_allproperties = p;
				}
				return _allproperties.Values;
			}
		}

		/// <summary>
		/// ������ ������
		/// </summary>
		public Dictionary<string, MetaOperation>.ValueCollection Operations { get { return _operations.Values; } }
		public Dictionary<string, MetaOperation>.KeyCollection OperationNames { get { return _operations.Keys; } }

		/// <summary>
		/// ����� ������ �� ���������
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

		public override string ID
		{
			get
			{
				return Name;
			}
		}

		public string LogicalDeleteExpressionString { get; set; }
		public string DefaultOrderByExpressionString { get; set; }

		/// <summary>
		/// ��� ������� ��� �������, ������� ��������� �� ������ �����
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

	public abstract partial class MetaProperty : MetaElement
	{
		string _captionShort = "";
		public string CaptionShort { get { return String.IsNullOrEmpty(_captionShort) ? Caption : _captionShort; } set { _captionShort = value; } }

		/// <summary>
		/// �����, � �������� ����������� ��������
		/// </summary>
		public MetaClass Parent { get; set; }

		public virtual string DefaultDBValue { get; set; }

		/// <summary>
		/// ��� ������
		/// </summary>
		public virtual IMetaPrimitiveType Type { get; set; }
		/// <summary>
		/// �������� �� �������� ������������ ��� ����������
		/// </summary>
		public bool IsRequired { get; set; }
		/// <summary>
		/// ������� �������: 1 - �������� ���� ��������, -1 - �������� ����� ���� ������� ������
		/// </summary>
		public int UpperBound { get { return _upperBound; } set { _upperBound = value; } }
		int _upperBound = 1;

		public override string ID
		{
			get
			{
				return Parent.Name + "." + Name;
			}
		}

		/// <summary>
		/// ��� ������� � ���� ������
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

		public abstract string GetStringValue<TClass, TValue>(TClass obj, string format = "", IFormatProvider provider = null);

	}

	public partial class MetaValueProperty : MetaProperty
	{
		/// <summary>
		/// �������� �������������
		/// </summary>
		public bool IsMultilingual { get; set; }

		public override string GetStringValue<TClass, TValue>(TClass obj, string format = "", IFormatProvider provider = null)
		{
			var pt = Type;
			if (pt.GetStringValue == null) return "";
			var valueGetter = GetValue as Func<TClass, TValue>;
			var getter = pt.GetStringValue as Func<TValue, string, IFormatProvider, string>;
			return getter(valueGetter(obj), format, provider);
		}
	}

	/// <summary>
	/// ������� ������
	/// </summary>
	public partial class MetaAttribute : MetaValueProperty
	{		
		/// <summary>
		/// �������� ����������������
		/// </summary>
		public bool IsIdentity { get; set; }
	}

	/// <summary>
	/// ���������� �������� ������
	/// </summary>
	public partial class MetaComputedAttribute : MetaValueProperty
	{
		public string GetExpressionString { get; set; }
		public string SetExpressionString { get; set; }
	}

	/// <summary>
	/// ���������� �� ������ ���� ������ �������� ������
	/// </summary>
	public partial class MetaPersistentComputedAttribute : MetaValueProperty
	{
		/// <summary>
		/// ���������
		/// </summary>
		public string Expression { get; set; }

	}

	/// <summary>
	/// ��� ����������
	/// </summary>
	public enum AssociationType
	{
		/// <summary>
		/// ���������
		/// </summary>
		Aggregation = 1,
		/// <summary>
		/// ����������
		/// </summary>
		Composition = 2,
		/// <summary>
		/// ��� �� ���������, �� ����������
		/// </summary>
		Default = 0
	}

	/// <summary>
	/// �������� ������ - ������ 
	/// </summary>
	public partial class MetaReference : MetaProperty
	{
		/// <summary>
		/// ��� ����������
		/// </summary>
		public AssociationType AssociationType { get; set; }

		internal string RefClassName { get; set; }
		MetaClass _refClass = null;
		/// <summary>
		/// �� ����� ����� ���������
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
		/// ��� ������
		/// </summary>
		public override IMetaPrimitiveType Type
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
		/// �������� �� ������ �������� (�.�. � ������, �� ������� ��������� �������� ���� ���� ������ �� ������ �����)
		/// </summary>
		public MetaReference InverseProperty
		{
			get
			{
				if (String.IsNullOrEmpty(InversePropertyName)) return null;
				if (_refInverseProperty == null && RefClass != null) _refInverseProperty = RefClass.GetProperty(InversePropertyName) as MetaReference;
				return _refInverseProperty;
			}
		}

		/// <summary>
		/// ��� ������� � ���� ������
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


		public override string GetStringValue<TClass, TValue>(TClass obj, string format = "", IFormatProvider provider = null)
		{
			var valueGetter = GetValue as Func<TClass, TValue>;
			TValue refObj = valueGetter(obj);
			if (refObj != null && refObj is Nephrite.Web.IWithTitle)
				return (refObj as Nephrite.Web.IWithTitle).Title;
			else
				return "";
		}
	}

	/// <summary>
	/// �������� ������
	/// </summary>
	public class MetaOperationParameter : MetaElement
	{
		/// <summary>
		/// ��� ������
		/// </summary>
		public IMetaParameterType Type { get; set; }
	}

	/// <summary>
	/// ����� ������
	/// </summary>
	public class MetaOperation : MetaElement
	{
		List<MetaOperationParameter> _parameters = new List<MetaOperationParameter>();

		/// <summary>
		/// �����, �������� ����������� �����
		/// </summary>
		public IMetaOperationContainer Parent { get; set; }
		/// <summary>
		/// ��������� ������
		/// </summary>
		public List<MetaOperationParameter> Parameters { get { return _parameters; } }
		/// <summary>
		/// ������
		/// </summary>
		public string Image { get; set; }

		public override string ID
		{
			get
			{
				return Parent.Name + "." + Name;
			}
		}

		public Action Invoke { get; set; }

		public string ActionString { get; set; }
		public string PredicateString { get; set; }

		public string ViewName { get; set; }
		public string ViewClass { get; set; }
		public string DTOClass { get; set; }
		public DTOClassKind DTOClassKind { get; set; }

		public string ParametersString
		{
			get
			{
				return string.Join(", ", Parameters.Select(o => o.Type.CLRType + " " + o.Name));
			}
		}
	}

	public enum DTOClassKind { Single, Queryable }

	public class MetaEnum : MetaElement
	{
		public MetaPackage Parent { get; set; }

		List<MetaEnumValue> _values = new List<MetaEnumValue>();
		public List<MetaEnumValue> Values { get { return _values; } }
	}

	public class MetaEnumValue : MetaElement
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

	public class MetaStereotype : MetaElement
	{
		public MetaElement Parent { get; set; }

		public override string ID
		{
			get
			{
				return "Stereotype." + Name;
			}
		}
	}

	public interface IWithMetadata
	{
		MetaClass MetaClass { get; }
	}

	
}