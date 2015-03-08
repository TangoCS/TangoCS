using System;
using System.Collections.Generic;
using System.Linq;

namespace Nephrite.Layout
{
	public class WinModal : ILayoutModal
	{
		string _id = "", _width = "", _top = "";

		public string ModalBegin(string clientID, string width, string top)
		{
			_id = clientID;
			_width = width;
			_top = top;

			return String.Format(@"<div id=""{0}"" style=""width:{1}; top:{2}; position: absolute; visibility: hidden;""><table border=""0"" style=""width: 100%; border-collapse:collapse"" cellspacing=""0"" cellpadding=""0"">", _id, _width, _top);
		}

		public string ModalEnd()
		{
			return "</table></div>";
		}

		public string ModalHeaderBegin()
		{
			return @"<tr>
	<td style=""vertical-align:top; width:8px;"" class=""windowLT""><div class=""windowLT""></div></td>
	
	<td id=""titleBar"" class=""windowCT"" style=""vertical-align:top; height:24px; width:100%"" titlebar=""titleBar"">
		<table border=""0"" style=""width: 100%; height:24px; border-collapse:collapse"" cellspacing=""0"" cellpadding=""0"">
		<tr>
		<td style=""width:100%; cursor: move; vertical-align: middle;"">";
		}

		public string ModalHeaderEnd()
		{
			return String.Format(@"</td>
		<td style=""vertical-align: top""><div class=""windowBtnClose Canvas"" onclick=""hide{0}();""></div></td>
		</tr>
		</table>
	</td>
	<td style=""vertical-align:top; width:8px;"" class=""windowRT""><div class=""windowRT""></div></td>
	</tr>", _id);
		}

		public string ModalBodyBegin()
		{
			return @"<tr>
	<td class=""windowLC""></td>
	<td style=""background-color:White; padding: 5px; border-top:1px solid #ABADB3;"">";
		}

		public string ModalBodyEnd()
		{
			return @"";
		}

		public string ModalFooterBegin()
		{
			return @"<table style=""width:100%; padding-top: 5px;"" cellspacing=""0"" cellpadding=""0"" border=""0""><tr>";
		}

		public string ModalFooterEnd()
		{
			return @"</tr></table></td>
	<td class=""windowRC""></td>
	</tr>
	<tr>
	<td style=""vertical-align:bottom; width:8px;"" class=""windowLB""><div class=""windowLB""></div></td>
	<td class=""windowCB""></td>
	<td style=""vertical-align:bottom; width:8px;"" class=""windowRB""><div class=""windowRB""></div></td>
	</tr>";
		}

		public string ModalFooterLeftBegin()
		{
			return @"<td style=""width:90%"">";
		}

		public string ModalFooterLeftEnd()
		{
			return "</td>";
		}

		public string ModalFooterRightBegin()
		{
			return @"<td style=""white-space:nowrap"">";
		}

		public string ModalFooterRightEnd()
		{
			return "</td>";
		}
	}
}