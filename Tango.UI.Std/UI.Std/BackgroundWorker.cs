using Tango.Html;
using Tango.UI.Controls;

namespace Tango.UI.Std
{
	[OnAction(typeof(BackgroundWorker), "run")]
	public class BackgroundWorker : ViewPagePart
	{
		[Inject]
		public ITypeActivatorCache Cache { get; set; }

		public override void OnLoad(ApiResponse response)
		{
			var taskService = Context.GetArg("taskservice");
			var taskAction = Context.GetArg("taskaction");

			var taskUrl = new ActionLink(Context)
				.RunAction(taskService, taskAction)
				.UseDefaultResolver()
				.Url;

			(var type, var invoker) = Cache.Get(taskService + "." + taskAction) ?? (null, null);
			if (type == null)
				return;

			var key = type.FullName + "." + taskAction;
			var prefix = (taskService + "_" + taskAction).ToLower();
			var notificationContainer = prefix + "_link";
			var title = Resources.Get(key);

			response.AddWidget("#title", title);
			response.AddAdjacentWidget("popup-backgroundworker", prefix + "_link", AdjacentHTMLPosition.AfterBegin, w => {
				w.A(a => a.ID(notificationContainer).Class("task-progress").OnClick($"dialog.open('{prefix}_console')"), () => {
					w.Div(() => {
						w.Write(title);
						w.Span("0%");
					});
					w.Div(() => {
						w.Span(a => a.Style("width:0%"));
					});
				});
			});
			response.AddClientAction("backgroundworker", "init", new {
				taskUrl,
				logContainer = ParentElement.ID,
				notificationContainer
			});
		}
	}

	public static class BackgroundWorkerExtensions
	{
		public static void BackgroundWorkerButton(this LayoutWriter w)
		{
			w.Li(a => a.ID("backgroundworker"), () => {
				w.Span(() => {
					w.I(a => a.Icon("clipboard").Style("position:relative"), () => {
						w.B(a => a.ID("backgroundworker_counter").Class("badge").Class("hide"), "0");
					});				
				});
			});
		}

		public static void BackgroundWorkerMenu(this LayoutWriter w)
		{
			w.DropDownForElement("backgroundworker", () => {
				w.Div(a => a.Class("backgroundworker-footer"));
			});
		}

		public static ActionLink ToBackgroundTask<T>(this ActionLink link, string taskAction)
		{
			var key = typeof(T).FullName + "." + taskAction;
			var dialogPrefix = typeof(T).Name + "_" + taskAction;

			return link.To(typeof(BackgroundWorker), "run", "this")
				.WithArg("taskservice", typeof(T).Name)
				.WithArg("taskaction", taskAction)
				.AsConsoleDialog(dialogPrefix)
				.WithTitle(link.Resources.Get(key));
		}
	}
}
