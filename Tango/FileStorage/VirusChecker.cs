namespace Tango.FileStorage
{
	public interface IVirusChecker
	{
		VirusCheckResult Check(string fileName, byte[] fileBytes);
	}

	public enum VirusCheckResult
	{
		OK, AntiViralFailure, AntiViralSuspicion
	}
}
