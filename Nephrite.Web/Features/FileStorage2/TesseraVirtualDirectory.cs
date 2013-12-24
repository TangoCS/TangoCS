using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Hosting;

namespace Nephrite.Web.FileStorage
{
	public class TesseraVirtualDirectory : VirtualDirectory
	{
		public TesseraVirtualDirectory(string virtualPath)
			: base(virtualPath)
		{

		}
		public override System.Collections.IEnumerable Children
		{
			get { return null; }
		}

		public override System.Collections.IEnumerable Directories
		{
			get { return null; }
		}

		public override System.Collections.IEnumerable Files
		{
			get { return null; }
		}
	}
}
