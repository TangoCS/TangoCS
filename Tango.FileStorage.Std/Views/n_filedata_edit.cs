using System;
using System.IO;
using Tango.Data;
using Tango.FileStorage.Std.Model;
using Tango.Html;
using Tango.Localization;
using Tango.UI;
using Tango.UI.Std;

namespace Tango.FileStorage.Std.Views
{
	[OnAction(typeof(N_FileData), "edit")]
	public class n_filedata_edit : default_edit
	{
		MetaN_FileData meta = new MetaN_FileData();

		bool CreateObjectMode => Context.GetArg(Constants.Id) == null;		
		Lazy<IStorageFile> _file;
		IStorageFile File => _file.Value;
		bool IsTxt => File.Extension.In(".sql", ".css", ".cs", ".xslt", ".txt", ".xml", ".json") || CreateObjectMode;
		bool IsImg => File.Extension.In(".jpg", ".gif", ".png");

		[Inject]
		protected IDataContext DataContext { get; set; }

		protected override void Submit(ApiResponse response)
		{
			DataContext.SubmitChanges();
		}

		protected override string Title => CreateObjectMode ? Resources.Caption(meta.GetInfo()) : File?.Name;

		public n_filedata_edit()
		{
			_file = new Lazy<IStorageFile>(() => {
				var folder = new DatabaseFolder(DataContext);
				return CreateObjectMode ? folder.CreateFile(Guid.NewGuid()) : folder.GetFile(Context.GetGuidArg(Constants.Id).Value);
			});
		}


		protected override void Form(LayoutWriter w)
		{
			w.FieldsBlockStd(() => {
				w.FormFieldTextBox(meta.Title, File.Name);
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
			File.Name = FormData.Parse<string>(meta.Title.Name);
			File.Extension = Path.GetExtension(File.Name).ToLower();

			if (IsTxt)
			{
				File.WriteAllText(FormData.Parse<string>("data"));
			}
		}

		protected override void ValidateFormData(ValidationMessageCollection val)
		{
			val.Check(Resources, meta.Title, FormData).NotEmpty();
		}

		
	}
}