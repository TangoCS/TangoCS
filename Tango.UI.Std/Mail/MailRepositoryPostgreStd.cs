using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Dapper;
using Tango.AccessControl;
using Tango.Data;
using Tango.Identity.Std;
using Tango.Localization;
using Tango.Model;

namespace Tango.Mail
{
    public class MailMessageAttachmentRepositoryPostgreStd : DapperRepository<MailMessageAttachment>
    {
        private const string _sql = @"
select att.FileID as FileGUID, att.MailMessageID, t.FileType, f.Title as FileTitle
from MailMessageAttachment att
left join N_FileData f on f.FileID = att.FileID
where att.MailMessageID = @mailmessageid
";
        public MailMessageAttachmentRepositoryPostgreStd(IDatabase database, IServiceProvider provider) : base(database, provider)
        {
            AllObjectsQuery = _sql;
        }
    }

    public class MailSettingsTemplateRepositoryPostgreStd : DapperRepository<MailSettingsTemplate>, IMailSettingsTemplateRepository
    {
        public MailSettingsTemplateRepositoryPostgreStd(IDatabase database, IServiceProvider provider) : base(database, provider)
        {
        }

        public string GetMailTemplateSql() => "select MailTemplateID, Title from MailTemplate";

        public string GetMailTemplateWhereMailSettingsIdSql()
        {
            return @"select mt.* from MailTemplate mt 
left join MailSettingsTemplate mst on mst.MailTemplateID = mt.MailTemplateID
where mst.MailSettingsID = @mailSettingsId";
        }
    }

    public class MailMessageRepositoryPostgreStd : DapperRepository<MailMessage>, IMailMessageRepository
    {
        private readonly IIdentityManager _identityManager;
        private readonly IAccessControl _accessControl;
        private string _allObjectsQuery;
        private List<int> _systems;

        private const string Sql = @"select mm.*, st.Title as MailMessageStatus, u.Title as LastModifiedUserTitle,
c.Title as MailCategoryTitle, ms.Title as MailSettingsTitle
from MailMessage mm
left join C_MailMessageStatus st on st.MailMessageStatusID = mm.MailMessageStatusID
left join spm_subject u on u.subjectid = mm.LastModifiedUserID
left join C_MailCategory c on c.MailCategoryID = mm.MailCategoryID
left join C_System s on c.SystemID = s.SystemID
left join MailSettings ms on ms.MailSettingsID = mm.MailSettingsID
";

        public MailMessageRepositoryPostgreStd(IDatabase database, IIdentityManager identityManager, IAccessControl accessControl, IServiceProvider provider) : base(database, provider)
        {
            _identityManager = identityManager;
            _accessControl = accessControl;
        }

        public override string AllObjectsQuery
        {
            get
            {
                if (!Parameters.ContainsKey("sys"))
                    Parameters.Add("sys", GetSystems().Any() ? GetSystems() : new List<int> { 0 });

                if (!Parameters.ContainsKey("hasRight"))
                    Parameters.Add("hasRight", HasRight());
                else
                    Parameters["hasRight"] = HasRight();

                return $@"{Sql} where
                    ((@hasRight = 0 and c.MailCategoryTypeID = 2) or (@hasRight = 1))
                and s.SystemID = any(@sys)";
            }
            set => _allObjectsQuery = value;
        }

        public string GetMailMessageByIdSql()
        {
            return $@"{Sql} where mm.MailMessageID = @mailMessageId";
        }

        private int HasRight()
        {
            const string rightSystemName = "mailmessage.admindelete";

            return _accessControl.Check(rightSystemName) ? 1 : 0;
        }

        private List<int> GetSystems()
        {
            if (_systems == null)
            {
                _systems = _identityManager.CurrentUser.Claims
                    .Where(x => x.ClaimType == ClaimTypes.System) /// ?????????????
                    .Select(x => int.Parse(x.ClaimValue)).ToList();
            }

            return _systems;
        }
    }

    public class MailSettingsRepositoryPostgreStd : DapperRepository<MailSettings>, IMailSettingsRepository
    {
        private const string Sql = @"select ms.*, mt.Title as MailTemplateTitle,
case when mst.MailSettingsTemplateID is not null then 1 else 0 end as HasTemplate,
concat(mc.Title, ' (', s.Title, ', ', mt.Title, ', ', mct.Title,')') as MailCategoryTitle
from MailSettings ms
left join MailSettingsTemplate mst on mst.MailSettingsID = ms.MailSettingsID
left join MailTemplate mt on mt.MailTemplateID = mst.MailTemplateID
left join C_MailCategory mc on mc.MailCategoryID = ms.MailCategoryID
left join C_System s on s.SystemID = mc.SystemID
left join C_MailCategoryType mct on mct.MailCategoryTypeID = mc.MailCategoryTypeID";

        public MailSettingsRepositoryPostgreStd(IDatabase database, IServiceProvider provider) : base(database, provider)
        {
            AllObjectsQuery = Sql;
        }

        public string GetMailTemplateSql() => "select MailTemplateID, Title from MailTemplate";
        public string GetMailCategorySql() => @"select c.MailCategoryID,        
        concat(c.Title, ' (', s.Title, ', ', mt.Title, ')') as Title
from C_MailCategory c
left join C_System s on s.SystemID = c.SystemID
left join C_MailCategoryType mt on mt.MailCategoryTypeID = c.MailCategoryTypeID";
    }

    public class MailCategoryRepositoryPostgreStd : DapperRepository<MailCategory>, IMailCategoryRepository
    {
        private const string Sql = @"select m.*, s.SystemID, s.Title as SystemName, mt.Title as MailCategoryTypeTitle
from C_MailCategory m
left join C_System s on s.SystemID = m.SystemID
left join C_MailCategoryType mt on mt.MailCategoryTypeID = m.MailCategoryTypeID";

        public MailCategoryRepositoryPostgreStd(IDatabase database, IServiceProvider provider) : base(database, provider)
        {
            AllObjectsQuery = Sql;
        }

        public IEnumerable<(string, int)> GetSystemNames()
        {
            return Database.Connection.Query<(string, int)>("select Title, SystemID from C_System");
        }

        public IEnumerable<(string, int)> GetMailCategoryTypes()
        {
            return Database.Connection.Query<(string, int)>("select Title, MailCategoryTypeID from C_MailCategoryType");
        }
    }

    public class MailTemplateRepositoryPostgreStd : DapperRepository<MailTemplate>, IMailTemplateRepository
    {
        private const string Sql = "select * from MailTemplate";

        public MailTemplateRepositoryPostgreStd(IDatabase database, IAccessControl accessControl,
            AccessControlOptions accessControlOptions, IServiceProvider provider) : base(database, provider)
        {
            var devMode = accessControlOptions.DeveloperAccess(accessControl);

            AllObjectsQuery = devMode ? Sql : $"{Sql} where issystem = false";
        }
    }

    public class MailHelperRepositoryPostgreStd : IMailHelperRepository
    {
        private readonly IDatabase _database;

        public MailHelperRepositoryPostgreStd(IDatabase database)
        {
            _database = database;
        }

        public MailSettings GetMailSettingsBySystemName(string systemName)
        {
            _database.Connection.InitDbConventions<MailSettings>();
            return _database.Connection.QueryFirstOrDefault<MailSettings>(
                @"select * from MailSettings where lower(SystemName) = lower(@systemname)", new { systemName });
        }

        public MailSettingsTemplate GetMailSettingsTemplateByMailSettingsId(int mailSettingsId)
        {
            _database.Connection.InitDbConventions<MailSettingsTemplate>();
            return _database.Connection.QueryFirstOrDefault<MailSettingsTemplate>(
                @"select * from MailSettingsTemplate where MailSettingsID = @mailSettingsId", new { mailSettingsId });
        }
    }

    /*[HasPredicates]
    public class MailMessagePredicate
    {
        [Predicate]
        [BindPredicate("mailmessage.delete")]
        public static bool ShowDeleteButton(MailMessage mailMessage, IAccessControl accessControl)
        {
            if (accessControl.Check("mailmessage.admindelete"))
                return true;

            return mailMessage.MailMessageStatusID == 1 || mailMessage.MailMessageStatusID == 3;
        }
    }*/
}
