using System;
using System.Linq;
using Nephrite.Meta;
using Nephrite.Meta.Fluent;

namespace Solution.Model
{
	public class ModelFactory
	{
		public MetaSolution CreateSolution()
		{
			var s = new MetaSolution();

			var pmm = s.AddPackage("mm", "Метамодель");
			var pSPM = s.AddPackage("SPM", "УПБ");
			var pSiteViews = s.AddPackage("SiteViews", "Представления сайта");
			var pSystem = s.AddPackage("System", "Системные классы");
			var pStereotypes = pmm.AddPackage("Stereotypes", "Стереотипы");
			var pSqlExecution = pSystem.AddPackage("SqlExecution", "Выполнение SQL");
			var pFileStorage = pSystem.AddPackage("FileStorage", "Файловое хранилище");
			var pChangeHistory = pSystem.AddPackage("ChangeHistory", "История изменений");
			var pNavigation = pSystem.AddPackage("Navigation", "Навигация");
			var pWorkflow = pmm.AddPackage("Workflow", "Рабочие процессы");
			var pTemplates = pmm.AddPackage("Templates", "Шаблоны классов");
			var pMasterPages = s.AddPackage("MasterPages", "Мастер-страницы");
			var pUtils = pmm.AddPackage("Utils", "Утилиты");
			var pFileStorage2 = s.AddPackage("FileStorage2", "Файловое хранилище");
			var pFIAS = s.AddPackage("FIAS", "ФИАС");
			var pOrgStructure = s.AddPackage("OrgStructure", "Орг. структура");
			var pCivilServants = s.AddPackage("CivilServants", "Госслужащие");
			var pDict = pCivilServants.AddPackage("Dict", "Справочники");
			var pReports = pCivilServants.AddPackage("Reports", "Отчеты");
			var pNetScan = s.AddPackage("NetScan", "Сетевое сканирование");
			var pAssemblyGen = s.AddPackage("AssemblyGen", "Генерация кода");

			mmPackage.Init(pmm);
			SPMPackage.Init(pSPM);
			SiteViewsPackage.Init(pSiteViews);
			SystemPackage.Init(pSystem);
			StereotypesPackage.Init(pStereotypes);
			SqlExecutionPackage.Init(pSqlExecution);
			FileStoragePackage.Init(pFileStorage);
			ChangeHistoryPackage.Init(pChangeHistory);
			NavigationPackage.Init(pNavigation);
			WorkflowPackage.Init(pWorkflow);
			TemplatesPackage.Init(pTemplates);
			MasterPagesPackage.Init(pMasterPages);
			UtilsPackage.Init(pUtils);
			FileStorage2Package.Init(pFileStorage2);
			FIASPackage.Init(pFIAS);
			OrgStructurePackage.Init(pOrgStructure);
			CivilServantsPackage.Init(pCivilServants);
			DictPackage.Init(pDict);
			ReportsPackage.Init(pReports);
			NetScanPackage.Init(pNetScan);
			AssemblyGenPackage.Init(pAssemblyGen);
			return s;
		}
	}

}
