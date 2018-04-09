using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.Data.Linq.Mapping;
using System.Data.Linq;
using Nephrite.Web;
using Nephrite.Web.SPM;
using Nephrite.Web.FileStorage;

namespace Solution.Model
{
	[global::System.Data.Linq.Mapping.TableAttribute(Name = "dbo.V_DbFolder")]
	public partial class DbFolder : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private string _Title;
		private string _Tag;
		private System.DateTime _LastModifiedDate;
		private System.DateTime? _PublishDate;
		private int _LastModifiedUserID;
		private int _IsValid;
		private bool _IsDeleted;
		private long _Size;
		private int _FileCount;
		private string _LastModifiedUserTitle;
		private int _CreatorID;
		private string _Creator;
		private System.Guid _ID;
		private System.Guid _SPMActionItemGUID;
		private string _Path;
		private char _StorageType;
		private string _StorageParameter;
		private System.Nullable<System.Guid> _ParentFolderID;
		private string _FullPath;
		private EntityRef<DbFolder> _ParentFolder;
		private EntitySet<DbFolder> _ChildFolders;
		private EntitySet<DbFile> _Files;
		private bool _EnableVersioning;
		
		public DbFolder()
		{
			this._ChildFolders = new EntitySet<DbFolder>(new Action<DbFolder>(this.attach_ChildFolders), new Action<DbFolder>(this.detach_ChildFolders));
			this._Files = new EntitySet<DbFile>(new Action<DbFile>(this.attach_DbFiles), new Action<DbFile>(this.detach_DbFiles));
			this._ParentFolder = default(EntityRef<DbFolder>);
			ID = Guid.NewGuid();
			CreatorID = AppSPM.GetCurrentSubjectID();
		}

		void Changed()
		{
			DateTime dt = DateTime.Now;
			this.SendPropertyChanging();
			this.LastModifiedDate = dt;
			this.SendPropertyChanged("LastModifiedDate");
			var sid = AppSPM.GetCurrentSubjectID();
			if (sid != LastModifiedUserID)
			{
				this.SendPropertyChanging();
				this.LastModifiedUserID = sid;
				this.SendPropertyChanged("LastModifiedUserID");
			}
		}

		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Title", DbType="NVarChar(255) NOT NULL", CanBeNull=false)]
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
					this.SendPropertyChanging();
					this._Title = value;
					this.SendPropertyChanged("Title");
					Changed();
				}
			}
		}

		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_Tag", DbType = "NVarChar(max) NULL", CanBeNull = true)]
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
					this.SendPropertyChanging();
					this._Tag = value;
					this.SendPropertyChanged("Tag");
					Changed();
				}
			}
		}

		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_PublishDate", DbType = "DateTime NULL", CanBeNull = true)]
		public DateTime? PublishDate
		{
			get
			{
				return this._PublishDate;
			}
			set
			{
				if ((this._PublishDate != value))
				{
					this.SendPropertyChanging();
					this._PublishDate = value;
					this.SendPropertyChanged("PublishDate");
					Changed();
				}
			}
		}

		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_StorageType", DbType = "Char(1) NOT NULL", CanBeNull = false)]
		public char StorageType
		{
			get
			{
				return this._StorageType;
			}
			set
			{
				if ((this._StorageType != value))
				{
					this.SendPropertyChanging();
					this._StorageType = value;
					this.SendPropertyChanged("StorageType");
					Changed();
				}
			}
		}

		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_StorageParameter", DbType = "NVarChar(max) NULL", CanBeNull = true)]
		public string StorageParameter
		{
			get
			{
				return this._StorageParameter;
			}
			set
			{
				if ((this._StorageParameter != value))
				{
					this.SendPropertyChanging();
					this._StorageParameter = value;
					this.SendPropertyChanged("StorageParameter");
					Changed();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_LastModifiedDate", DbType="DateTime NOT NULL")]
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
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_LastModifiedUserID", DbType="Int NOT NULL")]
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

		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_FileCount", DbType="Int NOT NULL", UpdateCheck=UpdateCheck.Never)]
		public int FileCount
		{
			get
			{
				return this._FileCount;
			}
			set
			{
				if ((this._FileCount != value))
				{
					this._FileCount = value;
				}
			}
		}

		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_IsValid", DbType = "Int NOT NULL")]
		public int IsValid
		{
			get
			{
				return this._IsValid;
			}
			set
			{
				if ((this._IsValid != value))
				{
					this._IsValid = value;
				}
			}
		}

		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_IsDeleted", DbType="Bit NOT NULL")]
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
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Size", DbType="BigInt", UpdateCheck=UpdateCheck.Never)]
		public long Size
		{
			get
			{
				return this._Size;
			}
			set
			{
				if ((this._Size != value))
				{
					this._Size = value;
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

		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_ID", DbType = "UniqueIdentifier NOT NULL", IsPrimaryKey = true)]
		public System.Guid ID
		{
			get
			{
				return this._ID;
			}
			set
			{
				if ((this._ID != value))
				{
					this._ID = value;
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
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Path", DbType="NVarChar(4000)")]
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

		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_FullPath", DbType = "NVarChar(max)")]
		public string FullPath
		{
			get
			{
				return this._FullPath;
			}
			set
			{
				if ((this._FullPath != value))
				{
					this._FullPath = value;
				}
			}
		}

		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_ParentFolderID", DbType = "UniqueIdentifier", Name = "ParentID")]
		public System.Nullable<System.Guid> ParentFolderID
		{
			get
			{
				return this._ParentFolderID;
			}
			set
			{
				if ((this._ParentFolderID != value))
				{
					if (this._ParentFolder.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this._ParentFolderID = value;
				}
			}
		}

		public event PropertyChangingEventHandler PropertyChanging;

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}

		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		[Association(Name = "FK_DbFolder_ParentFolder", Storage = "_ParentFolder", ThisKey = "ParentFolderID", OtherKey = "ID", IsForeignKey = true)]
		public DbFolder ParentFolder
		{
			get
			{
				return this._ParentFolder.Entity;
			}
			set
			{
				DbFolder previousValue = this._ParentFolder.Entity;
				if (((previousValue != value)
							|| (this._ParentFolder.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._ParentFolder.Entity = null;
						previousValue.ChildFolders.Remove(this);
					}
					this._ParentFolder.Entity = value;
					if ((value != null))
					{
						value.ChildFolders.Add(this);
						this._ParentFolderID = value.ID;
					}
					else
					{
						this._ParentFolderID = default(Nullable<Guid>);
					}
					this.SendPropertyChanged("ParentFolder");
					if (value != null)
						_FullPath = String.IsNullOrEmpty(value.FullPath) ? Title : (value.FullPath + "/" + Title);
					else
						_FullPath = Title;
					Changed();
				}
			}
		}

		[Association(Name = "FK_DbFolder_ParentFolder", Storage = "_ChildFolders", ThisKey = "ID", OtherKey = "ParentFolderID", DeleteRule = "NO ACTION")]
		public EntitySet<DbFolder> ChildFolders
		{
			get
			{
				return this._ChildFolders;
			}
			set
			{
				this._ChildFolders.Assign(value);
			}
		}

		[Association(Name = "FK_DbFile_ParentFolder", Storage = "_Files", ThisKey = "ID", OtherKey = "ParentFolderID", DeleteRule = "NO ACTION")]
		public EntitySet<DbFile> Files
		{
			get
			{
				return this._Files;
			}
			set
			{
				this._Files.Assign(value);
			}
		}

		[Column(Storage = "_EnableVersioning", DbType = "Bit NOT NULL", UpdateCheck = UpdateCheck.Never)]
		public bool EnableVersioning
		{
			get
			{
				return this._EnableVersioning;
			}
			set
			{
				if ((this._EnableVersioning != value))
				{
					this.SendPropertyChanging();
					this._EnableVersioning = value;
					this.SendPropertyChanged("EnableVersioning");
					Changed();
				}
			}
		}

		private void attach_ChildFolders(DbFolder entity)
		{
			this.SendPropertyChanging();
			entity.ParentFolder = this;
		}

		private void detach_ChildFolders(DbFolder entity)
		{
			this.SendPropertyChanging();
			entity.ParentFolder = null;
		}

		private void attach_DbFiles(DbFile entity)
		{
			this.SendPropertyChanging();
			entity.ParentFolder = this;
		}

		private void detach_DbFiles(DbFile entity)
		{
			this.SendPropertyChanging();
			entity.ParentFolder = null;
		}
	}
}