using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nephrite.Http
{
	public class WebSocketAcceptContext : IWebSocketAcceptContext
	{
		public virtual string SubProtocol { get; set; }
	}
}
