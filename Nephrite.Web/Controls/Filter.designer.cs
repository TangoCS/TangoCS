﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Nephrite.Web.Controls
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Data;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	using System.ComponentModel;
	using System;
	
	
	[global::System.Data.Linq.Mapping.DatabaseAttribute(Name="iims")]
	public partial class FilterDataContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Extensibility Method Definitions
    partial void OnCreated();
    partial void InsertN_Filter(N_Filter instance);
    partial void UpdateN_Filter(N_Filter instance);
    partial void DeleteN_Filter(N_Filter instance);
    #endregion
		
		public FilterDataContext() : 
				base(global::System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString, mappingSource)
		{
			OnCreated();
		}
		
		public FilterDataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public FilterDataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public FilterDataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public FilterDataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<N_Filter> N_Filters
		{
			get
			{
				return this.GetTable<N_Filter>();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.N_Filter")]
	public partial class N_Filter : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _FilterID;
		
		private string _ListName;
		
		private System.Xml.Linq.XElement _FilterValue;
		
		private string _FilterName;
		
		private bool _IsDefault;
		
		private System.Nullable<int> _SubjectID;
		
		private System.Nullable<int> _Group1Column;
		
		private System.Nullable<char> _Group1Sort;
		
		private System.Nullable<int> _Group2Column;
		
		private System.Nullable<char> _Group2Sort;
		
		private string _ListParms;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnFilterIDChanging(int value);
    partial void OnFilterIDChanged();
    partial void OnListNameChanging(string value);
    partial void OnListNameChanged();
    partial void OnFilterValueChanging(System.Xml.Linq.XElement value);
    partial void OnFilterValueChanged();
    partial void OnFilterNameChanging(string value);
    partial void OnFilterNameChanged();
    partial void OnIsDefaultChanging(bool value);
    partial void OnIsDefaultChanged();
    partial void OnSubjectIDChanging(System.Nullable<int> value);
    partial void OnSubjectIDChanged();
    partial void OnGroup1ColumnChanging(System.Nullable<int> value);
    partial void OnGroup1ColumnChanged();
    partial void OnGroup1SortChanging(System.Nullable<char> value);
    partial void OnGroup1SortChanged();
    partial void OnGroup2ColumnChanging(System.Nullable<int> value);
    partial void OnGroup2ColumnChanged();
    partial void OnGroup2SortChanging(System.Nullable<char> value);
    partial void OnGroup2SortChanged();
    partial void OnListParmsChanging(string value);
    partial void OnListParmsChanged();
    #endregion
		
		public N_Filter()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_FilterID", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int FilterID
		{
			get
			{
				return this._FilterID;
			}
			set
			{
				if ((this._FilterID != value))
				{
					this.OnFilterIDChanging(value);
					this.SendPropertyChanging();
					this._FilterID = value;
					this.SendPropertyChanged("FilterID");
					this.OnFilterIDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ListName", DbType="VarChar(50)")]
		public string ListName
		{
			get
			{
				return this._ListName;
			}
			set
			{
				if ((this._ListName != value))
				{
					this.OnListNameChanging(value);
					this.SendPropertyChanging();
					this._ListName = value;
					this.SendPropertyChanged("ListName");
					this.OnListNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_FilterValue", DbType="Xml", UpdateCheck=UpdateCheck.Never)]
		public System.Xml.Linq.XElement FilterValue
		{
			get
			{
				return this._FilterValue;
			}
			set
			{
				if ((this._FilterValue != value))
				{
					this.OnFilterValueChanging(value);
					this.SendPropertyChanging();
					this._FilterValue = value;
					this.SendPropertyChanged("FilterValue");
					this.OnFilterValueChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_FilterName", DbType="NVarChar(50)")]
		public string FilterName
		{
			get
			{
				return this._FilterName;
			}
			set
			{
				if ((this._FilterName != value))
				{
					this.OnFilterNameChanging(value);
					this.SendPropertyChanging();
					this._FilterName = value;
					this.SendPropertyChanged("FilterName");
					this.OnFilterNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_IsDefault", DbType="Bit NOT NULL")]
		public bool IsDefault
		{
			get
			{
				return this._IsDefault;
			}
			set
			{
				if ((this._IsDefault != value))
				{
					this.OnIsDefaultChanging(value);
					this.SendPropertyChanging();
					this._IsDefault = value;
					this.SendPropertyChanged("IsDefault");
					this.OnIsDefaultChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_SubjectID", DbType="Int")]
		public System.Nullable<int> SubjectID
		{
			get
			{
				return this._SubjectID;
			}
			set
			{
				if ((this._SubjectID != value))
				{
					this.OnSubjectIDChanging(value);
					this.SendPropertyChanging();
					this._SubjectID = value;
					this.SendPropertyChanged("SubjectID");
					this.OnSubjectIDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Group1Column", DbType="Int")]
		public System.Nullable<int> Group1Column
		{
			get
			{
				return this._Group1Column;
			}
			set
			{
				if ((this._Group1Column != value))
				{
					this.OnGroup1ColumnChanging(value);
					this.SendPropertyChanging();
					this._Group1Column = value;
					this.SendPropertyChanged("Group1Column");
					this.OnGroup1ColumnChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Group1Sort", DbType="Char(1)")]
		public System.Nullable<char> Group1Sort
		{
			get
			{
				return this._Group1Sort;
			}
			set
			{
				if ((this._Group1Sort != value))
				{
					this.OnGroup1SortChanging(value);
					this.SendPropertyChanging();
					this._Group1Sort = value;
					this.SendPropertyChanged("Group1Sort");
					this.OnGroup1SortChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Group2Column", DbType="Int")]
		public System.Nullable<int> Group2Column
		{
			get
			{
				return this._Group2Column;
			}
			set
			{
				if ((this._Group2Column != value))
				{
					this.OnGroup2ColumnChanging(value);
					this.SendPropertyChanging();
					this._Group2Column = value;
					this.SendPropertyChanged("Group2Column");
					this.OnGroup2ColumnChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Group2Sort", DbType="Char(1)")]
		public System.Nullable<char> Group2Sort
		{
			get
			{
				return this._Group2Sort;
			}
			set
			{
				if ((this._Group2Sort != value))
				{
					this.OnGroup2SortChanging(value);
					this.SendPropertyChanging();
					this._Group2Sort = value;
					this.SendPropertyChanged("Group2Sort");
					this.OnGroup2SortChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ListParms", DbType="VarChar(255)")]
		public string ListParms
		{
			get
			{
				return this._ListParms;
			}
			set
			{
				if ((this._ListParms != value))
				{
					this.OnListParmsChanging(value);
					this.SendPropertyChanging();
					this._ListParms = value;
					this.SendPropertyChanged("ListParms");
					this.OnListParmsChanged();
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
#pragma warning restore 1591