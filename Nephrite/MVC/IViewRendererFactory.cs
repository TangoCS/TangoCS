using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nephrite.MVC
{
	public interface IViewRendererFactory
	{
		IViewRenderer Create(Type rendererType);
	}
}
