using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nephrite.Meta;


namespace Nephrite.Web
{
	//public interface IMMObject : IModelObject
	//{
	//	bool IsDeleted { get; }
	//	DateTime LastModifiedDate { get; }
	//	int LastModifiedUserID { get; }
	//	//MM_ObjectType MMType { get; }
		
	//	bool IsLogicalDelete { get; }
	//	event EventHandler OnSaveChanges;
	//	void RaiseSaveChanges();
	//}

	//public class EmptyMMObject : IMMObject
	//{
	//	#region IMMObject Members

	//	public bool IsDeleted
	//	{
	//		get { return false; }
	//	}

	//	public DateTime LastModifiedDate
	//	{
	//		get { return DateTime.MinValue; }
	//	}

	//	public int LastModifiedUserID
	//	{
	//		get { return 0; }
	//	}

	//	public MetaClass MetaClass
	//	{
	//		get { return null; }
	//	}

	//	public bool IsLogicalDelete
	//	{
	//		get { return false; }
	//	}

	//	#endregion

	//	#region IModelObject Members

	//	public string Title
	//	{
	//		get { return string.Empty; }
	//	}

	//	public int ObjectID
	//	{
	//		get { return 0; }
	//	}

	//	public Guid ObjectGUID
	//	{
	//		get { return Guid.Empty; }
	//	}
	//	#endregion

	//	public static readonly EmptyMMObject Instance = new EmptyMMObject();
	//}
}
