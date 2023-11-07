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
		public int Timeout { get; } // timeout (min)
		public bool IsManualStart { get; }
		public abstract bool OneThread { get; }

		public LongOperationStatus Status { get; set; }
		public string DefaultTaskAssembly { get; set; }

		protected IServiceProvider provider;

		public LongOperationTicket(int timeout = 60, bool isManualStart = false)
		{
			ID = Guid.NewGuid();
			CreateDate = DateTime.Now;
			Timeout = timeout;
			IsManualStart = isManualStart;
		}

		public virtual void Run(IServiceProvider provider, IProgressLogger progressLogger)
		{
			RunDate = DateTime.Now;
			this.provider = provider;
		}
	}

	public enum LongOperationStatus
	{
		InQueue,
		Running
	}
}
