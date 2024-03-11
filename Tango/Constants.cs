using System.Collections.Generic;
using System.Linq;

namespace Tango
{
	public static class Constants
	{
		public static readonly string ServiceName = "service";
		public static readonly string ActionName = "action";
		public static readonly string ReturnUrl = "returnurl";
        public static readonly string ReturnUrl_CancelButton = "returnurl_0";
        public static readonly string ReturnState = "returnstate";
		/// <summary>
		/// Данный префикс указывается у параметров, которые необходимо передавать при ReturnUrl,
		/// и исключать из адресной строки.
		/// </summary>
		public static readonly string ReturnUrlDataPrefix = "~";

		public static readonly string Id = "oid";
		public static readonly string ObjectChangeRequestId = "ochrid";
		public static readonly string VersionId = "vid";
		public static readonly string ParentId = "parentid";
		public static readonly string FileId = "fileid";

		public static readonly string ShowLogsName = "showlogs";

		public static readonly string EventName = "e";
		public static readonly string EventReceiverName = "r";
		public static readonly string RootReceiverName = "p";
		public static readonly string FirstLoad = "firstload";
		public static readonly string Sender = "sender";
		public static readonly string ResponseType = "responsetype";

		public static readonly string Container = "c";
		public static readonly string ContainerPrefix = "c-prefix";
		public static readonly string ContainerType = "c-type";
		public static readonly string ContainerNew = "c-new";
		public static readonly string ContainerExternal = "c-external";
		public static readonly string ContainerService = "c-service";
		public static readonly string ContainerAction = "c-action";

		public static readonly string IEFormFix = "__dontcare";

		public static readonly string Lang = "lang";

		public static readonly string MetaHome = "_home";
		public static readonly string MetaCurrent = "_current";

		public static readonly string SelectedValues = "selectedvalues";

		public static readonly string[] InternalParms = new string[] { EventName, EventReceiverName,
			RootReceiverName, FirstLoad, Sender, ResponseType, Container, ContainerPrefix, ContainerType, ContainerNew,
			ContainerExternal, IEFormFix, MetaHome, MetaCurrent, ReturnState, ContainerService, ContainerAction
		};

		public static readonly string OpList = "viewlist";
		public static readonly string OpView = "view";
		public static readonly string OpEdit = "edit";
		public static readonly string OpCreateNew = "createnew";
		public static readonly string OpDelete = "delete";
		public static readonly string OpUndelete = "undelete";

		public static readonly int LongTimeout = 3600;
	}

	public static class DBConventions
	{
		public static string IDSuffix = "ID";
		public static string GUIDSuffix = "GUID";

		public static Dictionary<BaseNamingEntityCategory, string> EntityPrefix = 
			BaseNamingConventions.EntityPrefix.ToDictionary(i => i.Key, i => i.Value);
	}
	
	public static class BaseNamingConventions
	{
		public const string IDSuffix = "ID";
		public const string GUIDSuffix = "GUID";

		public static IReadOnlyDictionary<BaseNamingEntityCategory, string> EntityPrefix =
			new Dictionary<BaseNamingEntityCategory, string>
			{
				{BaseNamingEntityCategory.Dictionary, "C_"},
				{BaseNamingEntityCategory.Mail, string.Empty},
                {BaseNamingEntityCategory.Tasks, "TM_"},
            };
	}

	public enum BaseNamingEntityCategory
	{
		Dictionary,
		Tasks,
		UsersAndRoles,
		Mail,
		Notifications
	}
}
