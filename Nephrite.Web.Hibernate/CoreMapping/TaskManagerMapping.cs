using System;
using System.Collections.Generic;
using System.Linq;
//using Nephrite.Web.TaskManager;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace Nephrite.Hibernate.CoreMapping
{
	//public class ITM_TaskParameterMap : ClassMapping<ITM_TaskParameter>
	//{
	//	public ITM_TaskParameterMap()
	//	{
	//		Table("TM_TaskParameter");
	//		Lazy(true);
	//		Id(x => x.TaskParameterID, map => map.Generator(Generators.Identity));
	//		Discriminator(x => x.Formula("0"));
	//		Property(x => x.Title, map => map.NotNullable(true));
	//		Property(x => x.SysName, map => map.NotNullable(true));
	//		Property(x => x.Value);
	//		Property(x => x.SeqNo, map => map.NotNullable(true));
	//		Property(x => x.ParentID, map => map.NotNullable(true));
	//	}
	//}

	//public class ITM_TaskExecutionMap : ClassMapping<ITM_TaskExecution>
	//{
	//	public ITM_TaskExecutionMap()
	//	{
	//		Table("TM_TaskExecution");
	//		Lazy(true);
	//		Id(x => x.TaskExecutionID, map => map.Generator(Generators.Identity));
	//		Discriminator(x => x.Formula("0"));
	//		Property(x => x.StartDate, map => map.NotNullable(true));
	//		Property(x => x.FinishDate);
	//		Property(x => x.IsSuccessfull, map => { map.NotNullable(true); MappingConfig.BoolPropertyConfig(map); });
	//		Property(x => x.MachineName, map => map.NotNullable(true));
	//		Property(x => x.ResultXml);
	//		Property(x => x.ExecutionLog);
	//		Property(x => x.LastModifiedDate, map => map.NotNullable(true));
	//		Property(x => x.LastModifiedUserID, map => map.NotNullable(true));
	//		Property(x => x.TaskID, map => map.NotNullable(true));
	//	}
	//}

	//public class ITM_TaskMap : ClassMapping<ITM_Task>
	//{
	//	public ITM_TaskMap()
	//	{
	//		Table("TM_Task");
	//		Lazy(true);
	//		Id(x => x.TaskID, map => map.Generator(Generators.Identity));
	//		Discriminator(x => x.Formula("0"));
	//		Property(x => x.Title, map => map.NotNullable(true));
	//		Property(x => x.Class, map => map.NotNullable(true));
	//		Property(x => x.StartType, map => { map.NotNullable(true); MappingConfig.BoolPropertyConfig(map); });
	//		Property(x => x.Method, map => map.NotNullable(true));
	//		Property(x => x.Interval, map => map.NotNullable(true));
	//		Property(x => x.LastStartDate);
	//		Property(x => x.IsSuccessfull, map => { map.NotNullable(true); MappingConfig.BoolPropertyConfig(map); });
	//		Property(x => x.IsActive, map => { map.NotNullable(true); MappingConfig.BoolPropertyConfig(map); });
	//		Property(x => x.StartFromService, map => { map.NotNullable(true); MappingConfig.BoolPropertyConfig(map); });
	//		Property(x => x.ErrorLogID);
	//		Property(x => x.ExecutionTimeout, map => map.NotNullable(true));
	//	}
	//}
}