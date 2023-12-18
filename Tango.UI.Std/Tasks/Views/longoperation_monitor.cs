using System;
using System.Linq;
using System.Linq.Expressions;
using Tango;
using Tango.Html;
using Tango.Localization;
using Tango.LongOperation;
using Tango.UI;
using Tango.UI.Controls;
using Tango.UI.Std;

namespace Tango.Tasks
{
	[OnAction("longoperation", "monitor")]
	public class longoperation_monitor : default_list<LongOperationTicket>
	{
		protected override Func<string, Expression<Func<LongOperationTicket, bool>>> SearchExpression => s => o => o.ActionID == s.ToInt32();

		protected override IQueryable<LongOperationTicket> Data => LongOperationServer.Queue.AsQueryable();

		protected override void ToolbarLeft(MenuBuilder t)
		{
			t.Item(w => w.ActionTextButton(a => a.ToCurrent().WithTitle(Resources.Get("longoperation.updatequeue"))));
			//t.ItemActionText(x => x.To("longoperation", "updatequeue", AccessControl));
			t.ItemSeparator();
			t.ItemActionText(x => x.To("longoperation", "runtask", AccessControl));
		}

		protected override void FieldsInit(FieldCollection<LongOperationTicket, LongOperationTicket> f)
		{
			f.AddHeader(Context.Resources.Get<LongOperationTicket>("type"));
			f.AddHeader(o => o.ActionID);
			f.AddHeader(o => o.Title);
			f.AddHeader(o => o.CreateDate);
			f.AddHeader(o => o.RunDate);
			f.AddHeader(o => o.UserName);
			f.AddHeader(o => o.IsManualStart);
			f.AddHeader(o => o.OneThread);
			f.AddHeader(o => o.Priority);
			f.AddHeader(o => o.Status);

			f.AddCell(o => o.GetType().Name);
			f.AddCell(o => o.ActionID);
			f.AddCell(o => o.Title);
			f.AddCell(o => o.CreateDate.ToString("dd.MM.yyyy HH:mm:ss"));
			f.AddCell(o => o.RunDate?.ToString("dd.MM.yyyy HH:mm:ss"));
			f.AddCell(o => o.UserName);
			f.AddCell(o => o.IsManualStart.Icon());
			f.AddCell(o => o.OneThread.Icon());
			f.AddCell(o => o.Priority);
			f.AddCell(o => o.Status.ToString());
		}
	}
}