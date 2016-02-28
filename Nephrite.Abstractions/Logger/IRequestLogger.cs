namespace Nephrite.Logger
{
	public interface IRequestLogger
	{
		bool Enabled { get; }
		void Write(string message);
		void WriteFormat(string message, object[] args);
		string ToString();
	}
}
