using System.Reflection;
using Tango.AccessControl;
using Tango.Data;

namespace Tango.UI.Std
{
	public class BaseController : Controller
	{
		[Inject]
		public IDataContext DataContext { get; set; }

		public override bool CheckAccess(MethodInfo method)
		{
			var anon = method.GetCustomAttribute<AllowAnonymousAttribute>();
			if (anon != null) return true;

			var ac = Context.RequestServices.GetService(typeof(IAccessControl)) as IAccessControl;
			if (ac == null) return false;

			var so = method.GetCustomAttribute<SecurableObjectAttribute>();
			var soname = so != null ? so.Name : method.Name;

			return ac.Check(Context.Service + "." + soname);
		}
	}
}
