using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Web;
using Nephrite.Meta;
using Nephrite.Web.SPM;

namespace Nephrite.Web
{
	public interface IDC_EntityAudit : IDataContext
	{
		IQueryable<IN_ObjectChange> IN_ObjectChange { get; }
		IN_ObjectChange NewIN_ObjectChange();
	}


	public interface IWithTimeStamp
	{
		DateTime LastModifiedDate { get; set; }
		int LastModifiedUserID { get; set; }
	}

	public interface IWithoutEntityAudit
	{

	}

	public interface IWithPropertyAudit
	{

	}

	public static class IDC_UserActivityExtension
	{
		public static IN_ObjectChange NewUserActivity(this IDC_EntityAudit dc, string action, string objectKey, string objectName, string objectTitle)
		{
			var ua = dc.NewIN_ObjectChange();
			ua.Title = action;
			ua.IP = HttpContext.Current == null ? "" : HttpContext.Current.Request.UserHostAddress;
			ua.LastModifiedDate = DateTime.Now;
			ua.SubjectID = Subject.Current.ID;
			ua.ObjectKey = objectKey;
			ua.ObjectTypeSysName = objectName;
			ua.ObjectTypeTitle = objectName;
			ua.ObjectTitle = objectTitle;
			ua.UserTitle = Subject.Current.Title;
			ua.UserLogin = Subject.Current.Login;
			return ua;
		}
	}

	public interface IN_ObjectChange : IEntity
	{
		int ObjectChangeID { get; set; }
		System.DateTime LastModifiedDate { get; set; }
		string Title { get; set; }
		string IP { get; set; }

		string ObjectKey { get; set; }
		string ObjectTypeTitle { get; set; }
		string ObjectTypeSysName { get; set; }
		string ObjectTitle { get; set; }
		string UserTitle { get; set; }
		string UserLogin { get; set; }
		string Details { get; set; }

		int SubjectID { get; set; }
	}
}