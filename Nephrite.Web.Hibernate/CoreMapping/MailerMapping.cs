using System;
using System.Collections.Generic;
using System.Linq;
//using Nephrite.Web.Mailer;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Type;

namespace Nephrite.Hibernate.CoreMapping
{
	//public class IMailMessageMap : ClassMapping<IMailMessage>
	//{
	//	public IMailMessageMap()
	//	{
	//		Table("MailMessage");
	//		Lazy(true);
	//		Id(x => x.MailMessageID, map => map.Generator(Generators.Identity));
	//		Discriminator(x => x.Formula("0"));
	//		Property(x => x.Recipients);
	//		Property(x => x.Subject);
	//		Property(x => x.Body);
	//		Property(x => x.IsSent, map => { map.NotNullable(true); MappingConfig.BoolPropertyConfig(map); });
	//		Property(x => x.Attachment, map => map.Type<BinaryBlobType>());
	//		Property(x => x.AttachmentName);
	//		Property(x => x.Error);
	//		Property(x => x.CopyRecipients);
	//		Property(x => x.LastSendAttemptDate);
	//		Property(x => x.AttemptsToSendCount, map => map.NotNullable(true));
	//	}
	//}

	//public class IMailTemplateMap : ClassMapping<IMailTemplate>
	//{
	//	public IMailTemplateMap()
	//	{
	//		Table("MailTemplate");
	//		Lazy(true);
	//		Id(x => x.MailTemplateID, map => map.Generator(Generators.Identity));
	//		Discriminator(x => x.Formula("0"));
	//		Property(x => x.Title, map => map.NotNullable(true));
	//		Property(x => x.TemplateSubject, map => map.NotNullable(true));
	//		Property(x => x.TemplateBody, map => map.NotNullable(true));
	//		Property(x => x.Comment);
	//		Property(x => x.IsSystem, map => { map.NotNullable(true); MappingConfig.BoolPropertyConfig(map); });
	//	}
	//}
}