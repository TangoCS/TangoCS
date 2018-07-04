using System.Linq;
using Tango.Meta;

namespace Tango.UI
{
	public static class FluentExtensions
	{
		public static IMetaOperation Parm(this IMetaOperation op, IMetaParameterType type, string name)
		{
			var parm = op.Parameters.FirstOrDefault(o => o.Name.ToLower() == name.ToLower());
			if (parm == null)
			{
				parm = new MetaParameter { Name = name, Type = type };
				op.Parameters.Add(parm);
			}
			return op;
		}

		public static IMetaOperation ParmString(this IMetaOperation op, string name)
		{
			return op.Parm(TypeFactory.String, name);
		}
		public static IMetaOperation ParmInt(this IMetaOperation op, string name)
		{
			return op.Parm(TypeFactory.Int, name);
		}
		public static IMetaOperation ParmGuid(this IMetaOperation op, string name)
		{
			return op.Parm(TypeFactory.Guid, name);
		}
		public static IMetaOperation ParmGuidId(this IMetaOperation op)
		{
			return op.ParmGuid(Constants.Id);
		}
		public static IMetaOperation ParmIntId(this IMetaOperation op)
		{
			return op.ParmInt(Constants.Id);
		}
		public static IMetaOperation ParmReturnUrl(this IMetaOperation op)
		{
			return op.ParmString(Constants.ReturnUrl);
		}

		public static IMetaOperation WithImage(this IMetaOperation op, string name)
		{
			op.Image = name;
			return op;
		}


		public static IMetaReference<TClass, TRefClass> Aggregation<TClass, TRefClass>(this IMetaReference<TClass, TRefClass> r)
		{
			r.AssociationType = AssociationType.Aggregation;
			return r;
		}

		public static IMetaReference<TClass, TRefClass> Composition<TClass, TRefClass>(this IMetaReference<TClass, TRefClass> r)
		{
			r.AssociationType = AssociationType.Composition;
			return r;
		}

		public static ICanBeMultilingual Multilingual(this ICanBeMultilingual a)
		{
			a.IsMultilingual = true;
			return a;
		}
	}
}
