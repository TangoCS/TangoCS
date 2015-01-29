using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nephrite.Html;

namespace Tessera3.Views
{
	public class HomePage : HtmlControl
	{
		public override void Render()
		{
			DocType();
			Html(() =>
			{
				Head(() =>
				{
					Title(() => Write("Test"));
					Link((a) => { a.Rel = LinkRel.Stylesheet; a.Type = "text/css"; a.Href = "/css/CORE.CSS"; });
					Link((a) => { a.Rel = LinkRel.Stylesheet; a.Type = "text/css"; a.Href = "/THEMES/BREEZE2/theme.css"; });
					Link((a) => { a.Rel = LinkRel.Stylesheet; a.Type = "text/css"; a.Href = "/css/calendar.CSS"; });
					Link((a) => { a.Rel = LinkRel.Stylesheet; a.Type = "text/css"; a.Href = "/css/datepicker.CSS"; });
					Link((a) => { a.Rel = LinkRel.Stylesheet; a.Type = "text/css"; a.Href = "/css/STAND.CSS"; });
					Link((a) => { a.Rel = LinkRel.Stylesheet; a.Type = "text/css"; a.Href = "/css/home.CSS"; });
					Link((a) => { a.Rel = LinkRel.Stylesheet; a.Type = "text/css"; a.Href = "/css/Tessera.CSS"; });
					Meta((a) => { a.HttpEquiv = "content-type"; a.Content = "text/html; charset=utf-8"; });
				});
				Body((a) => { a.Style = "overflow:hidden;"; }, () =>
				{
					Form((a) => a.ID = "form1", () =>
					{
						Table((a) => { a.Style = "border-collapse:collapse; margin:0; left:0px; top:0px; width:100%; height:100%;position: fixed;//position: absolute;"; }, () =>
						{
							Tr(() =>
							{
								Td((a) => { a.ColSpan = 2; a.Style = "height:1px"; }, () =>
								{
									Write("HomeHeader");
								});
							});
							Tr(() =>
							{
								Td((a) => { a.Class = "ms-leftareacell ms-nav"; a.Style = "width: 150px; padding: 0px 3px 0px 2px"; }, () =>
								{
									Write("Navigation");
								});
								Td((a) => { a.Class = "ms-WPBody"; a.Style = "vertical-align: top; border-bottom: solid 1px #6F9DD9; border-left: solid 1px #6F9DD9; border-right: solid 1px #6F9DD9; width:100%"; }, () =>
								{
									Write("WorkAreaTitle");
									Div((a) => { a.Class = "ms-bodyareacell"; a.ID = "wsdiv"; a.Style = "overflow-x: auto; overflow-y:auto; position: relative; display:block"; }, () =>
									{
										Write("WorkArea");
									});
								});
							});
						});
					});
				});
			});
		}
	}
}