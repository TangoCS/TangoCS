namespace Nephrite.MVC
{
	public interface IViewRenderer
	{
		void RenderMessage(string message);
		void RenderMessage(string title, string message);
		void RenderView(string folder, string viewName, object viewData);
		void RenderHtml(string title, string html);
		bool IsStringResult { get; }
	}
}
