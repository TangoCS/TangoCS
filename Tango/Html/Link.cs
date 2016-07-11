using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tango.Html
{
	public interface ILink
	{
		string Title { get; }
		string Href { get; }
		string Description { get; }
		string Image { get; }
		string OnClick { get; }
		bool TargetBlank { get; }
		string AccessKey { get; }
		object Attributes { get; }
	}

	public class SimpleLink : ILink
	{
		public string Title { get; set; }
		public string Description { get; set; }
		public string Image { get; set; }
		public string OnClick { get; set; }
		public bool TargetBlank { get; set; }
		public string AccessKey { get; set; }
		public object Attributes { get; set; }
		public string Href { get; set; }
	}
}
