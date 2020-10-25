using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tango.RealTime;

namespace Tango.LongOperation
{
	public class LongOperationServer
	{
		public static readonly List<Task> Threads = new List<Task>();
		public static readonly LinkedList<LongOperationTicket> Queue = new LinkedList<LongOperationTicket>();
	}
}
