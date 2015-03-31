using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using System.ComponentModel;

namespace Nephrite.Web.Controls
{
	[ParseChildren(true)]
	[PersistChildren(false)]
	public class TabPage : System.Web.UI.UserControl, INamingContainer
	{
		protected void Page_Init(object sender, EventArgs e)
		{
			if (_content != null)
			{
				_contentInstance = new Control();
				_content.InstantiateIn(_contentInstance);
				Controls.Add(_contentInstance);
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{

		}

		Control _contentInstance;
		ITemplate _content = null;

		[TemplateInstance(TemplateInstance.Single)]
		[Browsable(false)]
		public virtual ITemplate ContentTemplate
		{
			get { return _content; }
			set { _content = value; }
		}

		public string Title { get; set; }
		public string Image { get; set; }
		public TabControl Owner { get; set; }
	}
}