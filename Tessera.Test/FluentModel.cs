using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Nephrite.Meta;
using Nephrite.Meta.Fluent;
using Nephrite.Web;
using Nephrite.Web.Controls;

namespace Tessera.Test
{
	interface ILot { }
	interface ISPM_Subject { }
	interface IC_Contractor { }
	interface ILotDoc { }
	interface IProjectActivity { }
	interface IC_Measure { }
	interface IC_OperationType { }

	class FluentModel
	{
		

		public MetaSolution Test()
		{
			var s = new MetaSolution();

			s.AddClass<IC_OperationType>()
				.IntKey()
				.TCLED().TimeStamp<SPM_Subject>();

			s.AddClass<ILot>()
				.IntKey()
				.TCLEVD().TimeStamp<SPM_Subject>()
				.Workflow()
				.Attribute("CreateDate", "Дата создания лота", MetaDateType.NotNull())
				.Attribute("StartCost", "Начальная цена лота (рубли РФ)", MetaDecimalType.Null())
				.Attribute("AgreementCost", "Стоимость договора (рубли РФ)", MetaDecimalType.Null())
				.Attribute("Copy_OrgUnit_Code", "Копия Код заказчика", MetaStringType.Null())
				.Attribute("Copy_OrgUnit_Title", "Копия Наименование заказчика", MetaStringType.Null())
				.Reference<IC_Contractor>("ContractorOuter", "Исполнитель")
				.Reference<ILotDoc>("Docs", "Документы", x => x.Multiple().Aggregation().InverseProperty("Lot"))
				.Reference<IProjectActivity>("ProjectActivity", "Заявка на работу");

			Func<Appendix, int> f = o => o.AppendixID;
			Expression<Func<Appendix, int>> f2 = o => o.AppendixID;
			//Sorter s = new Sorter();
			//s.AddSortColumn("", f2);

			//Expression<Func<C_OperationType, string>> f2 = o => o.Title;
			dynamic d = null;
			//Action<IC_OperationType, string> f3 = (o, v) => o.Title = v;
			//d = f3;
			
			return s;				
		}
	}
}
