using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using Nephrite.Web;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Nephrite.Meta;
using Nephrite.Web.CoreDataContext;
using Nephrite.Web.RSS;
using Nephrite.Web.MetaStorage;

namespace Nephrite.Web.FormsEngine
{
    public class FormsVirtualFile : VirtualFile
    {
        public FormsVirtualFile(string virtualPath) : base(virtualPath)
        {
        }

		public override Stream Open()
		{
			Stream stream = new MemoryStream();
			StreamWriter writer = new StreamWriter(stream, Encoding.UTF8);

			StringBuilder text = new StringBuilder();

			if (VirtualPath.ToLower().EndsWith(".rss.ascx"))
			{
				IN_RssFeed rf = (A.Model as IDC_RSS).IN_RssFeed.Single(o => o.SysName.ToLower() == VirtualPath.ToLower().Replace("/", "").Replace(".rss.ascx", ""));
				MetaClass mc = A.Meta.GetClass(rf.ObjectTypeSysName);
				text.AppendFormat(@"<%@ Control Language=""C#"" Inherits=""System.Web.UI.UserControl"" %>
					<%@ Import Namespace=""{0}""%>
					<%@ Import Namespace=""{0}.Controllers""%>
					<%@ Import Namespace=""{0}.Model""%>
					<%@ Import Namespace=""System.Data.Linq.SqlClient""%>", AppWeb.AppNamespace);

				text.AppendLine("<script runat=\"server\">");
				text.AppendLine("protected void Page_Load(object sender, EventArgs e)");
				text.AppendLine("{");
				text.AppendLine("    var feed = new Nephrite.Web.RSS.RssFeed();");
				text.AppendLine("    feed.Title = \"" + rf.Title.Replace("\"", "\\\"") + "\";");
				text.AppendLine("    feed.Link = Request.Url.ToString();");
				text.AppendLine("    feed.Description = \"" + rf.Description.Replace("\"", "\\\"") + "\";");
				text.AppendLine("    feed.Copyright = \"" + rf.Description.Replace("\"", "\\\"") + "\";");
				text.AppendLine("    feed.Ttl = " + rf.Ttl.ToString() + ";");
				text.AppendLine("    feed.WebMaster = \"" + rf.WebMaster + "\";");
				text.AppendLine("    Func<" + rf.ObjectTypeSysName + ", string> author = " + rf.Author + ";");
				text.AppendLine("    foreach (var o in App.DataContext." + rf.ObjectTypeSysName + rf.Predicate + ".ToList())");
				text.AppendLine("    {");
				text.AppendLine("        Nephrite.Metamodel.MView mv = new Nephrite.Metamodel.MView();");
				text.AppendLine("        Controls.Add(mv);");
				text.AppendLine("        mv.ViewFormSysName = \"" + rf.ViewFormSysName + "\";");
				text.AppendLine("        mv.SetViewData(o);");
				text.AppendLine("        System.IO.StringWriter sw = new System.IO.StringWriter();");
				text.AppendLine("        System.Web.UI.HtmlTextWriter tw = new System.Web.UI.HtmlTextWriter(sw);");
				text.AppendLine("        mv.RenderControl(tw);");
				text.AppendLine("        string descr = sw.ToString();");
				text.AppendLine("        feed.Items.Add(new Nephrite.Web.RSS.RssItem");
				text.AppendLine("        {");
				text.AppendLine("            Title = o.Title,");
				text.AppendLine("            Link = (new Uri(HttpContext.Current.Request.Url, HtmlHelperBase.Instance.ActionUrl<" + rf.ObjectTypeSysName + "Controller>(c => c.SiteView(o." + mc.Key.Name + ", Query.CreateReturnUrl())))).ToString()" + (String.IsNullOrEmpty(rf.LinkParams) ? "" : " + \"&" + rf.LinkParams + "\"") + ",");
				text.AppendLine("            Description = descr,");
				text.AppendLine("            Guid = (new Uri(HttpContext.Current.Request.Url, HtmlHelperBase.Instance.ActionUrl<" + rf.ObjectTypeSysName + "Controller>(c => c.SiteView(o." + mc.Key.Name + ", Query.CreateReturnUrl())))).ToString()" + (String.IsNullOrEmpty(rf.LinkParams) ? "" : " + \"&" + rf.LinkParams + "\"") + ",");
				text.AppendLine("            Author = author(o),");
				text.AppendLine("            PubDate = o." + rf.PubDate + "");
				text.AppendLine("        });");
				text.AppendLine("    }");
				text.AppendLine("    feed.Render();");
				text.AppendLine("}");
				text.AppendLine("</script>");

				writer.Write(text.ToString());
			}
			else if (VirtualPath.StartsWith(CustomControlManager.Path))
			{
				string key = VirtualPath.Substring(CustomControlManager.Path.Length + 1).Replace(".ascx", "");
				text.AppendFormat(@"<%@ Control Language=""C#"" Inherits=""Nephrite.Metamodel.CustomControlBase"" %><%@ Import Namespace=""{0}""%><%@ Import Namespace=""{0}.Model""%><%@ Import Namespace=""{0}.Controllers""%><%@ Import Namespace=""Nephrite.Web.Controls""%><%@ Import Namespace=""System.Data.Linq.SqlClient""%>", AppWeb.AppNamespace);
				text.AppendFormat("<%var ViewData = ({0})viewData;%>", CustomControlManager.GetViewDataType(key).FullName.Replace("+", "."));
				text.Append(Regex.Replace(CustomControlManager.GetText(key), "&lt;%.*%&gt;", Preparer, RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace));
				writer.Write(text.ToString());
			}
			else if (VirtualPath.EndsWith(".ascx.cs"))
			{
				var f = WebSiteCache.GetView(VirtualPath.Substring(0, VirtualPath.Length - 3));
				IMM_ObjectType objectType = ((IDC_MetaStorage)A.Model).IMM_ObjectType.FirstOrDefault(o => o.ObjectTypeID == f.ObjectTypeID);
				string viewclassname = (objectType != null ? objectType.SysName + "_" : "") + f.SysName;
				string basetype = String.Format(f.BaseClass, objectType != null ? objectType.SysName : "").Replace("<>", "");
				text.AppendFormat(@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using {0};
using {0}.Model;
using Nephrite.Web;
using Nephrite.Metamodel;
namespace {0}.ViewControls
{{
	public partial class {1} : {2}
	{{", AppWeb.AppNamespace, viewclassname, basetype);
				string baseClass = String.Format(f.BaseClass, objectType != null ? objectType.SysName : "");
				if (objectType != null && (baseClass.EndsWith("ViewControl<IQueryable<" + objectType.SysName + ">>") || baseClass.EndsWith("ViewControl<IQueryable<V_" + objectType.SysName + ">>") ||
					baseClass.EndsWith("ViewControl<IQueryable<HST_" + objectType.SysName + ">>") || baseClass.EndsWith("ViewControl<IQueryable<V_HST_" + objectType.SysName + ">>")))
				{
					text.AppendFormat(@"		public override void SetViewData(object viewData)
		{{
			base.SetViewData(((IQueryable)viewData).Cast<{0}>());
		}}", (objectType.IsMultiLingual ? (objectType.HistoryTypeCode == VersioningType.IdentifiersMiss ||
		   objectType.HistoryTypeCode == VersioningType.IdentifiersRetain ? "V_HST_" : "V_") :
		   (objectType.HistoryTypeCode == VersioningType.IdentifiersMiss ||
		   objectType.HistoryTypeCode == VersioningType.IdentifiersRetain ? "HST_" : "")) + objectType.SysName);
				}
				text.AppendFormat(@"		public override void RenderControl(HtmlTextWriter writer)
		{{
			bool showviewlinks = Url.Current.GetString(""showviews"") != """";
			if (showviewlinks)
				writer.Write(""<div style='border: 2px solid magenta; padding:1px; margin:0px;'><div style='float:left; border: 1px solid red; padding:3px; background-color:white;'><a target='_blank' title='" + HttpUtility.HtmlEncode(f.FullTitle) + "' href='/?mode=MM_FormView&action=Edit&oid=" + f.FormViewID.ToString() + @"'>edit</a></div>"");
			base.RenderControl(writer);
			if (showviewlinks)
				writer.Write(""</div>"");
		}}
	}}
}}");
				writer.Write(text.ToString());
			}
			else
			{
				var f = WebSiteCache.GetView(VirtualPath);
				if (f.TemplateTypeCode == TemplateType.Ashx || f.TemplateTypeCode == TemplateType.Asmx || f.TemplateTypeCode == TemplateType.Svc)
				{
					writer.Write(f.ViewTemplate);
				}
				else
				{
					string viewclassname = f.SysName;
					if (f.ObjectTypeID.HasValue)
						viewclassname = f.MM_ObjectType.SysName + "_" + viewclassname;
					if (f.TemplateTypeCode == TemplateType.Aspx)
						text.AppendFormat(@"<%@ Page Language=""C#"" AutoEventWireup=""true"" ValidateRequest=""false"" %><%@ Import Namespace=""{0}""%><%@ Import Namespace=""{0}.Model""%><%@ Import Namespace=""{0}.Controllers""%><%@ Import Namespace=""Nephrite.Web.Controls""%><%@ Import Namespace=""System.Data.Linq.SqlClient""%><%-- End of autogenerated header, do not remove this line --%>",
						AppWeb.AppNamespace, viewclassname);
					else
						text.AppendFormat(@"<%@ Control Language=""C#"" Inherits=""{0}.ViewControls.{1}"" CodeFile=""{2}.ascx.cs""  %><%@ Import Namespace=""{0}""%><%@ Import Namespace=""{0}.Model""%><%@ Import Namespace=""{0}.Controllers""%><%@ Import Namespace=""Nephrite.Web.Controls""%><%@ Import Namespace=""System.Data.Linq.SqlClient""%><%-- End of autogenerated header, do not remove this line --%>",
							AppWeb.AppNamespace, viewclassname, f.SysName);
					text.Append(f != null ? f.ViewTemplate : String.Empty);

					writer.Write(text.ToString());
				}
			}
			writer.Flush();
			stream.Seek(0, SeekOrigin.Begin);

			return stream;
		}

		string Preparer(Match m)
		{
			return HttpUtility.HtmlDecode(m.Value);
		}
    }
}
