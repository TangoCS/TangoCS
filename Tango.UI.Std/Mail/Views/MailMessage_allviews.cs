using System;
using System.Linq.Expressions;
using Tango.Data;
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
            
            f.AddCellWithSortAndFilter(o => o.MailCategoryTitle, o => o.MailCategoryTitle);
            f.AddCellWithSortAndFilter(o => o.Subject, o => o.Subject);
            f.AddCellWithSortAndFilter(o => o.Body, o => o.Body);
            f.AddCellWithSortAndFilter(o => o.AttachmentName, o => o.AttachmentName);
            f.AddCellWithSortAndFilter(o => o.CreateDate, o => o.CreateDate.DateTimeToString());
            f.AddCellWithSortAndFilter(o => o.TimeoutValue, o => o.TimeoutValue);
            f.AddCellWithSortAndFilter(o => o.StartSendDate, o => o.StartSendDate.DateTimeToString());
            f.AddCellWithSortAndFilter(o => o.FinishSendDate, o => o.FinishSendDate.DateTimeToString());
            f.AddCellWithSortAndFilter(o => o.MaxAttemptsToSendCount, o => o.MaxAttemptsToSendCount);
            f.AddCellWithSortAndFilter(o => o.Recipients, o => o.Recipients);
            f.AddCellWithSortAndFilter(o => o.CopyRecipients, o => o.CopyRecipients);
            f.AddCellWithSortAndFilter(o => o.LastModifiedUserTitle, o => o.LastModifiedUserTitle);
            f.AddCellWithSortAndFilter(o => o.MailMessageStatus, o => o.MailMessageStatus);
            f.AddCellWithSortAndFilter(o => o.AttemptsToSendCount, o => o.AttemptsToSendCount);
            f.AddCellWithSortAndFilter(o => o.LastSendAttemptDate, o => o.LastSendAttemptDate.DateTimeToString());
            f.AddCellWithSortAndFilter(o => o.Error, o => o.Error);
            f.AddActionsCell(
                o => al => al.To<MailMessageAttachment>("viewlist", AccessControl)
                .WithArg(Constants.Id, o.ID).WithArg("title", o.Subject).WithImage("hie").WithTitle("Состав письма"),
                o => al => al.ToDelete(AccessControl, o));
        }
    }
    
    [OnAction(typeof(MailMessageAttachment), "viewlist")]
    public class MailMessageAttachment_viewlist : default_list_rep<MailMessageAttachment>
    {
        public int MailMessageId { get; set; }

        protected override string FormTitle => $"Состав письма \"{Context.GetArg("title")}\"";

        protected override IRepository<MailMessageAttachment> GetRepository()
        {
            var rep = base.GetRepository();
            var mailmessageid = Context.GetIntArg(Constants.Id, 0);
            rep.Parameters.Add("mailmessageid", mailmessageid);
            return rep;
        }

        protected override void ToolbarLeft(MenuBuilder t)
        {
            t.ItemBack();
            t.ItemSeparator();
            t.ItemFilter(Filter);
        }

        protected override void FieldsInit(FieldCollection<MailMessageAttachment> fields)
        {
            fields.AddCellWithSortAndFilter(o => o.FileType, o => o.FileType);
            fields.AddCellWithSortAndFilter(o => o.FileTitle, o => o.FileTitle);
        }
    }

    [OnAction(typeof(MailMessage), "delete")]
    public class MailMessage_delete : default_delete<MailMessage, int>
    {
    }
}