using System;
using System.Collections.Generic;
using System.Linq;
using Tango.Meta;
using Tango.Localization;

namespace Tango.UI
{
	public static class ValidationBuilderExtensions
	{
		const string GROUP = "entitycheck";

		public static ValidationBuilder<T> Check<TClass, T>(this ValidationMessageCollection c, IResourceManager textResource, MetaAttribute<TClass, T> prop, DynamicDictionary dto)
		{
			return new ValidationBuilder<T>(textResource, c, prop.Name, textResource.Caption(prop), dto.Parse<T>(prop.Name));
		}

		public static ValidationBuilder<TKey> Check<TClass, T, TKey>(this ValidationMessageCollection c, IResourceManager textResource, MetaReference<TClass, T, TKey> prop, DynamicDictionary dto)
		{
			return new ValidationBuilder<TKey>(textResource, c, prop.Name, textResource.Caption(prop), dto.Parse<TKey>(prop.Name));
		}

		public static ValidationBuilder<T> Check<T>(this ValidationMessageCollection c, IResourceManager textResource, IMetaProperty prop, T value)
		{
			return new ValidationBuilder<T>(textResource, c, prop.Name, textResource.Caption(prop), value);
		}

		//public static ValidationBuilder<T> Check<TClass, T>(this ValidationMessageCollection c, Field<TClass, T> f)
		//	where TClass : class
		//{
		//	return new ValidationBuilder<T>(f.Resources, c, f.ID, f.Caption, f.Value);
		//}

		static string Msg<T>(ValidationBuilder<T> val, string msgType, string data = null)
		{
			return string.Format(val.Resources.Get($"common.validation.{msgType}"), val.ElementName, data);
		}

        public static ValidationBuilder<T> NotEmpty<T>(this ValidationBuilder<T> val, T defaultValue = default(T), ValidationMessageSeverity severity = ValidationMessageSeverity.Error)
        {
            if (val is bool)
                return val;
            if (val.Value == null || (val.Value is string && val.Value.ToString().IsEmpty()) || Equals(val.Value, defaultValue))
            {
                val.Collection.Add(GROUP, val.ElementID, Msg(val, "NotEmpty"), severity);
            }
            return val;
        }

		public static ValidationBuilder<T> NotNull<T>(this ValidationBuilder<T> val, ValidationMessageSeverity severity = ValidationMessageSeverity.Error)
		{
			if (val.Value == null || (val.Value is string && val.Value.ToString().IsEmpty()))
			{
				val.Collection.Add(GROUP, val.ElementID, Msg(val, "NotNull"), severity);
			}
			return val;
		}

		public static ValidationBuilder<T> Between<T>(this ValidationBuilder<T> val, T from, T to, bool incLeft = true, bool incRight = true, ValidationMessageSeverity severity = ValidationMessageSeverity.Error)
		{
			var comp = Comparer<T>.Default;
			var b = true;
			if (incLeft && incRight) b = comp.Compare(val.Value, from) >= 0 && comp.Compare(val.Value, to) <= 0;
			if (!incLeft && incRight) b = comp.Compare(val.Value, from) > 0 && comp.Compare(val.Value, to) <= 0;
			if (incLeft && !incRight) b = comp.Compare(val.Value, from) >= 0 && comp.Compare(val.Value, to) < 0;
			if (!incLeft && !incRight) b = comp.Compare(val.Value, from) > 0 && comp.Compare(val.Value, to) < 0;

			if (!b)
			{
				val.Collection.Add(GROUP, val.ElementID, Msg(val, "Between"), severity);
			}
			return val;
		}

		public static ValidationBuilder<T> GreaterThan<T>(this ValidationBuilder<T> val, T valueToCompare, ValidationMessageSeverity severity = ValidationMessageSeverity.Error)
		{
			if (!(Comparer<T>.Default.Compare(val.Value, valueToCompare) > 0))
			{
				val.Collection.Add(GROUP, val.ElementID, Msg(val, "GreaterThan"), severity);
			}
			return val;
		}

		public static ValidationBuilder<T> GreaterOrEqualThan<T>(this ValidationBuilder<T> val, T valueToCompare, ValidationMessageSeverity severity = ValidationMessageSeverity.Error)
		{
			if (!(Comparer<T>.Default.Compare(val.Value, valueToCompare) >= 0))
			{
				val.Collection.Add(GROUP, val.ElementID, Msg(val, "GreaterOrEqualThan"), severity);
			}
			return val;
		}

		public static ValidationBuilder<T> LessThan<T>(this ValidationBuilder<T> val, T valueToCompare, ValidationMessageSeverity severity = ValidationMessageSeverity.Error)
		{
			if (!(Comparer<T>.Default.Compare(val.Value, valueToCompare) < 0))
			{
				val.Collection.Add(GROUP, val.ElementID, Msg(val, "LessThan"), severity);
			}
			return val;
		}

		public static ValidationBuilder<T> LessOrEqualThan<T>(this ValidationBuilder<T> val, T valueToCompare, ValidationMessageSeverity severity = ValidationMessageSeverity.Error)
			
		{
			if (!(Comparer<T>.Default.Compare(val.Value, valueToCompare) <= 0))
			{
				val.Collection.Add(GROUP, val.ElementID, Msg(val, "LessOrEqualThan"), severity);
			}
			return val;
		}

		public static ValidationBuilder<string> MinLength(this ValidationBuilder<string> val, int min, ValidationMessageSeverity severity = ValidationMessageSeverity.Error)
		{
			if (val.Value is string)
			{
				string s = val.Value as string;
				if (!(s.Length >= min))
				{
					val.Collection.Add(GROUP, val.ElementID, Msg(val, "MinLength", min.ToString()), severity);
				}
			}
			return val;
		}

		public static ValidationBuilder<string> MaxLength(this ValidationBuilder<string> val, int max, ValidationMessageSeverity severity = ValidationMessageSeverity.Error)
		{
			if (val.Value is string)
			{
				string s = val.Value as string;
				if (!(s.Length <= max))
				{
					val.Collection.Add(GROUP, val.ElementID, Msg(val, "MaxLength", max.ToString()), severity);
				}
			}
			return val;
		}

		public static ValidationBuilder<string> CheckChars(this ValidationBuilder<string> val, string chars, ValidationMessageSeverity severity = ValidationMessageSeverity.Error)
		{
			foreach (char c in val.Value.ToCharArray())
			{
				if (!chars.Contains(c))
				{
					val.Collection.Add(GROUP, val.ElementID, Msg(val, "CheckChars", chars), severity);
					break;
				}
			}
			return val;
		}
	}
}
