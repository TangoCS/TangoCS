using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nephrite.Web.Reporting
{
	/// <summary>
	/// Агрегируемый столбец
	/// </summary>
	public class AggregateColumn
	{
		/// <summary>
		/// Заголовок столбца
		/// </summary>
		public string Caption { get; set; }

		/// <summary>
		/// SQL-выражение для расчета
		/// </summary>
		public string Selector { get; set; }

		/// <summary>
		/// Количество знаков после запятой
		/// </summary>
		public int DecimalPoints { get; set; }

		public bool Visible { get; set; }

		public AggregateColumn()
		{
			DecimalPoints = 0;
			Visible = true;
		}

		// Функция получения значения
		public Func<Report.ReportItem, decimal> ValueFunc { get; set; }
	}
}