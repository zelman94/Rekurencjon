﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Rekurencjon
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
	
	
	[global::System.Data.Linq.Mapping.DatabaseAttribute(Name="UltimateChangerDatabase")]
	public partial class DatabaseManagerDataContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Extensibility Method Definitions
    partial void OnCreated();
    partial void InsertBuild(Build instance);
    partial void UpdateBuild(Build instance);
    partial void DeleteBuild(Build instance);
    #endregion
		
		public DatabaseManagerDataContext() : 
				base(global::Rekurencjon.Properties.Settings.Default.UltimateChangerDatabaseConnectionString, mappingSource)
		{
			OnCreated();
		}
		
		public DatabaseManagerDataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public DatabaseManagerDataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public DatabaseManagerDataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public DatabaseManagerDataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<Build> Builds
		{
			get
			{
				return this.GetTable<Build>();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.Builds")]
	public partial class Build : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _Id;
		
		private string _Type;
		
		private string _Release;
		
		private string _Mode;
		
		private string _About;
		
		private string _Brand;
		
		private string _Oem;
		
		private System.Nullable<System.DateTime> _CreationDate;
		
		private string _BuildID;
		
		private string _Path;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIdChanging(int value);
    partial void OnIdChanged();
    partial void OnTypeChanging(string value);
    partial void OnTypeChanged();
    partial void OnReleaseChanging(string value);
    partial void OnReleaseChanged();
    partial void OnModeChanging(string value);
    partial void OnModeChanged();
    partial void OnAboutChanging(string value);
    partial void OnAboutChanged();
    partial void OnBrandChanging(string value);
    partial void OnBrandChanged();
    partial void OnOemChanging(string value);
    partial void OnOemChanged();
    partial void OnCreationDateChanging(System.Nullable<System.DateTime> value);
    partial void OnCreationDateChanged();
    partial void OnBuildIDChanging(string value);
    partial void OnBuildIDChanged();
    partial void OnPathChanging(string value);
    partial void OnPathChanged();
    #endregion
		
		public Build()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Id", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int Id
		{
			get
			{
				return this._Id;
			}
			set
			{
				if ((this._Id != value))
				{
					this.OnIdChanging(value);
					this.SendPropertyChanging();
					this._Id = value;
					this.SendPropertyChanged("Id");
					this.OnIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Type", DbType="NChar(20) NOT NULL", CanBeNull=false)]
		public string Type
		{
			get
			{
				return this._Type;
			}
			set
			{
				if ((this._Type != value))
				{
					this.OnTypeChanging(value);
					this.SendPropertyChanging();
					this._Type = value;
					this.SendPropertyChanged("Type");
					this.OnTypeChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Release", DbType="NChar(20) NOT NULL", CanBeNull=false)]
		public string Release
		{
			get
			{
				return this._Release;
			}
			set
			{
				if ((this._Release != value))
				{
					this.OnReleaseChanging(value);
					this.SendPropertyChanging();
					this._Release = value;
					this.SendPropertyChanged("Release");
					this.OnReleaseChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Mode", DbType="NChar(20) NOT NULL", CanBeNull=false)]
		public string Mode
		{
			get
			{
				return this._Mode;
			}
			set
			{
				if ((this._Mode != value))
				{
					this.OnModeChanging(value);
					this.SendPropertyChanging();
					this._Mode = value;
					this.SendPropertyChanged("Mode");
					this.OnModeChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_About", DbType="NChar(20) NOT NULL", CanBeNull=false)]
		public string About
		{
			get
			{
				return this._About;
			}
			set
			{
				if ((this._About != value))
				{
					this.OnAboutChanging(value);
					this.SendPropertyChanging();
					this._About = value;
					this.SendPropertyChanged("About");
					this.OnAboutChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Brand", DbType="NChar(20) NOT NULL", CanBeNull=false)]
		public string Brand
		{
			get
			{
				return this._Brand;
			}
			set
			{
				if ((this._Brand != value))
				{
					this.OnBrandChanging(value);
					this.SendPropertyChanging();
					this._Brand = value;
					this.SendPropertyChanged("Brand");
					this.OnBrandChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Oem", DbType="NChar(20) NOT NULL", CanBeNull=false)]
		public string Oem
		{
			get
			{
				return this._Oem;
			}
			set
			{
				if ((this._Oem != value))
				{
					this.OnOemChanging(value);
					this.SendPropertyChanging();
					this._Oem = value;
					this.SendPropertyChanged("Oem");
					this.OnOemChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_CreationDate", DbType="Date")]
		public System.Nullable<System.DateTime> CreationDate
		{
			get
			{
				return this._CreationDate;
			}
			set
			{
				if ((this._CreationDate != value))
				{
					this.OnCreationDateChanging(value);
					this.SendPropertyChanging();
					this._CreationDate = value;
					this.SendPropertyChanged("CreationDate");
					this.OnCreationDateChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_BuildID", DbType="Text NOT NULL", CanBeNull=false, UpdateCheck=UpdateCheck.Never)]
		public string BuildID
		{
			get
			{
				return this._BuildID;
			}
			set
			{
				if ((this._BuildID != value))
				{
					this.OnBuildIDChanging(value);
					this.SendPropertyChanging();
					this._BuildID = value;
					this.SendPropertyChanged("BuildID");
					this.OnBuildIDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Path", DbType="Text NOT NULL", CanBeNull=false, UpdateCheck=UpdateCheck.Never)]
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
					this.OnPathChanging(value);
					this.SendPropertyChanging();
					this._Path = value;
					this.SendPropertyChanged("Path");
					this.OnPathChanged();
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
