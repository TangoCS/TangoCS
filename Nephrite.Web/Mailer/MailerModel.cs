using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Web;

namespace Nephrite.Web.Mailer
{
	public interface IDC_Mailer : IDataContext
	{
		IQueryable<IMailMessage> MailMessage { get; }
		IQueryable<IMailTemplate> MailTemplate { get; }
		IMailMessage NewMailMessage();
	}

	public interface IMailMessage : IEntity
	{
		string Recipients { get; set; }
		string Subject { get; set; }
		string Body { get; set; }
		bool IsSent { get; set; }
		byte[] Attachment { get; set; }
		string AttachmentName { get; set; }
		string Error { get; set; }
		string CopyRecipients { get; set; }
		DateTime? LastSendAttemptDate { get; set; }
		int AttemptsToSendCount { get; set; }
		int MailMessageID { get; set; }
	}

	public interface IMailTemplate : IEntity
	{
		string Title { get; set; }
		string Comment { get; set; }
		bool IsSystem { get; set; }
		int MailTemplateID { get; set; }
		string TemplateBody { get; set; }
		string TemplateSubject { get; set; }
	}
}