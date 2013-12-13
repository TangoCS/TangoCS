using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Web;
using Nephrite.Meta;
using Nephrite.Web.SPM;

namespace Nephrite.Web.UserActivity
{
	public interface IDC_UserActivity : IDataContext
	{
		IQueryable<IUserActivity> UserActivity { get; }
		IUserActivity NewUserActivity();
	}

	public static class IDC_UserActivityExtension
	{
		public static IUserActivity NewUserActivity(this IDC_UserActivity dc, MetaOperation action, string objectKey, string title)
		{
			var ua = dc.NewUserActivity();
			ua.Action = action.Caption;
			ua.IP = HttpContext.Current.Request.UserHostAddress;
			ua.LastModifiedDate = DateTime.Now;
			ua.LastModifiedUserID = Subject.Current.ID;
			ua.ObjectKey = objectKey;
			ua.ObjectTypeSysName = action.Parent.Name;
			ua.ObjectTypeTitle = action.Parent.Caption;
			ua.Title = title;
			ua.UserTitle = Subject.Current.Title;
			return ua;
		}
	}

	public interface IUserActivity : IEntity
	{
		int UserActivityID { get; set; }
		int LastModifiedUserID { get; set; }
		System.DateTime LastModifiedDate { get; set; }
		string Title { get; set; }
		string ObjectKey { get; set; }
		string ObjectTypeSysName { get; set; }
		string Action { get; set; }
		string UserTitle { get; set; }
		string ObjectTypeTitle { get; set; }
		string IP { get; set; }
	}
}