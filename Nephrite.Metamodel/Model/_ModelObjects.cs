using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Linq.Expressions;
using Nephrite.Web;
using Nephrite.Meta;

namespace Nephrite.Metamodel.Model
{
	public partial class MM_FormView : IModelObject
	{
		public virtual int ObjectID
		{
            get { return FormViewID; }
		}
		public virtual Guid ObjectGUID
		{
			get { return Guid; }
		}

		public virtual MetaClass MetaClass
		{
			get
			{
				return A.Meta.GetClass("MM_FormView");
			}
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

	public partial class MM_Package : IModelObject
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
	}
}
