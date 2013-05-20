using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nephrite.Metamodel.Model2
{
	public class MM_FormFieldAttribute
	{
		public virtual int FormFieldAttributeID { get; set; }
		public virtual MM_FormField MM_FormField { get; set; }
		public virtual string Title { get; set; }
		public virtual string Value { get; set; }
		public virtual bool IsEvent { get; set; }
		public virtual bool IsDeleted { get; set; }
		public virtual System.DateTime LastModifiedDate { get; set; }
		public virtual int LastModifiedUserID { get; set; }
	}

	public class MM_Package
	{
		public MM_Package()
		{
			MM_Codifiers = new List<MM_Codifier>();
			MM_FormViews = new List<MM_FormView>();
			MM_ObjectTypes = new List<MM_ObjectType>();
			MM_Packages = new List<MM_Package>();
		}
		public virtual int PackageID { get; set; }
		public virtual MM_Package ParentPackage { get; set; }
		public virtual string Title { get; set; }
		public virtual string SysName { get; set; }
		public virtual bool IsDeleted { get; set; }
		public virtual System.DateTime LastModifiedDate { get; set; }
		public virtual int LastModifiedUserID { get; set; }
		public virtual System.Guid Guid { get; set; }
		public virtual bool IsDataReplicated { get; set; }
		public virtual string Version { get; set; }
		public virtual int SeqNo { get; set; }
		public virtual IList<MM_Codifier> MM_Codifiers { get; set; }
		public virtual IList<MM_FormView> MM_FormViews { get; set; }
		public virtual IList<MM_ObjectType> MM_ObjectTypes { get; set; }
		public virtual IList<MM_Package> MM_Packages { get; set; }
	}

	public class MM_ObjectProperty
	{
		public MM_ObjectProperty()
		{
			MM_FormFields = new List<MM_FormField>();
			MM_FormFieldGroups = new List<MM_FormFieldGroup>();
			MM_ObjectProperties = new List<MM_ObjectProperty>();
		}
		public virtual int ObjectPropertyID { get; set; }
		public virtual MM_ObjectType ObjectTypeID { get; set; }
		public virtual MM_ObjectProperty RefObjectProperty { get; set; }
		public virtual MM_ObjectType RefObjectTypeID { get; set; }
		public virtual MM_Codifier MM_Codifier { get; set; }
		public virtual string Title { get; set; }
		public virtual string SysName { get; set; }
		public virtual int SeqNo { get; set; }
		public virtual string TypeCode { get; set; }
		public virtual System.Guid Guid { get; set; }
		public virtual bool IsMultilingual { get; set; }
		public virtual bool IsPrimaryKey { get; set; }
		public virtual bool IsSystem { get; set; }
		public virtual bool IsNavigable { get; set; }
		public virtual bool IsAggregate { get; set; }
		public virtual int LowerBound { get; set; }
		public virtual int UpperBound { get; set; }
		public virtual string Expression { get; set; }
		public virtual bool IsReferenceToVersion { get; set; }
		public virtual string ValueFilter { get; set; }
		public virtual System.Nullable<int> Precision { get; set; }
		public virtual System.Nullable<int> Scale { get; set; }
		public virtual System.Nullable<int> Length { get; set; }
		public virtual string DeleteRule { get; set; }
		public virtual string KindCode { get; set; }
		public virtual string DefaultDBValue { get; set; }
		public virtual string Description { get; set; }
		public virtual bool IsDeleted { get; set; }
		public virtual System.DateTime LastModifiedDate { get; set; }
		public virtual int LastModifiedUserID { get; set; }
		public virtual bool IsIdentity { get; set; }
		public virtual IList<MM_FormField> MM_FormFields { get; set; }
		public virtual IList<MM_FormFieldGroup> MM_FormFieldGroups { get; set; }
		public virtual IList<MM_ObjectProperty> MM_ObjectProperties { get; set; }
	}

	public class MM_FormField
	{
		public MM_FormField()
		{
			MM_FormFieldAttributes = new List<MM_FormFieldAttribute>();
		}
		public virtual int FormFieldID { get; set; }
		public virtual MM_ObjectProperty MM_ObjectProperty { get; set; }
		public virtual MM_FormFieldGroup MM_FormFieldGroup { get; set; }
		public virtual System.Nullable<int> ControlName { get; set; }
		public virtual string Title { get; set; }
		public virtual string DefaultValue { get; set; }
		public virtual string Comment { get; set; }
		public virtual bool ShowInList { get; set; }
		public virtual bool ShowInEdit { get; set; }
		public virtual bool ShowInView { get; set; }
		public virtual string ValueFunction { get; set; }
		public virtual int SeqNo { get; set; }
		public virtual string ListColumnWidth { get; set; }
		public virtual bool ValueFunctionExecType { get; set; }
		public virtual string SortExpression { get; set; }
		public virtual bool IsDeleted { get; set; }
		public virtual System.DateTime LastModifiedDate { get; set; }
		public virtual int LastModifiedUserID { get; set; }
		public virtual IList<MM_FormFieldAttribute> MM_FormFieldAttributes { get; set; }
	}

	public class MM_CodifierValue
	{
		public virtual int CodifierValueID { get; set; }
		public virtual MM_Codifier MM_Codifier { get; set; }
		public virtual string Title { get; set; }
		public virtual string Code { get; set; }
		public virtual string SysName { get; set; }
		public virtual bool IsDeleted { get; set; }
		public virtual System.DateTime LastModifiedDate { get; set; }
		public virtual int LastModifiedUserID { get; set; }
		public virtual int SeqNo { get; set; }
	}

	public class MM_Codifier
	{
		public MM_Codifier()
		{
			MM_CodifierValues = new List<MM_CodifierValue>();
			MM_ObjectProperties = new List<MM_ObjectProperty>();
		}
		public virtual int CodifierID { get; set; }
		public virtual MM_Package MM_Package { get; set; }
		public virtual string Title { get; set; }
		public virtual string SysName { get; set; }
		public virtual System.Guid Guid { get; set; }
		public virtual bool IsDeleted { get; set; }
		public virtual System.DateTime LastModifiedDate { get; set; }
		public virtual int LastModifiedUserID { get; set; }
		public virtual IList<MM_CodifierValue> MM_CodifierValues { get; set; }
		public virtual IList<MM_ObjectProperty> MM_ObjectProperties { get; set; }
	}

	public class MM_MethodGroup
	{
		public MM_MethodGroup()
		{
			MM_MethodGroupItems = new List<MM_MethodGroupItem>();
		}
		public virtual int MethodGroupID { get; set; }
		public virtual MM_ObjectType MM_ObjectType { get; set; }
		public virtual string SysName { get; set; }
		public virtual IList<MM_MethodGroupItem> MM_MethodGroupItems { get; set; }
	}

	public class MM_MethodGroupItem
	{
		public MM_MethodGroupItem()
		{
			MM_MethodGroupItems = new List<MM_MethodGroupItem>();
		}
		public virtual int MethodGroupItemID { get; set; }
		public virtual MM_MethodGroup MM_MethodGroup { get; set; }
		public virtual MM_Method MM_Method { get; set; }
		public virtual MM_MethodGroupItem ParentMethodGroupItem { get; set; }
		public virtual string Title { get; set; }
		public virtual bool IsSeparator { get; set; }
		public virtual int SeqNo { get; set; }
		public virtual IList<MM_MethodGroupItem> MM_MethodGroupItems { get; set; }
	}

	public class MM_Method
	{
		public MM_Method()
		{
			MM_MethodGroupItems = new List<MM_MethodGroupItem>();
		}
		public virtual int MethodID { get; set; }
		public virtual MM_ObjectType MM_ObjectType { get; set; }
		public virtual MM_FormView MM_FormView { get; set; }
		public virtual string SysName { get; set; }
		public virtual string Title { get; set; }
		public virtual int SeqNo { get; set; }
		public virtual System.Guid Guid { get; set; }
		public virtual bool IsDefault { get; set; }
		public virtual string Icon { get; set; }
		public virtual string Code { get; set; }
		public virtual string Parameters { get; set; }
		public virtual string ViewPath { get; set; }
		public virtual string PredicateCode { get; set; }
		public virtual string Comment { get; set; }
		public virtual IList<MM_MethodGroupItem> MM_MethodGroupItems { get; set; }
	}

	public class MM_FormFieldGroup
	{
		public MM_FormFieldGroup()
		{
			MM_FormFields = new List<MM_FormField>();
		}
		public virtual int FormFieldGroupID { get; set; }
		public virtual MM_ObjectType MM_ObjectType { get; set; }
		public virtual MM_ObjectProperty MM_ObjectProperty { get; set; }
		public virtual int LastModifiedUserID { get; set; }
		public virtual string Title { get; set; }
		public virtual int SeqNo { get; set; }
		public virtual string SelectObjectPrefix { get; set; }
		public virtual string SelectObjectClass { get; set; }
		public virtual bool ShowTitle { get; set; }
		public virtual string SelectObjectDataTextField { get; set; }
		public virtual string SelectObjectFilter { get; set; }
		public virtual string SelectObjectSearchExpression { get; set; }
		public virtual bool IsDeleted { get; set; }
		public virtual System.DateTime LastModifiedDate { get; set; }
		public virtual IList<MM_FormField> MM_FormFields { get; set; }
	}

	public class MM_ObjectType
	{
		public MM_ObjectType()
		{
			MM_FormFieldGroups = new List<MM_FormFieldGroup>();
			MM_FormViews = new List<MM_FormView>();
			MM_Methods = new List<MM_Method>();
			MM_MethodGroups = new List<MM_MethodGroup>();
			MM_ObjectProperties = new List<MM_ObjectProperty>();
			MM_ObjectTypes = new List<MM_ObjectType>();
		}
		public virtual int ObjectTypeID { get; set; }
		public virtual MM_Package MM_Package { get; set; }
		public virtual MM_ObjectType BaseObjectType { get; set; }
		public virtual string Title { get; set; }
		public virtual string SysName { get; set; }
		public virtual bool IsEnableSPM { get; set; }
		public virtual System.Guid Guid { get; set; }
		public virtual bool IsSeparateTable { get; set; }
		public virtual bool IsTemplate { get; set; }
		public virtual string TitlePlural { get; set; }
		public virtual string DefaultOrderBy { get; set; }
		public virtual string LogicalDelete { get; set; }
		public virtual bool IsReplicate { get; set; }
		public virtual bool IsEnableUserViews { get; set; }
		public virtual string SecurityPackageSystemName { get; set; }
		public virtual bool IsEnableObjectHistory { get; set; }
		public virtual string Interface { get; set; }
		public virtual string HistoryTypeCode { get; set; }
		public virtual bool IsDataReplicated { get; set; }
		public virtual bool IsDeleted { get; set; }
		public virtual System.DateTime LastModifiedDate { get; set; }
		public virtual int LastModifiedUserID { get; set; }
		public virtual int SeqNo { get; set; }
		public virtual string Description { get; set; }
		public virtual IList<MM_FormFieldGroup> MM_FormFieldGroups { get; set; }
		public virtual IList<MM_FormView> MM_FormViews { get; set; }
		public virtual IList<MM_Method> MM_Methods { get; set; }
		public virtual IList<MM_MethodGroup> MM_MethodGroups { get; set; }
		public virtual IList<MM_ObjectProperty> MM_ObjectProperties { get; set; }
		public virtual IList<MM_ObjectType> MM_ObjectTypes { get; set; }
	}

	public class MM_FormView
	{
		public MM_FormView()
		{
			MM_Methods = new List<MM_Method>();
		}
		public virtual int FormViewID { get; set; }
		public virtual MM_ObjectType MM_ObjectType { get; set; }
		public virtual MM_Package MM_Package { get; set; }
		public virtual string Title { get; set; }
		public virtual string SysName { get; set; }
		public virtual string ViewTemplate { get; set; }
		public virtual string TemplateTypeCode { get; set; }
		public virtual System.DateTime LastModifiedDate { get; set; }
		public virtual System.Guid Guid { get; set; }
		public virtual bool IsCustom { get; set; }
		public virtual bool IsDeleted { get; set; }
		public virtual int LastModifiedUserID { get; set; }
		public virtual bool IsCaching { get; set; }
		public virtual string CacheKeyParams { get; set; }
		public virtual int CacheTimeout { get; set; }
		public virtual string BaseClass { get; set; }
		public virtual IList<MM_Method> MM_Methods { get; set; }

	}

	public class N_ReplicationObject
	{
		public virtual string ObjectTypeSysName { get; set; }
		public virtual int ObjectID { get; set; }
		public virtual System.DateTime ChangeDate { get; set; }
		#region NHibernate Composite Key Requirements
		public override bool Equals(object obj)
		{
			if (obj == null) return false;
			var t = obj as N_ReplicationObject;
			if (t == null) return false;
			if (ObjectTypeSysName == t.ObjectTypeSysName && ObjectID == t.ObjectID)
				return true;

			return false;
		}
		public override int GetHashCode()
		{
			int hash = 13;
			hash += ObjectTypeSysName.GetHashCode();
			hash += ObjectID.GetHashCode();

			return hash;
		}
		#endregion
	}


}