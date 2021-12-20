using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Tango;
using Tango.Data;
using Tango.FileStorage.Std.Model;
using Tango.Html;
using Tango.Localization;
using Tango.UI;
using Tango.UI.Controls;
using Tango.UI.Std;

namespace Tango.FileStorage.Std.Views
{
    [OnAction(typeof(N_FileData), "viewlist")]
    public class N_FileData_list : default_list<N_FileData>
    {
        protected override void ToolbarLeft(MenuBuilder left)
        {
            left.ItemFilter(Filter);
            left.ItemSeparator();
            left.ItemActionImageText(x => x.To<N_FileData>("Upload", AccessControl).WithImage("upload"));
        }

        protected override void FieldsInit(FieldCollection<N_FileData, N_FileData> f)
        {
            f.AddCellWithSortAndFilter(o => o.Title, (w, o) => w.ActionLink(al => al.ToView(AccessControl, o).WithTitle(o.Title)));
            f.AddCellWithSortAndFilter(o => o.LastModifiedDate, o => o.LastModifiedDate.DateTimeToString());
            f.AddCellWithSortAndFilter(o => o.Size, o => o.Size);
            f.AddCell(o => o.Owner, o => o.Owner);
            f.AddFilterCondition(o => o.Owner);
			f.AddCustomCell(f.Resources.Get("Common.Actions"), new ListColumn<N_FileData>(
				(a, o, i) => a.Style("text-align:center"),
				(w, o, i) => {
					w.ActionImage(al => al.To<N_FileData>("download", null).WithRequestMethod("POST").WithArg(Constants.Id, o.ID).WithImage("download").KeepTheSameUrl(), attrs: a => a.Data("responsetype", "arraybuffer"));
					w.ActionImage(al => al.ToDelete(AccessControl, o));
				}
			));
        }

        protected override IQueryable<N_FileData> Selector(IQueryable<N_FileData> data)
        {
            return data.Select(o => new N_FileData
            {
                Title = o.Title,
                Owner = o.Owner,
                Size = o.Size,
                LastModifiedDate = o.LastModifiedDate,
				FileGUID = o.FileGUID
			});
		}

		protected override Func<string, Expression<Func<N_FileData, bool>>> SearchExpression => s =>
		{
			if (Guid.TryParse(s, out Guid n))
				return o => o.Owner == n || o.Title.Contains(s);
			else
				return o => o.Title.Contains(s);
		};
	}

    [OnAction(typeof(N_FileData), "view")]
    public class N_FileData_view : default_view<N_FileData, Guid>
    {
        N_FileDataFields.DefaultGroup gr { get; set; }

        Lazy<IStorageFile> _file;
        IStorageFile File => _file.Value;
        bool IsTxt => ViewData.Extension.In(".sql", ".css", ".cs", ".xslt", ".txt", ".json");
        bool IsXml => ViewData.Extension.In(".xml");
        bool IsImg => ViewData.Extension.In(".jpg", ".gif", ".png");


        public N_FileData_view()
        {
            _file = new Lazy<IStorageFile>(() => {
                var folder = new DatabaseFolder(DataContext);
                return folder.GetFile(Context.GetGuidArg(Constants.Id).Value);
            });
        }

        protected override void ToolbarLeft(MenuBuilder t)
        {
            t.ItemBack();
            t.ItemSeparator();
            t.Item(w => w.PostBackButton(a => a.DataEvent(OnFileDownload).Data("responsetype", "arraybuffer").Class("actionbtn"), "Скачать"));
            t.ItemSeparator();
            t.ItemActionImageText(x => x.ToDelete(AccessControl, ViewData, Context.ReturnUrl.Get(1))
                                        .WithArg(Constants.ReturnUrl + "_0", Context.CreateReturnUrl(1)));
        }

        public ActionResult OnFileDownload()
        {
            return new FileResult(File.Name, File.ReadAllBytes());
        }

        protected override void Form(LayoutWriter w)
        {
            w.FieldsBlockStd(() => {
                w.PlainText(gr.Title);
            });

            if (IsTxt)
            {
                w.GroupTitle(Resources.Get("Common.Text"));
                var data = File.ReadAllText();
                w.TextArea("data", data, a => a.Style("width:100%; height:400px").Disabled(true));
            }
            else if (IsImg)
            {
                w.GroupTitle(Resources.Get("Common.Image"));
                w.Img(a => a.Src(""));
            }
            else if (IsXml)
            {
                w.GroupTitle(Resources.Get("Common.Xml"));
                var doc = System.Xml.Linq.XDocument.Parse(File.ReadAllText(), System.Xml.Linq.LoadOptions.PreserveWhitespace);
                w.Pre(() => w.Write(System.Net.WebUtility.HtmlEncode(doc.ToString())));
            }
        }
    }

    [OnAction(typeof(N_FileData), "edit")]
    public class N_FileData_edit : default_edit<N_FileData, Guid>
    {
        N_FileDataFields.DefaultGroup gr { get; set; }

        Lazy<IStorageFile> _file;
        IStorageFile File => _file.Value;
        bool IsTxt => File.Extension.In(".sql", ".css", ".cs", ".xslt", ".txt", ".xml", ".json") || CreateObjectMode;
        bool IsImg => File.Extension.In(".jpg", ".gif", ".png");

        protected override void Submit(ApiResponse response)
        {
            DataContext.SubmitChanges();
        }

        protected override string FormTitle => File?.Name;

        public N_FileData_edit()
        {
            _file = new Lazy<IStorageFile>(() => {
                var folder = new DatabaseFolder(DataContext);
                return CreateObjectMode ? folder.CreateFile(Guid.NewGuid()) : folder.GetFile(Context.GetGuidArg(Constants.Id).Value);
            });
        }

        protected override void Form(LayoutWriter w)
        {
            w.FieldsBlockStd(() => {
                w.TextBox(gr.Title);
            });

            if (IsTxt)
            {
                w.GroupTitle(Resources.Get("Common.Text"));
                var data = File.ReadAllText();
                w.TextArea("data", data, a => a.Style("width:100%; height:400px"));
            }
            else if (IsImg)
            {
                w.GroupTitle(Resources.Get("Common.Image"));
                w.Img(a => a.Src(""));
            }
        }

        protected override void ProcessFormData(ValidationMessageCollection val)
        {
            File.Name = FormData.Parse<string>(gr.Title.ID);
            File.Extension = Path.GetExtension(File.Name).ToLower();

            if (IsTxt)
            {
                File.WriteAllText(FormData.Parse<string>("data"));
            }
        }

        //protected override void ValidateFormData(ValidationMessageCollection val)
        //{
        //    val.Check(Resources, gr.Title.ID, gr.Title.ID, FormData).NotEmpty();
        //}
    }

    [OnAction(typeof(N_FileData), "upload")]
    public class N_FileData_upload : default_edit
    {
        [Inject]
        protected IDataContext DataContext { get; set; }

        protected override void Submit(ApiResponse response)
        {
            DataContext.SubmitChanges();
        }

        protected override string FormTitle => Resources.Get<N_FileData>("Upload");

        protected override void Form(LayoutWriter w)
        {
            w.FieldsBlockStd(() => {
                w.FormFieldFileUpload("file1", Resources.Get<N_FileData>("Upload", "file1"));
                w.FormFieldFileUpload("file2", Resources.Get<N_FileData>("Upload", "file2"));
                w.FormFieldFileUpload("file3", Resources.Get<N_FileData>("Upload", "file3"));
                w.FormFieldFileUpload("file4", Resources.Get<N_FileData>("Upload", "file4"));
                w.FormFieldFileUpload("file5", Resources.Get<N_FileData>("Upload", "file5"));
                w.FormFieldCheckBox("unzip", Resources.Get<N_FileData>("Upload", "unzip"), true);
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

    [OnAction(typeof(N_FileData), "download")]
    public class N_FileData_download : ViewPagePart
    {
        [Inject]
        IDataContext DataContext { get; set; }

        public override void OnLoad(ApiResponse response) { }

        public override ActionResult Execute()
        {
            var r = Context.EventReceiver;
            var recipient = r == null || r == ID?.ToLower() || r == ClientID?.ToLower() ? this : Context.EventReceivers.First(o => o.ClientID == r);

            return recipient.RunEvent("onfiledownload");
        }

        public ActionResult OnFileDownload()
        {
            var folder = new DatabaseFolder(DataContext);
            IStorageFile file = folder.GetFile(Context.GetGuidArg(Constants.Id).Value);
            var res = new FileResult(file.Name, file.ReadAllBytes());
            
            return res;
        }
    }

    [OnAction(typeof(N_FileData), "delete")]
    public class N_FileData_delete : default_delete<N_FileData, Guid> { }

    public static class ConvertExtentions
    {
        public static byte[] GetBytes(this string bytes)
        {
            var encoder = new byte[bytes.Length];
            for (int i = 0; i < bytes.Length; i++)
            {
                encoder[i] = (byte)bytes[i];
            }
            return encoder;
        }

        public static byte[] GetBytes(this string bytes, int start, int size)
        {
            var encoder = new byte[size];
            int count = start + size;
            for (int i = 0; i < count; i++)
            {
                encoder[i] = (byte)bytes[i];
            }
            return encoder;
        }

        public static string GetString(this byte[] bytes)
        {
            var encoder = new StringBuilder();
            foreach (byte t in bytes)
            {
                encoder.Append((char)t);
            }
            return encoder.ToString();
        }

        public static string GetString(this byte[] bytes, int start, int size)
        {
            var encoder = new StringBuilder();
            int count = start + size;
            for (int i = start; i < count; i++)
            {
                encoder.Append((char)bytes[i]);
            }
            return encoder.ToString();
        }
    }

	public static class N_FileDataFields
	{
		public class LastModifiedDate : EntityField<N_FileData, DateTime> { }
		public class Data : EntityField<N_FileData, byte[]> { }
		public class Extension : EntityField<N_FileData, string> { }
		public class Size : EntityField<N_FileData, long> { }
		public class Owner : EntityField<N_FileData, Guid?> { }

		public class DefaultGroup : FieldGroup
		{
			public CommonFields.Title Title { get; set; }
			public LastModifiedDate LastModifiedDate { get; set; }
			public Data Data { get; set; }
			public Extension Extension { get; set; }
			public Size Size { get; set; }
			public Owner Owner { get; set; }
		}
	}
}
