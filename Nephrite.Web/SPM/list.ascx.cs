using System;
using System.Linq;
using Nephrite.Web.Controls;

namespace Nephrite.Web.SPM
{
	public partial class list : ViewControl<IQueryable<SPM_Role>>
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			SetTitle("Роли и полномочия");

			filter.AddFieldString<SPM_Role>("Название", o => o.Title);

			toolbar.AddItemFilter(filter);
			toolbar.AddItemSeparator();
			toolbar.AddItem<SPMController>("add.png", "Создать", c => c.New());
		}
	}
}