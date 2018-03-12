using System;
using System.Reflection;
using Tango.Html;
using Tango.UI;

namespace Tango.FileStorage
{
	public class FileController : Controller
	{
		[HttpGet]
		public virtual ActionResult Get(Guid id)
		{
			ActionResult renderMessage(string message)
			{
				var w = new HtmlWriter();
				w.DocType();
				w.Html(() => {
					w.Head(() => {
						w.HeadTitle();
						w.HeadMeta(a => a.HttpEquiv("content-type").Content("text/html; charset=utf-8"));
					});
					w.Body(() => w.Write(message));
				});
				return new HtmlResult(w.ToString(), "");
			}

			var folder = Context.RequestServices.GetService(typeof(IStorageFolder<Guid>)) as IStorageFolder<Guid>;
			if (folder == null)
				return renderMessage("Нет удалось получить доступ к файловому хранилищу");

			var file = folder.GetFile(id);
			return file == null ?
				renderMessage("Файл был удален, не существует или недостаточно полномочий.") :
				new FileStreamResult(file);
		}

		public override bool CheckAccess(MethodInfo method)
		{
			return true;
		}
	}

	public class FileStreamResult : HttpResult
	{
		public FileStreamResult(IStorageFile file)
		{
			Headers.Add("content-disposition", "Attachment; FileName=\"" + Uri.EscapeDataString(file.Name) + "\"");
			ContentType = MimeMapping.GetMimeType(file.Extension);
			ContentFunc = ctx => file.ReadAllBytes();
		}
	}
}
