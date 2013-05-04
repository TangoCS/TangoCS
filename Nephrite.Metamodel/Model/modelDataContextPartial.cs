using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Transactions;
using System.Data.Linq;

namespace Nephrite.Metamodel.Model
{
	partial class modelDataContext
	{
		public override void SubmitChanges(ConflictMode failureMode)
		{
			using (var ts = new TransactionScope())
			{
				// Получить изменения
				var cs = GetChangeSet();

				// Сохранить объекты
				base.SubmitChanges(failureMode);

				// Создать историю создания
				foreach (var obj in cs.Inserts)
				{
					if (obj is BaseModelObject)
						((BaseModelObject)obj).WriteInsertObjectHistory();
				}

				// Создать историю изменения
				foreach (var obj in cs.Updates)
				{
					if (obj is BaseModelObject)
						((BaseModelObject)obj).WriteUpdateObjectHistory();
				}

				// Сохранить объекты истории изменений
				base.SubmitChanges(failureMode);
				ts.Complete();
			}
		}
	}
}
