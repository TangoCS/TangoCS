using System;
using Tango.Meta;
using Tango.Meta.Fluent;
using Tango;
using Abc.Model;
using Tango.UI;

namespace Tango.FileStorage.Std.Model
{
    public partial class N_Folder { }
    public partial class N_File { }
    public partial class N_FileData { }
    public partial class N_FileLibrary { }
    public partial class N_FileLibraryType { }
    public partial class N_DownloadLog { }
    public partial class V_N_FullFolder { }

    public partial struct StorageType : IEnum
    {
        public const string Disk = "D";
        public const string Database = "B";
    }

    public partial class Builder : SolutionBuilder
    {
        public void N_Folder(IMetaClass cls)
        {
            Class<N_Folder>(cls).IntKey()
            .TimeStamp<SPM_Subject, int>()
            .Title()
            .Attribute<bool>("IsDeleted", true, x => x.DefaultDBValue = "false")
            .Attribute<string>("Path")
            .Attribute<Guid>("Guid")
            .Attribute<string>("GuidPath")
            .Attribute<Guid>("SPMActionItemGUID")
            .Reference<N_FileLibrary, int>("FileLibrary")
            .Reference<N_Folder, int>("Parent", x => x.SetInverseProperty("Folders"))
            .Reference<N_Folder>("Folders", x => x.Aggregation().SetInverseProperty("Parent"))
            .Reference<N_File>("Files", x => x.Aggregation().SetInverseProperty("Folder"))
            .Operation("CreateNew", x => x.ParmInt("parentid").ParmReturnUrl())
            .OperationList()
            .Operation("Upload", x => x.ParmReturnUrl())
            .OperationEdit()
            .OperationView()
            .OperationDelete()
            .Operation("PackAndDownload", x => x.ParmIntId());
        }
        public void N_File(IMetaClass cls)
        {
            Class<N_File>(cls).IntKey()
            .TimeStamp<SPM_Subject, int>()
            .Title()
            .Attribute<string>("Password")
            .Attribute<DateTime>("BeginDate", x => x.DefaultDBValue = "(getdate())")
            .Attribute<DateTime>("EndDate", x => x.DefaultDBValue = "(2099-12-31)")
            .Attribute<string>("Extension", true)
            .Attribute<Guid>("Guid")
            .Attribute<string>("GuidPath")
            .Attribute<string>("StorageType", TypeFactory.Enum<StorageType>(), x => x.DefaultDBValue = "(B)")
            .Attribute<string>("Tag")
            .Attribute<int>("VersionNumber", x => x.DefaultDBValue = "(1)")
            .Attribute<DateTime?>("PublishDate")
            .Attribute<long>("Length")
            .Attribute<Guid?>("MainGUID")
            .Attribute<string>("Path")
            .Attribute<string>("StorageParameter")
            .PersistentComputedAttribute<bool>("IsDeleted", x => {
                x.Expression = "SELECT CAST(COALESCE(case when enddate>getdate() AND begindate<getdate() then 0 else 1 end, 1) as boolean) from n_file s where s.fileid=$1.fileid;";
                //x.Expression = "isnull(CONVERT([bit],case when [EndDate]>getdate() and [BeginDate]<getdate() then (0) else (1) end,(0)),(1))";
            })
            .Reference<N_Folder, int>("Folder", x => x.SetInverseProperty("Files"))
            .Reference<SPM_Subject, int?>("CheckedOutBy")
            .Reference<SPM_Subject, int>("Creator")
            .OperationDelete(x => x
                .ParmGuid("folderid")
                .ParmString("filekey")
                .ParmReturnUrl()
            )
            .OperationCreateNew(x => x
                .ParmInt("folderid")
                .ParmReturnUrl()
            )
            .OperationEdit(x => x
                .ParmGuid("folderid")
                .ParmString("filekey")
                .ParmReturnUrl()
            );
        }
        public void N_FileData(IMetaClass cls)
        {
            Class<N_FileData>(cls).GuidKey("FileGUID")
            .Attribute<string>("Extension")
            .Attribute<byte[]>("Data")
            .Title()
            .Attribute<long>("Size")
            .Attribute<DateTime>("LastModifiedDate")
            .Attribute<Guid?>("Owner")
            .OperationCreateNew()
            .OperationDelete()
            .OperationList()
            .OperationEdit()
            .Operation("Upload", x => x.ParmReturnUrl());
        }
        public void N_FileLibraryType(IMetaClass cls)
        {
            Class<N_FileLibraryType>(cls).IntKey()
            .TimeStamp<SPM_Subject, int>()
            .IsDeleted()
            .Title()
            .Attribute<string>("Extensions", true)
            .Attribute<string>("ClassName")
            .OperationCreateNew()
            .OperationEdit()
            .OperationDelete()
            .OperationList()
            .OperationUnDelete()
            .OperationView();
        }
        public void N_FileLibrary(IMetaClass cls)
        {
            Class<N_FileLibrary>(cls).IntKey()
            .Attribute<int>("MaxFileSize")
            .Attribute<string>("StorageType", TypeFactory.Enum<StorageType>(), x => x.DefaultDBValue = "(B)")
            .Attribute<string>("StorageParameter")
            .Reference<N_FileLibraryType, int>("FileLibraryType");
        }
        public void N_DownloadLog(IMetaClass cls)
        {
            Class<N_DownloadLog>(cls).IntKey()
            .TimeStamp<SPM_Subject, int>()
            .IsDeleted()
            .Attribute<Guid>("FileGUID")
            .Attribute<string>("IP")
            .Reference<N_File, int>("File")
            .OperationList();
        }
        public void V_N_FullFolder(IMetaClass cls)
        {
            Class<V_N_FullFolder>(cls).Persistence(PersistenceType.View)
            .IntKey("FolderID")
            .Attribute<int>("ParentID")
            .Attribute<int>("ArcLen");
        }
    }

}