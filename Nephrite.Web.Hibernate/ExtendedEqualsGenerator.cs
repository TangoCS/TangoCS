using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using NHibernate.Hql.Ast;
using NHibernate.Linq;
using NHibernate.Linq.Functions;

namespace Nhibernate.Extensions
{
	public static class GuidLinqExtensions
	{
		public static bool IsCompare(this Guid? source, Guid? compared)
		{

			return source == compared;
		}
	}
	public class GuidLinqGenerator : BaseHqlGeneratorForMethod
	{
		public GuidLinqGenerator()
		{
			SupportedMethods = new[] { ReflectionHelper.GetMethod(() => GuidLinqExtensions.IsCompare(null, null)) };
		}


		public override HqlTreeNode BuildHql(MethodInfo method, System.Linq.Expressions.Expression targetObject, System.Collections.ObjectModel.ReadOnlyCollection<System.Linq.Expressions.Expression> arguments, HqlTreeBuilder treeBuilder, NHibernate.Linq.Visitors.IHqlExpressionVisitor visitor)
		{
			return treeBuilder.Equality(
					visitor.Visit(targetObject).AsExpression(),
					visitor.Visit(arguments[0]).AsExpression());
		}
	}

	public class GuidToHqlGeneratorsRegistry : DefaultLinqToHqlGeneratorsRegistry
	{
		public GuidToHqlGeneratorsRegistry()
		{
			RegisterGenerator(ReflectionHelper.GetMethod(() => GuidLinqExtensions.IsCompare(null, null)),
						  new GuidLinqGenerator());
		}
	}
}