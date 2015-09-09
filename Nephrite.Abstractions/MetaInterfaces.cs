using System;
using System.Collections.Generic;
using System.Linq;
using Nephrite.Multilanguage;

namespace Nephrite.Meta
{
	public interface IMetaElement
	{
		string ID { get; }
		string Name { get; set; }
		string Description { get; set; }
		string Caption { get; set; }

		T Stereotype<T>() where T : IMetaStereotype;
		void AssignStereotype(IMetaStereotype stereotype);
    }

	public interface IMetaStereotype : IMetaElement
	{
		IMetaElement Parent { get; set; }
	}

	public partial interface IMetaClassifier : IMetaElement
	{
		string CLRType { get; }
		string ColumnName(string propName);
	}

	public interface IMetaSolution : IMetaElement
	{
		Dictionary<string, IMetaClass>.ValueCollection Classes { get; }
		Dictionary<string, IMetaEnum>.ValueCollection Enums { get; }

		IMetaClass GetClass(string name);
		IMetaEnum GetEnum(string name);
		IMetaOperation GetOperation(string className, string operationName);

		ITextResource TextResource { get; }
	}

	public interface IMetaClass : IMetaClassifier
	{
		Dictionary<string, IMetaProperty>.ValueCollection AllProperties { get; }
		IMetaClass BaseClass { get; }
		IMetaSolution Parent { get; set; }
		PersistenceType Persistent { get; set; }

		List<IMetaProperty> CompositeKey { get; }
		IMetaProperty Key { get; }

		bool IsMultilingual { get; }
		List<IMetaParameter> Parameters { get; }

		Dictionary<string, IMetaOperation>.KeyCollection OperationNames { get; }
		Dictionary<string, IMetaOperation>.ValueCollection Operations { get; }
		IMetaOperation GetOperation(string name);
		void AddOperation(IMetaOperation metaOperation);

		Dictionary<string, IMetaProperty>.ValueCollection Properties { get; }
		Dictionary<string, IMetaProperty>.KeyCollection PropertyNames { get; }
		IMetaProperty GetProperty(string name);
		void AddProperty(IMetaProperty metaProperty);

		string CaptionPlural { get; set; }
		string LogicalDeleteExpressionString { get; set; }
		string DefaultOrderByExpressionString { get; set; }
		IMetaOperation DefaultOperation { get; set; }

		List<Type> Interfaces { get; }
	}

	public interface IMetaProperty : IMetaElement
	{
		string CaptionShort { get; set; }
		string ColumnName { get; }
		string DefaultDBValue { get; set; }
		object GetValue { get; set; }
		object GetValueExpression { get; set; }
		bool IsRequired { get; set; }
		IMetaClass Parent { get; set; }
		object SetValue { get; set; }
		IMetaPrimitiveType Type { get; set; }
		int UpperBound { get; set; }

		string GetStringValue<TClass>(TClass obj, string format = "", IFormatProvider provider = null);
	}

	public interface IMetaValueProperty : IMetaProperty
	{
		bool IsMultilingual { get; set; }
	}

	public interface IMetaAttribute : IMetaValueProperty
	{
		bool IsIdentity { get; set; }
	}

	public interface IMetaComputedAttribute : IMetaValueProperty
	{
		string GetExpressionString { get; set; }
		string SetExpressionString { get; set; }
	}

	public interface IMetaPersistentComputedAttribute : IMetaValueProperty
	{
		string Expression { get; set; }
	}

	public interface IMetaReference : IMetaProperty
	{
		AssociationType AssociationType { get; set; }
		IMetaReference InverseProperty { get; }
		IMetaClass RefClass { get; }

		string DataTextField { get; set; }
		IQueryable AllObjects { get; set; }

		void SetRefClass(string refClassName);
		void SetInverseProperty(string inversePropertyName);
    }

	public interface IMetaOperation : IMetaElement
	{
		IMetaClass Parent { get; set; }
		
		string Image { get; set; }
		//Action Invoke { get; set; }
		List<IMetaParameter> Parameters { get; set; }

		[Obsolete]
		string ActionString { get; set; }
		[Obsolete]
		string ParametersString { get; }
		[Obsolete]
		string PredicateString { get; set; }

		//string ViewClass { get; set; }
		//string ViewName { get; set; }
		//DTOClassKind DTOClassKind { get; set; }
		//string DTOClass { get; set; }
		//ViewEngineType ViewEngine { get; set; }
		//InteractionType InteractionType { get; set; }
    }

	public interface IMetaEnum : IMetaElement
	{
		IMetaSolution Parent { get; set; }
		List<IMetaEnumValue> Values { get; }
	}

	public interface IMetaEnumValue : IMetaElement
	{
	}

	public interface IMetaParameter : IMetaElement
	{
		IMetaParameterType Type { get; set; }
	}

	public interface IMetaPrimitiveType : IMetaClassifier
	{
		bool NotNullable { get; }
		/// <summary>
		/// Func &lt;TValue, string, IFormatProvider, string&gt;
		/// </summary>
		object GetStringValue { get; }

		IMetaPrimitiveType Clone(bool notNullable);
	}

	public interface IMetaIdentifierType : IMetaPrimitiveType
	{
		string ColumnSuffix { get; }
	}

	public interface IMetaParameterType : IMetaPrimitiveType { }

	public interface IMetaNumericType : IMetaPrimitiveType { }

	public enum PersistenceType
	{
		None,
		Table,
		View,
		TableFunction,
		Procedure
	}

	//public enum DTOClassKind { Single, Queryable, None }
	//public enum ViewEngineType { WebForms, Razor }
	//public enum InteractionType { OneWayView, ViewWithSubmit, NoView }

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
}
