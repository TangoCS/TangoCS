using System;

namespace Tango.Logger
{
	public interface IProgressLogger
	{
		void SetItemsCount(int itemsCount);
		void SetProgress(int itemsCompleted);
		void WriteMessage(string message, int? itemsCompleted = null);
	}
}
