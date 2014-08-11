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
			var pmm = mmPackage.Create();
			var pSPM = SPMPackage.Create();
			var pSiteViews = SiteViewsPackage.Create();
			var pSystem = SystemPackage.Create();
			var pStereotypes = StereotypesPackage.Create();
			var pSqlExecution = SqlExecutionPackage.Create();
			var pFileStorage = FileStoragePackage.Create();
			var pChangeHistory = ChangeHistoryPackage.Create();
			var pNavigation = NavigationPackage.Create();
			var pWorkflow = WorkflowPackage.Create();
			var pTemplates = TemplatesPackage.Create();
			var pMasterPages = MasterPagesPackage.Create();
			var pUtils = UtilsPackage.Create();
			var pFileStorage2 = FileStorage2Package.Create();
			var pFIAS = FIASPackage.Create();
			var pOrgStructure = OrgStructurePackage.Create();
			var pCivilServants = CivilServantsPackage.Create();
			var pDict = DictPackage.Create();
			var pReports = ReportsPackage.Create();
			var pNetScan = NetScanPackage.Create();
			var pAssemblyGen = AssemblyGenPackage.Create();

			s.AddPackage(pmm);
			s.AddPackage(pSPM);
			s.AddPackage(pSiteViews);
			s.AddPackage(pSystem);
			pmm.AddPackage(pStereotypes);
			pSystem.AddPackage(pSqlExecution);
			pSystem.AddPackage(pFileStorage);
			pSystem.AddPackage(pChangeHistory);
			pSystem.AddPackage(pNavigation);
			pmm.AddPackage(pWorkflow);
			pmm.AddPackage(pTemplates);
			s.AddPackage(pMasterPages);
			pmm.AddPackage(pUtils);
			s.AddPackage(pFileStorage2);
			s.AddPackage(pFIAS);
			s.AddPackage(pOrgStructure);
			s.AddPackage(pCivilServants);
			pCivilServants.AddPackage(pDict);
			pCivilServants.AddPackage(pReports);
			s.AddPackage(pNetScan);
			s.AddPackage(pAssemblyGen);
			return s;
		}
	}

}
