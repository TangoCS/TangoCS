using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Linq.Expressions;
using Nephrite.Web;
using Nephrite.Meta;

namespace Nephrite.Metamodel.Model
{
	public partial class MM_FormView : IModelObject, IChildObject
	{
		public virtual int ObjectID
		{
            get { return FormViewID; }
		}

		public virtual MetaClass MetaClass
		{
			get
			{
				return A.Meta.GetClass("MM_FormView");
			}
		}

		public virtual IModelObject ParentObject
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

        public virtual void SetParent(int parentID)
        {
            ObjectTypeID = parentID;
        }

		public virtual void SetParent(Guid parentGUID)
		{
			MM_ObjectType = AppMM.DataContext.MM_ObjectTypes.SingleOrDefault(o => o.Guid == parentGUID);
		}

		public virtual Expression<Func<T, bool>> FilterByParentID<T>(int id) where T : IModelObject
        {
            return o => (o as MM_FormView).ObjectTypeID == id;
        }

		public virtual Expression<Func<T, bool>> FilterByParentGUID<T>(Guid guid) where T : IModelObject
		{
			return o => (o as MM_FormView).MM_ObjectType.Guid == guid;
		}
    }

    public partial class MM_Method : IModelObject, IWithSeqNo
	{
		public virtual int ObjectID
		{
            get { return MethodID; }
		}

		public virtual Guid ObjectGUID
		{
			get { return Guid; }
		}

		public virtual MetaClass MetaClass
		{
			get
			{
				return A.Meta.GetClass("MM_Method");
			}
		}

		public virtual string FullSysName
        {
            get { return MM_ObjectType.FullSysName + "." + SysName; }
        }
    }

	public partial class MM_ObjectProperty : IModelObject, IWithSeqNo
	{
		public virtual int ObjectID
		{
            get { return ObjectPropertyID; }
		}

		public virtual Guid ObjectGUID
		{
			get { return Guid; }
		}

		public virtual MetaClass MetaClass
		{
			get
			{
				return A.Meta.GetClass("MM_ObjectProperty");
			}
		}
	}

	public partial class MM_ObjectType : IModelObject
	{
		public virtual int ObjectID
		{
            get { return ObjectTypeID; }
		}

		public virtual Guid ObjectGUID
		{
			get { return Guid; }
		}

		public virtual MetaClass MetaClass
		{
			get
			{
				return A.Meta.GetClass("MM_ObjectType");
			}
		}
	}

	public partial class MM_Package : IModelObject, IChildObject
	{
		public virtual int ObjectID
		{
            get { return PackageID; }
		}

		public virtual Guid ObjectGUID
		{
			get { return Guid; }
		}

		public virtual MetaClass MetaClass
		{
			get
			{
				return A.Meta.GetClass("MM_Package");
			}
		}

		#region IChildObject Members

		public virtual IModelObject ParentObject
		{
			get
			{
				return ParentPackage;
			}
			set
			{
				ParentPackage = (MM_Package)value;
			}
		}

		public virtual void SetParent(int parentID)
		{
			if (parentID == 0)
				ParentPackageID = null;
			else
				ParentPackageID = parentID;
		}

		public virtual Expression<Func<T, bool>> FilterByParentID<T>(int id) where T : IModelObject
		{
			if (id > 0)
				return o => (o as MM_Package).ParentPackageID == id;
			else
				return o => (o as MM_Package).ParentPackageID == null;
		}

		public virtual void SetParent(Guid parentGUID)
		{
			if (parentGUID == Guid.Empty)
				ParentPackageID = null;
			else
				ParentPackage = AppMM.DataContext.MM_Packages.SingleOrDefault(o => o.Guid == parentGUID);
		}

		public virtual Expression<Func<T, bool>> FilterByParentGUID<T>(Guid guid) where T : IModelObject
		{
			if (guid != Guid.Empty)
				return o => (o as MM_Package).ParentPackage.Guid == guid;
			else
				return o => (o as MM_Package).ParentPackageID == null;
		}

		#endregion
	}
}
