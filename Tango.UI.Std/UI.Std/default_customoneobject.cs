using System.Linq;
using Tango.Data;
using Tango.Logic;

namespace Tango.UI.Std
{
	public abstract class default_customoneobject<T, TKey> : ViewPagePart
		where T : class, IWithKey<T, TKey>, new()
	{
		[Inject]
		protected IDataContext DataContext { get; set; }

		T _viewData = null;
		protected T ViewData {
			get {
				if (_viewData == null)
					_viewData = GetLogic();
				return _viewData;
			}
		}

		protected virtual T GetLogic()
		{
			TKey id = Context.AllArgs.Parse<TKey>(Constants.Id);
			return CommonLogic.GetFiltered<T, TKey>(DataContext, id);
		}
	}
}
