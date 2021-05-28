using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using Tango.Data;

namespace Tango.Mail
{
    [BaseNamingConventions(Category = BaseNamingEntityCategory.Mail)]
    public class MailMessageAttachment: IEntity, IWithKey<MailMessageAttachment, int>, IWithTitle
    {
        public int ID => MailMessageID;
        public Expression<Func<MailMessageAttachment, bool>> KeySelector(int id)
        {
            return o => o.MailMessageID == id;
        }
        
        [Column]
        public virtual int MailMessageID { get; set; }
        [Column]
        public virtual Guid FileGUID { get; set; }
        public string FileTitle { get; set; }
        public int DocumentID { get; set; } // TODO: временно. для показа 31 05 2021
        public string FileType { get; set; }
        public string Title { get; }
    }

    public class FileData
    {
        public Guid FileDataID { get; set; }
        public string Title { get; set; }
        public byte[] Data { get; set; }
    }
}