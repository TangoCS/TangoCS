using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace Nephrite.Web.FileStorage
{
	[global::System.Data.Linq.Mapping.TableAttribute(Name = "dbo.N_FileData")]
	internal partial class DbFileData : INotifyPropertyChanging, INotifyPropertyChanged
	{

		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

		private System.Data.Linq.Binary _Data;

		private string _Extension;

		private System.Guid _FileGUID;

		#region Extensibility Method Definitions
		partial void OnLoaded();
		partial void OnValidate(System.Data.Linq.ChangeAction action);
		partial void OnCreated();
		partial void OnFileIDChanging(int value);
		partial void OnFileIDChanged();
		partial void OnDataChanging(System.Data.Linq.Binary value);
		partial void OnDataChanged();
		partial void OnExtensionChanging(string value);
		partial void OnExtensionChanged();
		partial void OnFileGUIDChanging(System.Guid value);
		partial void OnFileGUIDChanged();
		#endregion

		public DbFileData()
		{
			OnCreated();
		}

		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_Data", DbType = "Image", UpdateCheck = UpdateCheck.Never)]
		public System.Data.Linq.Binary Data
		{
			get
			{
				return this._Data;
			}
			set
			{
				if ((this._Data != value))
				{
					this.OnDataChanging(value);
					this.SendPropertyChanging();
					this._Data = value;
					this.SendPropertyChanged("Data");
					this.OnDataChanged();
				}
			}
		}

		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_Extension", DbType = "NVarChar(50)")]
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
					this.OnExtensionChanging(value);
					this.SendPropertyChanging();
					this._Extension = value;
					this.SendPropertyChanged("Extension");
					this.OnExtensionChanged();
				}
			}
		}

		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_FileGUID", DbType = "UniqueIdentifier NOT NULL", IsPrimaryKey = true)]
		public System.Guid FileGUID
		{
			get
			{
				return this._FileGUID;
			}
			set
			{
				if ((this._FileGUID != value))
				{
					this.OnFileGUIDChanging(value);
					this.SendPropertyChanging();
					this._FileGUID = value;
					this.SendPropertyChanged("FileGUID");
					this.OnFileGUIDChanged();
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
	}
}