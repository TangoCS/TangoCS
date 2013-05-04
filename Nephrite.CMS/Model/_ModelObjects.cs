using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nephrite.Metamodel;
using Nephrite.Web;

namespace Nephrite.CMS.Model
{
	public partial class SiteObject : IModelObject, IMovableObject
	{
		public string Title
		{
			get { throw new NotImplementedException(); }
		}

		public int ObjectID
		{
			get { return SiteObjectID; }
		}

		public string GetClassName()
		{
			throw new NotImplementedException();
		}

		public Guid ObjectGUID
		{
			get { return Guid.Empty; }
		}
	}

	public partial class V_SiteSection : IModelObject, IMovableObject
	{

		

		public bool IsLogicalDelete
		{
			get { return false; }
		}

		public int ObjectID
		{
			get { return SiteSectionID; }
		}

		public Guid ObjectGUID
		{
			get { return Guid.Empty; }
		}

		public string GetClassName()
		{
			throw new NotImplementedException();
		}

		#region IMMObject Members


		public event EventHandler OnSaveChanges;

		public void RaiseSaveChanges()
		{
			throw new NotImplementedException();
		}

		#endregion
	}

	public partial class V_SiteObject : IModelObject
	{



		public bool IsLogicalDelete
		{
			get { return false; }
		}

		public int ObjectID
		{
			get { return SiteObjectID; }
		}

		public Guid ObjectGUID
		{
			get { return Guid.Empty; }
		}

		public string GetClassName()
		{
			throw new NotImplementedException();
		}

		#region IMMObject Members


		public event EventHandler OnSaveChanges;

		public void RaiseSaveChanges()
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
