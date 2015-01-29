using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Nephrite.Web.Controls;
using System.Linq.Expressions;
using Nephrite.Web.Layout;

namespace Nephrite.Web.Reporting
{
	public class ReportTable<T> : System.Web.UI.UserControl	where T : class
	{
		public string ReportType { get; set; }

		public IOrderedQueryable<T> ViewData { get; set; }
		public List<Column<T>> Columns { get; set; }
		public List<GroupColumn<T>> GroupColumns { get; set; }
		public List<AggregateColumn<T>> AggregateColumns { get; set; }

		public GroupColumn<T> Group1 { get; set; }
		public GroupColumn<T> Group2 { get; set; }
		public GroupColumn<T> Group3 { get; set; }

		public string Group1Sort { get; set; }
		public string Group2Sort { get; set; }
		public string Group3Sort { get; set; }

		public Sorter Sorter { get; set; }
		public Paging Pager { get; set; }

		protected void Page_Load(object sender, EventArgs e)
		{

		}

		protected IQueryable<T> ApplyOrderByGroup(IQueryable<T> src)
		{
			if (Group3 != null)
			{
				if (Group3Sort == "ASC")
					src = src.OrderBy<T, Object>(Group3.OrderBy);
				else
					src = src.OrderByDescending<T, Object>(Group3.OrderBy);
			}
			if (Group2 != null)
			{
				if (Group2Sort == "ASC")
					src = src.OrderBy<T, Object>(Group2.OrderBy);
				else
					src = src.OrderByDescending<T, Object>(Group2.OrderBy);
			}
			if (Group1 != null)
			{
				if (Group1Sort == "ASC")
					src = src.OrderBy<T, Object>(Group1.OrderBy);
				else
					src = src.OrderByDescending<T, Object>(Group1.OrderBy);
			}
			return src;
		}

		protected override void Render(HtmlTextWriter writer)
		{
			DateTime dtStart = DateTime.Now;
			string gr1lastvalue = "";
			string gr2lastvalue = "";
			string gr3lastvalue = "";
			IEnumerable<T> gr1list = null;
			IEnumerable<T> gr2list = null;
			IEnumerable<T> gr3list = null;
			Dictionary<string, decimal> totals = new Dictionary<string, decimal>();

			writer.Write("<table class='ms-listviewtable' cellpadding='0' cellspacing='0' style='margin-top: 15px' >");
			writer.Write("<tr class='ms-viewheadertr'>");
			writer.Write(HtmlHelperWSS.TH("№"));
								
			if (ReportType == "D")
			{
				if (Sorter != null)
				{
					foreach (Column<T> col in Columns)
					{
						switch (col.Type.ToLower())
						{
							case "string":
								writer.Write(HtmlHelperWSS.TH(Sorter.AddSortColumn<T, string>(col.Title, col.SortPropertyString)));
								break;
							case "datetime":
								writer.Write(HtmlHelperWSS.TH(Sorter.AddSortColumn<T, DateTime>(col.Title, col.SortPropertyDateTime)));
								break;
							case "int":
								writer.Write(HtmlHelperWSS.TH(Sorter.AddSortColumn<T, int>(col.Title, col.SortPropertyInt)));
								break;
							case "decimal":
								writer.Write(HtmlHelperWSS.TH(Sorter.AddSortColumn<T, decimal>(col.Title, col.SortPropertyDecimal)));
								break;
							case "bool":
								writer.Write(HtmlHelperWSS.TH(Sorter.AddSortColumn<T, bool>(col.Title, col.SortPropertyBool)));
								break;
							default:
								writer.Write(HtmlHelperWSS.TH(col.Title));
								break;
						}
					}
				}
				else
				{
					foreach (Column<T> col in Columns)
					{
						writer.Write(HtmlHelperWSS.TH(col.Title));
					}
				}
			}

			if (ReportType == "A")
			{
				writer.Write(HtmlHelperWSS.TH(""));
				foreach (AggregateColumn<T> col in AggregateColumns)
				{
					writer.Write(HtmlHelperWSS.TH(col.Title));
				}
				writer.Write("</tr>");

				if (Group1 != null)
				{
					var group1items = ViewData.Select(Group1.PropertyFunc).Distinct();
					if (Group1Sort == "ASC")
						group1items = group1items.OrderBy(o => o);
					else
						group1items = group1items.OrderByDescending(o => o);
					int levelNum1 = 0;
					foreach(var group1 in group1items.ToList())
					{
						writer.Write("<tr>");
						levelNum1++;
						var group1t = group1;
						if (Group1.TransformValue != null)
							group1t = Group1.TransformValue(group1t);
						if (group1t.IsEmpty())
							group1t = "Атрибут «" + Group1.Title + "» не задан";
						writer.Write("<td class='ms-vb2' style='mso-number-format:\"\\@\"; font-weight:bold'>" + levelNum1.ToString() + "</td>");
						writer.Write("<td class='ms-vb2' style='mso-number-format:\"\\@\"; font-weight:bold'>" + HttpUtility.HtmlEncode(group1t) + "</td>");
						var group1values = ViewData.Where(Group1.PropertyFilter(group1));
						foreach (AggregateColumn<T> col in AggregateColumns)
						{
							if (col.CanCompute(this))
							{
								decimal val = col.Property(group1values, this);

								writer.Write("<td class='ms-vb2' style='mso-number-format:\"\\@\";font-weight:bold'>" + val.ToString("0.##") + "</td>");
							}
							else
								writer.Write("<td class='ms-vb2' style='font-weight:bold'>-</td>");
						}
					
						writer.Write("</tr>");


						if (Group2 != null)
						{
							var group2items = ViewData.Where(Group1.PropertyFilter(group1)).Select(Group2.PropertyFunc).Distinct();
							if (Group2Sort == "ASC")
								group2items = group2items.OrderBy(o => o);
							else
								group2items = group2items.OrderByDescending(o => o);
							int levelNum2 = 0;
							foreach(var group2 in group2items.ToList())
							{
								writer.Write("<tr>");
								levelNum2++;
								var group2t = group2;
								if (Group2.TransformValue != null)
									group2t = Group2.TransformValue(group2t);
								if (group2.IsEmpty())
									group2t = "Атрибут «" + Group2.Title + "» не задан";

								writer.Write("<td class='ms-vb2' style='mso-number-format:\"\\@\"; padding-left:20px'>" + levelNum1.ToString() + "." + levelNum2.ToString() + "</td>");
								writer.Write("<td class='ms-vb2' style='mso-number-format:\"\\@\"; padding-left:20px'>" + HttpUtility.HtmlEncode(group2t) + "</td>");
								var group2values = ViewData.Where(Group1.PropertyFilter(group1)).Where(Group2.PropertyFilter(group2));
								foreach (AggregateColumn<T> col in AggregateColumns)
								{
									if (col.CanCompute(this))
									{
										decimal val = col.Property(group2values, this);
									
										writer.Write("<td class='ms-vb2' style='mso-number-format:\"\\@\";font-weight:bold'>" + val.ToString("0.##") + "</td>");
									}
									else
										writer.Write("<td class='ms-vb2' style='font-weight:bold'>-</td>");
								}
					
								writer.Write("</tr>");

		
								if (Group3 != null)
								{
									var group3items = ViewData.Where(Group1.PropertyFilter(group1)).Where(Group2.PropertyFilter(group2)).Select(Group3.PropertyFunc).Distinct();
									if (Group3Sort == "ASC")
										group3items = group3items.OrderBy(o => o);
									else
										group3items = group3items.OrderByDescending(o => o);
									int levelNum3 = 0;
									foreach(var group3 in group3items.ToList())
									{
										writer.Write("<tr>");
										levelNum3++;
										var group3t = group3;
										if (Group3.TransformValue != null)
											group3t = Group3.TransformValue(group3t);
										if (group3.IsEmpty())
											group3t = "Атрибут «" + Group3.Title + "» не задан";
										writer.Write("<td class='ms-vb2' style='mso-number-format:\"\\@\"; padding-left:40px'>" + levelNum1.ToString() + "." + levelNum2.ToString() + "." + levelNum3.ToString() + "</td>");
										writer.Write("<td class='ms-vb2' style='mso-number-format:\"\\@\"; padding-left:40px'>" + HttpUtility.HtmlEncode(group3t) + "</td>");
										var group3values = ViewData.Where(Group1.PropertyFilter(group1)).Where(Group2.PropertyFilter(group2)).Where(Group3.PropertyFilter(group3));
										foreach (AggregateColumn<T> col in AggregateColumns)
										{
											if (col.CanCompute(this))
											{
												decimal val = col.Property(group3values, this);
									
												writer.Write("<td class='ms-vb2' style='mso-number-format:\"\\@\";font-weight:bold'>" + val.ToString("0.##") + "</td>");
											}
											else
												writer.Write("<td class='ms-vb2' style='font-weight:bold'>-</td>");
										}
					
										writer.Write("</tr>");
									}
								}
							}
						}
					}
				}

				writer.Write("<tr style='font-weight:bold'><td class='ms-vb2' colspan='2'>Итого</td>");
				foreach (AggregateColumn<T> col in AggregateColumns)
				{
					writer.Write("<td class='ms-vb2' style='mso-number-format:\"\\@\";'>");
					writer.Write(col.Property(ViewData, this).ToString("0.##"));
					writer.Write("</td>");
				}
				writer.Write("</tr>");

				writer.Write("</table>");
				writer.Write("Время формирования: " + DateTime.Now.Subtract(dtStart).ToString());
				return;
			}

			writer.Write("</tr>");
			int i1 = 0;
			int i2 = 0;
			int i3 = 0;
			int ii = Pager != null ? (Pager.PageIndex - 1) * Pager.PageSize + 1 : (Query.GetInt("page", 1) - 1) * Settings.PageSize + 1;
			var rep = ApplyOrderByGroup((ReportType == "D" && Sorter != null ? Sorter.ApplyOrderBy(ViewData) : ViewData));
			if (Pager != null) rep = Pager.ApplyPaging(rep);
			//HtmlHelperBase.Instance.Repeater<T>(ApplyOrderByGroup((ReportType == "D" && Sorter != null ? Sorter.ApplyOrderBy(ViewData) : ViewData)), "", "ms-alternating", (o, css) =>
			HtmlHelperBase.Instance.Repeater<T>(rep, "", "ms-alternating", (o, css) =>
			{
				bool printgr1 = false;
				bool printgr2 = false;
				bool printgr3 = false;
				string gr1val = "";
				string gr2val = "";
				string gr3val = "";

				if (Group1 != null)
				{
					gr1val = Group1.Property(o, this);
					if (String.IsNullOrEmpty(gr1val))
						gr1val = "Атрибут «" + Group1.Title + "» не задан";
					else
						gr1val = gr1val.Trim();

					printgr1 = gr1lastvalue != gr1val;
					if (printgr1)
					{
						i1++;
						i2 = 0;
						i3 = 0;
						ii = 1;
					}
					gr1lastvalue = gr1val;
					if (printgr1 || (gr1list == null && ReportType == "A"))
					{
						gr2lastvalue = "";
						if (ReportType == "A")
						{
							gr1list = Group1.LevelFilter(ViewData, this, gr1val);
							gr2list = gr1list;
							gr3list = gr1list;
						}
					}
				}
				if (Group2 != null)
				{
					gr2val = Group2.Property(o, this);
					if (String.IsNullOrEmpty(gr2val))
						gr2val = "Атрибут «" + Group2.Title + "» не задан";
					else
						gr2val = gr2val.Trim();

					printgr2 = gr2lastvalue != gr2val;
					if (printgr2)
					{
						i2++;
						i3 = 0;
						ii = 1;
					}

					gr2lastvalue = gr2val;
					if (printgr2)
					{
						gr3lastvalue = "";
						if (ReportType == "A")
						{
							gr2list = Group2.LevelFilter(gr1list, this, gr2val);
							gr3list = gr2list;
						}
					}
				}
				if (Group3 != null)
				{
					gr3val = Group3.Property(o, this);
					if (String.IsNullOrEmpty(gr3val))
						gr3val = "Атрибут «" + Group3.Title + "» не задан";
					else
						gr3val = gr3val.Trim();

					printgr3 = gr3lastvalue != gr3val;
					if (printgr3)
					{
						i3++;
						ii = 1;
					}

					gr3lastvalue = gr3val;
					if (printgr3)
					{
						if (ReportType == "A")
						{
							gr3list = Group3.LevelFilter(gr2list, this, gr3val);
							if (gr3list == null)
								gr3list = gr2list;
						}
					}
				}

                		CurrentItem = o;
				if (printgr1)
				{
					CurrentLevel = 1;
					writer.Write("<tr>");
					string groupTitle = Group1.TitleProperty != null ? Group1.TitleProperty(o, this) : gr1val;
					if (ReportType == "D")
					{
						writer.Write("<td style='mso-number-format:\"\\@\"; font-size: 14pt; font-weight:bold'>" + i1.ToString() + "</td>");
						writer.Write("<td style='mso-number-format:\"\\@\"; font-size: 14pt; font-weight:bold' colspan='" + Columns.Count + "'>" + HttpUtility.HtmlEncode(groupTitle) + "</td>");
					}
					if (ReportType == "A")
					{
						writer.Write("<td class='ms-vb2' style='mso-number-format:\"\\@\"; font-weight:bold'>" + i1.ToString() + "</td>");
						writer.Write("<td class='ms-vb2' style='mso-number-format:\"\\@\"; font-weight:bold'>" + HttpUtility.HtmlEncode(groupTitle) + "</td>");
						foreach (AggregateColumn<T> col in AggregateColumns)
						{
							if (col.CanCompute(this))
							{
								decimal val = col.Property(gr1list.AsQueryable(), this);
								if (totals.ContainsKey(col.Title))
									totals[col.Title] += val;
								else
									totals[col.Title] = val;
								writer.Write("<td class='ms-vb2' style='mso-number-format:\"\\@\";font-weight:bold'>" + val.ToString("0.##") + "</td>");
							}
							else
								writer.Write("<td class='ms-vb2' style='font-weight:bold'>-</td>");
						}
					}
					writer.Write("</tr>");
				}


				if (printgr2 && (gr2list != null || ReportType == "D"))
				{
					CurrentLevel = 2;
					writer.Write("<tr>");
					string groupTitle = Group2.TitleProperty != null ? Group2.TitleProperty(o, this) : gr2val;
					if (ReportType == "D")
					{
						writer.Write("<td style='mso-number-format:\"\\@\"; font-size: 11pt; font-weight:bold' >" + i1.ToString() + "." + i2.ToString() + "</td>");
						writer.Write("<td style='mso-number-format:\"\\@\"; font-size: 11pt; font-weight:bold' colspan='" + Columns.Count + "'>" + HttpUtility.HtmlEncode(groupTitle) + "</td>");
					}
					if (ReportType == "A")
					{
						writer.Write("<td class='ms-vb2' style='mso-number-format:\"\\@\"; padding-left:20px'>" + i1.ToString() + "." + i2.ToString() + "</td>");
						writer.Write("<td class='ms-vb2' style='mso-number-format:\"\\@\"; padding-left:20px'>" + HttpUtility.HtmlEncode(groupTitle) + "</td>");
						foreach (AggregateColumn<T> col in AggregateColumns)
						{
                            if (col.CanCompute(this))
                                writer.Write("<td class='ms-vb2' style='mso-number-format:\"\\@\";'>" + col.Property(gr2list.AsQueryable(), this).ToString("0.##") + "</td>");
                            else
                                writer.Write("<td class='ms-vb2'>-</td>");
						}
					}
					writer.Write("</tr>");
				}


				if (printgr3 && (gr3list != null || ReportType == "D"))
				{
					CurrentLevel = 3;
		   			writer.Write("<tr>");
					string groupTitle = Group3.TitleProperty != null ? Group3.TitleProperty(o, this) : gr3val;
					if (ReportType == "D")
					{
						writer.Write("<td style='mso-number-format:\"\\@\"; font-weight:bold' >" + i1.ToString() + "." + i2.ToString() + "." + i3.ToString() + "</td>");
						writer.Write("<td style='mso-number-format:\"\\@\"; font-weight:bold' colspan='" + Columns.Count + "'>" + HttpUtility.HtmlEncode(groupTitle) + "</td>");
					}
					if (ReportType == "A")
					{
						writer.Write("<td class='ms-vb2' style='mso-number-format:\"\\@\"; padding-left:40px'>" + i1.ToString() + "." + i2.ToString() + "." + i3.ToString() + "</td>");
						writer.Write("<td class='ms-vb2' style='mso-number-format:\"\\@\"; padding-left:40px'>" + HttpUtility.HtmlEncode(groupTitle) + "</td>");
						foreach (AggregateColumn<T> col in AggregateColumns)
						{
                            if (col.CanCompute(this))
							    writer.Write("<td class='ms-vb2' style='mso-number-format:\"\\@\";'>" + col.Property(gr3list.AsQueryable(), this).ToString("0.##") + "</td>");
                            else
                                writer.Write("<td class='ms-vb2'>-</td>");
						}
					}
					writer.Write("</tr>");
				}



				if (ReportType == "D")
				{
					writer.Write("<tr class='" + css + "'>");
					writer.Write("<td class='ms-vb2' style='mso-number-format:\"\\@\";'>" + (i1 > 0 ? i1.ToString() + "." : "") + (i2 > 0 ? i2.ToString() + "." : "") + (i3 > 0 ? i3.ToString() + "." : "") + ii.ToString() + "</td>");
					foreach (Column<T> col in Columns)
					{
						string s = col.PropertyEx != null ? col.PropertyEx(o, this) : col.Property(o);
						if (col.Encoding) s = HttpUtility.HtmlEncode(s);
						if (col.Type == "decimal")
							writer.Write("<td class='ms-vb2' style='mso-number-format:Standard; text-align:right;'>" + s.Replace("&#160;", " ") + "</td>");
						else
							writer.Write("<td class='ms-vb2' style='mso-number-format:\"\\@\";'>" + s + "</td>");
					}
					writer.Write("</tr>");
				}
				ii++;

			});
            if (ReportType == "A")
            {
                writer.Write("<tr style='font-weight:bold'><td class='ms-vb2' colspan='2'>Итого</td>");
                foreach (AggregateColumn<T> col in AggregateColumns)
                {
                    writer.Write("<td class='ms-vb2' style='mso-number-format:\"\\@\";'>");
                    writer.Write(col.Property(ViewData, this).ToString("0.##"));
                    writer.Write("</td>");
                }
                writer.Write("</tr>");
            }
			writer.Write("</table>");

		}

        Dictionary<Func<decimal>, decimal> constants = new Dictionary<Func<decimal>, decimal>();
        public decimal CalcConstant(Func<decimal> f)
        {
            if (!constants.ContainsKey(f))
                constants.Add(f, f());
            return constants[f];
        }

        Dictionary<string, object> parameters = new Dictionary<string, object>();
        public object this[string parameterName]
        {
            get { return parameters[parameterName]; }
            set { parameters[parameterName] = value; }
        }

        public T CurrentItem { get; set; }
        public int CurrentLevel { get; set; }
    }

	public class Column<T> where T : class
	{
		bool _encoding = true;
		public string Title { get; set; }
		public Func<T, string> Property { get; set; }
		public Func<T, ReportTable<T>, string> PropertyEx { get; set; }
		public string Type { get; set; }
		public Expression<Func<T, string>> SortPropertyString { get; set; }
		public Expression<Func<T, bool>> SortPropertyBool { get; set; }
		public Expression<Func<T, DateTime>> SortPropertyDateTime { get; set; }
		public Expression<Func<T, int>> SortPropertyInt { get; set; }
		public Expression<Func<T, decimal>> SortPropertyDecimal { get; set; }
		public bool Encoding { get { return _encoding; } set { _encoding = value; } }
	}

	public class GroupColumn<T> where T : class
	{
		public string Title { get; set; }
		public string SqlColumn { get; set; }
		public Func<T, ReportTable<T>, string> Property { get; set; }
		public Expression<Func<T, string>> PropertyFunc { get; set; }
		public Func<string, Expression<Func<T, bool>>> PropertyFilter { get; set; }
		public Func<IEnumerable<T>, ReportTable<T>, string, IEnumerable<T>> LevelFilter { get; set; }
		public Expression<Func<T, object>> OrderBy { get; set; }
		public Func<string, string> TransformValue { get; set; }
		public Func<T, ReportTable<T>, string> TitleProperty { get; set; }
	}

	public class AggregateColumn<T> where T : class
	{
		public AggregateColumn()
		{
			CanCompute = r => true;
		}
		public string Title { get; set; }
		public Func<IQueryable<T>, ReportTable<T>, decimal> Property { get; set; }
		/// <summary>
		/// Признак, вычислять значение или ставить прочерк
		/// </summary>
		public Func<ReportTable<T>, bool> CanCompute { get; set; }
		
	}
}