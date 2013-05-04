using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Linq.Expressions;
using Nephrite.Web;

namespace Nephrite.Metamodel.Model
{
	public partial class MM_FormField : IModelObject, IChildObject
	{
		public int ObjectID
		{
            get { return FormFieldID; }
		}

		public Guid ObjectGUID
		{
			get { return Guid.Empty; }
		}

		public string GetClassName()
		{
			return "";
		}

		public IModelObject ParentObject
        {
            get
            {
                return MM_ObjectProperty;
            }
            set
            {
                MM_ObjectProperty = (MM_ObjectProperty)value;
            }
        }

        public void SetParent(int parentID)
        {
            ObjectPropertyID = parentID;
        }

        public Expression<Func<T, bool>> FilterByParentID<T>(int id) where T : IModelObject
        {
            return o => (o as MM_FormField).ObjectPropertyID == id;
		}

		public void SetParent(Guid parentGUID)
		{
			MM_ObjectProperty = AppMM.DataContext.MM_ObjectProperties.SingleOrDefault(o => o.Guid == parentGUID);
		}

		public Expression<Func<T, bool>> FilterByParentGUID<T>(Guid guid) where T : IModelObject
		{
			return o => (o as MM_FormField).MM_ObjectProperty.Guid == guid;
		}
    }

	public partial class MM_FormFieldGroup : IModelObject, IChildObject
	{
		public int ObjectID
		{
            get { return FormFieldGroupID; }
		}

		public Guid ObjectGUID
		{
			get { return Guid.Empty; }
		}

		public string GetClassName()
		{
			return "";
		}

		public IModelObject ParentObject
        {
            get
            {
                return MM_ObjectType;
            }
            set
            {
                MM_ObjectType = (MM_ObjectType)value;
            }
        }

        public void SetParent(int parentID)
        {
            ObjectTypeID = parentID;
        }

        public Expression<Func<T, bool>> FilterByParentID<T>(int id) where T : IModelObject
        {
            return o => (o as MM_FormFieldGroup).ObjectTypeID == id;
		}

		public void SetParent(Guid parentGUID)
		{
			MM_ObjectType = AppMM.DataContext.MM_ObjectTypes.SingleOrDefault(o => o.Guid == parentGUID);
		}

		public Expression<Func<T, bool>> FilterByParentGUID<T>(Guid guid) where T : IModelObject
		{
			return o => (o as MM_FormFieldGroup).MM_ObjectType.Guid == guid;
		}
    }


	public partial class MM_FormView : IModelObject, IChildObject
	{
		public int ObjectID
		{
            get { return FormViewID; }
		}

		public string GetClassName()
		{
			return "";
		}

		public IModelObject ParentObject
        {
            get
            {
                return MM_ObjectType;
            }
            set
            {
                MM_ObjectType = (MM_ObjectType)value;
            }
        }

        public void SetParent(int parentID)
        {
            ObjectTypeID = parentID;
        }

		public void SetParent(Guid parentGUID)
		{
			MM_ObjectType = AppMM.DataContext.MM_ObjectTypes.SingleOrDefault(o => o.Guid == parentGUID);
		}

        public Expression<Func<T, bool>> FilterByParentID<T>(int id) where T : IModelObject
        {
            return o => (o as MM_FormView).ObjectTypeID == id;
        }

		public Expression<Func<T, bool>> FilterByParentGUID<T>(Guid guid) where T : IModelObject
		{
			return o => (o as MM_FormView).MM_ObjectType.Guid == guid;
		}
    }

    public partial class MM_Method : IModelObject, IMovableObject
	{
		public int ObjectID
		{
            get { return MethodID; }
		}

		public Guid ObjectGUID
		{
			get { return Guid; }
		}

		public string GetClassName()
		{
			return "";
		}

        public string FullSysName
        {
            get { return MM_ObjectType.FullSysName + "." + SysName; }
        }
    }

	public partial class MM_ObjectProperty : IModelObject, IMovableObject
	{
		public int ObjectID
		{
            get { return ObjectPropertyID; }
		}

		public Guid ObjectGUID
		{
			get { return Guid; }
		}

		public string GetClassName()
		{
			return "";
		}
	}

	public partial class MM_ObjectType : IModelObject
	{
		public int ObjectID
		{
            get { return ObjectTypeID; }
		}

		public Guid ObjectGUID
		{
			get { return Guid; }
		}

		public string GetClassName()
		{
			return "";
		}
	}

	public partial class MM_Package : IModelObject, IChildObject
	{
		public int ObjectID
		{
            get { return PackageID; }
		}

		public Guid ObjectGUID
		{
			get { return Guid; }
		}

		public string GetClassName()
		{
			return "";
		}

		#region IChildObject Members

		public IModelObject ParentObject
		{
			get
			{
				return MM_Package1;
			}
			set
			{
				MM_Package1 = (MM_Package)value;
			}
		}

		public void SetParent(int parentID)
		{
			if (parentID == 0)
				ParentPackageID = null;
			else
				ParentPackageID = parentID;
		}

		public Expression<Func<T, bool>> FilterByParentID<T>(int id) where T : IModelObject
		{
			if (id > 0)
				return o => (o as MM_Package).ParentPackageID == id;
			else
				return o => (o as MM_Package).ParentPackageID == null;
		}

		public void SetParent(Guid parentGUID)
		{
			if (parentGUID == Guid.Empty)
				ParentPackageID = null;
			else
				MM_Package1 = AppMM.DataContext.MM_Packages.SingleOrDefault(o => o.Guid == parentGUID);
		}

		public Expression<Func<T, bool>> FilterByParentGUID<T>(Guid guid) where T : IModelObject
		{
			if (guid != Guid.Empty)
				return o => (o as MM_Package).MM_Package1.Guid == guid;
			else
				return o => (o as MM_Package).ParentPackageID == null;
		}

		#endregion
	}
	
	public partial class MM_TaggedValueType : IModelObject, IMovableObject
	{
		public int ObjectID
		{
            get { return TaggedValueTypeID; }
		}

		public Guid ObjectGUID
		{
			get { return Guid; }
		}
		
        public string GetClassName()
		{
			return "";
		}
	}

    public partial class MM_MethodGroup : IModelObject
    {
        public string Title
        {
            get { return SysName; }
        }

        public int ObjectID
        {
            get { return MethodGroupID; }
        }

		public Guid ObjectGUID
		{
			get { return Guid.Empty; }
		}

        public string GetClassName()
        {
            return "";
        }
    }

    public partial class MM_MethodGroupItem : IModelObject, IMovableObject
    {
        public int ObjectID
        {
            get { return MethodGroupItemID; }
        }

		public Guid ObjectGUID
		{
			get { return Guid.Empty; }
		}

        public string GetClassName()
        {
            return "";
        }

        public string Name
        {
            get
            {
                return IsSeparator ? "------------" : (MethodID.HasValue ? MM_Method.Title : Title);
            }
        }
    }

	public partial class MM_DataValidation : IModelObject, IChildObject
	{
		public string Title
		{
			get { return Expression; }
		}

		public int ObjectID
		{
			get { return DataValidationID; }
		}

		public Guid ObjectGUID
		{
			get { return Guid.Empty; }
		}

		public string GetClassName()
		{
			return "";
		}

		public IModelObject ParentObject
		{
			get
			{
				return MM_ObjectType;
			}
			set
			{
				MM_ObjectType = (MM_ObjectType)value;
			}
		}

		public void SetParent(int parentID)
		{
			ObjectTypeID = parentID;
		}

		public Expression<Func<T, bool>> FilterByParentID<T>(int id) where T : IModelObject
		{
			return o => (o as MM_DataValidation).ObjectTypeID == id;
		}

		public void SetParent(Guid parentGUID)
		{
			MM_ObjectType = AppMM.DataContext.MM_ObjectTypes.SingleOrDefault(o => o.Guid == parentGUID);
		}

		public Expression<Func<T, bool>> FilterByParentGUID<T>(Guid guid) where T : IModelObject
		{
			return o => (o as MM_DataValidation).MM_ObjectType.Guid == guid;
		}
	}
}
