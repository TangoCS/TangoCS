using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Nephrite.Web.Controllers;
using System.Linq.Expressions;
using Nephrite.Web.SettingsManager;

namespace Nephrite.Web.Controls
{
	public partial class InputMultiStrings : System.Web.UI.UserControl
	{
		protected List<String> _data
		{
			get
			{
				return ViewState["_data"] as List<String>;
			}
			set
			{
				ViewState["_data"] = value;
			}
		}

		public List<string> Data
		{
			get
			{
				return _data;
			}
		}

		public void AddItem(string data)
		{
			if (_data == null) _data = new List<String>();
			if (String.IsNullOrEmpty(data)) return;
			_data.Add(data);
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			bAddString.ImageUrl = Settings.ImagesPath + "addrow.png";
		}

		protected void bAddString_Click(object sender, EventArgs e)
		{
			AddItem(tbInput.Text);
			tbInput.Text = "";
		}

		protected void bDeleteString_Click(object sender, EventArgs e)
		{
			_data.RemoveAt(plm.Value.ToInt32(0));
		}

		protected void bUpString_Click(object sender, EventArgs e)
		{
            _data.Reverse(plm_up.Value.ToInt32(0) - 1, 2);
		}

		protected void bDownString_Click(object sender, EventArgs e)
		{
            _data.Reverse(plm_down.Value.ToInt32(0), 2);
		}
	}
}