using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nephrite.Meta
{
	public interface IMetaElement
	{
		string ID { get; }
		string Name { get; set; }
		string Description { get; set; }
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

		Dictionary<string, IMetaProperty>.ValueCollection Properties { get; }
		Dictionary<string, IMetaProperty>.KeyCollection PropertyNames { get; }
		IMetaProperty GetProperty(string name);

		string CaptionPlural { get; set; }
		string LogicalDeleteExpressionString { get; set; }
		string DefaultOrderByExpressionString { get; set; }
		IMetaOperation DefaultOperation { get; set; }
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

		string GetStringValue<TClass, TValue>(TClass obj, string format = "", IFormatProvider provider = null);
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
	}

	public interface IMetaOperation : IMetaElement
	{
		string ActionString { get; set; }
		string DTOClass { get; set; }
		DTOClassKind DTOClassKind { get; set; }
		string Image { get; set; }
		Action Invoke { get; set; }
		List<IMetaParameter> Parameters { get; set; }
		string ParametersString { get; }
		IMetaClass Parent { get; set; }
		string PredicateString { get; set; }
		string ViewClass { get; set; }
		string ViewName { get; set; }
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

	public enum DTOClassKind { Single, Queryable, None }

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
