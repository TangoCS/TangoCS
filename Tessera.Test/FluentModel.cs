using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nephrite.Meta;
using Nephrite.Meta.Fluent;
using Nephrite.Web;

namespace Tessera.Test
{
	class FluentModel
	{
		public MetaPackage TestPackage()
		{
			var p = new MetaPackage("TestPackage");

			p.AddClass("C_OperationType")
				.IntKey()
				.TCLED().LogicalDelete().TimeStamp();

			p.AddClass("Lot")
				.IntKey()
				.TCLEVD().LogicalDelete().TimeStamp()
				.Workflow()
				.Attribute("CreateDate", "Дата создания лота", TypeFactory.Date(true))
				.Attribute("StartCost",	"Начальная цена лота (рубли РФ)", TypeFactory.Decimal(false))
				.Attribute("AgreementCost",	"Стоимость договора (рубли РФ)", TypeFactory.Decimal(false))
				.Attribute("Copy_OrgUnit_Code", "Копия Код заказчика", TypeFactory.String(false))
				.Attribute("Copy_OrgUnit_Title", "Копия Наименование заказчика", TypeFactory.String(false))
				.Reference("ContractorOuter", "Исполнитель", x => x.To("C_Contractor"))
				.Reference("Docs", "Документы", x => x.To("LotDoc").Multiple().Aggregation().InverseProperty("Lot"))
				.Reference("ProjectActivity", "Заявка на работу", x => x.To("ProjectActivity"))
				.Attribute("ContractName", "Предмет договора", TypeFactory.String(false))
				.Attribute("CodeOKVED", "Код продукции по ОКВЭД", TypeFactory.String(false))
				.Attribute("CodeOKDP", "Код продукции по ОКДП", TypeFactory.String(false))
				.Attribute("ProductRequired", "Требования к закупаемой продукции", TypeFactory.String(false))
				.Reference("Measure", "Единица измерения продукции", x => x.To("C_Measure"))
				.Attribute("ProductNumber", "Количество закупаемой продукции", TypeFactory.Decimal(false))
				.Attribute("PlannedDate", "Планируемая дата публикации извещения", TypeFactory.Date(false))
				.Attribute("PerformDate", "Плановый срок исполнения контракта", TypeFactory.Date(false))
				.Attribute("CostInfo", "Включенные расходы", TypeFactory.String(false))
				.Attribute("ProductOrder", "Форма, сроки и порядок оплаты продукции", TypeFactory.String(false))
				.Attribute("AuctionStep", "Шаг аукциона", TypeFactory.String(false))
				.Attribute("RatingCriteria", "Критерии оценки заявок", TypeFactory.String(false))
				.Attribute("Privilege", "Предоставляемые преимущества", TypeFactory.String(false))
				.Attribute("ProvisionRequest", "Размер обеспечения заявки, срок и порядок внесения, реквизиты для перечисления", TypeFactory.String(false))
				.Attribute("ProvisionContract", "Размер обеспечения контракта, срок и порядок предоставления", TypeFactory.String(false))
				.Attribute("ElectronicForm", "Электронная форма", TypeFactory.Boolean(true))
				.Reference("PlanStage", "Позиция плана", x => x.To("PurchasePlanStage"))
				.Reference("SourceFinance", "Источник финансирования", x => x.To("C_SourceFinance"))
				.Reference("PlaceMethod", "Способ размещения", x => x.To("C_PurchaseMethod").Required())
				.Reference("Region", "Регион поставки", x => x.To("C_Region"))
				.Attribute("Number", "Номер лота", TypeFactory.Int(true))
				.Reference("PurchaseProcedure", "Закупочная процедура", x => x.To("PurchaseProcedure"))
				.Attribute("BudgetName", "Наименование бюджета", TypeFactory.String(false))
				.Attribute("DeleteDate", "Дата удаления", TypeFactory.DateTime(false))
				.Reference("PurchaseContract", "Контракт", x => x.To("PurchaseContract").Multiple().InverseProperty("Lot"))
				.Attribute("PurchaseYear", "Год закупки", TypeFactory.Int(false))
				.Reference("PurchaseProcedureLot", "Закупочная процедура", x => x.To("PurchaseProcedureLot").Multiple().InverseProperty("Lot"))
				.Reference("PurchaseProvider", "Закупка у ЕП", x => x.To("PurchaseProvider").Multiple().InverseProperty("Lot"))
				.Attribute("ProductTitle", "Наименование закупаемой продукции", TypeFactory.String(false))
				.Attribute("OkvedCodeTitle", "Наименование кода ОКВЭД", TypeFactory.String(false))
				.Attribute("OkdpCodeTitle", "Наименование кода продукции по ОКДП", TypeFactory.String(false))
				.Reference("FinanceLevel", "Уровень бюджета", x => x.To("C_FinanceLevel"))
				.Reference("BudgetClassificationExpenseCode", "Расходный код бюджетной классификации", x => x.To("C_BudgetClassificationExpenseCode"))
				.Reference("Stages", "Позиции плана", x => x.To("PurchasePlanStage").Multiple().InverseProperty("Lot"))
				.Reference("PurchasePlanStage", "Позиция плана, в которую включен лот", x => x.To("PurchasePlanStage"))
				.Reference("ContractItems", "Предметы контракта", x => x.To("PurchaseContractItem").Multiple().InverseProperty("Lot").Aggregation())
				.Reference("RegistrationObject", "Объект учета", x => x.To("RegistrationObject"))
				.Reference("OrgUnit", "Заказчик", x => x.To("OrgUnit"))
				.Attribute("ExternalID", "Внешний идентификатор", TypeFactory.Int(false))
				.PersistentComputedAttribute("DisplayTitle", "Отображаемое наименование Вычислимое", TypeFactory.String(false));

			return p;				
		}
	}
}
