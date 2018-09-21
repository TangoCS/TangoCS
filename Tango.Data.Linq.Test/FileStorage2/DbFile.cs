using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.IO;
using Nephrite.Web;
using Nephrite.Web.SPM;
using Nephrite.Web.FileStorage;

namespace Solution.Model
{
	[global::System.Data.Linq.Mapping.TableAttribute(Name = "dbo.V_DbFile")]
	public partial class DbFile : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

		private long _Size;
		private string _Title;
		private System.Guid _ID;
		private System.Guid _SPMActionItemGUID;
		private string _Extension;
		private string _Path;
		private string _FullPath;
		private System.DateTime _LastModifiedDate;
		private System.DateTime? _PublishDate;
		private System.Nullable<Guid> _ParentFolderID;
		private int _LastModifiedUserID;
		private int _CreatorID;
		private string _Creator;
		private bool _IsDeleted;
		private string _LastModifiedUserTitle;
		private EntityRef<DbFolder> _ParentFolder;
		private System.Nullable<Guid> _FeatureGUID;
		private System.Nullable<int> _CheckedOutByID;
		private string _CheckedOutBy;
		private System.Nullable<Guid> _MainID;
		private int _VersionNumber;
		private string _Tag;
		private System.Data.Linq.Binary _DataHash;

		public DbFile()
		{
			this._ParentFolder = default(EntityRef<DbFolder>);
			_ID = Guid.NewGuid();
			CreatorID = AppSPM.GetCurrentSubjectID();
		}

		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_CheckedOutBy", DbType = "VarChar(255) NOT NULL", Name = "CheckedOutBy")]
		public string CheckedOutBy
		{
			get
			{
				return this._CheckedOutBy;
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

		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_CheckedOutByID", DbType = "Int NULL")]
		public System.Nullable<int> CheckedOutByID
		{
			get
			{
				return this._CheckedOutByID;
			}
			set
			{
				if ((this._CheckedOutByID != value))
				{
					this.SendPropertyChanging();
					this._CheckedOutByID = value;
					this.SendPropertyChanged("CheckedOutByID");
					Changed();
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

		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_Size", DbType = "BigInt NOT NULL")]
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
					this.SendPropertyChanging();
					this._Size = value;
					this.SendPropertyChanged("Size");
				}
			}
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
					this.SendPropertyChanging();
					this._Title = value;
					this.SendPropertyChanged("Title");
					Extension = System.IO.Path.GetExtension(_Title);
					Changed();
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
                this._ID = value;
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

		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_MainID", DbType = "UniqueIdentifier NULL")]
		public Nullable<System.Guid> MainID
		{
			get
			{
				return this._MainID;
			}
		}

		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_Extension", DbType = "NVarChar(50) NOT NULL", CanBeNull = false)]
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
					this.SendPropertyChanging();
					this._Extension = value;
					this.SendPropertyChanged("Extension");
				}
			}
		}

		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_Path", DbType = "NVarChar(MAX)")]
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
					this.SendPropertyChanging();
					this._Path = value;
					this.SendPropertyChanged("Path");
				}
			}
		}

		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_FullPath", DbType = "NVarChar(MAX)")]
		public string FullPath
		{
			get
			{
				return this._FullPath;
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
					this.SendPropertyChanging();
					this._LastModifiedDate = value;
					this.SendPropertyChanged("LastModifiedDate");
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

		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_VersionNumber", DbType = "Int NOT NULL")]
		public int VersionNumber
		{
			get
			{
				return this._VersionNumber;
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
					this._ParentFolderID = value;
				}
			}
		}

		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_FeatureGUID", DbType = "UniqueIdentifier", Name = "FeatureGUID")]
		public System.Nullable<System.Guid> FeatureGUID
		{
			get
			{
				return this._FeatureGUID;
			}
			set
			{
				if ((this._FeatureGUID != value))
				{
					this._FeatureGUID = value;
					Changed();
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
		}

		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_Tag", DbType = "NVarChar(MAX) NOT NULL")]
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

		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_DataHash", DbType = "Binary(16)", CanBeNull = true)]
		public System.Data.Linq.Binary DataHash
		{
			get
			{
				return this._DataHash;
			}
			set
			{
				if ((this._DataHash != value))
				{
					this.SendPropertyChanging();
					this._DataHash = value;
					this.SendPropertyChanged("DataHash");
				}
			}
		}

		void Changed()
		{
			if (CheckedOutByID.HasValue && CheckedOutByID.Value != AppSPM.GetCurrentSubjectID())
				throw new Exception("Файл " + Title + " извлечен другим пользователем!");
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

		[Association(Name = "FK_DbFile_ParentFolder", Storage = "_ParentFolder", ThisKey = "ParentFolderID", OtherKey = "ID", IsForeignKey = true)]
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
						previousValue.Files.Remove(this);
					}
					this._ParentFolder.Entity = value;
					if ((value != null))
					{
						value.Files.Add(this);
						this._ParentFolderID = value.ID;
					}
					else
					{
						this._ParentFolderID = default(Nullable<Guid>);
					}
					this.SendPropertyChanged("ParentFolder");
				}
			}
		}
	}
}