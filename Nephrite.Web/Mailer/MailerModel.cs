﻿using System;
using System.Collections.Generic;
using System.Linq;
using Nephrite.Data;

namespace Nephrite.Web.Mailer
{
	public interface IDC_Mailer : IDataContext
	{
		ITable<IMailMessage> IMailMessage { get; }
		ITable<IMailTemplate> IMailTemplate { get; }
		IMailMessage NewIMailMessage();
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