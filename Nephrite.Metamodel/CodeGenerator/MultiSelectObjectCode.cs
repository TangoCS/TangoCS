using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nephrite.Metamodel.Model;
using System.Text;
using Nephrite.Web;

namespace Nephrite.Metamodel
{
	internal class MultiSelectObjectHierarchicCode : ControlCode
	{
		string controlID = "";
		//MM_FormFieldAttribute ffa_class;
		MM_ObjectType ot = null;
		MM_FormFieldAttribute ffa_dtf;
		MM_FormFieldAttribute ffa_filter;
		MM_FormFieldAttribute ffa_se;
		string table = "";
		string dvf = "";
		string idCol;

		public MultiSelectObjectHierarchicCode(MM_FormField field)
			: base(field)
		{
			controlID = field.MM_ObjectProperty.SysName;
			idCol = _field.MM_ObjectProperty.RefObjectType.MM_ObjectProperties.Single(o => o.IsPrimaryKey).SysName;
			ffa_dtf = _field.MM_FormFieldAttributes.SingleOrDefault(o => o.Title == "DataTextField");
			ffa_filter = _field.MM_FormFieldAttributes.SingleOrDefault(o => o.Title == "Filter");
			ffa_se = _field.MM_FormFieldAttributes.SingleOrDefault(o => o.Title == "SearchExpression");
			ot = _field.MM_ObjectProperty.RefObjectType;
			table = ot.SysName;
			if (!String.IsNullOrEmpty(ffa_filter.Value))
				table += ".Where(" + ffa_filter.Value + ")";
			dvf = ot.MM_ObjectProperties.Single(o => o.IsPrimaryKey).SysName;
		}
		public MultiSelectObjectHierarchicCode(MM_FormField field, string prefix)
			: base(field, prefix)
		{
			controlID = prefix + field.MM_ObjectProperty.SysName;
			idCol = _field.MM_ObjectProperty.RefObjectType.MM_ObjectProperties.Single(o => o.IsPrimaryKey).SysName;
			ffa_dtf = _field.MM_FormFieldAttributes.SingleOrDefault(o => o.Title == "DataTextField");
			ffa_filter = _field.MM_FormFieldAttributes.SingleOrDefault(o => o.Title == "Filter");
			ffa_se = _field.MM_FormFieldAttributes.SingleOrDefault(o => o.Title == "SearchExpression");
			ot = _field.MM_ObjectProperty.RefObjectType;
			table = ot.SysName;
			if (!String.IsNullOrEmpty(ffa_filter.Value))
				table += ".Where(" + ffa_filter.Value + ")";
			dvf = ot.MM_ObjectProperties.Single(o => o.IsPrimaryKey).SysName;
		}

		public override string Control()
		{
			
			return @"<%=Html.InternalActionLink(select" + controlID + @".RenderRun(), ""Выбрать объекты"", ""list.gif"", true)%>
                <asp:UpdatePanel runat=""server"" ID=""up" + controlID + @""" UpdateMode=""Conditional"">
                <ContentTemplate>
                <% int i" + controlID + @" = 0;
                  foreach (int id in _" + controlID + @")
                {
                               var o = App.DataContext." + _field.MM_ObjectProperty.RefObjectType.SysName + @".Single(o1 => o1." + idCol + @" == id);
                %>
                <%=o.Title%>
                <a href=""#"" onclick=""return Delete" + controlID + @"('<%=o.ObjectID%>');"">
                <img src=""/_layouts/images/n/delete.gif"" alt=""Удалить"" class=""middle"" /></a><%if (i" + controlID + @" != _" + controlID + @".Count - 1) { %><br /> <%} %>
                <% i" + controlID + @"++;
                }%>
                <asp:LinkButton ID=""bDelete" + controlID + @""" runat=""server"" OnClick=""bDelete" + controlID + @"_Click"" Text=""""/>
                <input type=""hidden"" id=""h" + controlID + @"ID"" name=""h" + controlID + @"ID"" runat=""server"" />

                </ContentTemplate>
                </asp:UpdatePanel>";
		}

		public override string RootLayout()
		{
			return @"<nw:SelectObjectHierarchic ID=""select" + controlID + @""" MultipleSelect=""true"" Height=""Auto"" DataTextField=""" + ffa_dtf.Value + @"""
 Title=""Выберите объекты"" runat=""server"" PageSize=""15"" OnSelected=""select" + controlID + @"_Selected"" />
<script type=""text/javascript"">
function Delete" + controlID + @"(id)
{
                document.getElementById(""<%= h" + controlID + @"ID.ClientID %>"").value = id;
                <%= Page.ClientScript.GetPostBackEventReference(bDelete" + controlID + @", """", true) %>;
                return false;
}
</script>";
		}

		string ClassName(MM_ObjectType ot)
		{
			if (ot.MM_ObjectProperties.Any(o => o.IsMultilingual))
				return "V_" + table + ".Where(o => o.LanguageCode == Nephrite.Metamodel.AppMM.CurrentLanguage.LanguageCode)";
			return ot.SysName;
		}

		public override string ViewOnLoad()
		{
			string sort = ot.DefaultOrderBy ?? "o => o.Title";
			if (!String.IsNullOrEmpty(sort) && !sort.ToLower().StartsWith("orderby")) sort = "OrderBy(" + sort + ")";
			StringBuilder res = new StringBuilder();
			res.AppendLine("select" + controlID + ".AllObjects = App.DataContext." + ClassName(ot) + (String.IsNullOrEmpty(sort) ? "" : ("." + sort)) + ";");
			if (!String.IsNullOrEmpty(ffa_se.Value))
				res.AppendLine("select" + controlID + ".SearchExpression = " + ffa_se.Value + ";");
			res.AppendLine("select" + controlID + ".Type = typeof(" + ot.SysName + ");");
			return res.ToString();
		}

		public override string Load(string var)
		{
			return String.Format("_{0} = ViewData.{0}.Select(o => o.{1}).ToList();", controlID, idCol);
		}

		public override string SetValue(string value)
		{
			return "_" + controlID + " = " + value + ";";
		}

		public override string Save(string var)
		{
			return @"foreach(var id in _" + controlID + @")
                {
                               ViewData." + controlID + @".Add(App.DataContext." + _field.MM_ObjectProperty.RefObjectType.SysName + @".Single(o => o." + idCol + @" == id));
                }
                foreach(var obj in ViewData." + controlID + @".ToList())
                {
                               if (!_" + controlID + @".Contains(obj.ObjectID))
                                               ViewData." + controlID + @".Remove(obj);
                }";
		}

		public override string Events()
		{
			return @"protected List<int> _" + controlID + @"
{
                get { if (ViewState[""_" + controlID + @"""] != null) return ViewState[""_" + controlID + @"""] as List<int>; else return new List<int>();}
                set {       ViewState[""_" + controlID + @"""] = value; }
}
protected void select" + controlID + @"_Selected(object sender, SelectObjectHierarchicEventArgs e)
{
                foreach(var id in e.ObjectIDs)
                               if (!_" + controlID + @".Contains(id)) {_" + controlID + @".Add(id);}
                foreach(var id in _" + controlID + @".ToList())
                               if (!e.ObjectIDs.Contains(id)) {_" + controlID + @".Remove(id);}

                up" + controlID + @".Update();
}
protected void bDelete" + controlID + @"_Click(object sender, EventArgs e)
{
                int id = int.Parse(h" + controlID + @"ID.Value);
                if (_" + controlID + @".Contains(id)) _" + controlID + @".Remove(id);
}";
		}

		public override string SetDefaultOrEmpty()
		{
			return "_" + controlID + " = new List<int>()";
		}

		public override string ValidateRequired()
		{
			return "if (_" + controlID + ".Count == 0) requiredMsg += \"<li>" +
				(_field.FormFieldGroupID.HasValue ? (_field.MM_FormFieldGroup.Title + " \\\\ ") : "") +
				_field.MM_ObjectProperty.Title + "</li>\";";
		}


	}


	internal class MultiSelectObjectHierarchicTextCode : ControlCode
	{
		string controlID = "";
		//MM_FormFieldAttribute ffa_class;
		MM_ObjectType ot = null;
		MM_FormFieldAttribute ffa_dtf;
		MM_FormFieldAttribute ffa_dvf;
		MM_FormFieldAttribute ffa_c;

		MM_FormFieldAttribute ffa_filter;
		MM_FormFieldAttribute ffa_se;
		string table = "";
		string dvf = "";
		string idCol;

		public MultiSelectObjectHierarchicTextCode(MM_FormField field)
			: base(field)
		{
			controlID = field.MM_ObjectProperty.SysName;
			
			ffa_dtf = _field.MM_FormFieldAttributes.SingleOrDefault(o => o.Title == "DataTextField");
			ffa_dvf = _field.MM_FormFieldAttributes.SingleOrDefault(o => o.Title == "DataValueField");
			ffa_c = _field.MM_FormFieldAttributes.SingleOrDefault(o => o.Title == "Class");
			ot = AppMM.DataContext.MM_ObjectTypes.Single(o => o.SysName == ffa_c.Value);
			idCol = ot.MM_ObjectProperties.Single(o => o.IsPrimaryKey).SysName;

			ffa_filter = _field.MM_FormFieldAttributes.SingleOrDefault(o => o.Title == "Filter");
			ffa_se = _field.MM_FormFieldAttributes.SingleOrDefault(o => o.Title == "SearchExpression");
			
			table = ot.SysName;
			if (!String.IsNullOrEmpty(ffa_filter.Value))
				table += ".Where(" + ffa_filter.Value + ")";
			dvf = ot.MM_ObjectProperties.Single(o => o.IsPrimaryKey).SysName;
		}
		public MultiSelectObjectHierarchicTextCode(MM_FormField field, string prefix)
			: base(field, prefix)
		{
			
			ffa_dtf = _field.MM_FormFieldAttributes.SingleOrDefault(o => o.Title == "DataTextField");
			ffa_filter = _field.MM_FormFieldAttributes.SingleOrDefault(o => o.Title == "Filter");
			ffa_se = _field.MM_FormFieldAttributes.SingleOrDefault(o => o.Title == "SearchExpression");
			ffa_dvf = _field.MM_FormFieldAttributes.SingleOrDefault(o => o.Title == "DataValueField");
			ffa_c = _field.MM_FormFieldAttributes.SingleOrDefault(o => o.Title == "Class");
			ot = AppMM.DataContext.MM_ObjectTypes.Single(o => o.SysName == ffa_c.Value);
			controlID = prefix + field.MM_ObjectProperty.SysName;
			idCol = ot.MM_ObjectProperties.Single(o => o.IsPrimaryKey).SysName;
			

			table = ot.SysName;
			if (!String.IsNullOrEmpty(ffa_filter.Value))
				table += ".Where(" + ffa_filter.Value + ")";
			dvf = ot.MM_ObjectProperties.Single(o => o.IsPrimaryKey).SysName;
		}

		public override string Control()
		{

			return @"<%=Html.InternalActionLink(select" + controlID + @".RenderRun(), ""Выбрать объекты"", ""list.gif"", true)%>
                <asp:UpdatePanel runat=""server"" ID=""up" + controlID + @""" UpdateMode=""Conditional"">
                <ContentTemplate>
                <% int i" + controlID + @" = 0;
                  foreach (int id in _" + controlID + @")
                {
                               var o = App.DataContext." + ot.SysName + @".Single(o1 => o1." + idCol + @" == id);
                %>
                <%=o.Title%>
                <a href=""#"" onclick=""return Delete" + controlID + @"('<%=o.ObjectID%>');"">
                <img src=""/_layouts/images/n/delete.gif"" alt=""Удалить"" class=""middle"" /></a><%if (i" + controlID + @" != _" + controlID + @".Count - 1) { %><br /> <%} %>
                <% i" + controlID + @"++;
                }%>
                <asp:LinkButton ID=""bDelete" + controlID + @""" runat=""server"" OnClick=""bDelete" + controlID + @"_Click"" Text=""""/>
                <input type=""hidden"" id=""h" + controlID + @"ID"" name=""h" + controlID + @"ID"" runat=""server"" />

                </ContentTemplate>
                </asp:UpdatePanel>";
		}

		public override string RootLayout()
		{
			return @"<nw:SelectObjectHierarchic ID=""select" + controlID + @""" MultipleSelect=""true"" Height=""Auto"" DataTextField=""" + ffa_dtf.Value + @"""
 Title=""Выберите объекты"" runat=""server"" PageSize=""15"" OnSelected=""select" + controlID + @"_Selected"" />
<script type=""text/javascript"">
function Delete" + controlID + @"(id)
{
                document.getElementById(""<%= h" + controlID + @"ID.ClientID %>"").value = id;
                <%= Page.ClientScript.GetPostBackEventReference(bDelete" + controlID + @", """", true) %>;
                return false;
}
</script>";
		}

		string ClassName(MM_ObjectType ot)
		{
			if (ot.MM_ObjectProperties.Any(o => o.IsMultilingual))
				return "V_" + table + ".Where(o => o.LanguageCode == Nephrite.Metamodel.AppMM.CurrentLanguage.LanguageCode)";
			return ot.SysName;
		}

		public override string ViewOnLoad()
		{
			string sort = ot.DefaultOrderBy ?? "o => o.Title";
			if (!String.IsNullOrEmpty(sort) && !sort.ToLower().StartsWith("orderby")) sort = "OrderBy(" + sort + ")";
			StringBuilder res = new StringBuilder();
			res.AppendLine("select" + controlID + ".AllObjects = " + AppMM.DBName() + ".App.DataContext." + ClassName(ot) + (String.IsNullOrEmpty(sort) ? "" : ("." + sort)) + ";");
			if (!String.IsNullOrEmpty(ffa_se.Value))
				res.AppendLine("select" + controlID + ".SearchExpression = " + ffa_se.Value + ";");
			res.AppendLine("select" + controlID + ".Type = typeof(" + ot.SysName + ");");
			return res.ToString();
		}

		public override string Load(string var)
		{
			return "_" + controlID + " = !String.IsNullOrEmpty(ViewData." + controlID + ") ? ViewData." + controlID + ".Split(new char[] {';'}).Where(o => !String.IsNullOrEmpty(o.Trim())).Select(o => o.ToInt32(0)).ToList() : new List<int>();";
		}

		public override string SetValue(string value)
		{
			return "_" + controlID + " = " + value + ";";
		}

		public override string Save(string var)
		{
			return "ViewData." + controlID + " = _" + controlID + ".Select(o => o.ToString()).Join(\";\") + \";\";";
		}

		public override string Events()
		{
			return @"protected List<int> _" + controlID + @"
{
                get { if (ViewState[""_" + controlID + @"""] != null) return ViewState[""_" + controlID + @"""] as List<int>; else return new List<int>();}
                set {       ViewState[""_" + controlID + @"""] = value; }
}
protected void select" + controlID + @"_Selected(object sender, SelectObjectHierarchicEventArgs e)
{
                foreach(var id in e.ObjectIDs)
                               if (!_" + controlID + @".Contains(id)) {_" + controlID + @".Add(id);}
                foreach(var id in _" + controlID + @".ToList())
                               if (!e.ObjectIDs.Contains(id)) {_" + controlID + @".Remove(id);}

                up" + controlID + @".Update();
}
protected void bDelete" + controlID + @"_Click(object sender, EventArgs e)
{
                int id = int.Parse(h" + controlID + @"ID.Value);
                if (_" + controlID + @".Contains(id)) _" + controlID + @".Remove(id);
}";
		}

		public override string SetDefaultOrEmpty()
		{
			return "_" + controlID + " = new List<int>()";
		}

		public override string ValidateRequired()
		{
			return "if (_" + controlID + ".Count == 0) requiredMsg += \"<li>" +
				(_field.FormFieldGroupID.HasValue ? (_field.MM_FormFieldGroup.Title + " \\\\ ") : "") +
				_field.MM_ObjectProperty.Title + "</li>\";";
		}


	}



	internal class MultiObjectCode : ControlCode
	{
		string controlID = "";
		//MM_FormFieldAttribute ffa_class;
		MM_ObjectType ot = null;
		MM_ObjectType otowner = null;
		MM_FormFieldAttribute ffa_dtf;
		MM_FormFieldAttribute ffa_filter;
		MM_FormFieldAttribute ffa_se;
		string table = "";
		string dvf = "";

		public MultiObjectCode(MM_FormField field)
			: base(field)
		{
			controlID = "mo" + field.MM_ObjectProperty.SysName;
			otowner = field.MM_ObjectProperty.MM_ObjectType;
			//ffa_class = _field.MM_FormFieldAttributes.SingleOrDefault(o => o.Title == "Class");
			ffa_dtf = _field.MM_FormFieldAttributes.SingleOrDefault(o => o.Title == "DataTextField");
			ffa_filter = _field.MM_FormFieldAttributes.SingleOrDefault(o => o.Title == "Filter");
			ffa_se = _field.MM_FormFieldAttributes.SingleOrDefault(o => o.Title == "SearchExpression");
			MM_ObjectType ot = _field.MM_ObjectProperty.RefObjectType;
			table = ot.SysName;
			if (!String.IsNullOrEmpty(ffa_filter.Value))
				table += ".Where(" + ffa_filter.Value + ")";
			dvf = ot.MM_ObjectProperties.Single(o => o.IsPrimaryKey).SysName;
		}
		public MultiObjectCode(MM_FormField field, string prefix)
			: base(field, prefix)
		{
			controlID = "mo" + prefix + field.MM_ObjectProperty.SysName;
			otowner = field.MM_ObjectProperty.MM_ObjectType;
			//ffa_class = _field.MM_FormFieldAttributes.SingleOrDefault(o => o.Title == "Class");
			ffa_dtf = _field.MM_FormFieldAttributes.SingleOrDefault(o => o.Title == "DataTextField");
			ffa_filter = _field.MM_FormFieldAttributes.SingleOrDefault(o => o.Title == "Filter");
			ffa_se = _field.MM_FormFieldAttributes.SingleOrDefault(o => o.Title == "SearchExpression");
			ot = _field.MM_ObjectProperty.RefObjectType;
			table = ot.SysName;
			if (!String.IsNullOrEmpty(ffa_filter.Value))
				table += ".Where(" + ffa_filter.Value + ")";
			dvf = ot.MM_ObjectProperties.Single(o => o.IsPrimaryKey).SysName;
		}

		public override string Control()
		{

			return @"<nw:MultiObject runat=""server"" ID=""" + controlID + @""" />";
		}

		public override string ViewOnLoad()
		{
			string sort = ot.DefaultOrderBy ?? "o => o.Title";
			if (!sort.ToLower().StartsWith("orderby")) sort = "OrderBy(" + sort + ")";
			StringBuilder res = new StringBuilder();
			res.AppendLine(controlID + ".AllObjects = App.DataContext." + ClassName(ot) + "." + sort + ";");
			if (!String.IsNullOrEmpty(ffa_se.Value))
				res.AppendLine(controlID + ".SearchExpression = " + ffa_se.Value + ";");
			res.AppendLine("\t" + controlID + ".Type = typeof(" + ot.SysName + ");");
			return res.ToString();
		}

		public override string SetValue(string value)
		{
			return controlID + ".Text = " + value + ";";
		}

		string ClassName(MM_ObjectType ot)
		{
			if (ot.MM_ObjectProperties.Any(o => o.IsMultilingual))
				return "V_" + table + ".Where(o => o.LanguageCode == Nephrite.Metamodel.AppMM.CurrentLanguage.LanguageCode)";
			return ot.SysName;
		}

		public override string Load(string var)
		{
			return controlID + ".LoadObjects<" + ot.SysName + ">(" + var + "." + _field.MM_ObjectProperty.SysName + ");";
		}

		public override string Save(string var)
		{
			StringBuilder res = new StringBuilder();
			
            res.AppendLine("\t" + var + "." + _field.MM_ObjectProperty.SysName + ".Sync(" + controlID + ".GetSelected<" +  ot.SysName + ">())");
			

			return res.ToString();
		}

		public override string SetDefaultOrEmpty()
		{
			if (!String.IsNullOrEmpty(_field.DefaultValue))
				return "";
			else
			{
				return controlID + ".Clear()";
			}
		}

		public override string ValidateRequired()
		{
			return "if (" + controlID + ".ObjectIDs.Count() == 0) requiredMsg += \"<li>" +
				(_field.FormFieldGroupID.HasValue ? (_field.MM_FormFieldGroup.Title + " \\\\ ") : "") +
				_field.MM_ObjectProperty.Title + "</li>\";";
		}
	}
}
