using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Dapper;
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
            return o => 
                o.Subject.ToLower().Contains(s.ToLower()) || 
                o.Recipients.ToLower().Contains(s.ToLower()) ||
                o.Body.ToLower().Contains(s.ToLower()); 
        };

        // protected override IRepository<MailMessage> GetRepository()
        // {
        //     var mailmessageid = Context.GetIntArg("mailmessageid");
        //     base.GetRepository().Parameters.Add("mailmessageid", mailmessageid);
        //     return base.GetRepository();
        // }

        protected override void ToolbarLeft(MenuBuilder t)
        {
            t.ItemFilter(Filter);
        }

        protected override IQueryable<MailMessage> DefaultOrderBy(IQueryable<MailMessage> data)
        {
            return data.OrderByDescending(x => x.CreateDate);
        }

        protected override void FieldsInit(FieldCollection<MailMessage> f)
        {
            f.SetRowID(o => o.MailMessageID.ToString());
            f.AddCellWithSortAndFilter(o => o.MailMessageID,
                (w, o) => w.ActionLink(al => al.ToView<MailMessage>(AccessControl, o.MailMessageID).WithTitle(o.MailMessageID),
                    a => a.Title("Карточка письма")));
            f.AddCellWithSortAndFilter(o => o.MailCategoryTitle, o => o.MailCategoryTitle);
            f.AddCellWithSortAndFilter(o => o.Subject, o => o.Subject);
            f.AddCellWithSortAndFilter(o => o.Body, o => o.Body);
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
                o => al => al.To<MailMessageAttachment>("attachments", AccessControl)
                    .WithArg(Constants.Id, o.ID).WithArg("title", o.Subject).WithImage("hie")
                    .WithTitle("Состав письма"),
                o => al => al.ToDelete<MailMessage>(AccessControl, o.MailMessageID, new object[] {o, AccessControl})); // TODO: сделать нормально. сейчас через костыльный предикат
        }
    }

    [OnAction(typeof(MailMessage), "view")]
    public class MailMessage_view : default_view_rep<MailMessage, int>
    {
        [Inject] public IMailMessageRepository MailMessageRepository { get; set; }
        protected override string FormTitle => ViewData.Subject;
        protected MailMessageFields.DefaultGroup Group { get; set; }
        
        private MailMessageAttachment_viewlist _attachmentViewlist;

        public override void OnInit()
        {
            base.OnInit();
            _attachmentViewlist = CreateControl<MailMessageAttachment_viewlist>("attachmentlist", c => {
                c.MailMessageId = ViewData.MailMessageID;
                c.Sections.RenderContentTitle = false;
            });
        }

        protected override void Form(LayoutWriter w)
        {
            w.FieldsBlockStd(() =>
            {
                w.PlainText(Group.MailCategoryTitle);
                w.PlainText(Group.Subject);
                w.PlainText(Group.Body);
                w.PlainText(Group.CreateDate);
                w.PlainText(Group.TimeoutValue);
                w.PlainText(Group.StartSendDate);
                w.PlainText(Group.FinishSendDate);
                w.PlainText(Group.MaxAttemptsToSendCount);
                w.PlainText(Group.Recipients);
                w.PlainText(Group.CopyRecipients);
                w.PlainText(Group.LastModifiedUserTitle);
                w.PlainText(Group.MailMessageStatus);
                w.PlainText(Group.AttemptsToSendCount);
                w.PlainText(Group.LastSendAttemptDate);
                w.PlainText(Group.Error);
            });
        }

        protected override void ToolbarLeft(MenuBuilder t)
        {
            t.ItemBack();
        }

        protected override MailMessage GetExistingEntity()
        {
            var id = Context.GetIntArg(Constants.Id);
            var obj = Database.Connection.QueryFirstOrDefault<MailMessage>(MailMessageRepository.GetMailMessageByIdSql(), new {@mailmessageid = id});
            return obj;
        }
        
        protected override void LinkedData(LayoutWriter w)
        {
            w.GroupTitle(() =>
            {
                w.Write("Состав письма");
            });
            _attachmentViewlist.Render(w);
        }
    }
    
    [OnAction(typeof(MailMessageAttachment), "attachments")]
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
            fields.AddCellWithSortAndFilter(o => o.DocumentID, o => o.DocumentID); // TODO: временно. для показа 31 05 2021
            fields.AddCellWithSortAndFilter(o => o.FileType, o => o.FileType);
            fields.AddCellWithSortAndFilter(o => o.FileTitle, o => o.FileTitle);
        }
    }

    [OnAction(typeof(MailMessage), "delete")]
    public class MailMessage_delete : default_delete<MailMessage, int>
    {
        [Inject] protected MailHelper MailHelper { get; set; }
        protected override void BeforeDelete(IEnumerable<int> ids)
        {
            //Database.Connection.InitDbConventions<MailMessage>();
            foreach (var id in ids)
            {
                var mailMessage = Database.Repository<MailMessage>().GetById(id);
                if(mailMessage != null)
                    MailHelper.DeleteMailMessage(mailMessage);
            }
        }
    }
}