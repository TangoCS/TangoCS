using System;

namespace Tango.Logger
{
	public interface IRealTimeProgressLogger : IProgressLogger
	{
		//string WriteLogHistory();

	}
	public interface IProgressLogger
	{
		void SetItemsCount(int itemsCount);
		void SetProgress(int itemsCompleted);
		void WriteMessage(string message, int? itemsCompleted = null);
		void WriteExeptionMessage(Exception ex);

	}

	public class ConsoleLogger : IProgressLogger
	{
		public void SetItemsCount(int itemsCount)
		{
			Console.WriteLine("Items: " + itemsCount.ToString());
		}

		public void SetProgress(int itemsCompleted)
		{

		}
		public void WriteExeptionMessage(Exception ex)
		{		
		
			
		}

		public void WriteMessage(string message, int? itemsCompleted = null)
		{
			Console.WriteLine(message);
		}
		
	}
}
