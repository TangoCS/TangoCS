using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Tango.Data;

namespace Tango.Localization
{
	public class TextResourceOptions
	{
		public SortedSet<string> NotFoundResources { get; } = new SortedSet<string>();

		public IReadOnlyDictionary<string, string> Resources { get; set; }
		public IReadOnlyDictionary<string, string> Images { get; set; }
	}

	public class TextResource : ITextResource
	{
		ILanguage _language;
		public static TextResourceOptions Options { get; set; }

		public TextResource(ILanguage language)
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

	public interface IWithResourceKey
	{
		Type GetResourceKey();
	}

	public class RecourceKeyInfo
	{
		public Stack<string> Parts { get; set; } = new Stack<string>(4);
		public Type Type { get; set; }
		public string Suffix { get; set; }
		public bool SuffixIsOptional { get; set; }
	}

	public static class TextResourceExtensions
	{
		public static string Get<T>(this ITextResource textResource, Expression<Func<T, object>> exp)
			where T : IWithResourceKey
		{
			return textResource.Get(exp.GetResourceKey());
		}

		public static string Get<T, TValue>(this ITextResource textResource, Expression<Func<T, TValue>> exp)
			where T : IWithResourceKey
		{
			return textResource.Get(exp.GetResourceKey());
		}

		public static string Get<T>(this ITextResource textResource, Expression<Func<T, object>> exp, string suffix)
			where T : IWithResourceKey
		{
			var k = exp.GetResourceKey();
			k.Suffix = suffix;
			return textResource.Get(k);
		}

		public static string Get(this ITextResource textResource, string key, string suffix)
		{
			return textResource.Get(key + "-" + suffix);
		}

		public static string Get<T>(this ITextResource textResource, string suffix)
			where T : IEntity
		{
			return textResource.Get(typeof(T).FullName, suffix);
		}

		public static string Get(this ITextResource textResource, RecourceKeyInfo k)
		{
			var p = k.Parts.Join(".");
			var t = k.Type.FullName;
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


		public static RecourceKeyInfo GetResourceKey<TIn, TOut>(this Expression<Func<TIn, TOut>> exp)
			where TIn : IWithResourceKey
		{
			if (exp == null) throw new ArgumentNullException("exp");
			RecourceKeyInfo res = new RecourceKeyInfo();
			GetResourceKeyInt(exp.Body, res);
			return res;
		}

		static void GetResourceKeyInt(Expression exp, RecourceKeyInfo res)
		{
			if (typeof(MethodCallExpression).IsAssignableFrom(exp.GetType()))
			{
				var methodExp = exp as MethodCallExpression;
				GetResourceKeyInt(methodExp.Object ?? methodExp.Arguments[0], res);
				return;
			}
			if (typeof(ParameterExpression).IsAssignableFrom(exp.GetType()))
			{
				res.Type = exp.Type;
				return;
			}
			var memberExp = exp as MemberExpression;
			if (memberExp == null)
				throw new Exception("Wrong format of the expression");
			if (!memberExp.Expression.Type.IsValueType)
				res.Parts.Push(memberExp.Member.Name);
			if (memberExp.Expression != null)
				GetResourceKeyInt(memberExp.Expression, res);
		}
	}

	public static class TextResourceSpecialExtensions
	{
		public static string Description<T, TValue>(this ITextResource textResource, Expression<Func<T, TValue>> exp)
			where T : IWithResourceKey
		{
			return textResource.Description(exp.GetResourceKey());
		}

		public static string CaptionShort<T, TValue>(this ITextResource textResource, Expression<Func<T, TValue>> exp)
			where T : IWithResourceKey
		{
			return textResource.CaptionShort(exp.GetResourceKey());
		}

		public static string Description(this ITextResource textResource, RecourceKeyInfo k)
		{
			k.Suffix = "description";
			return textResource.Get(k);
		}

		public static string CaptionShort(this ITextResource textResource, RecourceKeyInfo k)
		{
			k.Suffix = "s";
			k.SuffixIsOptional = true;
			return textResource.Get(k);
		}

		public static string CaptionPlural<T>(this ITextResource textResource)
			where T : IEntity
		{
			return textResource.Get(typeof(T).FullName, "pl");
		}
	}
}
