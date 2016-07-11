using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Hosting;
using System.IO;

namespace Nephrite.Web.FileStorage
{
	public class TesseraVirtualFile : VirtualFile
	{
		public TesseraVirtualFile(string virtualPath)
			: base(virtualPath)
		{
		}

		public override Stream Open()
		{
			var file = FileStorageManager.GetFile(VirtualPath.StartsWith("/") ? VirtualPath.Substring(1) : VirtualPath);
			if (file == null)
				throw new FileNotFoundException("Файл не найден в файловом хранилище", VirtualPath);

			return new MemoryStream(file.GetBytes());
		}
	}
}
