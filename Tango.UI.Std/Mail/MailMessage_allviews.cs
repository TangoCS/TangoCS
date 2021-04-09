using System;
using System.Linq.Expressions;
using Tango.UI;
using Tango.UI.Controls;
using Tango.UI.Std;

namespace Tango.Mail
{
    [OnAction(typeof(MailMessage), "viewlist")]
    public class MailMessage_viewlist : default_list_rep<MailMessage>
    {
        
        protected override Func<string, Expression<Func<MailMessage, bool>>> SearchExpression => s => {
            return o => o.Subject.Contains(s) || o.Recipients.Contains(s); 
        };

        protected override void ToolbarLeft(MenuBuilder t)
        {
            t.ItemFilter(Filter);
        }
		
        protected override void FieldsInit(FieldCollection<MailMessage> f)
        {
            f.SetRowID(o => o.MailMessageID.ToString());

            f.AddCellWithSortAndFilter(o => o.Recipients, o => o.Recipients);
            f.AddCellWithSortAndFilter(o => o.Subject, o => o.Subject);
            f.AddCellWithSortAndFilter(o => o.Body, o => o.Body);
            f.AddCellWithSortAndFilter(o => o.MailMessageStatus, o => o.MailMessageStatus);
            f.AddCellWithSortAndFilter(o => o.TimeoutValue, o => o.TimeoutValue);
            f.AddCellWithSortAndFilter(o => o.AttachmentName, o => o.AttachmentName);
            f.AddCellWithSortAndFilter(o => o.Error, o => o.Error);
            f.AddCellWithSortAndFilter(o => o.CopyRecipients, o => o.CopyRecipients);
            f.AddCellWithSortAndFilter(o => o.LastSendAttemptDate, o => o.LastSendAttemptDate.DateTimeToString());
            f.AddCellWithSortAndFilter(o => o.AttemptsToSendCount, o => o.AttemptsToSendCount);
            f.AddCellWithSortAndFilter(o => o.LastModifiedDate, o => o.LastModifiedDate.DateTimeToString());
        }
    }
}