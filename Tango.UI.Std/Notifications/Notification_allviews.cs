using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tango.Data;
using Tango.Html;
using Tango.UI;
using Tango.UI.Controls;
using Tango.UI.Std;

namespace Tango.Notifications
{
	[OnAction(typeof(Notification), "viewlist")]
	public class Notification_list : default_list_rep<Notification>
	{
		[Inject]
		public NotificationsHelper NotificationsHelper { get; set; }
		protected override IQueryable<Notification> DefaultOrderBy(IQueryable<Notification> data)
		{
			return data.OrderByDescending(o => o.CreateDate);
		}

		protected override void ToolbarLeft(MenuBuilder t)
		{
			t.ItemHeader("Извещения");
		}

		protected override void ToolbarRight(MenuBuilder t)
		{
			if (_itemsCount > 0)
				t.Item(w => w.ActionLink(a => a.ToCurrent().KeepTheSameUrl().PostEvent(OnClearAll).WithTitle("Очистить все")));
		}

		public override void OnInit()
		{
			base.OnInit();
			Renderer = new NotificationListRenderer<Notification>();
			Sections.RenderPaging = false;
			Paging.PageSize = 1000;
		}

		public override void OnLoad(ApiResponse response)
		{
			base.OnLoad(response);
			response.AddWidget("buttonsbar", w => {
				w.ActionLink(a => a.To<Notification>("viewlistall", AccessControl, null, null).WithTitle("Просмотреть все"));
			});
		}

		protected override void FieldsInit(FieldCollection<Notification> f)
		{
			f.RowAttributes += (a, o, i) => {
				a.Class("notification").OnClickPostEvent(OnClear).DataParm(Constants.Id, o.NotificationID);
			};

			f.AddCell((w, o) => {
				w.Div(a => a.Class("pic"), () => w.Icon(o.NotificationCategoryIcon));
				w.Div(a => a.Class("text"), () => {
					w.Div(o.NotificationText);
					w.Div(a => a.Class("time"), o.CreateDate.ToString("dd.MM.yyyy HH:mm"));
				});
			});
		}

		public void OnClearAll(ApiResponse response)
		{
			((INotificationRepository)Repository).MarkReadAll();

			response.AddWidget(Sections.ContentBody, Render);
			RenderToolbar(response);
			AfterRender(response);
			response.AddWidget("#notifications", w => NotificationsHelper.TopMenuIcon(w));
		}

		public void OnClear(ApiResponse response)
		{
			var id = Context.GetIntArg(Constants.Id, 0);
			((INotificationRepository)Repository).MarkRead(id);			

			response.AddWidget(Sections.ContentBody, Render);
			RenderToolbar(response);
			AfterRender(response);
			response.AddWidget("#notifications", w => NotificationsHelper.TopMenuIcon(w));
		}

		public class NotificationListRenderer<TResult> : ListRendererAbstract<TResult>
		{
			public override void Render(LayoutWriter w, IEnumerable<TResult> result, IFieldCollection<TResult> fields)
			{
				w.Div(fields.ListAttributes, () => {
					if (result.Count() > 0)
					{
						int i = 1;
						foreach (var o in result)
						{
							var r = new RowInfo<TResult> { RowNum = i, Level = 0 };
							w.Div(a => fields.RowAttributes?.Invoke(a, o, r), () => {
								foreach (var col in fields.Cells)
									foreach (var c in col.AsEnumerable())
										c.Content(w, o, r);
							});
							i++;
						}
					}
					else
						w.Div(a => a.Class("nodata"), "Нет новых извещений.");
				});
			}
		}
	}


	[OnAction(typeof(Notification), "viewlistall")]
	public class Info_Notification_list_all : default_list_rep<Notification>
	{
		protected override IRepository<Notification> GetRepository()
		{
			return ((INotificationRepository)base.GetRepository()).ReturnAllNotifications();
		}

		protected override IQueryable<Notification> DefaultOrderBy(IQueryable<Notification> data)
		{
			return data.OrderByDescending(o => o.CreateDate);
		}

		protected override void ToolbarLeft(MenuBuilder t)
		{

		}

		protected override void FieldsInit(FieldCollection<Notification> f)
		{
			f.AddCellWithSort(o => o.CreateDate, o => o.CreateDate.ToString("dd.MM.yyyy HH:mm"));
			f.AddCell(o => o.NotificationText, o => o.NotificationText);
		}

		public void OnClearAll(ApiResponse response)
		{
		}
	}
}
