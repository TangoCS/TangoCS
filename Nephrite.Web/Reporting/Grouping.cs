using System;
using System.Collections.Generic;
using System.Linq;

namespace Nephrite.Web.Reporting
{
	/// <summary>
	/// Группировка
	/// </summary>
	public class Grouping
	{
		/// <summary>
		/// Наименование группировки
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// Столбец группировки
		/// </summary>
		public string KeyColumn { get; set; }

		/// <summary>
		/// Функция для получения наименованиея по коду или ключу
		/// </summary>
		public Func<string, string> GetTitle { get; set; }

		/// <summary>
		/// Сортировка по возрастанию
		/// </summary>
		public bool SortAsc { get; set; }

		/// <summary>
		/// SQL-выражение для вычисления наименования по столбцу группировки
		/// </summary>
		public string TitleSelector { get; set; }
	}
}