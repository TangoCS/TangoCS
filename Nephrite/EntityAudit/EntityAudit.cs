using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nephrite.Data;
using Nephrite.Identity;

namespace Nephrite.EntityAudit
{
	public interface IWithoutEntityAudit
	{

	}

	public interface IWithPropertyAudit
	{

	}

	public interface IDC_EntityAudit : IDataContext
	{
		ITable<IN_ObjectChange> IN_ObjectChange { get; }
		ITable<IN_ObjectPropertyChange> IN_ObjectPropertyChange { get; }
		IN_ObjectChange NewIN_ObjectChange();
		IN_ObjectPropertyChange NewIN_ObjectPropertyChange();
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

	public interface IN_ObjectPropertyChange : IEntity
	{
		System.Int32 ObjectPropertyChangeID { get; set; }
		System.String Title { get; set; }
		System.String PropertySysName { get; set; }
		System.String OldValue { get; set; }
		System.String NewValue { get; set; }
		System.String OldValueTitle { get; set; }
		System.String NewValueTitle { get; set; }
		System.Int32 ObjectChangeID { get; set; }
		IN_ObjectChange ObjectChange { get; set; }
	}

	public static class IDC_UserActivityExtension
	{
		public static IN_ObjectChange NewIN_ObjectChange(this IDC_EntityAudit dc, IdentityUser<int> subj, string action, string objectKey, string className, string objectTitle)
		{
			var ua = dc.NewIN_ObjectChange();
			ua.Title = action;
			ua.IP = ""; //HttpContext.Current == null ? "" : HttpContext.Current.Request.UserHostAddress;
			ua.LastModifiedDate = DateTime.Now;
			ua.SubjectID = subj.Id;
			ua.ObjectKey = objectKey;
			ua.ObjectTypeSysName = className;
			ua.ObjectTypeTitle = "";
			ua.ObjectTitle = objectTitle;
			ua.UserTitle = subj.Title;
			ua.UserLogin = subj.UserName;
			ua.Details = "";
			return ua;
		}
	}
}
