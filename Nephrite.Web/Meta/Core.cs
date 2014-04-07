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
	public abstract class MetaElement
	{
		public MetaElement()
		{
		}
		public MetaElement(string name)
		{
			Name = name;
		}
		public MetaElement(string name, string caption)
		{
			Name = name;
			Caption = caption;
		}
		public MetaElement(string name, string caption, string description)
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

		/// <summary>
		/// ������ ������
		/// </summary>
		public Dictionary<string, MetaPackage>.ValueCollection Packages { get { return _packagesbyname.Values; } }
		/// <summary>
		/// ������ ������
		/// </summary>
		public Dictionary<string, MetaClass>.ValueCollection Classes { get { return _classesbyname.Values; } }

		//public string EntryPoint { get; }
		public void AddClass(MetaClass metaClass)
		{
			metaClass.Solution = this;
			_classesbyname.Add(metaClass.Name.ToLower(), metaClass);
		}

		public void AddPackage(MetaPackage metaPackage)
		{
			metaPackage.Solution = this;
			_packagesbyname.Add(metaPackage.Name.ToLower(), metaPackage);
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
	}

	public class MetaPackage : MetaElement
	{
		protected string ParentID { get; set; }
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

		/// <summary>
		/// ��������� ������
		/// </summary>
		public Dictionary<string, MetaPackage>.ValueCollection Packages { get { return _packages.Values; } }
		/// <summary>
		/// ������ ������
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

		public override string ID
		{
			get
			{
				return Name;
			}
		}
	}

	public abstract partial class MetaClassifier : MetaElement
	{
		public abstract string CLRType { get; }
		public virtual string ColumnName(string propName)
		{
			return propName;
		}
	
	}

	public class MetaClass : MetaClassifier
	{
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
		/// <summary>
		/// ������ ������
		/// </summary>
		public Dictionary<string, MetaOperation>.ValueCollection Operations { get { return _operations.Values; } }

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

		/// <summary>
		/// ��� ������� ��� �������, ������� ��������� �� ������ �����
		/// </summary>
		public override string ColumnName(string propName)
		{
			return propName + (Key.Type as IMetaIdentifierType).ColumnSuffix;
		}
	}

	public abstract class MetaProperty : MetaElement
	{
		/// <summary>
		/// �����, � �������� ����������� ��������
		/// </summary>
		public MetaClass Parent { get; set; }
		/// <summary>
		/// ��� ������
		/// </summary>
		public virtual string DataType { get; set; }

		public virtual string DefaultDBValue { get; set; }

		/// <summary>
		/// ��� ������
		/// </summary>
		public virtual MetaClassifier Type { get; set; }
		/// <summary>
		/// �������� �� �������� ������������ ��� ����������
		/// </summary>
		public bool IsRequired { get; set; }
		/// <summary>
		/// ������� �������: 1 - �������� ���� ��������, -1 - �������� ����� ���� ������� ������
		/// </summary>
		public int UpperBound { get; set; }

		public override string ID
		{
			get
			{
				return Parent.Name + ".P." + Name;
			}
		}

		/// <summary>
		/// ��� ������� � ���� ������
		/// </summary>
		public virtual string ColumnName
		{
			get { return Type == null ? Name : Type.ColumnName(Name); }
		}

	}

	/// <summary>
	/// ������� ������
	/// </summary>
	public class MetaAttribute : MetaProperty
	{
		/// <summary>
		/// �������� �������������
		/// </summary>
		public bool IsMultilingual { get; set; }
		/// <summary>
		/// �������� ����������������
		/// </summary>
		public bool IsIdentity { get; set; }
		/// <summary>
		/// �������� ���������
		/// </summary>
		public string IsKey { get; set; }
	}

	/// <summary>
	/// ���������� �������� ������
	/// </summary>
	public class MetaComputedAttribute : MetaProperty
	{
		/// <summary>
		/// ��� ��� get
		/// </summary>
		public string GetExpression { get; set; }
		/// <summary>
		/// ��� ��� set
		/// </summary>
		public string SetExpression { get; set; }
	}

	/// <summary>
	/// ���������� �� ������ ���� ������ �������� ������
	/// </summary>
	public class MetaPersistentComputedAttribute : MetaProperty
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
	public class MetaReference : MetaProperty
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
		/// <summary>
		/// �������� ���������
		/// </summary>
		public string IsKey { get; set; }
	}

	/// <summary>
	/// �������� ������
	/// </summary>
	public class MetaOperationParameter : MetaElement
	{
		/// <summary>
		/// ��� ������
		/// </summary>
		public MetaClassifier Type { get; set; }
	}

	/// <summary>
	/// ����� ������
	/// </summary>
	public class MetaOperation : MetaElement
	{
		Dictionary<string, MetaOperationParameter> _parameters = new Dictionary<string, MetaOperationParameter>();

		/// <summary>
		/// �����, �������� ����������� �����
		/// </summary>
		public MetaClass Parent { get; set; }
		/// <summary>
		/// ��������� ������
		/// </summary>
		public Dictionary<string, MetaOperationParameter> Parameters { get { return _parameters; } }
		/// <summary>
		/// ������
		/// </summary>
		public string Image { get; set; }
		//public bool IsDefault { get; set; }

		public override string ID
		{
			get
			{
				return Parent.Name + ".O." + Name;
			}
		}
		/// <summary>
		/// �� ���������
		/// </summary>
		public string IsDefault { get; set; }
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

	public static class Stereotypes
	{
		[ThreadStatic]
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

	public interface IWithMetadata
	{
		MetaClass MetaClass { get; }
	}
}