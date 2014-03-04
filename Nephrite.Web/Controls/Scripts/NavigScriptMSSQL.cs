using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nephrite.Web.Controls
{
    public class NavigScriptMSSQL : INavigScript
    {

        public string Menu
        {
            get
            {
                return @"SELECT [t0].[Title], [t0].[URL] AS [Url], [t0].[Type] AS [MenuItemType], [t0].[ImageURL] AS [ImageUrl], 
							(CASE 
								WHEN [t2].[test] IS NOT NULL THEN CONVERT(NVarChar(MAX),[t6].[SysName])
								WHEN [t4].[test] IS NOT NULL THEN [t7].[SysName] + 'Pck'
								ELSE CONVERT(NVarChar(MAX),'')
							 END) AS [ClassName], 
							(CASE 
								WHEN ([t2].[test] IS NOT NULL) AND ([t6].[IsEnableSPM] = 1) THEN 1
								WHEN NOT (([t2].[test] IS NOT NULL) AND ([t6].[IsEnableSPM] = 1)) THEN 0
								ELSE NULL
							 END) AS [EnableSPM], 
							(CASE 
								WHEN [t0].[Type] = 'V' THEN [t4].[SysName]
								ELSE [t2].[SysName]
							 END) AS [MethodSysName], [t0].[SeqNo], CAST( [t0].[ParentGUID] AS VARCHAR(max)) AS [_ParentMenuItemGUID], CAST( [t0].[NavigItemGUID]  AS VARCHAR(max)) AS [_MenuItemGUID], 
							CAST( (CASE 
								WHEN [t0].[Type] = 'V' THEN [t0].[SPMActionGUID]
								ELSE [t2].[Guid]
							 END) AS VARCHAR(max))  AS [_SPMActionGUID]
						FROM [dbo].[V_N_NavigItem] AS [t0]
						LEFT OUTER JOIN (
							SELECT 1 AS [test], [t1].[MethodID], [t1].[SysName], [t1].[ObjectTypeID], [t1].[Guid]
							FROM [dbo].[MM_Method] AS [t1]
							) AS [t2] ON [t0].[MethodID] = ([t2].[MethodID])
						LEFT OUTER JOIN (
							SELECT 1 AS [test], [t3].[FormViewID], [t3].[SysName], [t3].[PackageID]
							FROM [dbo].[MM_FormView] AS [t3]
							) AS [t4] ON [t0].[FormViewID] = ([t4].[FormViewID])
						INNER JOIN [dbo].[N_Navig] AS [t5] ON [t5].[NavigGUID] = [t0].[NavigGUID]
						LEFT OUTER JOIN [dbo].[MM_ObjectType] AS [t6] ON [t6].[ObjectTypeID] = [t2].[ObjectTypeID]
						LEFT OUTER JOIN [dbo].[MM_Package] AS [t7] ON [t7].[PackageID] = [t4].[PackageID]
						WHERE (NOT ([t0].[IsDeleted] = 1)) AND ([t5].[SysName] = 'MainMenu') AND ([t0].[LanguageCode] = 'ru')";
            }

        }
    }
}