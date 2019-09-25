using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Tango.Data;

namespace Tango.Localization
{
	public class DefaultResourceManagerOptions
	{
		public SortedSet<string> NotFoundResources { get; } = new SortedSet<string>();

		public IReadOnlyDictionary<string, string> Resources { get; set; }
		public IReadOnlyDictionary<string, string> Images { get; set; }
	}

	public class DefaultResourceManager : IResourceManager
	{
		ILanguage _language;
		public static DefaultResourceManagerOptions Options { get; set; }

		public DefaultResourceManager(ILanguage language)
		{
			_language = language;
		}

		public string Get(string key)
		{
			var text = "";
			if (!TryGet(key, out text))
			{
				int i = key.LastIndexOf(".");
				if (i < 0 || !TryGet("Common" + key.Substring(i), out text))
				{
					SetNotFound(key);
					return key;
				}
			}
			return text;
		}

		public bool TryGet(string key, out string result)
		{
			return Options.Resources.TryGetValue(key + "-" + _language.Current.Code, out result);
		}


		public string GetImageName(string key)
		{
			var text = "";
			if (Options.Images.TryGetValue(key, out text))
				return text;
			else
				return "";
		}

		public void SetNotFound(string key)
		{
			Options.NotFoundResources.Add(key.ToLower());
		}

		public IEnumerable<string> GetNotFound()
		{
			return Options.NotFoundResources;
		}
	}

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class ResourceTypeAttribute : Attribute
	{
		public Type Type { get; set; }
		public ResourceTypeAttribute(Type type)
		{
			Type = type;
		}
	}
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class ResourceNameAttribute : Attribute
	{
		public string Name { get; set; }
		public ResourceNameAttribute(string name)
		{
			Name = name;
		}
	}

	public class ResourceKeyInfo
	{
		public Stack<string> Parts { get; set; } = new Stack<string>(4);
		public string Name { get; set; }
		public string Suffix { get; set; }
		public bool SuffixIsOptional { get; set; }
	}

	public static class ResourceManagerExtensions
	{
		public static string Get<T>(this IResourceManager textResource, Expression<Func<T, object>> exp)
		{
			return textResource.Get(exp.GetResourceKey());
		}

		public static string Get<T>(this IResourceManager textResource, Expression<Func<T, object>> exp, string suffix)
		{
			var k = exp.GetResourceKey();
			k.Suffix = suffix;
			return textResource.Get(k);
		}

		public static string Get(this IResourceManager textResource, string key, string suffix)
		{
			return textResource.Get(key + "-" + suffix);
		}

		public static string GetExt<T>(this IResourceManager textResource, string suffix)
		{
			return textResource.Get(typeof(T).GetResourceName(), suffix);
		}

		public static string Get<T>(this IResourceManager textResource, string key)
		{
			return textResource.Get(typeof(T).GetResourceName() + "." + key);
		}

		public static string Get<T>(this IResourceManager textResource, string key, string suffix)
		{
			return textResource.Get(typeof(T).GetResourceName() + "." + key, suffix);
		}

		public static string Get(this IResourceManager textResource, Type t, string key)
		{
			return textResource.Get(t.GetResourceName() + "." + key);
		}

		public static string Get(this IResourceManager textResource, Type t, string key, string suffix)
		{
			return textResource.Get(t.GetResourceName() + "." + key, suffix);
		}

		public static string Get(this IResourceManager textResource, ResourceKeyInfo k)
		{
			var p = k.Parts.Join(".");
			var t = k.Name;
			var s = "";
			if (!k.Suffix.IsEmpty()) s += "-" + k.Suffix;

			var text = "";
			if (textResource.TryGet($"{t}.{p}{s}", out text))
				return text;
			else if (k.SuffixIsOptional)
				if (textResource.TryGet($"{t}.{p}", out text))
					return text;

			if (k.Parts.Count == 1)
			{
				if (textResource.TryGet("Common." + k.Parts.Pop(), out text))
					return text;
			}

			var key = k.SuffixIsOptional ? $"{t}.{p}" : $"{t}.{p}{s}";
			textResource.SetNotFound(key);
			return key;
		}


		//public static ResourceKeyInfo GetResourceKey<TFunc>(this Expression<TFunc> exp)
		public static ResourceKeyInfo GetResourceKey(this LambdaExpression exp)
		{
			if (exp == null) throw new ArgumentNullException("exp");
			ResourceKeyInfo res = new ResourceKeyInfo();
			GetResourceKeyInt(exp.Body, res);
			return res;
		}

		public static Type GetResourceType(this Type type)
		{
			var res = type;
			var attr = res.GetCustomAttributes(typeof(ResourceTypeAttribute), false);
			if (attr != null && attr.Length > 0)
				res = (attr[0] as ResourceTypeAttribute).Type;
			return res;
		}

		static string GetResourceName(this Type type)
		{
			var attr = type.GetCustomAttributes(typeof(ResourceNameAttribute), false);
			if (attr != null && attr.Length > 0)
				return (attr[0] as ResourceNameAttribute).Name;

			attr = type.GetCustomAttributes(typeof(ResourceTypeAttribute), false);
			if (attr != null && attr.Length > 0)
				type = (attr[0] as ResourceTypeAttribute).Type;
			return type.FullName;
		}

		static void GetResourceKeyInt(Expression exp, ResourceKeyInfo res)
		{
			var t = exp.GetType();
			if (typeof(MethodCallExpression).IsAssignableFrom(t))
			{
				var methodExp = exp as MethodCallExpression;
				GetResourceKeyInt(methodExp.Object ?? methodExp.Arguments[0], res);
				return;
			}
			else if (typeof(ParameterExpression).IsAssignableFrom(t))
			{
				res.Name = GetResourceName(exp.Type);
				return;
			}
			else if (t == typeof(UnaryExpression))
			{
				var uExp = exp as UnaryExpression;
				GetResourceKeyInt(uExp.Operand, res);
				return;
			}
			else if (typeof(BinaryExpression).IsAssignableFrom(t))
			{
				var bExp = exp as BinaryExpression;
				GetResourceKeyInt(bExp.Left, res);
				return;
			}

			if (!(exp is MemberExpression memberExp))
				throw new Exception("Wrong format of the expression");
			if (!memberExp.Expression.Type.IsValueType)
			{
				var customName = memberExp.Member.GetCustomAttribute<ResourceNameAttribute>();
				if (customName != null)
					res.Parts.Push(customName.Name);
				else
					res.Parts.Push(memberExp.Member.Name);
			}
			if (memberExp.Expression != null)
				GetResourceKeyInt(memberExp.Expression, res);
		}
	}

	public static class ResourceManagerSpecialExtensions
	{
		public static string Description<T, T2>(this IResourceManager textResource, Expression<Func<T, T2>> exp)
		{
			return textResource.Description(exp.GetResourceKey());
		}

		public static string CaptionShort<T, T2>(this IResourceManager textResource, Expression<Func<T, T2>> exp)
		{
			return textResource.CaptionShort(exp.GetResourceKey());
		}

		public static string Description(this IResourceManager textResource, ResourceKeyInfo k)
		{
			k.Suffix = "description";
			return textResource.Get(k);
		}

		public static string CaptionShort(this IResourceManager textResource, ResourceKeyInfo k)
		{
			k.Suffix = "s";
			k.SuffixIsOptional = true;
			return textResource.Get(k);
		}

		public static string CaptionShort<T>(this IResourceManager textResource, string key)
		{
			var k = new ResourceKeyInfo {
				Name = typeof(T).FullName
			};
			k.Parts.Push(key);
			k.Suffix = "s";
			k.SuffixIsOptional = true;
			return textResource.Get(k);
		}

		public static string CaptionPlural<T>(this IResourceManager textResource)
		{
			var t = typeof(T);
			var attr = t.GetCustomAttributes(typeof(ResourceTypeAttribute), false);
			if (attr != null && attr.Length > 0)
				t = (attr[0] as ResourceTypeAttribute).Type;
			return textResource.Get(t.FullName, "pl");
		}
	}
}
