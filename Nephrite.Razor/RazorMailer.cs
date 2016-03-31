using Nephrite.Data;
using Nephrite.Mailer;
using RazorEngine;
using RazorEngine.Templating;
using Dapper;

namespace Nephrite.Razor
{
	public class RazorMailer : IMailer
	{
		IDataContext _dc;
		public RazorMailer(IDataContext dc)
		{
			_dc = dc;
		}

		public void Send(string emailAddress, string mailTemplateName, object viewData)
		{
			string subject, body;
			if (Engine.Razor.IsTemplateCached(mailTemplateName, viewData.GetType()))
			{
				body = Engine.Razor.Run(mailTemplateName, viewData.GetType(), viewData);
				subject = Engine.Razor.Run(mailTemplateName + "-subject", viewData.GetType(), viewData);
			}
			else
			{
				var t = _dc.Connection.ExecuteScalar<TemplateDTO>("select templatebody, templatesubject from mailtemplate where sysname = @p1", mailTemplateName);
				body = Engine.Razor.RunCompile(t.templatebody, mailTemplateName, viewData.GetType(), viewData);
				subject = Engine.Razor.RunCompile(t.templatesubject, mailTemplateName + "-subject", viewData.GetType(), viewData);
			}

			_dc.CommandOnSubmit("insert into MailMessage (Recipients, Subject, Body) values (@p1, @p2, @p3)", emailAddress, subject, body); 		}

		class TemplateDTO
		{
			public string templatebody { get; set; }
			public string templatesubject { get; set; }
		}
	}
}
