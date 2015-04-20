using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nephrite.MVC
{
	public interface IViewRenderer
	{
		void RenderMessage(string message);
		void RenderMessage(string title, string message);
		void RenderView(string viewName, object viewData);
		void RenderView(string folder, string viewName, object viewData);
	}
}
