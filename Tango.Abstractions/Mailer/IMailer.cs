namespace Tango.Mailer
{
	public interface IMailer
	{
		void Send(string emailAddress, string mailTemplateName, object viewData);
	}
}
