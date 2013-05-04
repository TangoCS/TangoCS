using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nephrite.Metamodel.Model;
using System.Text;

namespace Nephrite.Metamodel
{
	public partial class ViewCode
	{
		public static string View(MM_ObjectType objectType)
		{
			string idCol = objectType.MM_ObjectProperties.Single(o => o.IsPrimaryKey).SysName;
			var cols = from f in AppMM.DataContext.MM_FormFields
					   where f.MM_ObjectProperty.ObjectTypeID == objectType.ObjectTypeID
					   && f.ShowInView
					   orderby f.SeqNo
					   select f;

			var ctables = from p in AppMM.DataContext.MM_ObjectProperties
						  where p.ObjectTypeID == objectType.ObjectTypeID &&
						  p.MM_FormField.ShowInView && p.IsAggregate &&
						  p.TypeCode == ObjectPropertyType.Object
						  orderby p.MM_FormField.SeqNo
						  select p;

			var groups = from g in objectType.MM_FormFieldGroups orderby g.SeqNo select g;
			var selectobjectgroups = groups.Where(o => o.SelectObjectPropertyID.HasValue || !String.IsNullOrEmpty(o.SelectObjectClass));
			var nongroupcols = cols.Where(o => !o.FormFieldGroupID.HasValue);

			StringBuilder res = new StringBuilder();
			res.AppendLine(@"<nw:Toolbar ID=""toolbar"" runat=""server"" />");

			if (cols.Count() == 0)
			{
				return "ERROR: Нет ни одного поля, отображаемого в представлении View";
			}
			
			if (nongroupcols.Count() > 0)
			{
				res.AppendLine(@"<%=Layout.FormTableBegin(new {style=""width:700px""})%>");
				foreach (MM_FormField p in nongroupcols)
				{
					res.AppendLine(@"<%=Layout.FormRowBegin(""" + p.MM_ObjectProperty.Title + @""")%>");
					res.AppendLine(CodeGenHelper.GetViewFieldValue(p.MM_ObjectProperty));
					res.AppendLine("<%=Layout.FormRowEnd()%>");
				}
				res.AppendLine("<%=Layout.FormTableEnd() %>");
			}

			foreach (MM_FormFieldGroup g in groups.Where(o => o.MM_FormFields.Count > 0))
			{
				res.AppendLine(@"<%=Layout.GroupTitle(""" + g.Title + @""")%>");
				res.AppendLine(@"<%=Layout.FormTableBegin(new {style=""width:700px""})%>");
				foreach (MM_FormField p in g.MM_FormFields.OrderBy(o => o.SeqNo))
				{
					res.AppendLine(@"<%=Layout.FormRowBegin(""" + p.MM_ObjectProperty.Title + @""")%>");
					res.AppendLine(CodeGenHelper.GetViewFieldValue(p.MM_ObjectProperty));
					res.AppendLine("<%=Layout.FormRowEnd()%>");
				}
				res.AppendLine("<%=Layout.FormTableEnd() %>");
			}

			res.AppendLine("<%=HtmlHelperWSS.FormToolbarBegin() %>");
			res.AppendLine("<%=HtmlHelperWSS.FormToolbarWhiteSpace() %>");
			res.AppendLine("<%=HtmlHelperWSS.FormToolbarItemBegin() %>");
			res.AppendLine(@"<nw:BackButton runat=""server"" ID=""BackButton"" />");
			res.AppendLine("<%=HtmlHelperWSS.FormToolbarItemEnd() %>");
			res.AppendLine("<%=HtmlHelperWSS.FormToolbarEnd() %>");


			// дочерние списки
			foreach (MM_ObjectProperty p in ctables)
			{
				MM_ObjectProperty ppID = p.RefObjectType.MM_ObjectProperties.Single(o => o.IsPrimaryKey);
				var selectobjectctablegroups = 
					(from g in p.RefObjectType.MM_FormFieldGroups orderby g.SeqNo select g).
					Where(o => o.SelectObjectPropertyID.HasValue || !String.IsNullOrEmpty(o.SelectObjectClass));

				res.AppendLine("<%=Layout.GroupTitleBegin() %>");
				res.Append(p.RefObjectType.TitlePlural).
					AppendLine(@"&nbsp;<%=ViewData.IsDeleted ? String.Empty : Html.InternalLink(m" + p.SysName + @".RenderRun(), ""Добавить"", ""additem.png"", true)%>");
				res.AppendLine("<%=Layout.GroupTitleEnd() %>");

				var ctablecols = from f in AppMM.DataContext.MM_FormFields
								 where f.MM_ObjectProperty.ObjectTypeID == p.RefObjectTypeID
								 orderby f.MM_ObjectProperty.SeqNo
								 select f.MM_ObjectProperty;
				var nongroupctablecols = ctablecols.Where(o => !o.MM_FormField.FormFieldGroupID.HasValue);

				res.AppendLine(@"<%=Layout.ListTableBegin(new {style=""width:700px""}) %>");
				res.AppendLine("<%=Layout.ListHeaderBegin() %>");

				foreach (MM_ObjectProperty pp in ctablecols.Where(o => o.MM_FormField.ShowInList))
				{
					res.Append(@"<%=Layout.TH(""").
						Append(String.IsNullOrEmpty(pp.MM_FormField.Title) ? pp.Title : pp.MM_FormField.Title).
						Append(@"""").
						Append(String.IsNullOrEmpty(pp.MM_FormField.ListColumnWidth) ? "" : (@",""" + pp.MM_FormField.ListColumnWidth + @"""")).
						AppendLine(@")%>");
				}
				res.AppendLine(@"<%=Layout.TH(""Действия"", new {style=""width:90px""})%>");
				res.AppendLine("<%=Layout.ListHeaderEnd() %>");
				string deforderby = String.IsNullOrEmpty(p.RefObjectType.DefaultOrderBy) ? "" : (".OrderBy(" + p.RefObjectType.DefaultOrderBy + ")");
				res.AppendLine("<% Html.Repeater(ViewData." + p.SysName + deforderby + @", """", HtmlHelperWSS.CSSClassAlternating, (o, css) => {  %>");
			
				res.AppendLine("<%=Layout.ListRowBegin(o.IsDeleted ? \"deletedItem\": css) %>");

				string linkCol = ctablecols.OrderBy(o => o.MM_FormField.SeqNo).FirstOrDefault(o => o.MM_FormField.ShowInList && (o.TypeCode == ObjectPropertyType.String || o.TypeCode == ObjectPropertyType.Number || o.TypeCode == ObjectPropertyType.Object)).SysName;
				foreach (MM_ObjectProperty pp in ctablecols.Where(o => o.MM_FormField.ShowInList))
				{
					res.Append("<%=Layout.TD(").Append(CodeGenHelper.GetCellValue(p.RefObjectType, pp, ppID, linkCol, true, "m" + p.SysName)).AppendLine(")%>");
				}

				res.AppendLine(@"<%=Layout.TDBegin(new {style=""text-align:center""})%>");
				res.AppendLine("<% if (!o.IsDeleted) { %>");
				if (p.RefObjectType.MM_ObjectProperties.Count(o => o.SysName == "SeqNo") > 0)
				{
					res.AppendLine(@"<%=Html.ActionImage<" + p.RefObjectType.SysName + "Controller>(oc => oc.MoveUp(o." + ppID.SysName + @", Query.CreateReturnUrl()), ""Переместить вверх"", ""arrow_up.png"")%>");
					res.AppendLine(@"<%=Html.ActionImage<" + p.RefObjectType.SysName + "Controller>(oc => oc.MoveDown(o." + ppID.SysName + @", Query.CreateReturnUrl()), ""Переместить вниз"", ""arrow_down.png"")%>");
				}
				res.AppendLine(@"<%=Html.ActionImage<" + p.RefObjectType.SysName + "Controller>(oc => oc.Delete(o." + ppID.SysName + @", Query.CreateReturnUrl()), ""Удалить"", ""delete.gif"")%>");
				res.AppendLine("<% } %>");
				res.AppendLine("<%=Layout.TDEnd()%>");

				res.AppendLine("<%=Layout.ListRowEnd() %>");
				res.AppendLine("<%}); %>");
				res.AppendLine("<%=Layout.ListTableEnd() %>");

				// модальное окно
				res.AppendLine("");
				res.AppendLine(@"<nw:ModalDialog ID=""m" + p.SysName + @""" Top=""150px"" runat=""server"" Width=""600px"" Title=""" + p.Title + @""" OnOKClick=""m" + p.SysName + @"_OKClick"" OnPopulate=""m" + p.SysName + @"_Populate"">");
				res.AppendLine("<ContentTemplate>");

				res.AppendLine(EditForm(p.RefObjectType, "", p.SysName));

				// ObjectList
				foreach (MM_ObjectProperty pp in ctablecols.Where(o => o.MM_FormField.ShowInEdit && o.UpperBound != 1 && o.IsAggregate))
				{
					res.AppendLine("");
					res.AppendLine(@"<nw:ObjectList Type=""" + AppMM.DBName() + ".Model." + pp.RefObjectType.SysName + ", " + AppMM.DBName() + ".Model" +
						@""" ID=""ol" + pp.SysName + @""" runat=""server"" Title=""" + pp.Title +
							@""" AddingTooltip=""Добавить"" OnAddItem=""ol" + pp.SysName +
							@"_AddItem"" OnSaveItem=""ol" + pp.SysName + @"_SaveItem"" OnEditItem=""ol"
							+ pp.SysName + @"_EditItem"">");
					res.AppendLine("<EditTemplate>");


					res.AppendLine(EditForm(pp.RefObjectType, "100%", pp.RefObjectType.SysName));

					res.AppendLine("</EditTemplate>");
					res.AppendLine("</nw:ObjectList>");
					res.AppendLine("");
				}
				res.AppendLine(@"<asp:Label ID=""lMsg" + p.SysName + @""" runat=""server"" Text="""" ForeColor=""Red"" />");

				res.AppendLine("</ContentTemplate>");
				res.AppendLine("</nw:ModalDialog>");
				res.AppendLine("");

				foreach (MM_ObjectProperty pp in ctablecols)
				{
					ControlCode c = ControlCode.Create(pp.MM_FormField, p.SysName);
					if (c != null)
					{
						string s = c.RootLayout();
						if (!String.IsNullOrEmpty(s))
							res.AppendLine(s);
					}
				}
				foreach (MM_FormFieldGroup g in selectobjectctablegroups)
				{
					FieldGroupSelectObject fgc = new FieldGroupSelectObject(g, p.SysName);
					res.AppendLine(fgc.RootLayout());
				}

				foreach (MM_ObjectProperty pp in ctablecols.Where(o => o.MM_FormField.ShowInEdit && o.UpperBound != 1 && o.IsAggregate))
				{
					var olfields = from f in pp.RefObjectType.MM_ObjectProperties
								   where f.MM_FormField.ShowInEdit
								   orderby f.MM_FormField.SeqNo
								   select f.MM_FormField;

					var selectobjectolgroups =
					(from g in pp.RefObjectType.MM_FormFieldGroups orderby g.SeqNo select g).
					Where(o => o.SelectObjectPropertyID.HasValue || !String.IsNullOrEmpty(o.SelectObjectClass));

					foreach (MM_FormField ppp in olfields)
					{
						ControlCode c = ControlCode.Create(ppp, pp.RefObjectType.SysName);
						if (c != null)
						{
							string s = c.RootLayout();
							if (!String.IsNullOrEmpty(s))
								res.AppendLine(s);
						}
					}

					foreach (MM_FormFieldGroup g in selectobjectolgroups)
					{
						FieldGroupSelectObject fgc = new FieldGroupSelectObject(g, pp.RefObjectType.SysName);
						res.AppendLine(fgc.RootLayout());
					}
				}
				res.AppendLine("");
				
			}

			res.AppendLine(@"<br />
<br />
<span style=""font-size:xx-small""><%=""Последнее изменение: "" + ViewData.LastModifiedDate.DateToString() + "" "" + ViewData.LastModifiedDate.TimeToString() + "" "" + ViewData.LastModifiedUser.Title %></span>
");


			// скрипт
			res.AppendLine(@"<script runat=""server"">");
			res.AppendLine("");

			res.AppendLine("protected void Page_Load(object sender, EventArgs e)");
			res.AppendLine("{");
			res.AppendLine("\tSetTitle(\"" + objectType.Title + "\");");

			MM_MethodGroup forview = objectType.MM_MethodGroups.SingleOrDefault(o => o.SysName == "forview");
			if (forview == null)
			{
				res.AppendLine("\tif (!ViewData.IsDeleted) {");
				res.AppendLine("\t\ttoolbar.AddItem<" + objectType.SysName + @"Controller>(""edititem.gif"", ""Редактировать"", c => c.Edit(ViewData." + idCol + ", Query.CreateReturnUrl()));");
				res.AppendLine("\t\ttoolbar.AddItemSeparator();");
				res.AppendLine("\t\ttoolbar.AddItem<" + objectType.SysName + @"Controller>(""delete.gif"", ""Удалить"", c => c.Delete(ViewData." + idCol + ", Query.CreateReturnUrl()));");
				res.AppendLine("\t}");
				res.AppendLine("\telse");
				res.AppendLine("\t\ttoolbar.Visible = false;");
			}
			else
			{
				foreach (MM_MethodGroupItem mgi in forview.MM_MethodGroupItems.Where(o => !o.ParentMethodGroupItemID.HasValue).OrderBy(o => o.SeqNo))
				{
					if (mgi.IsSeparator)
						res.AppendLine("\t\ttoolbar.AddItemSeparator();");
					if (mgi.MethodID.HasValue)
					{
						if (!String.IsNullOrEmpty(mgi.MM_Method.PredicateCode))
							res.AppendLine("\tif (" + objectType.SysName + "Controller.Predicate" + mgi.MM_Method.SysName + "(ViewData))").Append("\t");
						res.AppendLine("\ttoolbar.AddItem<" + objectType.SysName + @"Controller>(""" + mgi.MM_Method.Icon + @""", """ + mgi.MM_Method.Title + @""", c => c." + mgi.MM_Method.SysName + @"(ViewData." + idCol + ", Query.CreateReturnUrl()));");
					}
					if (mgi.MM_MethodGroupItems.Count > 0)
					{
						res.AppendLine("\tvar mgi" + mgi.SeqNo.ToString() + @" = toolbar.AddPopupMenuLarge(""" + mgi.Title + @""");");
						foreach (MM_MethodGroupItem cmgi in mgi.MM_MethodGroupItems.OrderBy(o => o.SeqNo))
						{
							if (cmgi.IsSeparator)
								res.AppendLine("\tmgi" + mgi.SeqNo.ToString() + @".AddItemSeparator();");
							if (cmgi.MethodID.HasValue)
							{
								if (!String.IsNullOrEmpty(cmgi.MM_Method.PredicateCode))
									res.AppendLine("\tif (" + objectType.SysName + "Controller.Predicate" + cmgi.MM_Method.SysName + "(ViewData))").Append("\t");
								res.AppendLine("\tmgi" + mgi.SeqNo.ToString() + @".AddItem(""" + cmgi.MM_Method.Title + @""", Html.ActionUrl<" + objectType.SysName + @"Controller>(c => c." + cmgi.MM_Method.SysName + @"(ViewData." + idCol + @", Query.CreateReturnUrl())), """ + cmgi.MM_Method.Icon + @""", """");");
							}
						}
					}
				}
			}

			res.AppendLine("");
			foreach (MM_ObjectProperty p in ctables)
			{
				var ctablecols = from f in AppMM.DataContext.MM_FormFields
								 where f.MM_ObjectProperty.ObjectTypeID == p.RefObjectTypeID
								 orderby f.MM_ObjectProperty.SeqNo
								 select f.MM_ObjectProperty;
				var selectobjectctablegroups =
					(from g in p.RefObjectType.MM_FormFieldGroups orderby g.SeqNo select g).
					Where(o => o.SelectObjectPropertyID.HasValue || !String.IsNullOrEmpty(o.SelectObjectClass));

				foreach (MM_ObjectProperty pp in ctablecols)
				{
					ControlCode c = ControlCode.Create(pp.MM_FormField, p.SysName);
					if (c != null)
					{
						string s = c.ViewOnLoad();
						if (!String.IsNullOrEmpty(s))
							res.AppendLine("\t" + s);
					}
				}
				foreach (MM_FormFieldGroup g in selectobjectctablegroups)
				{
					FieldGroupSelectObject fgc = new FieldGroupSelectObject(g, p.SysName);
					res.AppendLine("\t" + fgc.ViewOnLoad());
					res.AppendLine("\tselect" + fgc.ControlID + ".TargetModalDialog = m" + p.SysName + ";");
				}

				foreach (MM_ObjectProperty pp in ctablecols.Where(o => o.MM_FormField.ShowInEdit && o.UpperBound != 1 && o.IsAggregate))
				{
					var olfields = from f in pp.RefObjectType.MM_ObjectProperties
								   where f.MM_FormField.ShowInEdit
								   orderby f.MM_FormField.SeqNo
								   select f.MM_FormField;

					var selectobjectolgroups =
					(from g in pp.RefObjectType.MM_FormFieldGroups orderby g.SeqNo select g).
					Where(o => o.SelectObjectPropertyID.HasValue || !String.IsNullOrEmpty(o.SelectObjectClass));

					foreach (MM_FormField ppp in olfields)
					{
						ControlCode c = ControlCode.Create(ppp, pp.RefObjectType.SysName);
						if (c != null)
						{
							string s = c.ViewOnLoad();
							if (!String.IsNullOrEmpty(s))
								res.AppendLine("\t" + s);
						}
					}

					foreach (MM_FormFieldGroup g in selectobjectolgroups)
					{
						FieldGroupSelectObject fgc = new FieldGroupSelectObject(g, pp.RefObjectType.SysName);
						res.AppendLine("\t" + fgc.ViewOnLoad());
						res.AppendLine("\tselect" + fgc.ControlID + ".TargetModalDialog = m" + p.SysName + ";");
					}
				}

				foreach (MM_ObjectProperty pp in ctablecols.Where(o => o.UpperBound != 1 && o.IsAggregate && o.MM_FormField.ShowInEdit))
				{
					res.AppendLine("\tol" + pp.SysName + ".DataContext = App.DataContext;");
				}
			}
			res.AppendLine("}");


			// код модальных окон
			foreach (MM_ObjectProperty p in ctables)
			{
				MM_ObjectProperty ppID = p.RefObjectType.MM_ObjectProperties.Single(o => o.IsPrimaryKey);
				var ctablecols = from f in AppMM.DataContext.MM_FormFields
								 where f.MM_ObjectProperty.ObjectTypeID == p.RefObjectTypeID
								 orderby f.MM_ObjectProperty.SeqNo
								 select f.MM_ObjectProperty;
				var olists = ctablecols.Where(o => o.MM_FormField.ShowInEdit && o.UpperBound != 1 && o.IsAggregate);
				var selectobjectctablegroups =
					(from g in p.RefObjectType.MM_FormFieldGroups orderby g.SeqNo select g).
					Where(o => o.SelectObjectPropertyID.HasValue || !String.IsNullOrEmpty(o.SelectObjectClass));

				res.AppendLine("protected void m" + p.SysName + "_Populate(object sender, EventArgs e)");
				res.AppendLine("{");

				foreach (MM_ObjectProperty pp in ctablecols.Where(o => o.MM_FormField.ShowInEdit && o.UpperBound == 1))
				{
					ControlCode c = ControlCode.Create(pp.MM_FormField, p.SysName);
					if (c != null)
					{
						string s = c.Init();
						if (!String.IsNullOrEmpty(s))
							res.AppendLine("\t" + s);
					}
				}
				foreach (MM_ObjectProperty pp in olists)
				{
					var otfields = from f in pp.RefObjectType.MM_ObjectProperties
								   where f.MM_FormField.ShowInEdit
								   orderby f.MM_FormField.SeqNo
								   select f.MM_FormField;
					foreach (MM_FormField f in otfields)
						res.AppendLine("\tol" + pp.SysName + @".AddColumn(""" + f.Title + @""", o => " + CodeGenHelper.GetStringValue(f.MM_ObjectProperty, "o") + ");");
				}

				res.AppendLine("\tif (m" + p.SysName + ".IsFirstPopulate)");
				res.AppendLine("\t{");
				res.AppendLine("\t\tlMsg" + p.SysName + ".Text = \"\";");
				foreach (MM_ObjectProperty pp in olists)
				{
					res.AppendLine("\t\tol" + pp.SysName + ".ClearItems();");
				}
				res.AppendLine("\t\tif (m" + p.SysName + ".Argument.ToInt32(0) > 0)");
				res.AppendLine("\t\t{");
				res.AppendLine("\t\t\t" + p.RefObjectType.SysName + " obj = ViewData." + p.SysName + ".SingleOrDefault(o => o." + ppID.SysName + " == m" + p.SysName + ".Argument.ToInt32(0));");
				res.AppendLine("\t\t\tif (obj != null)");
				res.AppendLine("\t\t\t{");
				foreach (MM_ObjectProperty pp in ctablecols.Where(o => o.MM_FormField.ShowInEdit && o.UpperBound == 1))
				{
					ControlCode c = ControlCode.Create(pp.MM_FormField, p.SysName);
					if (c != null) res.AppendLine("\t\t\t\t" + c.Load("obj"));
				}

				foreach (MM_ObjectProperty pp in olists)
				{
					res.AppendLine("");
					res.AppendLine("\t\t\t\tol" + pp.SysName + ".LoadData(obj." + pp.SysName + ");");
					res.AppendLine("");
				}
				res.AppendLine("\t\t\t\tm" + p.SysName + ".MessageBoxMode = obj.IsDeleted;");
				res.AppendLine("\t\t\t}");
				res.AppendLine("\t\t}");
				res.AppendLine("\t\telse");
				res.AppendLine("\t\t{");
				foreach (MM_ObjectProperty pp in ctablecols.Where(o => o.MM_FormField.ShowInEdit && o.UpperBound == 1))
				{
					ControlCode c = ControlCode.Create(pp.MM_FormField, p.SysName);
					if (c != null) res.AppendLine("\t\t\t" + c.SetDefaultOrEmpty());
				}
				res.AppendLine("\t\t}");
				res.AppendLine("\t}");
				res.AppendLine("}");

				res.AppendLine("protected void m" + p.SysName + "_OKClick(object sender, EventArgs e)");
				res.AppendLine("{");

				#region fields validate
				int reqCnt = ctablecols.Count(o => o.LowerBound > 0);
				if (reqCnt > 0)
					res.AppendLine("\tstring requiredMsg = \"\";");

				foreach (MM_ObjectProperty pp in ctablecols.Where(o => o.MM_FormField.ShowInEdit && o.LowerBound > 0))
				{
					ControlCode c = ControlCode.Create(pp.MM_FormField, p.SysName);
					if (c != null)
					{
						string v = c.ValidateRequired();
						if (!String.IsNullOrEmpty(v)) res.AppendLine("\t" + v);
					}
				}

				if (reqCnt > 0)
				{
					res.AppendLine("\tif (!String.IsNullOrEmpty(requiredMsg))");
					res.AppendLine("\t{");
					res.AppendLine("\t\tlMsg" + p.SysName + ".Text = \"<div class='savewarning'>Необходимо задать значения следующих полей:<ul>\" + requiredMsg + \"</ul></div>\";");
					res.AppendLine("\t\tm" + p.SysName + ".Reopen();");
					res.AppendLine("\t\treturn;");
					res.AppendLine("\t}");
				}
				#endregion



				res.AppendLine("\t" + p.RefObjectType.SysName + " obj = null;");
				res.AppendLine("\t" + "if (m" + p.SysName + ".Argument.ToInt32(0) > 0)");
				res.AppendLine("\t{");
				res.AppendLine("\t\tobj = App.DataContext." + p.RefObjectType.SysName + ".SingleOrDefault(o => o." + ppID.SysName + " == m" + p.SysName + ".Argument.ToInt32(0));");
				res.AppendLine("\t}");

				res.AppendLine("\tif (obj == null)");
				res.AppendLine("\t{");
				res.AppendLine("\t\tobj = new " + p.RefObjectType.SysName + " { " + p.RefObjectProperty.SysName + "ID = ViewData." + idCol + " };");
				res.AppendLine("\t\tApp.DataContext." + p.RefObjectType.SysName + ".InsertOnSubmit(obj);");
				res.AppendLine("\t}");

				foreach (MM_ObjectProperty pp in ctablecols.Where(o => o.MM_FormField.ShowInEdit && o.UpperBound == 1))
				{
					ControlCode c = ControlCode.Create(pp.MM_FormField, p.SysName);
					if (c != null) res.AppendLine("\t" + c.Save("obj"));
				}


				foreach (MM_ObjectProperty pp in olists)
				{
					res.AppendLine("");
					res.AppendLine("\tforeach (var oli in ol" + pp.SysName + ".GetItems())");
					res.AppendLine("\t{");
					res.AppendLine("\t\toli." + pp.RefObjectProperty.SysName + " = obj;");
					res.AppendLine("\t\toli.LastModifiedDate = DateTime.Now;");
					res.AppendLine("\t}");
					res.AppendLine("\tol" + pp.SysName + ".Update();");
					res.AppendLine("");
				}

				res.AppendLine("");

				var calccols = from f in AppMM.DataContext.MM_FormFields
							   where f.MM_ObjectProperty.ObjectTypeID == p.RefObjectTypeID
							   && f.ValueFunction != null && f.ValueFunction != ""
							   orderby f.SeqNo
							   select f;

				foreach (MM_FormField pp in calccols)
				{
					res.AppendLine("\tobj." + pp.MM_ObjectProperty.SysName +
						" = (new Func<" + pp.MM_ObjectProperty.MM_ObjectType.SysName + ", " +
						CodeGenHelper.GetCSharpType(pp.MM_ObjectProperty.TypeCode, pp.MM_ObjectProperty.UpperBound) +
						">(" + pp.ValueFunction + ")).Invoke(obj);");
				}

				res.AppendLine("");

				res.AppendLine("\t" + p.RefObjectType.SysName + "Controller c = new " + p.RefObjectType.SysName + "Controller();");
				res.AppendLine("\tc.Update(obj);");
				//res.AppendLine("\tApp.DataContext.SubmitChanges();");
				res.AppendLine("\tBaseController.RedirectTo<" + objectType.SysName + "Controller>(cc => cc.View(ViewData." + idCol + ", Query.GetString(\"returnurl\")));");

				res.AppendLine("}");

				// код ObjectLists
				foreach (MM_ObjectProperty pp in olists)
				{
					var otfields = from f in pp.RefObjectType.MM_ObjectProperties
								   where f.MM_FormField.ShowInEdit
								   orderby f.MM_FormField.SeqNo
								   select f.MM_FormField;

					var selectobjectolgroups =
					(from g in pp.RefObjectType.MM_FormFieldGroups orderby g.SeqNo select g).
					Where(o => o.SelectObjectPropertyID.HasValue || !String.IsNullOrEmpty(o.SelectObjectClass));

					res.AppendLine("");
					res.AppendLine("protected void ol" + pp.SysName + "_EditItem(object sender, ObjectListEventArgs<" + pp.RefObjectType.SysName + "> e)");
					res.AppendLine("{");
					foreach (MM_FormField ppp in otfields)
					{
						ControlCode c = ControlCode.Create(ppp, pp.RefObjectType.SysName);
						if (c != null)
						{
							res.AppendLine("\t" + c.Load("e.Data"));
						}
					}

					res.AppendLine("}");
					res.AppendLine("");
					res.AppendLine("protected void ol" + pp.SysName + "_AddItem(object sender, EventArgs e)");
					res.AppendLine("{");
					foreach (MM_FormField ppp in otfields)
					{
						ControlCode c = ControlCode.Create(ppp, pp.RefObjectType.SysName);
						if (c != null)
						{
							string s = c.Init();
							if (!String.IsNullOrEmpty(s)) res.AppendLine("\t" + c.Init());
							res.AppendLine("\t" + c.SetDefaultOrEmpty());
						}
					}
					res.AppendLine("}");
					res.AppendLine("");
					res.AppendLine("protected void ol" + pp.SysName + "_SaveItem(object sender, ObjectListSaveEventArgs<" + pp.RefObjectType.SysName + "> e)");
					res.AppendLine("{");

					reqCnt = otfields.Count(o => o.MM_ObjectProperty.LowerBound > 0);
					if (reqCnt > 0)
						res.AppendLine("\tstring requiredMsg = \"\";");

					foreach (MM_FormField ppp in otfields.Where(o => o.ShowInEdit && o.MM_ObjectProperty.LowerBound > 0))
					{
						ControlCode c = ControlCode.Create(ppp, pp.RefObjectType.SysName);
						if (c != null)
						{
							string v = c.ValidateRequired();
							if (!String.IsNullOrEmpty(v)) res.AppendLine("\t" + v);
						}
					}

					if (reqCnt > 0)
					{
						res.AppendLine("\tif (!String.IsNullOrEmpty(requiredMsg))");
						res.AppendLine("\t{");
						res.AppendLine("\t\te.Message = \"<div class='savewarning'>Необходимо задать значения следующих полей:<ul>\" + requiredMsg + \"</ul></div>\";");
						res.AppendLine("\t\te.Cancel = true;");
						res.AppendLine("\t\treturn;");
						res.AppendLine("\t}");
					}

					foreach (MM_FormField ppp in otfields)
					{
						ControlCode c = ControlCode.Create(ppp, pp.RefObjectType.SysName);
						if (c != null)
						{
							res.AppendLine("\t" + c.Save("e.Data"));
						}
					}
					res.AppendLine("}");
					res.AppendLine("");
					foreach (MM_FormField ppp in otfields)
					{
						ControlCode c = ControlCode.Create(ppp, pp.RefObjectType.SysName);
						if (c != null)
						{
							string s = c.Events();
							if (!String.IsNullOrEmpty(s))
								res.AppendLine(s);
						}
					}
					res.AppendLine("");

					foreach (MM_FormFieldGroup g in selectobjectolgroups)
					{
						FieldGroupSelectObject fgc = new FieldGroupSelectObject(g, pp.RefObjectType.SysName);
						res.AppendLine(fgc.Events());
					}
				}

				foreach (MM_ObjectProperty pp in ctablecols.Where(o => o.MM_FormField.ShowInEdit && o.UpperBound == 1))
				{
					ControlCode c = ControlCode.Create(pp.MM_FormField, p.SysName);
					if (c != null)
					{
						string s = c.Events();
						if (!String.IsNullOrEmpty(s))
							res.AppendLine(s);
					}
				}

				foreach (MM_FormFieldGroup g in selectobjectctablegroups)
				{
					FieldGroupSelectObject fgc = new FieldGroupSelectObject(g, p.SysName);
					res.AppendLine(fgc.Events());
				}



			}

			res.AppendLine("</script>");
			return res.ToString();
		}
	}
}
