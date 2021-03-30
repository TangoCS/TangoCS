using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using OfficeOpenXml;
using System.Globalization;
using AngleSharp.Css.Parser;
using AngleSharp.Css.Dom;
using System.Xml;

namespace Tango.Excel
{
	public class ExcelWriter : IContentWriter, IDisposable
	{
		public string NewLine => "\n";

		ExcelPackage p;
		ExcelWorksheet s;
		CssParser cssParser = new CssParser();
		int r = 1;
		int c = 1;
		int startcol = 1;
		int totalColumns = 1;
		HashSet<int> divs;
		HashSet<int> divs_nomerge;

		Dictionary<string, Action<ExcelRange>> classes = new Dictionary<string, Action<ExcelRange>>();

		public int CurrentRow => r;
		public ExcelRange CurrentCell => s.Cells[r, c];

		public void SetClassAction(string @class, Action<ExcelRange> cells)
		{
			classes[@class] = cells;
		}

		public ExcelWriter(bool fullPrecision = true)
		{
			p = new ExcelPackage();
			SetFullPrecisionAttribute(p, fullPrecision);
		}

		public ExcelWriter(System.IO.Stream template)
		{
			p = new ExcelPackage(template);
		}

		public ExcelWriter(System.IO.FileInfo fileInfo)
		{
			p = new ExcelPackage(fileInfo);

		}

		private const string CalculationPropertiesNodeName = "calcPr";
		private const string FullPrecisionAttributeName = "fullPrecision";
		private const string FullPrecisionOffValue = "0";
		private void SetFullPrecisionAttribute(ExcelPackage ep, bool fullPrecision)
		{
			var attributeCollection = GetCalculationPropertiesAttributeCollection(ep); // If we can't get the attribute collection, then we can't set the attribute 
			if (attributeCollection == null) return;
			var fullPrecisionAttr = GetAttribute(ep, FullPrecisionAttributeName);
			if (fullPrecision)
			{
				if (fullPrecisionAttr != null)
					attributeCollection.Remove(fullPrecisionAttr);
				else
				{
					if (fullPrecisionAttr == null)
					{ // easiest way to set the value of the attribute 
						var newAttr = ep.Workbook.WorkbookXml.CreateAttribute(FullPrecisionAttributeName);
						newAttr.Value = FullPrecisionOffValue;

						attributeCollection.Append(newAttr);
					}
					else
					{
						fullPrecisionAttr.Value = FullPrecisionOffValue;
					}
				}
			}

		}
		private XmlAttribute GetAttribute(ExcelPackage ep, string attributeName)
		{
			var attributeCollection = GetCalculationPropertiesAttributeCollection(ep); // return null if fullPrecision does not exists 
			return attributeCollection?[attributeName];
		}
		private XmlAttributeCollection GetCalculationPropertiesAttributeCollection(ExcelPackage ep)
		{
			var xmlNode = GetCalculationPropertiesNode(ep); if (xmlNode == null || xmlNode.Attributes?.Count == 0) return null; return xmlNode.Attributes;
		}
		private XmlNode GetCalculationPropertiesNode(ExcelPackage ep)
		{
			var xmlNodeList = ep.Workbook.WorkbookXml.GetElementsByTagName(CalculationPropertiesNodeName);

			if (xmlNodeList.Count == 0)
				return null;

			return xmlNodeList[0];
		}

		public void CreateVBAProject()
		{
			p.Workbook.CreateVBAProject();
		}

		public void AddVBACode(string moduleName, string vbaCode)
		{
			var m = p.Workbook.VbaProject.Modules.AddModule(moduleName);
			m.Code = vbaCode;
		}

		public void Sheet(int index, Action inner)
		{
			s = p.Workbook.Worksheets[index];
			Sheet(inner);
		}

		public void Sheet(int index, string name, Action inner)
		{
			s = p.Workbook.Worksheets[index];
			s.Name = name;
			Sheet(inner);
		}

		public List<string> WorkSheetsNames()
		{
			return p.Workbook.Worksheets.Select(x => x.Name.ToLower()).ToList();
		}

		/// <summary>
		/// Расположение итоговых данных
		/// Итоги в строках под данными 
		/// </summary>
		/// <param name="flag"></param>
		/// <param name="index">Индекс Sheet</param>
		public void SetOutLineSummaryBelow(int index, bool flag)
		{
			p.Workbook.Worksheets[index].OutLineSummaryBelow = flag;
		}

		/// <summary>
		/// Расположение итоговых данных
		/// Итоги в строках под данными 
		/// </summary>
		/// <param name="flag"></param>
		public void SetOutLineSummaryBelowAllSheet(bool flag)
		{
			for (int i = 0; i < p.Workbook.Worksheets.Count; i++)
			{
				SetOutLineSummaryBelow(i, flag);
			}
		}

		public void SetActiveSheet(int pos)
		{
			p.Workbook.View.ActiveTab = pos;
			//s = p.Workbook.Worksheets[pos];	
		}
		public void GroupColumns(int startCol, int finishCol, int sheetIndex, bool isCollapsed)
		{		
			for (int i = startCol; i <= finishCol; i++)
			{
				p.Workbook.Worksheets[sheetIndex].Column(i).OutlineLevel = 1;
				p.Workbook.Worksheets[sheetIndex].Column(i).Collapsed = isCollapsed;
			}
						
		}

		public void Sheet(string name, Action inner, Action<ExcelWorksheet> style = null)
		{
			s = p.Workbook.Worksheets.Add(name);
			s.Cells.Style.Font.Name = "Times New Roman";
			style?.Invoke(s);
			Sheet(inner);
		}

		void Sheet(Action inner)
		{
			totalColumns = 1;
			divs = new HashSet<int>();
			divs_nomerge = new HashSet<int>();
			r = 1;
			c = 1;
			startcol = 1;
			inner();
			divs.RemoveWhere(i => divs_nomerge.Contains(i));
			foreach (int row in divs)
			{
				s.Cells[row, startcol, row, totalColumns].Merge = true;
				s.Cells[row, startcol].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
			}
			for (int i = 1; i < totalColumns; i++)
			{
				if (s.Column(i).Width == 0)
					s.Column(i).AutoFit();
			}
		}

		public void SetAutoFit(int col, double width = double.NaN)
		{
			var column = s.Column(col);
			if (double.IsNaN(width))
				column.AutoFit();
			else
				column.AutoFit(width);
		}

		public void SetWidth(int col, double width)
		{
			s.Column(col).Width = width;
		}

		public void SetHeight(int row, double height)
		{
			s.Row(row).Height = height;
		}

		public void SetOddFooterCenteredNumPage(Func<string, string, string> content)
		{
			foreach(var list in s.Workbook.Worksheets)
				s.HeaderFooter.OddFooter.CenteredText = content(ExcelHeaderFooter.PageNumber, ExcelHeaderFooter.NumberOfPages);
		}

		[Obsolete]
		public void SetOddFooterCenterNumPage()
		{
			foreach(var list in s.Workbook.Worksheets)
				list.HeaderFooter.OddFooter.CenteredText = $"Страница {ExcelHeaderFooter.PageNumber} из {ExcelHeaderFooter.NumberOfPages}";
		}

		public void Div(Action<IContentItemAttributes> attributes, Action inner)
		{
			inner?.Invoke();
			var cia = new CIAttributes();
			cia.SetWriter(this);
			attributes?.Invoke(cia);
			cia.Apply(s.Cells[r, c]);
			divs.Add(r);
			r++;
			c = startcol;
		}

		public void Table(Action<IContentItemAttributes> attributes = null, Action inner = null)
		{
			int fromRow = r;
			int cols = totalColumns;
			totalColumns = 1;
			inner?.Invoke();
			var cia = new CIAttributes();
			cia.SetWriter(this);
			attributes?.Invoke(cia);
			if (r > fromRow)
				cia.Apply(s.Cells[fromRow, startcol, r - 1, totalColumns]);
			if (cols > totalColumns)
				totalColumns = cols;
		}

		public void Td(Action<ITdAttributes> attributes = null, Action inner = null)
		{
			var tda = new TdAttributes();
			tda.SetWriter(this);
			attributes?.Invoke(tda);
			tda.Apply();
			inner?.Invoke();
			while (s.Cells[r, c + 1].Merge)
				c++;
			if (c > totalColumns)
				totalColumns = c;
			c++;
		}

		public void Th(Action<IThAttributes> attributes = null, Action inner = null)
		{
			var tha = new ThAttributes();
			tha.SetWriter(this);
			attributes?.Invoke(tha);
			tha.Apply();
			inner?.Invoke();
			do
			{
				if (c > totalColumns)
					totalColumns = c;
				c++;
			} while (s.Cells[r, c].Merge);
		}

		public void Tr(Action<IContentItemAttributes> attributes, Action inner = null)
		{
			inner?.Invoke();
			var cia = new CIAttributes();
			cia.SetWriter(this);
			attributes?.Invoke(cia);
			cia.Apply(s.Cells[r, startcol, r, c - 1]);
			r++;
			c = startcol;
			while (s.Cells[r, c].Merge)
			{
				if (c < totalColumns)
					c++;
				else
				{
					r++;
					c = startcol;
				}
			}
		}

		public void Write(string text)
		{
			if (text != null)
			{
				text = text.Replace("&nbsp;", " ");
				string formula = s.Cells[r, c].FormulaR1C1;
				s.Cells[r, c].Value = text;
				if ((formula ?? "") != "")
					s.Cells[r, c].FormulaR1C1 = formula;
			}
		}

		public string Write(IEnumerable<string> text)
		{
			if (text != null)
			{
				return String.Join(NewLine, text);
			}
			return null;
		}

		public void WriteFormula(string formula, bool r1c1format = true, bool calculate = false)
		{
			if (formula != null)
			{
				if (r1c1format)
					s.Cells[r, c].FormulaR1C1 = formula;
				else
					s.Cells[r, c].Formula = formula;

				if (calculate)
					s.Cells[r, c].Calculate();
			}
		}

		public void Style(string range, Action<ExcelRange> cells)
		{
			cells(s.Cells[range]);
		}

		public void Style(int fromRow, int fromCol, int toRow, int toCol, Action<ExcelRange> cells)
		{
			cells(s.Cells[fromRow, fromCol, toRow, toCol]);
		}

		public void Move(int row, int col)
		{
			r = row;
			c = col;
			startcol = col;
		}

		public byte[] GetBytes()
		{
			using (var msout = new System.IO.MemoryStream())
			{
				p.Workbook.FullCalcOnLoad = true;
				p.SaveAs(msout);
				msout.Flush();
				return msout.ToArray();
			}
		}

		public void Dispose()
		{
			p?.Dispose();
		}

		Dictionary<ExcelRange, Tuple<decimal, string>> values = new Dictionary<ExcelRange, Tuple<decimal, string>>();
		public void Td(Action<ITdAttributes> attributes, decimal? n, string format)
		{
			Td(attributes, () => {
				string formula = s.Cells[r, c].FormulaR1C1;
				s.Cells[r, c].Value = n;
				if ((formula ?? "") != "")
				{
					s.Cells[r, c].FormulaR1C1 = formula;
					if (n.HasValue)
						values[s.Cells[r, c]] = new Tuple<decimal, string>(n.Value, format);
				}
				if (format == "n0")
					s.Cells[r, c].Style.Numberformat.Format = "#,##0";
				if (format == "n1")
					s.Cells[r, c].Style.Numberformat.Format = "#,##0.0";
				if (format == "n2")
					s.Cells[r, c].Style.Numberformat.Format = "#,##0.00";
				if (format == "n3")
					s.Cells[r, c].Style.Numberformat.Format = "#,##0.000";
				if (format == "n4")
					s.Cells[r, c].Style.Numberformat.Format = "#,##0.0000";
				if (format == "n5")
					s.Cells[r, c].Style.Numberformat.Format = "#,##0.00000";
				if (format == "n6")
					s.Cells[r, c].Style.Numberformat.Format = "#,##0.000000";
				if (format == "n7")
					s.Cells[r, c].Style.Numberformat.Format = "#,##0.0000000";
				if (format == "n8")
					s.Cells[r, c].Style.Numberformat.Format = "#,##0.00000000";
				if (format == "n9")
					s.Cells[r, c].Style.Numberformat.Format = "#,##0.000000000";
				if (format == "n10")
					s.Cells[r, c].Style.Numberformat.Format = "#,##0.0000000000";
			});
		}

		public void Th(Action<IThAttributes> attributes, decimal? n, string format)
		{
			Th(attributes, () => {
				string formula = s.Cells[r, c].FormulaR1C1;
				s.Cells[r, c].Value = n;
				if ((formula ?? "") != "")
				{
					s.Cells[r, c].FormulaR1C1 = formula;
					if (n.HasValue)
						values[s.Cells[r, c]] = new Tuple<decimal, string>(n.Value, format);
				}
				if (format == "n0")
					s.Cells[r, c].Style.Numberformat.Format = "#,##0";
				if (format == "n1")
					s.Cells[r, c].Style.Numberformat.Format = "#,##0.0";
				if (format == "n2")
					s.Cells[r, c].Style.Numberformat.Format = "#,##0.00";
				if (format == "n3")
					s.Cells[r, c].Style.Numberformat.Format = "#,##0.000";
				if (format == "n4")
					s.Cells[r, c].Style.Numberformat.Format = "#,##0.0000";
				if (format == "n5")
					s.Cells[r, c].Style.Numberformat.Format = "#,##0.00000";
				if (format == "n6")
					s.Cells[r, c].Style.Numberformat.Format = "#,##0.000000";
				if (format == "n7")
					s.Cells[r, c].Style.Numberformat.Format = "#,##0.0000000";
			});
		}
		public void Calculate()
		{
			p.Workbook.Calculate();
		}

		public List<string> Validate()
		{
			Calculate();
			List<string> errors = new List<string>();
			foreach (var kv in values)
			{
				if (kv.Key.Value is OfficeOpenXml.eErrorType || !(kv.Key.Value is double || kv.Key.Value is decimal))
					errors.Add($"Значение в ячейке {kv.Key.FullAddress} не вычислено: \"{kv.Key.Text}\", формула: {kv.Key.Formula}");
				else if (kv.Key.Value is double && ((double)kv.Key.Value).ToString(kv.Value.Item2) != kv.Value.Item1.ToString(kv.Value.Item2))
					errors.Add($"Вычисленное значение в ячейке {kv.Key.FullAddress} \"{((double)kv.Key.Value).ToString(kv.Value.Item2)}\" не равно \"{kv.Value.Item1.ToString(kv.Value.Item2)}\", формула: {kv.Key.Formula}");
				else if (kv.Key.Value is decimal && ((decimal)kv.Key.Value).ToString(kv.Value.Item2) != kv.Value.Item1.ToString(kv.Value.Item2))
					errors.Add($"Вычисленное значение в ячейке {kv.Key.FullAddress} \"{((decimal)kv.Key.Value).ToString(kv.Value.Item2)}\" не равно \"{kv.Value.Item1.ToString(kv.Value.Item2)}\", формула: {kv.Key.Formula}");
			}
			return errors;
		}

		class TdAttributes : ITdAttributes
		{
			ExcelWriter writer;
			int colSpan = 1;
			int rowSpan = 1;
			bool right;
			ICssStyleDeclaration style;
			string formula;
			double width;
			string[] classes = new string[0];

			public ITdAttributes Class(string value, bool replaceExisting = false)
			{
				if (value == null)
				{
					if (replaceExisting)
						classes = new string[0];
					return this;
				}
				if (value == "r")
					right = true;
				else
					classes = classes.ToList().Union(value.Split(' ')).ToArray();
				return this;
			}

			public ITdAttributes ColSpan(int value)
			{
				if (value > 1) colSpan = value;
				return this;
			}

			public ITdAttributes Extended<TValue>(string key, TValue value)
			{
				if (key == Xlsx.FormulaR1C1)
					formula = value as string;
				else if (key == Xlsx.AutoFilter)
					writer.s.Cells[writer.r, writer.c].AutoFilter = true;
				else if (key == Xlsx.OutlineLevel)
					writer.s.Row(writer.r).OutlineLevel = Convert.ToInt32(value);
				else if (key == Xlsx.Collapsed)
					writer.s.Row(writer.r).Collapsed = Convert.ToBoolean(value);
				else if (key == Xlsx.XlsxHeight)
					writer.s.Row(writer.r).Height = Convert.ToDouble(value);
				return this;
			}

			public ITdAttributes ID<TValue>(TValue value)
			{
				return this;
			}

			public ITdAttributes RowSpan(int value)
			{
				if (value > 1) rowSpan = value;
				return this;
			}

			public void SetWriter(IContentWriter writer)
			{
				this.writer = (ExcelWriter)writer;
			}

			public ITdAttributes Style(string value, bool replaceExisting = false)
			{
				var ss = writer.cssParser.ParseStyleSheet(".someClass{" + value + "}");
				style = (ss.Rules.First() as ICssStyleRule).Style;
				return this;
			}

			public void Apply()
			{
				writer.s.Cells[writer.r, writer.c].Style.WrapText = true;
				if (rowSpan > 1 || colSpan > 1)
					writer.s.Cells[writer.r, writer.c, writer.r + rowSpan - 1, writer.c + colSpan - 1].Merge = true;
				if (right)
					writer.s.Cells[writer.r, writer.c].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
				if (style?.GetTextAlign() == "center")
					writer.s.Cells[writer.r, writer.c].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
				if (style?.GetTextAlign() == "right")
					writer.s.Cells[writer.r, writer.c].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
				if (style?.GetTextAlign() == "left")
					writer.s.Cells[writer.r, writer.c].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
				if (style?.GetVerticalAlign() == "middle")
					writer.s.Cells[writer.r, writer.c].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
				if (style?.GetVerticalAlign() == "top")
					writer.s.Cells[writer.r, writer.c].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
				if (style?.GetFontWeight() == "bold")
					writer.s.Cells[writer.r, writer.c].Style.Font.Bold = true;
				if (style?.GetFontStyle() == "italic")
					writer.s.Cells[writer.r, writer.c].Style.Font.Italic = true;
				if (style?.GetWhiteSpace() == "nowrap")
					writer.s.Cells[writer.r, writer.c].Style.WrapText = false;
				if ((style?.GetFontSize() ?? "") != "")
					writer.s.Cells[writer.r, writer.c].Style.Font.Size = style.GetFontSize().Replace("pt", "").Trim().ToInt32(0);
				if (width > 0)
					writer.s.Column(writer.c).Width = width;
				if ((style?.GetPaddingLeft() ?? "") != "")
					writer.s.Cells[writer.r, writer.c].Style.Indent = style.GetPaddingLeft().Replace("px", "").Trim().ToInt32(0) / 10;
				if (string.IsNullOrEmpty(style?.GetFontFamily()) == false)
					writer.s.Cells[writer.r, writer.c].Style.Font.Name = style?.GetFontFamily();
				if (formula != null)
				{
					writer.s.Cells[writer.r, writer.c].FormulaR1C1 = formula;
				}
				foreach (var cls in classes)
					if (writer.classes.ContainsKey(cls))
						writer.classes[cls](writer.s.Cells[writer.r, writer.c]);
				if ((style?.GetBackgroundColor() ?? "") != "")
				{
					writer.s.Cells[writer.r, writer.c].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
					const string pattern = @"rgba?[(](\d{1,3})\s?,\s?(\d{1,3})\s?,\s?(\d{1,3})\s?,\s?(\d{1,3})\s?[)]";
					var match = Regex.Match(style?.GetBackgroundColor(), pattern);
					var r = byte.Parse(match.Groups[1].Value);
					var g = byte.Parse(match.Groups[2].Value);
					var b = byte.Parse(match.Groups[3].Value);
					writer.s.Cells[writer.r, writer.c].Style.Fill.BackgroundColor.SetColor(0, r, g, b);
				}
				if ((style?.GetColor() ?? "") != "")
				{
					const string pattern = @"rgba?[(](\d{1,3})\s?,\s?(\d{1,3})\s?,\s?(\d{1,3})\s?,\s?(\d{1,3})\s?[)]";
					var match = Regex.Match(style?.GetColor(), pattern);
					var r = byte.Parse(match.Groups[1].Value);
					var g = byte.Parse(match.Groups[2].Value);
					var b = byte.Parse(match.Groups[3].Value);
					writer.s.Cells[writer.r, writer.c].Style.Font.Color.SetColor(0, r, g, b);
				}
			}

			public ITdAttributes Title(string value)
			{
				return this;
			}

			public ITdAttributes Data<TValue>(string key, TValue value)
			{
				return this;
			}
		}

		class ThAttributes : IThAttributes
		{
			ExcelWriter writer;
			int colSpan = 1;
			int rowSpan = 1;
			bool right;
			ICssStyleDeclaration style;
			string formula;
			double width;
			string[] classes = new string[0];

			public IThAttributes Class(string value, bool replaceExisting = false)
			{
				if (value == "r")
					right = true;
				else
					classes = classes.ToList().Union(value.Split(' ')).ToArray();
				return this;
			}

			public IThAttributes ColSpan(int value)
			{
				if (value > 1) colSpan = value;
				return this;
			}

			public IThAttributes Extended<TValue>(string key, TValue value)
			{
				if (key == Xlsx.FormulaR1C1)
					formula = value as string;
				else if (key == Xlsx.AutoFilter)
					writer.s.Cells[writer.r, writer.c].AutoFilter = true;
				else if (key == Xlsx.OutlineLevel)
					writer.s.Row(writer.r).OutlineLevel = Convert.ToInt32(value);
				return this;
			}

			public IThAttributes ID<TValue>(TValue value)
			{
				return this;
			}

			public IThAttributes RowSpan(int value)
			{
				if (value > 1) rowSpan = value;
				return this;
			}

			public void SetWriter(IContentWriter writer)
			{
				this.writer = (ExcelWriter)writer;
			}

			public IThAttributes Style(string value, bool replaceExisting = false)
			{
				var ss = writer.cssParser.ParseStyleSheet(".someClass{" + value + "}");
				style = (ss.Rules.First() as ICssStyleRule).Style;
				return this;
			}

			public void Apply()
			{
				writer.s.Cells[writer.r, writer.c].Style.WrapText = true;
				writer.s.Cells[writer.r, writer.c].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
				writer.s.Cells[writer.r, writer.c].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
				writer.s.Cells[writer.r, writer.c].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
				writer.s.Cells[writer.r, writer.c].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;

				if (rowSpan > 1 || colSpan > 1)
					writer.s.Cells[writer.r, writer.c, writer.r + rowSpan - 1, writer.c + colSpan - 1].Merge = true; ;
				if (right)
					writer.s.Cells[writer.r, writer.c].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
				if (style?.GetTextAlign() == "center")
					writer.s.Cells[writer.r, writer.c].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
				if (style?.GetTextAlign() == "right")
					writer.s.Cells[writer.r, writer.c].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
				if (style?.GetTextAlign() == "left")
					writer.s.Cells[writer.r, writer.c].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
				if (style?.GetVerticalAlign() == "middle")
					writer.s.Cells[writer.r, writer.c].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
				if (style?.GetVerticalAlign() == "top")
					writer.s.Cells[writer.r, writer.c].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
				if ((style?.GetPaddingLeft() ?? "") != "")
					writer.s.Cells[writer.r, writer.c].Style.Indent = style.GetPaddingLeft().Replace("px", "").Trim().ToInt32(0) / 10;
				if (style?.GetFontWeight() == "bold")
					writer.s.Cells[writer.r, writer.c].Style.Font.Bold = true;
				if (style?.GetFontStyle() == "italic")
					writer.s.Cells[writer.r, writer.c].Style.Font.Italic = true;
				if (width > 0)
					writer.s.Column(writer.c).Width = width;
				if (formula != null)
				{
					writer.s.Cells[writer.r, writer.c].FormulaR1C1 = formula;
				}
				foreach (var cls in classes)
					if (writer.classes.ContainsKey(cls))
						writer.classes[cls](writer.s.Cells[writer.r, writer.c]);
			}

			public IThAttributes Title(string value)
			{
				return this;
			}

			public IThAttributes Data<TValue>(string key, TValue value)
			{
				return this;
			}
		}

		class CIAttributes : IContentItemAttributes
		{
			ExcelWriter writer;
			ICssStyleDeclaration style;
			string formula;
			string[] classes = new string[0];

			public IContentItemAttributes Class(string value, bool replaceExisting = false)
			{
				classes = value.Split(' ');
				return this;
			}

			public IContentItemAttributes Extended<TValue>(string key, TValue value)
			{
				if (key == Xlsx.FormulaR1C1)
					formula = value as string;
				else if (key == Xlsx.XlsxHeight)
					writer.s.Row(writer.r).Height = Convert.ToInt32(value);
				else if (key == Xlsx.NoMerge)
					writer.divs_nomerge.Add(writer.r);
				return this;
			}

			public IContentItemAttributes ID<TValue>(TValue value)
			{
				return this;
			}

			public void SetWriter(IContentWriter writer)
			{
				this.writer = (ExcelWriter)writer;
			}

			public IContentItemAttributes Style(string value, bool replaceExisting = false)
			{
				var ss = writer.cssParser.ParseStyleSheet(".someClass{" + value + "}");
				style = (ss.Rules.First() as ICssStyleRule).Style;
				return this;
			}

			public void Apply(ExcelRange range)
			{
				if (style?.GetTextAlign() == "center")
					range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
				if (style?.GetTextAlign() == "right")
					range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
				if (style?.GetTextAlign() == "left")
					range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
				if (style?.GetFontWeight() == "bold")
					range.Style.Font.Bold = true;
				if (style?.GetFontStyle() == "italic")
					range.Style.Font.Italic = true;
				if ((style?.GetBackgroundColor() ?? "") != "")
				{
					range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
					const string pattern = @"rgba?[(](\d{1,3})\s?,\s?(\d{1,3})\s?,\s?(\d{1,3})\s?,\s?(\d{1,3})\s?[)]";
					var match = Regex.Match(style?.GetBackgroundColor(), pattern);
					var r = byte.Parse(match.Groups[1].Value);
					var g = byte.Parse(match.Groups[2].Value);
					var b = byte.Parse(match.Groups[3].Value);
					range.Style.Fill.BackgroundColor.SetColor(0, r, g, b);
				}
				foreach (var cls in classes)
					if (writer.classes.ContainsKey(cls))
						writer.classes[cls](range);
				if (formula != null)
				{
					range.FormulaR1C1 = formula;
				}
			}

			public IContentItemAttributes Title(string value)
			{
				return this;
			}

			public IContentItemAttributes Data<TValue>(string key, TValue value)
			{
				return this;
			}
		}
	}
	public static class Xlsx
	{
		public const string FormulaR1C1 = "FormulaR1C1";
		public const string AutoFilter = "AutoFilter";
		public const string OutlineLevel = "OutlineLevel";
		public const string XlsxHeight = "SetHeight";
		public const string NoMerge = "NoMerge";
		public const string Collapsed = "Collapsed";
	}

	public static class XlsxExtensions
	{
		public static IContentItemAttributes<T> XlsxFormula<T>(this IContentItemAttributes<T> a, string formula)
			where T : IContentItemAttributes<T>
		{
			a.Extended(Xlsx.FormulaR1C1, formula);
			return a;
		}

		public static void XlsxEnableAutoFilter<T>(this IContentItemAttributes<T> a)
			where T : IContentItemAttributes<T>
		{
			a.Extended(Xlsx.AutoFilter, "1");
		}

		public static void XlsxOutlineLevel<T>(this IContentItemAttributes<T> a, int level, bool collapsed = false)
			where T : IContentItemAttributes<T>
		{
			a.Extended(Xlsx.OutlineLevel, level);
			if (collapsed)
				a.Extended(Xlsx.Collapsed, true);
		}

		public static void XlsxOutlineLevel<T>(this IContentItemAttributes<T> a, int[] level)
			where T : IContentItemAttributes<T>
		{
			foreach (var currLev in level)
				if (currLev != 0)
					a.Extended(Xlsx.OutlineLevel, currLev);
		}

		public static void XlsxSetHeight<T>(this IContentItemAttributes<T> a, double px)
			where T : IContentItemAttributes<T>
		{
			a.Extended(Xlsx.XlsxHeight, px);
		}

		public static void XlsxNoMerge<T>(this IContentItemAttributes<T> a)
			where T : IContentItemAttributes<T>
		{
			a.Extended(Xlsx.NoMerge, "1");
		}
	}
}
