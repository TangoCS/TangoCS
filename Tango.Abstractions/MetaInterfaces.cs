using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Tango.Meta
{
	public interface IMetaElement
	{
		string ID { get; }

		T Stereotype<T>() where T : IMetaStereotype;
		void AssignStereotype(IMetaStereotype stereotype);
	}

	public interface IMetaNamedElement : IMetaElement
	{
		string Name { get; set; }
		string Namespace { get; set; }
		//string Description { get; set; }
		//string Caption { get; set; }
	}

	public interface IMetaStereotype : IMetaNamedElement
	{
		IMetaElement Parent { get; set; }
	}

	//public partial interface IMetaClassifier : IMetaNamedElement
	//{
	//	string CLRType { get; }
	//	string ColumnName(string propName);
	//}

	public interface IMetaSolution : IMetaNamedElement
	{
		IEnumerable<IMetaClass> Classes { get; }
		IEnumerable<IMetaEnum> Enums { get; }

		IMetaClass GetClass(string name);
		IMetaEnum GetEnum(string name);
		IMetaOperation GetOperation(string className, string operationName);

		//ITextResource TextResource { get; }
	}

	public interface IMetaClass : IMetaNamedElement//IMetaClassifier
	{
		IEnumerable<IMetaProperty> AllProperties { get; }
		IMetaClass BaseClass { get; }
		IMetaSolution Parent { get; set; }
		PersistenceType Persistent { get; set; }

		List<IMetaProperty> CompositeKey { get; }
		IMetaProperty Key { get; }

		bool IsMultilingual { get; }
		List<IMetaParameter> Parameters { get; }

		IEnumerable<IMetaOperation> Operations { get; }
		IEnumerable<string> OperationNames { get; }
		IMetaOperation GetOperation(string name);
		void AddOperation(IMetaOperation metaOperation);

		IEnumerable<IMetaProperty> Properties { get; }
		IEnumerable<string> PropertyNames { get; }
		IMetaProperty GetProperty(string name);
		void AddProperty(IMetaProperty metaProperty);

		//string CaptionPlural { get; set; }
		string LogicalDeleteExpressionString { get; set; }
		string DefaultOrderByExpressionString { get; set; }
		IMetaOperation DefaultOperation { get; set; }

		List<Type> Interfaces { get; }
	}

	public interface IMetaProperty : IMetaNamedElement
	{
		//string CaptionShort { get; set; }
		string ColumnName { get; }
		string DefaultDBValue { get; set; }
		bool IsRequired { get; set; }
		IMetaClass Parent { get; set; }
		IMetaPrimitiveType Type { get; set; }
		int UpperBound { get; set; }
	}

	public interface IMetaProperty<TClass, TValue> : IMetaProperty
	{
		Func<TClass, TValue> GetValue { get; set; }
		Action<TClass, TValue> SetValue { get; set; }
		Expression<Func<TClass, TValue>> GetValueExpression { get; set; }

		string GetStringValue(TClass obj, string format = "", IFormatProvider provider = null);
	}

	public interface ICanBeMultilingual
	{
		bool IsMultilingual { get; set; }
	}

	public interface ICanBeIdentity
	{
		bool IsIdentity { get; set; }
	}

	public interface IMetaValueProperty : IMetaProperty { }
	public interface IMetaComputedAttribute : IMetaValueProperty { }
	public interface IMetaPersistentComputedAttribute : IMetaValueProperty, ICanBeMultilingual
	{
		string Expression { get; set; }
	}

	public interface IMetaReference : IMetaProperty
	{
		AssociationType AssociationType { get; set; }
		IMetaProperty InverseProperty { get; }
		IMetaClass RefClass { get; }

		void SetRefClass(string refClassName);
		void SetInverseProperty(string inversePropertyName);
	}

	public interface IMetaReference<TClass, TRefClass> : IMetaReference, IMetaProperty<TClass, IQueryable<TRefClass>>
	{
	}

	public interface IMetaReference<TClass, TRefClass, TKey> : IMetaReference, IMetaProperty<TClass, TRefClass>
	{
		Func<TClass, TKey> GetValueID { get; set; }
		Action<TClass, TKey> SetValueID { get; set; }
		Expression<Func<TClass, TKey>> GetValueIDExpression { get; set; }
	}

	public interface IMetaOperation : IMetaNamedElement
	{
		IMetaClass Parent { get; set; }
		
		string Image { get; set; }
		List<IMetaParameter> Parameters { get; set; }

		[Obsolete]
		string ActionString { get; set; }
		[Obsolete]
		string ParametersString { get; }
		[Obsolete]
		string PredicateString { get; set; }
    }

	public interface IMetaEnum : IMetaNamedElement
	{
		IMetaSolution Parent { get; set; }
		List<IMetaEnumValue> Values { get; }
	}

	public interface IMetaEnumValue : IMetaNamedElement
	{
	}

	public interface IMetaParameter : IMetaNamedElement
	{
		IMetaParameterType Type { get; set; }
	}

	public partial interface IMetaPrimitiveType //: IMetaClassifier
	{
		//bool NotNullable { get; }


		//string ColumnSuffix { get; }

		string ToCSharpType(bool nullable);
		//IMetaPrimitiveType Clone(bool notNullable);
	}

	//public interface IMetaPrimitiveType<T>
	//{
	//	Func<T, string, IFormatProvider, string> GetStringValue { get; }
	//}

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
