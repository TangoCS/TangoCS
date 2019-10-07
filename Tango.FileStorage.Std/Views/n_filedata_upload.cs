using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using Tango.Data;
using Tango.FileStorage.Std.Model;
using Tango.Localization;
using Tango.UI;
using Tango.UI.Std;

namespace Tango.FileStorage.Std.Views
{
	[OnAction(typeof(N_FileData), "upload")]
	public class n_filedata_upload : default_edit
	{
		MetaN_FileData meta = new MetaN_FileData();

		[Inject]
		protected IDataContext DataContext { get; set; }

		protected override void Submit(ApiResponse response)
		{
			DataContext.SubmitChanges();
		}

		protected override string Title => Resources.Caption(meta.Upload);

		protected override void Form(LayoutWriter w)
		{
			w.FieldsBlockStd(() => {
				w.FormFieldFileUpload("file1", Resources.CaptionFor(meta.Upload, "file1"));
				w.FormFieldFileUpload("file2", Resources.CaptionFor(meta.Upload, "file2"));
				w.FormFieldFileUpload("file3", Resources.CaptionFor(meta.Upload, "file3"));
				w.FormFieldFileUpload("file4", Resources.CaptionFor(meta.Upload, "file4"));
				w.FormFieldFileUpload("file5", Resources.CaptionFor(meta.Upload, "file5"));
				w.FormFieldCheckBox("unzip", Resources.CaptionFor(meta.Upload, "unzip"), true);
			});
		}

		protected override void ProcessFormData(ValidationMessageCollection val)
		{
			if (FormData["file1"] is PostedFileInfo) Save(FormData["file1"] as PostedFileInfo, val);
			if (FormData["file2"] is PostedFileInfo) Save(FormData["file2"] as PostedFileInfo, val);
			if (FormData["file3"] is PostedFileInfo) Save(FormData["file3"] as PostedFileInfo, val);
			if (FormData["file4"] is PostedFileInfo) Save(FormData["file4"] as PostedFileInfo, val);
			if (FormData["file5"] is PostedFileInfo) Save(FormData["file5"] as PostedFileInfo, val);
		}

		void Save(PostedFileInfo fi, ValidationMessageCollection val)
		{
			var folder = new DatabaseFolder(DataContext);

			if (fi.FileName.EndsWith(".zip") && FormData.Parse<bool>("unzip"))
			{
				Unzip(folder, fi, val);
			}
			else
			{
				var file = folder.GetOrCreateFile(fi.FileName);
				file.WriteAllBytes(fi.FileBytes);
			}
		}

		void Unzip(DatabaseFolder folder, PostedFileInfo fi, ValidationMessageCollection val)
		{
			try
			{
				using (ZipArchive zf = new ZipArchive(new MemoryStream(fi.FileBytes), ZipArchiveMode.Read, false, Encoding.GetEncoding(866)))
				{
					foreach (ZipArchiveEntry ze in zf.Entries)
					{
						var file = folder.GetOrCreateFile(ze.Name);
						using (var s = ze.Open())
						{
							var bytes = new byte[ze.Length];
							s.Read(bytes, 0, bytes.Length);
							file.WriteAllBytes(bytes);
						}
					}
				}
			}
			catch (Exception ex)
			{
				val.Add("entitycheck", "msg", "Error while unzipping file " + Path.GetFileName(fi.FileName) + ": " + ex.Message);
			}
		}

		protected override void ValidateFormData(ValidationMessageCollection val)
		{
			
		}
	}
}
