using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using OfficeOpenXml;
namespace Tango.Excel
{
	public class ExcelWriter : IContentWriter, IDisposable
    {
		public string NewLine => "\n";

		ExcelPackage p;
        ExcelWorksheet s;
        AngleSharp.Parser.Css.CssParser cssParser = new AngleSharp.Parser.Css.CssParser();
        int r = 1;
        int c = 1;
        int startcol = 1;
        int totalColumns = 1;
        List<int> divs;

        Dictionary<string, Action<ExcelRange>> classes = new Dictionary<string, Action<ExcelRange>>();

        public void SetClassAction(string @class, Action<ExcelRange> cells)
        {
            classes[@class] = cells;
        }

        public ExcelWriter()
        {
            p = new ExcelPackage();            
        }

        public ExcelWriter(System.IO.Stream template)
        {
            p = new ExcelPackage(template);
        }

        public void Sheet(int index, Action inner)
        {
            Sheet(p.Workbook.Worksheets[index].Name, inner);
        }

        public void Sheet(string name, Action inner)
        {
            s = p.Workbook.Worksheets[name] ?? p.Workbook.Worksheets.Add(name);
            totalColumns = 1;
            divs = new List<int>();
            r = 1;
            c = 1;
            startcol = 1;
            inner();
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

        public void SetWidth(int col, double width)
        {
            s.Column(col).Width = width;
        }

        public void SetHeight(int row, double height)
        {
            s.Row(row).Height = height;
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
            if (c > totalColumns)
                totalColumns = c;
            c++;
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
                text = text.Replace("&nbsp;", "");
                string formula = s.Cells[r, c].FormulaR1C1;
                if (decimal.TryParse(text.Replace(" ", "").Replace(",", "."), out decimal d))
                {
                    s.Cells[r, c].Value = d;
                    s.Cells[r, c].Style.Numberformat.Format = Regex.Replace(text.Replace(",", ".").Replace("-", ""), @"\d", "0");
                }
                else
                    s.Cells[r, c].Value = text;
                if ((formula ?? "") != "")
                    s.Cells[r, c].FormulaR1C1 = formula;
            }
        }

        public void Style(string range, Action<ExcelRange> cells)
        {
            cells(s.Cells[range]);
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

        class TdAttributes : ITdAttributes
        {
            ExcelWriter writer;
            int colSpan = 1;
            int rowSpan = 1;
            bool right;
            AngleSharp.Dom.Css.ICssStyleDeclaration style;
            string formula;
            double width;

            public ITdAttributes Class(string value, bool replaceExisting = false)
            {
                if (value == "r")
                    right = true;
                else
                    throw new NotImplementedException();
                return this;
            }

            public ITdAttributes ColSpan(int value)
            {
                if (value > 1) colSpan = value;
                return this;
            }

            public ITdAttributes Extended<TValue>(string key, TValue value)
            {
                if (key == "FormulaR1C1")
                    formula = value as string;
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
                var ss = writer.cssParser.ParseStylesheet(".someClass{" + value + "}");
                style = (ss.Rules.First() as AngleSharp.Dom.Css.ICssStyleRule).Style;
                return this;
            }

            public void Apply()
            {
                writer.s.Cells[writer.r, writer.c].Style.WrapText = true;
                if (rowSpan > 1 || colSpan > 1)
                    writer.s.Cells[writer.r, writer.c, writer.r + rowSpan - 1, writer.c + colSpan - 1].Merge = true;
                if (right)
                    writer.s.Cells[writer.r, writer.c].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                if (style?.TextAlign == "center")
                    writer.s.Cells[writer.r, writer.c].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                if (style?.FontWeight == "bold")
                    writer.s.Cells[writer.r, writer.c].Style.Font.Bold = true;
                if (style?.FontStyle == "italic")
                    writer.s.Cells[writer.r, writer.c].Style.Font.Italic = true;
                if (style?.WhiteSpace == "nowrap")
                    writer.s.Cells[writer.r, writer.c].Style.WrapText = false;
                if (width > 0)
                    writer.s.Column(writer.c).Width = width;
                if (formula != null)
                {
                    writer.s.Cells[writer.r, writer.c].FormulaR1C1 = formula;
                }
            }
        }

        class ThAttributes : IThAttributes
        {
            ExcelWriter writer;
            int colSpan = 1;
            int rowSpan = 1;
            bool right;
            AngleSharp.Dom.Css.ICssStyleDeclaration style;
            string formula;
            double width;

            public IThAttributes Class(string value, bool replaceExisting = false)
            {
                if (value == "r")
                    right = true;
                else
                    throw new NotImplementedException();
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
                var ss = writer.cssParser.ParseStylesheet(".someClass{" + value + "}");
                style = (ss.Rules.First() as AngleSharp.Dom.Css.ICssStyleRule).Style;
                return this;
            }

            public void Apply()
            {
                writer.s.Cells[writer.r, writer.c].Style.WrapText = true;
                writer.s.Cells[writer.r, writer.c].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                writer.s.Cells[writer.r, writer.c + 1].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                writer.s.Cells[writer.r, writer.c].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                writer.s.Cells[writer.r + 1, writer.c].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;

                if (rowSpan > 1 || colSpan > 1)
                    writer.s.Cells[writer.r, writer.c, writer.r + rowSpan - 1, writer.c + colSpan - 1].Merge = true; ;
                if (right)
                    writer.s.Cells[writer.r, writer.c].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                if (style?.TextAlign == "center")
                    writer.s.Cells[writer.r, writer.c].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                if (style?.FontWeight == "bold")
                    writer.s.Cells[writer.r, writer.c].Style.Font.Bold = true;
                if (style?.FontStyle == "italic")
                    writer.s.Cells[writer.r, writer.c].Style.Font.Italic = true;
                if (width > 0)
                    writer.s.Column(writer.c).Width = width;
                if (formula != null)
                {
                    writer.s.Cells[writer.r, writer.c].FormulaR1C1 = formula;
                }
            }
        }

        class CIAttributes : IContentItemAttributes
        {
            ExcelWriter writer;
            AngleSharp.Dom.Css.ICssStyleDeclaration style;
            string formula;
            string[] classes = new string[0];

            public IContentItemAttributes Class(string value, bool replaceExisting = false)
            {
                classes = value.Split(' ');
                return this;
            }

            public IContentItemAttributes Extended<TValue>(string key, TValue value)
            {
                if (key == "FormulaR1C1")
                    formula = value as string;
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
                var ss = writer.cssParser.ParseStylesheet(".someClass{" + value + "}");
                style = (ss.Rules.First() as AngleSharp.Dom.Css.ICssStyleRule).Style;
                return this;
            }

            public void Apply(ExcelRange range)
            {
                if (style?.TextAlign == "center")
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                if (style?.FontWeight == "bold")
                    range.Style.Font.Bold = true;
                if (style?.FontStyle == "italic")
                    range.Style.Font.Italic = true;
                foreach (var cls in classes)
                    if (writer.classes.ContainsKey(cls))
                        writer.classes[cls](range);
                if (formula != null)
                {
                    range.FormulaR1C1 = formula;
                }
            }
        }
    }
    public static class Xlsx
    {
        public const string FormulaR1C1 = "FormulaR1C1";
    }
}
