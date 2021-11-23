using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Tango.AccessControl.Std
{
	public class PredicateChecker : IPredicateChecker, ITypeObserver
	{
		static Dictionary<string, Func<object, IServiceProvider, BoolResult>> _predicates = new Dictionary<string, Func<object, IServiceProvider, BoolResult>>();

		IServiceProvider _serviceProvider;

		public PredicateChecker(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

		public BoolResult Check(string securableObjectKey, object predicateContext)
		{
			string key = securableObjectKey.ToUpper();
			if (!_predicates.ContainsKey(key)) return BoolResult.True;
			return _predicates[key](predicateContext, _serviceProvider);
		}

		public void LookOver(Type t)
		{
			var attr = t.GetCustomAttribute<HasPredicatesAttribute>();
			if (attr == null) return;

			var methods = t.GetMethods(BindingFlags.Static | BindingFlags.Public);
			foreach (var m in methods)
			{
				var f = PreparePredicate(m);
				if (f != null)
				{
					var attrs = m.GetCustomAttributes<BindPredicateAttribute>();
					foreach(var a in attrs)
						_predicates.Add(a.SecurableObjectKey.ToUpper(), f);
				}
			}
		}
	
		MethodInfo getService = typeof(IServiceProvider).GetMethod("GetService");
		ConstructorInfo newBoolResult = typeof(BoolResult).GetConstructor(new Type[] { typeof(bool), typeof(string) });

		Func<object, IServiceProvider, BoolResult> PreparePredicate(MethodInfo m)
		{
			var attr = m.GetCustomAttribute<PredicateAttribute>();
			if (attr == null) return null;

			var parms = m.GetParameters();
			if (attr.HasContext && parms.Length == 0) return null;

			if (m.ReturnType != typeof(bool) && m.ReturnType != typeof(BoolResult))
				return null;

			ParameterExpression obj = Expression.Parameter(typeof(object), "obj");
			ParameterExpression services = Expression.Parameter(typeof(IServiceProvider), "services");

			List<Expression> args = new List<Expression>();
			for (int i = 0; i < parms.Length; i++)
			{
				var t = parms[i].ParameterType;
				if (i == 0 && attr.HasContext)
					args.Add(Expression.Convert(obj, t));
				else
				{
					args.Add(Expression.Convert(Expression.Call(services, getService, Expression.Constant(t)), t));
				}
			}

			Expression res = Expression.Call(m, args);
			if (m.ReturnType == typeof(bool))
			{
				res = Expression.New(newBoolResult, res, Expression.Constant(""));
			}

			return Expression.Lambda<Func<object, IServiceProvider, BoolResult>>(res, obj, services).Compile();
		}
	}

	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public sealed class HasPredicatesAttribute : Attribute
	{

	}

	[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
	public sealed class PredicateAttribute : Attribute
	{
		public PredicateAttribute(bool hasContext = true)
		{
			HasContext = hasContext;
		}

		public bool HasContext { get; private set; }
	}

	[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
	public sealed class BindPredicateAttribute : Attribute
	{
		public BindPredicateAttribute(string securableObjectKey)
		{
			SecurableObjectKey = securableObjectKey;
		}

		public string SecurableObjectKey { get; private set; }
	}
}
