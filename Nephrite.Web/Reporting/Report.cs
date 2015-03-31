using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Globalization;

using System.Data;
using Nephrite.Layout;

namespace Nephrite.Web.Reporting
{
	public class Report : System.Web.UI.UserControl
	{
		public object TableAttributes { get; set; }
		public List<AggregateColumn> AggregateColumns { get; set; }
		public List<Grouping> Groupings { get; set; }
		public IQueryable DataSource { get; set; }
		public string Sql { get; private set; }
		public int MaxRows { get; set; }

		List<List<ReportItem>> Data = new List<List<ReportItem>>();

		public Report()
		{
			AggregateColumns = new List<AggregateColumn>();
			Groupings = new List<Grouping>();
			MaxRows = 50000;
		}

		protected override void Render(System.Web.UI.HtmlTextWriter writer)
		{
			if (DataSource == null)
				return;
			int rowCount = 0;
			// Загрузка данных из БД
			// Сформируем запрос
			try
			{
				for (int gnum = 0; gnum <= Groupings.Count; gnum++)
				{
					string query = "SELECT ";
					string groupings = Groupings.Take(gnum).Select(o => o.KeyColumn).Join(", ");
					query += groupings + (groupings.IsEmpty() ? "" : ", ");
					for (int i = 0; i < AggregateColumns.Count; i++)
					{
						if (i > 0)
							query += ", ";
						query += AggregateColumns[i].Selector + " as AC" + i.ToString();
					}
					var cmd = A.Model.GetCommand(DataSource);
					query += cmd.CommandText.Substring(cmd.CommandText.ToUpper().IndexOf(" FROM "));
					query = query.Substring(0, query.ToUpper().LastIndexOf(" ORDER BY "));
					if (!groupings.IsEmpty())
					{
						query += " GROUP BY " + groupings;
						if (Groupings.Take(gnum).Any(o => !o.TitleSelector.IsEmpty()))
						{
							var q1 = "FROM (" + query + ") tg";
							var qs = "SELECT ";
							int i = 0;
							foreach(var g in Groupings.Take(gnum))
							{
								qs += (g.TitleSelector ?? g.KeyColumn) + " as G" + i.ToString() + ", ";
								i++;
							}
							for (int i1 = 0; i1 < AggregateColumns.Count; i1++)
							{
								if (i1 > 0)
									qs += ", ";
								qs += "tg.AC" + i1.ToString();
							}
							query = qs + " FROM (" + query + ") tg ORDER BY ";
							for (int i1 = 0; i1 < gnum; i1++)
							{
								if (i1 > 0)
									query += ", ";
								query += "G" + i1.ToString();
							}
						}
						else
							query += " ORDER BY " + Groupings.Take(gnum).Select(o => o.KeyColumn + (o.SortAsc ? "" : " DESC")).Join(", ");
					}
					cmd.CommandText = query;
					// Выполним его
					Sql = cmd.CommandText;
					var list = new List<ReportItem>();
					Data.Add(list);
					
					var state = cmd.Connection.State;
					if (state != ConnectionState.Open) cmd.Connection.Open();
					try
					{
						using (var dr = cmd.ExecuteReader())
						{
							var ac = AggregateColumns.Count;
							while (dr.Read())
							{
								var item = new ReportItem(ac, gnum, gnum == 0 ? null : Groupings[gnum - 1]);
								for (int i = 0; i < gnum; i++)
									item.GroupKeys[i] = dr.IsDBNull(i) ? null : dr.GetValue(i).ToString();
								for (int i = 0; i < ac; i++)
									item.Values[i] = dr.IsDBNull(i + gnum) ? 0 : Convert.ToDecimal(dr.GetValue(i + gnum));

								list.Add(item);
								rowCount++;
								if (rowCount > MaxRows)
									throw new MaxRowsException();
							}
						}
					}
					finally
					{
						if (state != ConnectionState.Open) cmd.Connection.Close();
					}
				}
			}
			catch (MaxRowsException)
			{
				writer.Write("<span style='color:red; font-size: big'>Количество строк отчета превышает " + MaxRows.ToString() + ". Измените критерии фильтра или группировки.</span>");
				return;
			}

			writer.Write(AppLayout.Current.List.ListTableBegin(TableAttributes));
			writer.Write(AppLayout.Current.List.ListHeaderBegin(null));
			
			RenderColumnHeader(writer, "№");
			RenderColumnHeader(writer, "");

			foreach (var col in AggregateColumns)
			{
				if (!col.Visible)
					continue;
				RenderColumnHeader(writer, col.Caption);
			}
			writer.Write(AppLayout.Current.List.ListHeaderEnd());

			if (Groupings.Count > 0)
				RenderGroup(writer, 0, "", Data[1], Data[0][0]);
			// Рендер итогов
			RenderRow(writer, "Итого", "", Data[0][0], "font-weight:bold", 0);

			writer.Write(AppLayout.Current.List.ListTableEnd());
		}

		/// <summary>
		/// Вывод заголовка столбца
		/// </summary>
		/// <param name="writer">Выходной поток</param>
		/// <param name="html">Разметка</param>
		void RenderColumnHeader(System.Web.UI.HtmlTextWriter writer, string html)
		{
			writer.Write(AppLayout.Current.List.THBegin(null));
			writer.Write(html);
			writer.Write(AppLayout.Current.List.THEnd());
		}

		/// <summary>
		/// Вывод группы строк
		/// </summary>
		/// <param name="writer">Выходной поток</param>
		/// <param name="group">Порядковый номер группы</param>
		/// <param name="seqNo">Порядковый номер родительской строки</param>
		/// <param name="parentData">Массив данных</param>
		void RenderGroup(System.Web.UI.HtmlTextWriter writer, int group, string seqNo, IEnumerable<ReportItem> parentData, ReportItem parent)
		{
			// Получить набор значений группировки
			Func<string, string> getTitleFunc = Groupings[group].GetTitle == null ? o => o : Groupings[group].GetTitle;
			
			var groups = parentData.Select(o => o.GroupKeys[group]).Select(o => new { Key = o, Value = getTitleFunc(o) });
			
			int index = 0;
			foreach (var val in parentData)
			{
				index++;
				string seqNoNew = seqNo + index.ToString();
				string grValue = getTitleFunc(val.GroupKeys[group]);
				val.Parent = parent;
				RenderRow(writer, grValue.IsEmpty() ? Groupings[group].Title + " - не задано" : grValue, seqNoNew, val, group == 0 ? "font-weight:bold" : "", group * 20);
				// Рендер дочерних групп
				if (group < Groupings.Count - 1)
				{
					var childData = Data[group + 2].AsEnumerable();
					for (int x = 0; x <= group; x++)
						childData = childData.Where(o => o.GroupKeys[x] == val.GroupKeys[x]).ToArray();
					RenderGroup(writer, group + 1, seqNoNew + ".", childData, val);
				}
			}
		}

		bool alternating = false;
		/// <summary>
		/// Вывод строки отчета
		/// </summary>
		/// <param name="writer">Выходной поток</param>
		/// <param name="groupCaption">Заголовок группы</param>
		/// <param name="seqNo">Порядковый номер строки</param>
		/// <param name="data">Массив данных</param>
		void RenderRow(System.Web.UI.HtmlTextWriter writer, string groupCaption, string seqNo, ReportItem data, string style, int padding)
		{
			string formatStr1 = "<tr {4}style='{0}'><td style='mso-number-format:\"\\@\";padding-right:4px;top{1}'>{2}</td><td style='mso-number-format:\"\\@\";{1}'>{3}</td>";
			string formatStr2 = "<td class='ms-vb2' style='mso-number-format:\"\\@\";text-align:right'>{0}</td>";
			string paddingStyle = padding == 0 ? "" : "padding-left: " + padding.ToString() + "px";

			writer.Write(String.Format(formatStr1, style, paddingStyle, seqNo, groupCaption, alternating ? "class='ms-alternating' " : ""));
			for (int i = 0; i < AggregateColumns.Count; i++)
			{
				if (!AggregateColumns[i].Visible)
					continue;
				writer.Write(String.Format(formatStr2, (AggregateColumns[i].ValueFunc == null ? data.Values[i] : AggregateColumns[i].ValueFunc(data)).ToString("###,###,###,###,##0." + "".PadLeft(AggregateColumns[i].DecimalPoints, '0'), CultureInfo.GetCultureInfo(1049))));
			}
			writer.Write("</tr>");
			alternating = !alternating;
		}

		/// <summary>
		/// Элемент отчета
		/// </summary>
		public class ReportItem
		{
			public string[] GroupKeys;
			public decimal[] Values;
			public ReportItem Parent;
			public Grouping Grouping;

			public ReportItem(int columnCount, int groupCount, Grouping grouping)
			{
				Values = new decimal[columnCount];
				GroupKeys = new string[groupCount];
				Grouping = grouping;
			}
		}

		class MaxRowsException : ApplicationException
		{
		}
	}
}