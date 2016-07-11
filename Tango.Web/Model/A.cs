using System;
using System.Collections;
using System.Web;
using Nephrite.Data;
using Nephrite.Meta;

namespace Nephrite.Web
{
	public class A
	{
		public static IDataContext Model
		{
			get
			{
				return A.Items["SolutionDataContext"] as IDataContext;
			}
			set
			{
				A.Items["SolutionDataContext"] = value;
			}
		}

		public static IServiceProvider RequestServices
		{
			get
			{
				return A.Items["RequestServices"] as IServiceProvider;
			}
			set
			{
				A.Items["RequestServices"] = value;
			}
		}

		public static MetaSolution Meta { get; set; }

		public static IDictionary Items
		{
			get
			{
				if (HttpContext.Current != null) return HttpContext.Current.Items;

				Hashtable ht = AppDomain.CurrentDomain.GetData("ContextItems") as Hashtable;
				if (ht == null)
				{
					ht = new Hashtable();
					AppDomain.CurrentDomain.SetData("ContextItems", ht);
				}
				return ht;

			}
		}
	}

	public static class DIExtensions
	{
		public static T GetService<T>(this IServiceProvider provider)
		{
			return (T)provider.GetService(typeof(T));
		}
	}

	
}