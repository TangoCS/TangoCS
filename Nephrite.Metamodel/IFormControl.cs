using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Nephrite.Metamodel.Model;

namespace Nephrite.Metamodel
{
	public interface IXMLFormControl
	{
		void SetValue(XElement element);
		XElement GetValue(XElement primaryElement);
		bool HasValue { get; }
	}

	public interface IFormatableXMLFormControl : IXMLFormControl
	{
		XElement GetValue(XElement primaryElement, string format);
	}
}
