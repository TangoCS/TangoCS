using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using Nephrite.Web.Model;

namespace Nephrite.Web.Mailer
{
	public static class MailerTask
	{
		public static void Run(string server, int port, string login, string password, int timeout, bool enablessl,
			string fromEmail, string fromName, int pause, string testRecipient, int encoding)
		{
			using (var dc = Base.Model.NewDataContext() as IDC_Mailer)
			{
				var list = dc.MailMessage.Where(mm => !mm.IsSent && mm.AttemptsToSendCount < 5 &&
					(mm.LastSendAttemptDate == null ||	mm.LastSendAttemptDate.Value.AddHours(2 * mm.AttemptsToSendCount) < DateTime.Now)).ToList();
				if (list.Count == 0)
					return;

				SmtpClient client = new SmtpClient(server, port);
				client.Credentials = new NetworkCredential(login, password);
				client.Timeout = timeout * 1000;
				client.EnableSsl = enablessl;

				foreach (var msg in list)
				{
					System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();

					try
					{
						if (testRecipient != "")
							message.To.Add(testRecipient);
						else
						{
							ParseAddress(message.To, msg.Recipients, encoding);
							ParseAddress(message.CC, msg.CopyRecipients, encoding);
						}
					}
					catch
					{
						msg.Error = "Некорректный адрес e-mail";
						dc.SubmitChanges();
						continue;
					}
					message.From = new MailAddress(fromEmail, fromName, Encoding.GetEncoding(encoding));
					message.Subject = msg.Subject;
					message.Body = msg.Body;
					message.Priority = MailPriority.Normal;
					message.IsBodyHtml = true;
					message.SubjectEncoding = Encoding.GetEncoding(encoding);
					message.BodyEncoding = Encoding.GetEncoding(encoding);

					if (!String.IsNullOrEmpty(msg.AttachmentName))
					{
						message.Attachments.Add(new Attachment(new MemoryStream(msg.Attachment.ToArray()), msg.AttachmentName));
					}

					msg.AttemptsToSendCount++;
					msg.LastSendAttemptDate = DateTime.Now;
					try
					{

						client.Send(message);
						msg.IsSent = true;
						msg.Error = "";
					}
					catch (SmtpException se)
					{
						msg.Error = "Status Code: " + se.StatusCode + " " + se.Message;
					}
					catch (Exception ex)
					{
						msg.Error = "Error: " + ex.Message;
						if (ex.InnerException != null)
							msg.Error += Environment.NewLine + ex.InnerException.Message;
					}
					dc.SubmitChanges();
					Thread.Sleep(pause * 1000);
				}
			}
		}

		const string addrRegex = "(.*)<(.*)>";
		static void ParseAddress(MailAddressCollection c, string recipients, int encoding)
		{
			if (recipients == null)
				return;
			string[] addr = recipients.Split(';');
			foreach (var a in addr)
			{
				if (a.ValidateEmail())
					c.Add(a);
				else
				{
					Match m = Regex.Match(a, addrRegex);
					if (m != null && m.Success)
					{
						if (m.Groups[2].Value.ValidateEmail())
							c.Add(new MailAddress(m.Groups[2].Value, m.Groups[1].Value, Encoding.GetEncoding(encoding)));
					}
				}
			}
		}

	}
}
