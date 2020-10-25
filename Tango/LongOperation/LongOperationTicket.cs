using System;
using System.Collections.Generic;
using System.Text;
using Tango.Logger;

namespace Tango.LongOperation
{
	public abstract class LongOperationTicket
	{
		public Guid ID { get; }
		public abstract string Title { get; }
		public abstract int ActionID { get; }
		public DateTime CreateDate { get; }
		public DateTime? RunDate { get; private set; }
		// timeout (min)
		public int Timeout { get; }
		public LongOperationStatus Status { get; set; }

		public LongOperationTicket(int timeout = 60)
		{
			ID = Guid.NewGuid();
			CreateDate = DateTime.Now;
			Timeout = timeout;
		}

		public virtual void Run(IServiceProvider provider, IProgressLogger progressLogger)
		{
			RunDate = DateTime.Now;
		}
	}

	public enum LongOperationStatus
	{
		InQueue,
		Running
	}
}
