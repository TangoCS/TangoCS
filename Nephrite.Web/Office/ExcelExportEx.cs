using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Nephrite.Web.Office
{
	public class Columns<O, T> : IEnumerable<IColumn>
	{
		protected Func<O, IEnumerable<T>> getObjects;
		
		public Columns(Func<O, IEnumerable<T>> objects)
		{
			getObjects = objects;
		}

		protected List<IColumn<T>> columns = new List<IColumn<T>>();

		#region AddColumn
		public void AddColumn(string header, Func<T, string> selector)
		{
			columns.Add(new Column<T>
			{
				Header = header,
				Selector = o => new string[] { selector.Invoke(o) }
			});
		}

		public void AddColumn(string header, Func<T, IEnumerable<string>> selector)
		{
			columns.Add(new Column<T>
			{
				Header = header,
				Selector = selector
			});
		}

		public void AddColumn(string header, Func<T, int?> selector)
		{
			columns.Add(new Column<T>
			{
				Header = header,
				Selector = o => new string[] { selector.Invoke(o).ToString() }
			});
		}

		public void AddColumn(string header, Func<T, decimal?> selector)
		{
			columns.Add(new Column<T>
			{
				Header = header,
				Selector = o =>
					{
						var d = selector.Invoke(o);
						return d.HasValue ? new string[] { d.Value.ToString(CultureInfo.InvariantCulture) } : new string[] { "" };
					}
			});
		}

		public void AddColumn(string header, Func<T, bool?> selector)
		{
			columns.Add(new Column<T>
			{
				Header = header,
				Selector = o =>
				{
					var b = selector.Invoke(o);
					if (b.HasValue)
						return new string[] { b.Value ? "да" : "нет" };
					return new string[] { "" };
				}
			});
		}
		#endregion

		IEnumerator<IColumn> IEnumerable<IColumn>.GetEnumerator()
		{
			return columns.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return columns.GetEnumerator();
		}
	}

	public class Export<T> : Columns<int, T>
	{
		public Export(IEnumerable<T> objects) : base(o => objects)
		{
		}
		
		public ColumnGroup<T, C> AddColumnGroup<C>(string header, Func<T, IEnumerable<C>> items)
		{
			var cg = new ColumnGroup<T, C>(items);
			cg.Header = header;
			columns.Add(cg);
			return cg;
		}

		public int StartRow = 1;
		public string ListName = "Лист1";
		public Action<OfficeOpenXml.ExcelRange> BeforeExport;

		public void Return(string fileName)
		{
			using (var ms = new MemoryStream())
			{
				using (var p = new OfficeOpenXml.ExcelPackage(ms))
				{
					var sheet = p.Workbook.Worksheets.Add(ListName);
					// 
					if (BeforeExport != null)
						BeforeExport(sheet.Cells);
					// Создать заголовки столбцов
					int row = StartRow, col = 1;
					int headerRows = columns.Where(o => o.Visible).Max(o => o.HeaderRows);
					foreach (var c in columns)
					{
						if (!c.Visible)
							continue;
						col = c.WriteHeader(sheet.Cells, row, col);

						if (c.HeaderRows == 1 && headerRows == 2)
							sheet.Cells[StartRow, col - 1, StartRow + 1, col - 1].Merge = true;
					}
					row += headerRows;
					sheet.Cells[StartRow, 1, row - 1, col - 1].Style.Font.Bold = true;
					foreach (var o in getObjects(0))
					{
						int rows = 1;
						col = 1;
						foreach (var c in columns)
						{
							if (!c.Visible)
								continue;
							col = c.WriteValue(sheet.Cells, o, row, col, ref rows);
						}
						row += rows;
					}
					sheet.Cells[StartRow, 1, row, col - 1].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
					sheet.Cells[StartRow, 1, row - 1, col].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
					for (int i = 1; i < col; i++)
						sheet.Column(i).AutoFit();
					p.Save();
				}
				ms.Flush();
				WordDocumentGenerator.Download(fileName, ms.ToArray());
			}
		}
	}

	public class Column<T> : IColumn<T>
	{
		public string Header { get; set; }
		public Func<T, IEnumerable<string>> Selector;
		public int WriteHeader(OfficeOpenXml.ExcelRange range, int rowNumber, int colNumber)
		{
			range[rowNumber, colNumber].Value = Header;
			return colNumber + 1;
		}
		public int WriteValue(OfficeOpenXml.ExcelRange range, T obj, int row, int col, ref int rows)
		{
			int r = 0;
			try
			{
				foreach (var s in Selector(obj))
				{
					range[row + r, col].Value = s;
					r++;
				}
			}
			catch (NullReferenceException) { }
			if (r > rows)
				rows = r;
			return col + 1;
		}
		public int HeaderRows
		{
			get { return 1; }
		}
		public bool Visible { get; set; }
		public Column()
		{
			Visible = true;
		}
	}

	public class ColumnGroup<T, C> : Columns<T, C>, IColumn<T>, IColumnGroup
	{
		public string Header { get; set; }
		public ColumnGroup(Func<T, IEnumerable<C>> selector) : base(selector)
		{
			Visible = true;
		}
		public int WriteHeader(OfficeOpenXml.ExcelRange range, int rowNumber, int colNumber)
		{
			int colCount = 0;
			range[rowNumber, colNumber].Value = Header;
			range[rowNumber, colNumber].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
			for (int i = 0; i < columns.Count; i++)
			{
				if (columns[i].Visible)
				{
					range[rowNumber + 1, colNumber + colCount].Value = columns[i].Header;
					colCount++;
				}
			}
			range[rowNumber, colNumber, 1, colNumber + colCount - 1].Merge = true;
			return colNumber + colCount;
		}
		public int WriteValue(OfficeOpenXml.ExcelRange range, T obj, int row, int col, ref int rows)
		{
			int r = 0;
			try
			{
				foreach (var child in getObjects(obj))
				{
					if (child == null)
						continue;
					int colCount = 0;
					for (int c = 0; c < columns.Count;c++ )
					{
						if (columns[c].Visible)
						{
							columns[c].WriteValue(range, child, row + r, col + colCount, ref rows);
							colCount++;
						}
					}
					r++;
				}
			}
			catch (NullReferenceException)
			{
			}
			col += columns.Count(o => o.Visible); 
			if (r > rows)
				rows = r;
			return col;
		}
		public int HeaderRows
		{
			get { return 2; }
		}
		public bool Visible { get; set; }
	}

	public interface IColumn<T> : IColumn
	{
		new string Header { get; set; }
		int WriteHeader(OfficeOpenXml.ExcelRange range, int rowNumber, int colNumber);
		int WriteValue(OfficeOpenXml.ExcelRange range, T obj, int row, int col, ref int rows);
		int HeaderRows { get; }
	}
	public interface IColumn
	{
		string Header { get; }
		bool Visible { get; set; }
	}
	public interface IColumnGroup : IColumn, IEnumerable<IColumn>
	{
	}
}
