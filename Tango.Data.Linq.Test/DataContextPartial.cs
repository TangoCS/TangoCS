using Nephrite.Metamodel;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Data.Linq.SqlClient;
using System.Linq;
using System.Transactions;

namespace Solution.Model
{
	[Provider(typeof(PgProvider))]
	partial class modelDataContext
	{
		Dictionary<object, object> originals;
		public object GetOriginalEntityState(object obj)
		{
			if (originals.ContainsKey(obj))
				return originals[obj];
			return null;
		}
		List<Action> afterSaveActions = new List<Action>();
		List<Action> beforeSaveActions = new List<Action>();
		public List<Action> AfterSaveActions
		{
			get { return afterSaveActions; }
		}
		public List<Action> BeforeSaveActions
		{
			get { return beforeSaveActions; }
		}

		public override void SubmitChanges(ConflictMode failureMode)
		{
			using (var ts = new TransactionScope(System.Transactions.TransactionScopeOption.Required, new TimeSpan(0, 3, 0)))
			{
				// Получить изменения
				var cs = GetChangeSet();
				foreach (var obj in cs.Inserts)
				{
					//if (obj is IMMObject)
					//	((IMMObject)obj).RaiseSaveChanges();
					foreach (var p in obj.GetType().GetProperties())
					{
						if (p.PropertyType == typeof(DateTime) && p.CanWrite)
						{
							if (((DateTime)p.GetValue(obj, null)).Year < 1800)
								p.SetValue(obj, new DateTime(1900, 1, 1), null);
						}
					}
				}
				//foreach (var obj in cs.Updates)
				//{
				//	if (obj is IMMObject)
				//		((IMMObject)obj).RaiseSaveChanges();
				//}

				// Создать историю удаления
				foreach (var obj in cs.Deletes)
				{
					if (obj is BaseModelObject)
						((BaseModelObject)obj).WriteDeleteObjectHistory();
					if (obj is IMMObject)
						OnDeleteMMObject(obj as IMMObject);
				}

				foreach (var action in BeforeSaveActions)
					action();
				originals = cs.Updates.ToDictionary(o => o, o => GetTable(o.GetType()).GetOriginalEntityState(o));
				// Сохранить объекты
				base.SubmitChanges(failureMode);

				foreach (var action in AfterSaveActions)
					action();

				// Создать историю создания
				foreach (var obj in cs.Inserts)
				{
					if (obj is BaseModelObject)
						((BaseModelObject)obj).WriteInsertObjectHistory();
					if (obj is IMMObject)
						OnInsertMMObject(obj as IMMObject);
				}

				// Создать историю изменения
				foreach (var obj in cs.Updates)
				{
					//System.IO.File.AppendAllText(""c:\\Deploy\\Fund2\\DbBackup\\log.txt"", obj.GetType().Name+""\r\n"");
					if (obj is BaseModelObject)
						((BaseModelObject)obj).WriteUpdateObjectHistory();
					if (obj is IMMObject)
						OnUpdateMMObject(obj as IMMObject);
				}

				// Сохранить объекты истории изменений
				base.SubmitChanges(failureMode);
				ts.Complete();
			}
		}
		partial void OnInsertMMObject(IMMObject obj);
		partial void OnDeleteMMObject(IMMObject obj);
		partial void OnUpdateMMObject(IMMObject obj);
	}
}
