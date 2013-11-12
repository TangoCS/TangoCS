using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using Nephrite.Web;
using System.Text;
using System.Threading;
using System.Text.RegularExpressions;
using Nephrite.Metamodel.Model;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.IO;
using System.Drawing;
using OfficeOpenXml.Style.XmlAccess;
using Nephrite.Web.SettingsManager;

namespace Nephrite.Metamodel.Controls
{
	public partial class ListExport : System.Web.UI.UserControl
	{
		public string ObjectTypeSysName { get; set; }
		public string FileName { get; set; }
		public IQueryable Objects { get; set; }
		protected int ObjectCount;
		protected int MaxCount;
		Dictionary<string, Func<object, string>> customProps = new Dictionary<string, Func<object, string>>();
		protected MM_ObjectType ot;
		public Func<MM_ObjectProperty, MM_ObjectProperty, string> GetColumnCaption;
		public ListExport()
		{
			GetColumnCaption = (prop1, prop2) => prop1.Title + (prop2 == null ? "" : (" / " + prop2.Title));
		}
		public void AddCustomProperty(string title, string before, Func<object, string> func)
		{
			string name = "";
			int seqNo = 1;
			if (before == "")
			{
				name = "customProperty_" + customProps.Count.ToString("000");
				seqNo = ot.MM_ObjectProperties.Max(o => o.SeqNo) + 1;
			}
			else
			{
				var beforeProp = ot.MM_ObjectProperties.Single(o => o.SysName == before);
				seqNo = beforeProp.SeqNo;
				int n = 1;
				while (ot.MM_ObjectProperties.Any(o => o.SysName == before + "." + n.ToString("000")))
					n++;
				name = before + "." + n.ToString("000");
			}

			customProps.Add(name, func);


			/*ot.MM_ObjectProperties.Add(new MM_ObjectProperty
			{
				Title = title,
				SysName = name,
				SeqNo = seqNo,
				MM_FormField = new MM_FormField { ShowInView = true },
				UpperBound = 1,
				ObjectPropertyID = ot.MM_ObjectProperties.Max(o => o.ObjectPropertyID) + 1
			});*/

		}


		protected List<string> requiredProperties = new List<string>();
		public List<string> RequiredProperties
		{
			get { return requiredProperties; }
		}

		protected void Page_Init(object sender, EventArgs e)
		{
			ot = AppMM.DataContext.MM_ObjectTypes.Single(o => o.SysName == ObjectTypeSysName);
		}
		protected void Page_Load(object sender, EventArgs e)
		{
			ObjectCount = Objects.Cast<IModelObject>().Count();
			MaxCount = AppSettings.Get("maxexportrows").ToInt32(32000);
			bOK.Visible = ObjectCount <= MaxCount;
		}

		protected void bOK_Click(object sender, EventArgs e)
		{
			lbProgress.Text = "Экспорт выполняется... 0%";
			hfGuid.Value = RunExport().ToString();
			bOK.Enabled = false;
			lbProgress.Text = "Экспорт выполняется... " + GetPercentage(hfGuid.Value.ToGuid()).ToString() + "%";
			bOK.Enabled = false;
			if (IsReady(hfGuid.Value.ToGuid()))
			{
				lbProgress.Text = "Экспорт завершен";
				ScriptManager.RegisterStartupScript(up, up.GetType(), "downloadresult", Page.ClientScript.GetPostBackEventReference(lbDownload, ""), true);
				bOK.Enabled = true;
			}
		}

		protected void timer_Tick(object sender, EventArgs e)
		{

		}

		protected void lbDownload_Click(object sender, EventArgs e)
		{
			GetResult(hfGuid.Value.ToGuid());
		}



		class ExportTask
		{
			public ExcelPackage Data { get; set; }
			public int PercentDone { get; set; }
			public bool IsReady { get; set; }
			public DateTime ReadyTime { get; set; }
			public Exception Exception { get; set; }
			public IEnumerable Objects { get; set; }
			public List<string> Items { get; set; }
		}

		static object locker = new object();
		static Dictionary<Guid, ExportTask> Tasks
		{
			get
			{
				if (HttpContext.Current.Application["ExcelExportTasks"] == null)
				{
					lock (locker)
					{
						if (HttpContext.Current.Application["ExcelExportTasks"] == null)
							HttpContext.Current.Application["ExcelExportTasks"] = new Dictionary<Guid, ExportTask>();
					}
				}
				return (Dictionary<Guid, ExportTask>)HttpContext.Current.Application["ExcelExportTasks"];
			}
		}

		Guid RunExport()
		{
			Guid g = Guid.NewGuid();
			ExportTask task = new ExportTask
			{
				PercentDone = 0,
				Data = new ExcelPackage(),
				Objects = Objects.Cast<object>().ToList(),
				Items = new List<string>(),

			};
			Tasks.Add(g, task);

			var items = task.Items;
			var file = new FileInfo(this.FileName);

			task.Data = new ExcelPackage(file);
			task.Data.Workbook.Properties.Title = FileName;
			ExcelWorksheet wsheetRus = task.Data.Workbook.Worksheets.Add("РУС");

			SetSheetHeader(wsheetRus, items);
			FillSheet(wsheetRus, task);

			return g;
		}

		private void SetSheetHeader(ExcelWorksheet wsheet, List<string> items)
		{
			var ot = AppMM.DataContext.MM_ObjectTypes.Single(o => o.SysName == ObjectTypeSysName);
			int numberCol = 1;

			CreateStdHeaderColumn(wsheet, numberCol, "№");
			foreach (var prop1 in ot.MM_ObjectProperties.Where(o => o.MM_FormField != null && o.MM_FormField.ShowInView && o.UpperBound == 1).OrderBy(o => o.SeqNo))
			{
				if ((Page.Request.Form["ExportProperty" + prop1.ObjectPropertyID.ToString()] == "on" && prop1.RefObjectType == null) || requiredProperties.Contains(prop1.SysName))
				{
					if (prop1.TypeCode == ObjectPropertyType.Code)
						items.Add(prop1.SysName + "CodifierTitle");
					else
						items.Add(prop1.SysName);
					numberCol++;
					CreateStdHeaderColumn(wsheet, numberCol, GetColumnCaption(prop1, null));

				}
				if (prop1.RefObjectType != null)
				{
					foreach (var prop2 in prop1.RefObjectType.MM_ObjectProperties.Where(o => !o.IsPrimaryKey && o.MM_FormField != null && o.MM_FormField.ShowInView && o.UpperBound == 1).OrderBy(o => o.SeqNo))
					{
						if (Page.Request.Form["ExportProperty" + prop1.ObjectPropertyID.ToString() + "_" + prop2.ObjectPropertyID.ToString()] == "on")
						{
							if (prop2.TypeCode == ObjectPropertyType.Code)
								items.Add(prop1.SysName + "." + prop2.SysName + "CodifierTitle");
							else
								items.Add(prop1.SysName + "." + prop2.SysName);
							numberCol++;
							CreateStdHeaderColumn(wsheet, numberCol, GetColumnCaption(prop1, prop2));
						}
					}
				}

			}
		}

		private void CreateStdHeaderColumn(ExcelWorksheet sheet, int col, string value)
		{
			CreateStdHeaderColumn(sheet, col, value, 1);
		}

		private void CreateStdHeaderColumn(ExcelWorksheet sheet, int col, string value, int startRow)
		{
			using (ExcelRange r = sheet.Cells[startRow, col, 1, col])
			{
				SetStyle(r, true);
				GrayLine(r);

				r.Value = value;

				sheet.Column(col).AutoFit();
				AutoFitHeightRow(r, 47);
			}
		}

		private void FillSheet(ExcelWorksheet sheet, ExportTask task)
		{
			try
			{
				List<string> items = task.Items;
				List<object> objlist = new List<object>();
				foreach (object o in Objects)
					objlist.Add(o);

				int numberRow = 1;
				int i = 0;

				foreach (object o in objlist)
				{
					numberRow++;
					i++;
					int numberColl = 1;

					PrintCell(numberRow, numberColl, i.ToString(), sheet);
					foreach (string prop in items)
					{
						numberColl++;
						if (customProps.ContainsKey(prop))
							PrintCell(numberRow, numberColl, GetProperty(o, prop), sheet);
						else
							PrintCell(numberRow, numberColl, HttpUtility.HtmlEncode(Regex.Replace(GetProperty(o, prop).Replace("</p>", "\r\n"), "<.*?>", "")), sheet);
					}

					task.PercentDone = 100 * i / objlist.Count;
				}
			}
			catch (Exception e)
			{
				task.Exception = e;
			}
			finally
			{
				task.IsReady = true;
				task.ReadyTime = DateTime.Now;
			}
		}

		private void PrintCell(int startRow, int col, string getTitle, ExcelWorksheet sheet)
		{
			using (ExcelRange r = sheet.Cells[startRow, col])
			{
				SetStyle(r, false);

				if (getTitle.Contains("href"))
				{
					string href = Regex.Replace(getTitle, "^.*href='", "");
					href = Regex.Replace(href, "'>.*$", "");

					string value = Regex.Replace(getTitle, "^.*'>", "");
					value = Regex.Replace(value, "</a>.*$", "");

					r.Formula = "HYPERLINK(\"" + href + "\",\"" + HttpUtility.HtmlDecode(value) + "\")";
					r.Style.Font.Color.SetColor(Color.Blue);
					r.Style.Font.UnderLine = true;
				}
				else r.Value = HttpUtility.HtmlDecode(getTitle);
			}
		}

		protected void SetStyle(ExcelRange p0, bool isBold)
		{
			p0.Style.WrapText = true;
			p0.Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
			p0.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
			p0.Style.Font.Bold = isBold;
			p0.Style.Font.Size = 8;

			p0.Style.Border.Top.Style = ExcelBorderStyle.Thin;
			p0.Style.Border.Left.Style = ExcelBorderStyle.Thin;
			p0.Style.Border.Right.Style = ExcelBorderStyle.Thin;
			p0.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
		}

		private static void GrayLine(ExcelRange r)
		{
			r.Style.Fill.PatternType = ExcelFillStyle.Solid;
			r.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
		}

		protected void AutoFitHeightRow(ExcelRange range, int hiddenCol)
		{
			using (ExcelRange r = range.Worksheet.Cells[range.Start.Row, hiddenCol])
			{
				if (range.Value != null)
				{
					r.Worksheet.Column(hiddenCol).Width = RangeWidth(range);
					r.Style.Font.Size = range.Style.Font.Size;
					r.Style.WrapText = true;
					r.Value = range.Value.ToString();
					r.Worksheet.Column(hiddenCol).Hidden = true;
				}
			}
		}

		protected double RangeHeight(ExcelRange r)
		{
			double sum = 0;
			for (int i = r.Start.Row; i <= r.End.Row; i++)
				sum += r.Worksheet.Row(i).Height;
			return sum;
		}

		protected double RangeWidth(ExcelRange r)
		{
			double sum = 0;
			for (int i = r.Start.Column; i <= r.End.Column; i++)
				sum += r.Worksheet.Column(i).Width;
			return sum;
		}

		int GetPercentage(Guid g)
		{
			var list = Tasks;
			if (list.ContainsKey(g))
				return list[g].PercentDone;
			return 0;
		}

		bool IsReady(Guid g)
		{
			var list = Tasks;
			if (list.ContainsKey(g))
				return list[g].IsReady;
			return false;
		}

		void GetResult(Guid g)
		{
			var list = Tasks;
			if (list.ContainsKey(g))
			{
				var item = list[g];
				list.Remove(g);

				HttpResponse response = HttpContext.Current.Response;
				HttpRequest request = HttpContext.Current.Request;

				response.AppendHeader("Content-Type", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
				response.AppendHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlPathEncode(FileName) + ";");
				response.Charset = "utf-8";
				if (request.Browser.Browser == "IE")
					response.HeaderEncoding = Encoding.Default;
				else
					response.HeaderEncoding = Encoding.UTF8;

				response.BinaryWrite(item.Data.GetAsByteArray());
				response.End();
				item.Data.Dispose();
			}
		}

		string GetProperty(object o, string prop)
		{
			if (customProps.ContainsKey(prop))
			{
				return customProps[prop](o);
			}
			if (prop.IndexOf('.') > 0)
			{
				string prop1 = prop.Substring(0, prop.IndexOf('.'));
				var p1 = o.GetType().GetProperty(prop1);
				o = p1.GetValue(o, null);
				if (o == null)
					return "";
				prop = prop.Substring(prop.IndexOf('.') + 1);
			}
			var p = o.GetType().GetProperty(prop);
			if (p == null)
				throw new Exception(o.GetType().Name + "." + prop + " не существует");
			object val = p.GetValue(o, null);
			if (val == null)
				return "";
			if (val is bool)
				return (bool)val ? "да" : "нет";
			if (val is DateTime)
				return ((DateTime)val).DateTimeToString();
			if (val is string)
				return (string)val;
			if (val is int)
				return val.ToString();
			if (val is decimal)
				return val.ToString();
			if (val is IModelObject)
				return ((IModelObject)val).Title;

			return "???";
		}
	}
}