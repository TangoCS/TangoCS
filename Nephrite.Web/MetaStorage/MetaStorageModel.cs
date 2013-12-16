using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nephrite.Web.MetaStorage
{
	public interface IDC_MetaStorage
	{
		IQueryable<IMM_ObjectType> IMM_ObjectType { get; }
		IQueryable<IMM_FormView> IMM_FormView { get; }
		IQueryable<IMM_Package> IMM_Package { get; }
		IQueryable<IN_Cache> IN_Cache { get; }
	}

	public interface IMM_ObjectType : IEntity
	{
		int ObjectTypeID { get; set; }
		string Title { get; set; }
		string SysName { get; set; }
		bool IsEnableSPM { get; set; }
		System.Guid Guid { get; set; }
		bool IsSeparateTable { get; set; }
		bool IsTemplate { get; set; }
		string TitlePlural { get; set; }
		string DefaultOrderBy { get; set; }
		string LogicalDelete { get; set; }
		bool IsReplicate { get; set; }
		bool IsEnableUserViews { get; set; }
		string SecurityPackageSystemName { get; set; }
		bool IsEnableObjectHistory { get; set; }
		string Interface { get; set; }
		string HistoryTypeCode { get; set; }
		bool IsDataReplicated { get; set; }
		bool IsDeleted { get; set; }
		System.DateTime LastModifiedDate { get; set; }
		int LastModifiedUserID { get; set; }
		int SeqNo { get; set; }
		string Description { get; set; }
		Nullable<System.Int32> PackageID { get; set; }
		Nullable<System.Int32> BaseObjectTypeID { get; set; }		
	}

	public interface IMM_FormView : IEntity
	{
		int FormViewID { get; set; }
		string Title { get; set; }
		string SysName { get; set; }
		string ViewTemplate { get; set; }
		string TemplateTypeCode { get; set; }
		System.DateTime LastModifiedDate { get; set; }
		System.Guid Guid { get; set; }
		bool IsCustom { get; set; }
		bool IsDeleted { get; set; }
		int LastModifiedUserID { get; set; }
		bool IsCaching { get; set; }
		string CacheKeyParams { get; set; }
		int CacheTimeout { get; set; }
		string BaseClass { get; set; }
		Nullable<System.Int32> ObjectTypeID { get; set; }
		Nullable<System.Int32> PackageID { get; set; }
	}

	public interface IMM_Package : IEntity
	{
		int PackageID { get; set; }
		int? ParentPackageID { get; set; }
		string Title { get; set; }
		string SysName { get; set; }
		bool IsDeleted { get; set; }
		DateTime LastModifiedDate { get; set; }
		int LastModifiedUserID { get; set; }
		Guid Guid { get; set; }
		bool IsDataReplicated { get; set; }
		string Version { get; set; }
		int SeqNo { get; set; }
	}

	public interface IN_Cache : IEntity
	{
		DateTime TimeStamp { get; set; }
	}
}