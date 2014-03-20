using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace Nephrite.Metamodel.Model
{
	/*public class MM_FormFieldAttributeMap : ClassMapping<MM_FormFieldAttribute>
	{

		public MM_FormFieldAttributeMap()
		{
			Schema("dbo");
			Lazy(true);
			Id(x => x.FormFieldAttributeID, map => map.Generator(Generators.Identity));
			Property(x => x.Title, map => map.NotNullable(true));
			Property(x => x.Value, map => map.NotNullable(true));
			Property(x => x.IsEvent, map => map.NotNullable(true));
			Property(x => x.IsDeleted, map => map.NotNullable(true));
			Property(x => x.LastModifiedDate, map => map.NotNullable(true));
			Property(x => x.LastModifiedUserID, map => map.NotNullable(true));
			ManyToOne(x => x.MM_FormField, map =>
			{
				map.Column("FormFieldID");
				map.PropertyRef("FormFieldID");
				map.Cascade(Cascade.None);
			});

		}
	}*/

	public class MM_PackageMap : ClassMapping<MM_Package>
	{

		public MM_PackageMap()
		{
			Schema("dbo");
			Lazy(true);
			Id(x => x.PackageID, map => map.Generator(Generators.Identity));
			Property(x => x.Title, map => map.NotNullable(true));
			Property(x => x.SysName, map => map.NotNullable(true));
			Property(x => x.IsDeleted, map => map.NotNullable(true));
			Property(x => x.LastModifiedDate, map => map.NotNullable(true));
			Property(x => x.LastModifiedUserID, map => map.NotNullable(true));
			Property(x => x.Guid, map => map.NotNullable(true));
			Property(x => x.IsDataReplicated, map => map.NotNullable(true));
			Property(x => x.Version);
			Property(x => x.SeqNo, map => map.NotNullable(true));

			//Property(x => x.ParentPackageID);
			ManyToOne(x => x.ParentPackage, map =>
			{
				map.Column("ParentPackageID");
				//map.NotNullable(true);
				map.Cascade(Cascade.None);
			});

			Bag(x => x.MM_Codifiers, colmap => { colmap.Key(x => x.Column("PackageID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
			Bag(x => x.MM_FormViews, colmap => { colmap.Key(x => x.Column("PackageID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
			Bag(x => x.MM_ObjectTypes, colmap => { colmap.Key(x => x.Column("PackageID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
			Bag(x => x.MM_Packages, colmap => { colmap.Key(x => x.Column("ParentPackageID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
		}
	}

	public class MM_ObjectPropertyMap : ClassMapping<MM_ObjectProperty>
	{

		public MM_ObjectPropertyMap()
		{
			Schema("dbo");
			Lazy(true);
			Id(x => x.ObjectPropertyID, map => map.Generator(Generators.Identity));
			Property(x => x.Title, map => map.NotNullable(true));
			Property(x => x.SysName, map => map.NotNullable(true));
			Property(x => x.SeqNo, map => map.NotNullable(true));
			Property(x => x.TypeCode, map => map.NotNullable(true));
			Property(x => x.Guid, map => map.NotNullable(true));
			Property(x => x.IsMultilingual, map => map.NotNullable(true));
			Property(x => x.IsPrimaryKey, map => map.NotNullable(true));
			Property(x => x.IsSystem, map => map.NotNullable(true));
			Property(x => x.IsNavigable, map => map.NotNullable(true));
			Property(x => x.IsAggregate, map => map.NotNullable(true));
			Property(x => x.LowerBound, map => map.NotNullable(true));
			Property(x => x.UpperBound, map => map.NotNullable(true));
			Property(x => x.Expression);
			Property(x => x.IsReferenceToVersion, map => map.NotNullable(true));
			Property(x => x.ValueFilter);
			Property(x => x.Precision);
			Property(x => x.Scale);
			Property(x => x.Length);
			Property(x => x.DeleteRule, map => map.NotNullable(true));
			Property(x => x.KindCode, map => map.NotNullable(true));
			Property(x => x.DefaultDBValue);
			Property(x => x.Description);
			Property(x => x.IsDeleted, map => map.NotNullable(true));
			Property(x => x.LastModifiedDate, map => map.NotNullable(true));
			Property(x => x.LastModifiedUserID, map => map.NotNullable(true));
			Property(x => x.IsIdentity, map => map.NotNullable(true));

			Property(x => x.ObjectTypeID, map => { map.NotNullable(true); map.Formula("ObjectTypeID"); });
			ManyToOne(x => x.ObjectType, map =>
			{
				map.Column("ObjectTypeID");
				//map.PropertyRef("ObjectTypeID");
				map.Cascade(Cascade.None);
			});

			Property(x => x.RefObjectPropertyID, map => map.Formula("RefObjectPropertyID"));
			ManyToOne(x => x.RefObjectProperty, map =>
			{
				map.Column("RefObjectPropertyID");
				//map.PropertyRef("ObjectPropertyID");
				map.Cascade(Cascade.None);
			});

			Property(x => x.RefObjectTypeID, map => map.Formula("RefObjectTypeID"));
			ManyToOne(x => x.RefObjectType, map =>
			{
				map.Column("RefObjectTypeID");
				//map.PropertyRef("ObjectTypeID");
				map.Cascade(Cascade.None);
			});

            Property(x => x.CodifierID, map => map.Formula("CodifierID"));
			ManyToOne(x => x.MM_Codifier, map =>
			{
				map.Column("CodifierID");
				//map.PropertyRef("CodifierID");
				map.Cascade(Cascade.None);
			});

			OneToOne(x => x.MM_FormField, map =>
			{
				
			});

			//Bag(x => x.MM_FormFields, colmap => { colmap.Key(x => x.Column("ObjectPropertyID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
			//Bag(x => x.MM_FormFieldGroups, colmap => { colmap.Key(x => x.Column("SelectObjectPropertyID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
			Bag(x => x.MM_ObjectProperties, colmap => { colmap.Key(x => x.Column("RefObjectPropertyID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
		}
	}

	public class MM_FormFieldMap : ClassMapping<MM_FormField>
	{

		public MM_FormFieldMap()
		{
			Schema("dbo");
			Lazy(true);
			Id(x => x.FormFieldID, map => map.Generator(Generators.Identity));
			Property(x => x.ControlName);
			Property(x => x.Title);
			Property(x => x.DefaultValue);
			Property(x => x.Comment);
			Property(x => x.ShowInList, map => map.NotNullable(true));
			Property(x => x.ShowInEdit, map => map.NotNullable(true));
			Property(x => x.ShowInView, map => map.NotNullable(true));
			Property(x => x.ValueFunction);
			Property(x => x.SeqNo, map => map.NotNullable(true));
			Property(x => x.ListColumnWidth);
			Property(x => x.ValueFunctionExecType, map => map.NotNullable(true));
			Property(x => x.SortExpression);
			Property(x => x.IsDeleted, map => map.NotNullable(true));
			Property(x => x.LastModifiedDate, map => map.NotNullable(true));
			Property(x => x.LastModifiedUserID, map => map.NotNullable(true));
			ManyToOne(x => x.MM_ObjectProperty, map =>
			{
				map.Column("ObjectPropertyID");
				//map.PropertyRef("ObjectPropertyID");
				map.Cascade(Cascade.None);
			});

			

			//ManyToOne(x => x.MM_FormFieldGroup, map =>
			//{
			//	map.Column("FormFieldGroupID");
			//	map.PropertyRef("FormFieldGroupID");
			//	map.NotNullable(true);
			//	map.Cascade(Cascade.None);
			//});

			//Bag(x => x.MM_FormFieldAttributes, colmap => { colmap.Key(x => x.Column("FormFieldID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
		}
	}

	public class MM_CodifierValueMap : ClassMapping<MM_CodifierValue>
	{
		public MM_CodifierValueMap()
		{
			Schema("dbo");
			Lazy(true);
			Id(x => x.CodifierValueID, map => map.Generator(Generators.Identity));
			Property(x => x.Title, map => map.NotNullable(true));
			Property(x => x.Code, map => map.NotNullable(true));
			Property(x => x.SysName, map => map.NotNullable(true));
			Property(x => x.IsDeleted, map => map.NotNullable(true));
			Property(x => x.LastModifiedDate, map => map.NotNullable(true));
			Property(x => x.LastModifiedUserID, map => map.NotNullable(true));
			Property(x => x.SeqNo, map => map.NotNullable(true));
			ManyToOne(x => x.MM_Codifier, map =>
			{
				map.Column("CodifierID");
				//map.PropertyRef("CodifierID");
				map.Cascade(Cascade.None);
			});
		}
	}

	public class MM_CodifierMap : ClassMapping<MM_Codifier>
	{
		public MM_CodifierMap()
		{
			Schema("dbo");
			Lazy(true);
			Id(x => x.CodifierID, map => map.Generator(Generators.Identity));
			Property(x => x.Title, map => map.NotNullable(true));
			Property(x => x.SysName, map => map.NotNullable(true));
			Property(x => x.Guid, map => map.NotNullable(true));
			Property(x => x.IsDeleted, map => map.NotNullable(true));
			Property(x => x.LastModifiedDate, map => map.NotNullable(true));
			Property(x => x.LastModifiedUserID, map => map.NotNullable(true));
			ManyToOne(x => x.MM_Package, map => { map.Column("PackageID"); map.Cascade(Cascade.None); });

			Bag(x => x.MM_CodifierValues, colmap => { colmap.Key(x => x.Column("CodifierID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
			Bag(x => x.MM_ObjectProperties, colmap => { colmap.Key(x => x.Column("CodifierID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
		}
	}

	/*
	public class MM_MethodGroupMap : ClassMapping<MM_MethodGroup>
	{
		public MM_MethodGroupMap()
		{
			Schema("dbo");
			Lazy(true);
			Id(x => x.MethodGroupID, map => map.Generator(Generators.Identity));
			Property(x => x.SysName);
			ManyToOne(x => x.MM_ObjectType, map =>
			{
				map.Column("ObjectTypeID");
				map.PropertyRef("ObjectTypeID");
				map.Cascade(Cascade.None);
			});

			Bag(x => x.MM_MethodGroupItems, colmap => { colmap.Key(x => x.Column("MethodGroupID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
		}
	}

	public class MM_MethodGroupItemMap : ClassMapping<MM_MethodGroupItem>
	{
		public MM_MethodGroupItemMap()
		{
			Schema("dbo");
			Lazy(true);
			Id(x => x.MethodGroupItemID, map => map.Generator(Generators.Identity));
			Property(x => x.Title, map => map.NotNullable(true));
			Property(x => x.IsSeparator, map => map.NotNullable(true));
			Property(x => x.SeqNo, map => map.NotNullable(true));
			ManyToOne(x => x.MM_MethodGroup, map =>
			{
				map.Column("MethodGroupID");
				map.PropertyRef("MethodGroupID");
				map.Cascade(Cascade.None);
			});

			ManyToOne(x => x.MM_Method, map =>
			{
				map.Column("MethodID");
				map.PropertyRef("MethodID");
				map.NotNullable(true);
				map.Cascade(Cascade.None);
			});

			ManyToOne(x => x.ParentMethodGroupItem, map =>
			{
				map.Column("ParentMethodGroupItemID");
				map.PropertyRef("MethodGroupItemID");
				map.NotNullable(true);
				map.Cascade(Cascade.None);
			});

			Bag(x => x.MM_MethodGroupItems, colmap => { colmap.Key(x => x.Column("ParentMethodGroupItemID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
		}
	}
	*/

	public class MM_MethodMap : ClassMapping<MM_Method>
	{
		public MM_MethodMap()
		{
			Schema("dbo");
			Lazy(true);
			Id(x => x.MethodID, map => map.Generator(Generators.Identity));
			Property(x => x.SysName, map => map.NotNullable(true));
			Property(x => x.Title, map => map.NotNullable(true));
			Property(x => x.SeqNo, map => map.NotNullable(true));
			Property(x => x.Guid, map => map.NotNullable(true));
			Property(x => x.IsDefault, map => map.NotNullable(true));
			Property(x => x.Icon);
			Property(x => x.Code);
			Property(x => x.Parameters);
			Property(x => x.ViewPath);
			Property(x => x.PredicateCode);
			Property(x => x.Comment);
			ManyToOne(x => x.MM_ObjectType, map =>
			{
				map.Column("ObjectTypeID");
				//map.PropertyRef("ObjectTypeID");
				map.Cascade(Cascade.None);
			});

			ManyToOne(x => x.MM_FormView, map =>
			{
				map.Column("FormViewID");
				//map.PropertyRef("FormViewID");
				//map.NotNullable(true);
				map.Cascade(Cascade.None);
			});

			//Bag(x => x.MM_MethodGroupItems, colmap => { colmap.Key(x => x.Column("MethodID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
		}
	}

	/*
	public class MM_FormFieldGroupMap : ClassMapping<MM_FormFieldGroup>
	{
		public MM_FormFieldGroupMap()
		{
			Schema("dbo");
			Lazy(true);
			Id(x => x.FormFieldGroupID, map => map.Generator(Generators.Identity));
			Property(x => x.Title, map => map.NotNullable(true));
			Property(x => x.SeqNo, map => map.NotNullable(true));
			Property(x => x.SelectObjectPrefix);
			Property(x => x.SelectObjectClass);
			Property(x => x.ShowTitle, map => map.NotNullable(true));
			Property(x => x.SelectObjectDataTextField);
			Property(x => x.SelectObjectFilter);
			Property(x => x.SelectObjectSearchExpression);
			Property(x => x.IsDeleted, map => map.NotNullable(true));
			Property(x => x.LastModifiedDate, map => map.NotNullable(true));
			Property(x => x.LastModifiedUserID, map => map.NotNullable(true));
			ManyToOne(x => x.MM_ObjectType, map =>
			{
				map.Column("ObjectTypeID");
				map.PropertyRef("ObjectTypeID");
				map.Cascade(Cascade.None);
			});

			ManyToOne(x => x.MM_ObjectProperty, map =>
			{
				map.Column("SelectObjectPropertyID");
				map.PropertyRef("ObjectPropertyID");
				map.NotNullable(true);
				map.Cascade(Cascade.None);
			});

			Bag(x => x.MM_FormFields, colmap => { colmap.Key(x => x.Column("FormFieldGroupID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
		}
	}
	 * */

	public class MM_ObjectTypeMap : ClassMapping<MM_ObjectType>
	{

		public MM_ObjectTypeMap()
		{
			Schema("dbo");
			//Lazy(true);
			Id(x => x.ObjectTypeID, map => map.Generator(Generators.Identity));
			Property(x => x.Title, map => map.NotNullable(true));
			Property(x => x.SysName, map => map.NotNullable(true));
			Property(x => x.IsEnableSPM, map => map.NotNullable(true));
			Property(x => x.Guid, map => map.NotNullable(true));
			Property(x => x.IsSeparateTable, map => map.NotNullable(true));
			Property(x => x.IsTemplate, map => map.NotNullable(true));
			Property(x => x.TitlePlural);
			Property(x => x.DefaultOrderBy);
			Property(x => x.LogicalDelete);
			Property(x => x.IsReplicate, map => map.NotNullable(true));
			Property(x => x.IsEnableUserViews, map => map.NotNullable(true));
			Property(x => x.SecurityPackageSystemName);
			Property(x => x.IsEnableObjectHistory, map => map.NotNullable(true));
			Property(x => x.Interface);
			Property(x => x.HistoryTypeCode, map => map.NotNullable(true));
			Property(x => x.IsDataReplicated, map => map.NotNullable(true));
			Property(x => x.IsDeleted, map => map.NotNullable(true));
			Property(x => x.LastModifiedDate, map => map.NotNullable(true));
			Property(x => x.LastModifiedUserID, map => map.NotNullable(true));
			Property(x => x.SeqNo, map => map.NotNullable(true));
			Property(x => x.Description);

			Property(x => x.PackageID, map => map.Formula("PackageID"));
			ManyToOne(x => x.MM_Package, map => 
			{ 
				map.Column("PackageID"); 
				map.Cascade(Cascade.None); 
			});

			Property(x => x.BaseObjectTypeID, map => map.Formula("BaseObjectTypeID"));
			ManyToOne(x => x.BaseObjectType, map =>
			{
				map.Column("BaseObjectTypeID");
				map.Cascade(Cascade.None);
			});

			//Bag(x => x.MM_FormFieldGroups, colmap => { colmap.Key(x => x.Column("ObjectTypeID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
			//Bag(x => x.MM_FormViews, colmap => { colmap.Key(x => x.Column("ObjectTypeID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
			//Bag(x => x.MM_Methods, colmap => { colmap.Key(x => x.Column("ObjectTypeID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
			//Bag(x => x.MM_MethodGroups, colmap => { colmap.Key(x => x.Column("ObjectTypeID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
			//Bag(x => x.MM_ObjectProperties, colmap => { colmap.Key(x => x.Column("ObjectTypeID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
			//Bag(x => x.MM_ObjectTypes, colmap => { colmap.Key(x => x.Column("BaseObjectTypeID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
		}
	}

	public class MM_FormViewMap : ClassMapping<MM_FormView>
	{
		public MM_FormViewMap()
		{
			Schema("dbo");
			//Lazy(true);
			Id(x => x.FormViewID, map => map.Generator(Generators.Identity));
			Property(x => x.Title, map => map.NotNullable(true));
			Property(x => x.SysName, map => map.NotNullable(true));
			Property(x => x.ViewTemplate);
			Property(x => x.TemplateTypeCode);
			Property(x => x.LastModifiedDate, map => map.NotNullable(true));
			Property(x => x.Guid, map => map.NotNullable(true));
			Property(x => x.IsCustom, map => map.NotNullable(true));
			Property(x => x.IsDeleted, map => map.NotNullable(true));
			Property(x => x.LastModifiedUserID, map => map.NotNullable(true));
			Property(x => x.IsCaching, map => map.NotNullable(true));
			Property(x => x.CacheKeyParams);
			Property(x => x.CacheTimeout, map => map.NotNullable(true));
			Property(x => x.BaseClass, map => map.NotNullable(true));

			Property(x => x.ObjectTypeID, map => map.Formula("ObjectTypeID"));
			ManyToOne(x => x.MM_ObjectType, map =>
			{
				map.Column("ObjectTypeID");
				map.Cascade(Cascade.None);
			});

			Property(x => x.PackageID, map => map.Formula("PackageID"));
			ManyToOne(x => x.MM_Package, map =>
			{
				map.Column("PackageID");
				map.Cascade(Cascade.None);
			});

			//Bag(x => x.MM_Methods, colmap => { colmap.Key(x => x.Column("FormViewID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
		}
	}

	public class N_ReplicationObjectMap : ClassMapping<N_ReplicationObject>
	{
		public N_ReplicationObjectMap()
		{
			Schema("dbo");
			Lazy(true);
			ComposedId(compId =>
			{
				compId.Property(x => x.ObjectTypeSysName, m => m.Column("ObjectTypeSysName"));
				compId.Property(x => x.ObjectID, m => m.Column("ObjectID"));
			});
			Property(x => x.ChangeDate, map => map.NotNullable(true));
		}
	}

	public class N_CacheMap : ClassMapping<N_Cache>
	{
		public N_CacheMap()
		{
			Schema("dbo");
			Lazy(true);
			Property(x => x.TimeStamp, map => map.NotNullable(true));
		}
	}
	
}