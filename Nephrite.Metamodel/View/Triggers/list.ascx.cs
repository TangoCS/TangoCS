using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Nephrite.Web;
using Nephrite.Web.Controls;
using Microsoft.SqlServer.Management.Smo;
using System.Linq.Expressions;
using System.Data.Linq.SqlClient;
using Nephrite.Metamodel.Controllers;

namespace Nephrite.Metamodel.View
{
	public partial class Triggers_list : ViewControl
	{
		protected IQueryable<Trigger> viewData;
		protected void Page_Load(object sender, EventArgs e)
		{
			SetTitle("Триггеры таблиц");

			DbGenerator gen = new DbGenerator(false, false);
			viewData = gen.GetTableTriggers().AsQueryable();
			
			toolbar.AddItemFilter(filter);
			toolbar.AddItemSeparator();
			toolbar.AddItem<TriggersController>("add.png", "Создать", c => c.CreateNew(Query.CreateReturnUrl()));

			filter.AddFieldString<Trigger>("Таблица", o => ((Microsoft.SqlServer.Management.Smo.Table)o.Parent).Name);
			
			string search = "<input type=\"text\" onblur=\"if(this.value ==''){this.value='Поиск';this.className = 'filterInput TextItalic';this.s=''}\"" +
					" onfocus=\"if(this.s!='t'){this.value='';this.className = 'filterInput filterInputActive';this.s='t';}\" name=\"qfind\" autocomplete=\"Off\"" +
					" value=\"Поиск\" class=\"filterInput TextItalic\" id=\"qfind\" onkeydown=\"return event.keyCode != 13;\" onkeyup=\"filter();\"/>";
			toolbar.AddRightItemText(search);

			toolbar.AddRightItemSeparator();
			toolbar.EnableViews(filter);

			SearchExpression = s => (o => ((Microsoft.SqlServer.Management.Smo.Table)o.Parent).Name.ToLower().Contains(s.ToLower()) || o.Name.ToLower().Contains(s.ToLower()));
		}
		public Func<string, Expression<Func<Trigger, bool>>> SearchExpression { get; set; }
	}
}