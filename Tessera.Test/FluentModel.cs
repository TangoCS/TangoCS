using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nephrite.Meta;
using Nephrite.Meta.Fluent;
using Nephrite.Web;

namespace Tessera.Test
{
	class Lot { }
	class SPM_Subject { }
	class C_Contractor { }
	class LotDoc { }
	class ProjectActivity { }
	class C_Measure { }
	class C_OperationType { }

	class FluentModel
	{
		public MetaPackage TestPackage()
		{
			var p = new MetaPackage("TestPackage");

			p.AddClass<C_OperationType>()
				.IntKey()
				.TCLED().LogicalDelete().TimeStamp<SPM_Subject>();

			p.AddClass<Lot>()
				.IntKey()
				.TCLEVD().LogicalDelete().TimeStamp<SPM_Subject>()
				.Workflow()
				.Attribute("CreateDate", "Дата создания лота", TypeFactory.Date(true))
				.Attribute("StartCost", "Начальная цена лота (рубли РФ)", TypeFactory.Decimal(false))
				.Attribute("AgreementCost", "Стоимость договора (рубли РФ)", TypeFactory.Decimal(false))
				.Attribute("Copy_OrgUnit_Code", "Копия Код заказчика", TypeFactory.String(false))
				.Attribute("Copy_OrgUnit_Title", "Копия Наименование заказчика", TypeFactory.String(false))
				.Reference<C_Contractor>("ContractorOuter", "Исполнитель")
				.Reference<LotDoc>("Docs", "Документы", x => x.Multiple().Aggregation().InverseProperty("Lot"))
				.Reference<ProjectActivity>("ProjectActivity", "Заявка на работу")
				.Attribute("ContractName", "Предмет договора", TypeFactory.String(false));
				

			return p;				
		}
	}
}
