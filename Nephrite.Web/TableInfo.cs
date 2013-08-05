using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using System.Data.Linq;

namespace Nephrite.Web
{
	/*
	public static class TableInfo
	{
		static DateTime tableInfoDate = new DateTime(2010, 1, 1);
		static object locker = new object();
		static Dictionary<string, DateTime> tableInfos = new Dictionary<string,DateTime>();

		//static Func<HCoreDataContext, DateTime, IEnumerable<N_TableInfo>> getN_TableInfos;

		static void Refresh()
		{
			if (DateTime.Now.Subtract(tableInfoDate).TotalSeconds > 1)
			{

				if (Monitor.TryEnter(locker))
				{
					try
					{
						//if (getN_TableInfos == null)
						//	getN_TableInfos = CompiledQuery.Compile<HCoreDataContext, DateTime, IEnumerable<N_TableInfo>>((db, date) =>
						//	db.N_TableInfos.Where(o => o.LastDataModify > date));
						//var tables = getN_TableInfos(AppWeb.DataContext, tableInfoDate).ToList();
						var tables = AppWeb.DataContext.N_TableInfos.Where(o => o.LastDataModify > tableInfoDate).ToList();
						tableInfoDate = DateTime.Now;
						foreach (var t in tables)
							tableInfos[t.TableName] = t.LastDataModify;
					}
					finally
					{
						Monitor.Exit(locker);
					}
				}
			}
		}

		/// <summary>
		/// Получить дату последнего изменения данных в таблице с заданным именем
		/// </summary>
		/// <param name="tableName">Имя таблицы</param>
		/// <returns>Дата последнего изменения данных или DateTime.MinValue, если информации по таблице нет</returns>
		public static DateTime GetTableLastModifyDate(string tableName)
		{
			Refresh();
			if (tableInfos.ContainsKey(tableName))
				return tableInfos[tableName];
			return DateTime.MinValue;
		}

		/// <summary>
		/// Получить максимальную дату последнего изменения данных в таблицах с заданными именами (разделены запятыми)
		/// </summary>
		/// <param name="tableNames"></param>
		/// <returns></returns>
		public static DateTime GetMaxLastModifyDate(string tableNames)
		{
			return GetMaxLastModifyDate(tableNames.Split(','));
		}

		/// <summary>
		/// Получить максимальную дату последнего изменения данных в таблицах с заданными именами
		/// </summary>
		/// <param name="tableNames"></param>
		/// <returns></returns>
		public static DateTime GetMaxLastModifyDate(string[] tables)
		{
			Refresh();
			DateTime d = DateTime.MinValue;
			for (int i = 0; i < tables.Length; i++)
			{
				if (tableInfos.ContainsKey(tables[i]) && tableInfos[tables[i]] > d)
					d = tableInfos[tables[i]];
			}
			return d;
		}
	}
	*/
}