using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nephrite.Web.FileStorage;

namespace Solution.Model
{
	[global::System.Data.Linq.Mapping.TableAttribute(Name = "dbo.V_DbItem")]
	public partial class DbItem
	{
		private string _Title;
		private System.DateTime _LastModifiedDate;
		private int _LastModifiedUserID;
		private bool _IsDeleted;
		private long _Length;
		private string _LastModifiedUserTitle;
		private int _Type;
		private System.Guid _Guid;
		private System.Guid _SPMActionItemGUID;
		private string _Path;
		private string _Extension;
		private Nullable<Guid> _ParentID;
		private string _FullPath;
		private bool _EnableVersioning;
		private int? _CheckedOutByID;
		private string _CheckedOutBy;
		private string _Tag;
		private string _Creator;
		private int _CreatorID;
		private DateTime? _PublishDate;

		public DbItem()
		{
		}

		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_Title", DbType = "NVarChar(MAX) NOT NULL", CanBeNull = false)]
		public string Title
		{
			get
			{
				return this._Title;
			}
			set
			{
				if ((this._Title != value))
				{
					this._Title = value;
				}
			}
		}

		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_LastModifiedDate", DbType = "DateTime NOT NULL")]
		public System.DateTime LastModifiedDate
		{
			get
			{
				return this._LastModifiedDate;
			}
			set
			{
				if ((this._LastModifiedDate != value))
				{
					this._LastModifiedDate = value;
				}
			}
		}

		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_PublishDate", DbType = "DateTime NULL")]
		public System.DateTime? PublishDate
		{
			get
			{
				return this._PublishDate;
			}
			set
			{
				if ((this._PublishDate != value))
				{
					this._PublishDate = value;
				}
			}
		}

		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_LastModifiedUserID", DbType = "Int NOT NULL")]
		public int LastModifiedUserID
		{
			get
			{
				return this._LastModifiedUserID;
			}
			set
			{
				if ((this._LastModifiedUserID != value))
				{
					this._LastModifiedUserID = value;
				}
			}
		}

		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_CreatorID", DbType = "Int NOT NULL")]
		public int CreatorID
		{
			get
			{
				return this._CreatorID;
			}
			set
			{
				if ((this._CreatorID != value))
				{
					this._CreatorID = value;
				}
			}
		}

		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_ParentID", DbType = "UniqueIdentifier")]
		public System.Nullable<Guid> ParentID
		{
			get
			{
				return this._ParentID;
			}
			set
			{
				if ((this._ParentID != value))
				{
					this._ParentID = value;
				}
			}
		}

		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_IsDeleted", DbType = "Bit NOT NULL")]
		public bool IsDeleted
		{
			get
			{
				return this._IsDeleted;
			}
			set
			{
				if ((this._IsDeleted != value))
				{
					this._IsDeleted = value;
				}
			}
		}

		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_Length", DbType = "BigInt NOT NULL", Name="Length")]
		public long Size
		{
			get
			{
				return this._Length;
			}
			set
			{
				if ((this._Length != value))
				{
					this._Length = value;
				}
			}
		}

		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_Creator", DbType = "VarChar(255) NOT NULL", CanBeNull = false, Name = "Creator")]
		public string Creator
		{
			get
			{
				return this._Creator;
			}
			set
			{
				if ((this._Creator != value))
				{
					this._Creator = value;
				}
			}
		}

		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_LastModifiedUserTitle", DbType = "VarChar(255) NOT NULL", CanBeNull = false, Name = "LastModifiedUserTitle")]
		public string LastModifiedUserName
		{
			get
			{
				return this._LastModifiedUserTitle;
			}
			set
			{
				if ((this._LastModifiedUserTitle != value))
				{
					this._LastModifiedUserTitle = value;
				}
			}
		}

		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_Path", DbType = "NVarChar(MAX) NOT NULL", CanBeNull = false)]
		public string Path
		{
			get
			{
				return this._Path;
			}
			set
			{
				if ((this._Path != value))
				{
					this._Path = value;
				}
			}
		}

		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_Tag", DbType = "NVarChar(MAX) NULL", CanBeNull = true)]
		public string Tag
		{
			get
			{
				return this._Tag;
			}
			set
			{
				if ((this._Tag != value))
				{
					this._Tag = value;
				}
			}
		}

		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_Extension", DbType = "NVarChar(MAX) NOT NULL", CanBeNull = false)]
		public string Extension
		{
			get
			{
				return this._Extension;
			}
			set
			{
				if ((this._Extension != value))
				{
					this._Extension = value;
				}
			}
		}

		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_Type", DbType = "Int NOT NULL", Name = "Type")]
		public DbItemType Type
		{
			get
			{
				return (DbItemType)this._Type;
			}
			set
			{
				if ((this._Type != (int)value))
				{
					this._Type = (int)value;
				}
			}
		}

		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_Guid", DbType = "UniqueIdentifier NOT NULL", Name="Guid")]
		public System.Guid ID
		{
			get
			{
				return this._Guid;
			}
			set
			{
				if ((this._Guid != value))
				{
					this._Guid = value;
				}
			}
		}

		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_SPMActionItemGUID", DbType = "UniqueIdentifier NOT NULL")]
		public System.Guid SPMActionItemGUID
		{
			get
			{
				return this._SPMActionItemGUID;
			}
		}

		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_EnableVersioning", DbType = "Bit NOT NULL")]
		public bool EnableVersioning
		{
			get
			{
				return this._EnableVersioning;
			}
		}

		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_FullPath", DbType = "NVarChar(4000)")]
		public string FullPath
		{
			get
			{
				return this._FullPath;
			}
		}

		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_CheckedOutByID", DbType = "Int NULL")]
		public int? CheckedOutByID
		{
			get
			{
				return this._CheckedOutByID;
			}
		}

		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_CheckedOutBy", DbType = "NVarChar(4000)")]
		public string CheckedOutBy
		{
			get
			{
				return this._CheckedOutBy;
			}
		}
	}
	
}