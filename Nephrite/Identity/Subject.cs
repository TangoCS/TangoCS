using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using Microsoft.Framework.DependencyInjection;

namespace Nephrite.Identity
{
	public class Subject<TKey>
	{
		public TKey ID { get; set; }
		public string Name { get; set; }
		public string Title { get; set; }
		public string Email { get; set; }
		public string PasswordHash { get; set; }
		public bool IsActive { get; set; }
		public bool IsDeleted { get; set; }
		public string SID { get; set; }
		public bool MustChangePassword { get; set; }

		public static Subject<TKey> Current
		{
			get
			{
				return DI.RequestServices.GetService<IIdentityManager<TKey>>().CurrentSubject;
			}
		}

		public static Subject<TKey> System
		{
			get
			{
				return DI.RequestServices.GetService<IIdentityManager<TKey>>().SystemSubject;
			}
		}

		public void Run(Action action)
		{
			var oldSubject = Current;
			var ctx = DI.RequestServices.GetService<IIdentityManager<TKey>>().HttpContext;
			ctx.Items["CurrentSubject2"] = this;
			action();
			ctx.Items["CurrentSubject2"] = oldSubject;
		}
	}

	public class Subject : Subject<int>
	{

	}
}
