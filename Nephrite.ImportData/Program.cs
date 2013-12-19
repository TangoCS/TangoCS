using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IBM.Data.DB2;
using Nephrite.Meta.Database;

namespace Nephrite.ImportData
{
	public class nfile
	{
		public byte[] data { get; set; }
		public string ext { get; set; }
		public string guid { get; set; }
	}

	class Program
	{
		static void Main(string[] args)
		{
			SqlConnection con = new SqlConnection("Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=servants1;Data Source=TOSHIBA-TOSH\\SQL2008");
			var schema = new SqlServerMetadataReader().ReadSchema("DBO");
			StringBuilder result = new StringBuilder();
			//Constraints(false, schema, result);
			var listTable = "MM_Package,MM_ObjectType,MM_FormView,MM_Method,N_MenuItem,MM_MethodGroup,MM_MethodGroupItem,MM_Codifier,MM_ObjectProperty,MM_FormFieldGroup,MM_FormField,MM_FormFieldAttribute,MM_DataValidation,MM_CodifierValue,MM_Predicate,WF_Workflow,WF_Activity,WF_Transition,CMSFormView,N_Node,N_NodeData,N_Navig,N_NavigItem,N_NavigItemData,Appendix,C_CitizenCategory,C_CourtSessionStatus,C_DocClass,C_DocName,C_DocTaskType,C_DocType,C_DocTypeAppendix,C_ExtraPayPost,C_FIAS_ActualStatus,C_FIAS_AddressObject,C_FIAS_AddressObjectType,C_FIAS_CenterStatus,C_FIAS_CurrentStatus,C_FIAS_EstateStatus,C_FIAS_House,C_FIAS_HouseInterval,C_FIAS_HouseStateStatus,C_FIAS_IntervalStatus,C_FIAS_Landmark,C_FIAS_NormativeDocument,C_FIAS_OperationStatus,C_FIAS_StructureStatus,C_Help,C_InfoDocType,C_JuridicalInstance,C_Language,C_MassCalcStatus,C_MassOperationState,C_MassOperationType,C_OperationReason,C_OperationType,C_OrgUnitHie,C_OrgUnitType,C_OrgUnitTypeData,C_PaymentStatus,C_PaymentType,C_PensionerCategory,C_PensionType,C_Post,C_PostPart,C_PostSalary,C_PostSalaryIndexing,C_PostSalaryIndexingPostParts,C_PostSalaryIndexingPosts,C_PostType,C_ProvisionMode,C_RaiseRatio,C_RegionalRatio,C_RegLog,C_RequestCategory,C_RequestMethod,C_RequestResultCategory,C_RestrictionRatio,C_RFSubject,C_Scanner,C_SenderCategory,C_Seniority,C_Sign,CalendarDay,Citizen,CitizenPension,CitizenRequest,CMSFormView,Complaint,ControlTask,CourtSession,Doc,DocAsso,DocTask,DocTaskComplaint,DocTaskOperation,DocTaskOperationIndexing,DocTaskOperationRaiseRatios,DocTaskRequest,DocTaskTransition,DocTransition,Employee,EmployeeData,ErrorLog,HST_N_TimeZone,IMP_Employee,IMP_OperationReason,IMP_OrgUnit,IMP_OrgUnit_PostPart,IMP_OrgUnitAsso,IMP_PFR,IMP_PFRRFSubject,IMP_Post_Fed,IMP_Post_Fed_Salary,IMP_Post_Gov,IMP_Post_Gov_Salary,IMP_Post_Mil,IMP_Post_Mil_Salary,IMP_Post_Other,IMP_PostPart,IMP_PostSalaryIndexing,IMP_PostSalaryIndexingPostParts,IMP_RegionalRatio,IMP_RFSubject,InfoDoc,InfoDocAccess,JuridicalCase,MailMessage,MailTemplate,MassCalc,MassCalcCitizens,MassCalcOrgUnits,MassCalcPaymentStatuses,MassCalcPensionerCategories,MassCalcPostParts,MassCalcPosts,MassCalcRFSubjects,MassOperationQueue,MM_DBProgrammability,MM_FormView1,MM_Group,MM_GroupAsso,MM_MethodParameter,MM_ObjectTypeStereotypes,MM_TaggedValueType,MMS_ChangeLog,MMS_ClassStereotype,MMS_Replication,MMS_SiteInfoobjects,MMS_Versioning,N_AppVar,N_Cache,N_DDL,N_DownloadLog,N_File,N_FileLibrary,N_FileLibraryType,N_Filter,N_Folder,N_FolderPredicate,N_ObjectChange,N_ObjectPropertyChange,N_ReplicationObject,N_RssFeed,N_Settings,N_SettingsGroup,N_SiteNode,N_SqlStatementLog,N_TableInfo,N_TextResource,N_TextResourceData,N_TimeZone,N_VirusScanLog,OfficeNote,OfficeNoteDocs,OrgUnit,OrgUnitAsso,OrgUnitData,OrgUnitHieRule,OrgUnitPostParts,OrgUnitPosts,OrgUnitRFSubjects,OutDocSend,OutDocSendDoc,PensionFile,ScanUserSettings,SiteInfoobjects,SPM_Action,SPM_ActionAsso,SPM_AvailableRoleForGrant,SPM_C_RoleType,SPM_CasheFlag,SPM_Role,SPM_RoleAccess,SPM_RoleAsso,SPM_RoleGroup,SPM_Subject,SPM_SubjectAccess,SPM_SubjectDelegate,SPM_SubjectRole,sysdiagrams,TestDoc,TestDocRow,TM_Task,TM_TaskExecution,TM_TaskParameter,UserActivity,WorkInfo,sysdiagrams,N_FileData".Split(',').ToList();
			var listItems = "30ca8918-5a5e-4f2a-b19b-001194d1346f,784bc6b1-c57e-4354-86a6-004c44c40984,06f8ec7b-41a2-4abb-852d-006b58d194bf,445227be-9fea-4777-8d8a-00dadc36e563,30bd2ee8-7768-44b9-9211-01440d980a53,da4a001f-b854-42c2-b0bd-014c5c580df5,b81cc9ec-aa3d-4d49-9abb-01bf9d1a0c0d,e96c3d0a-86a0-46ea-9a3c-01d6945e5e63,af6ded44-d38e-40dc-a3db-01f08c89a904,39bef69b-29f3-490a-bf9f-021104d7c992,254bd9e9-34c9-4c21-8421-0213a9a0dc82,1935d8ff-7098-495a-acc4-02967349e46c,6c9893c6-5fc0-4068-b000-02fa3f7035e7,a58e5648-9a61-447e-944b-03094f565d45,a2e217c6-c6e4-4c90-bdc8-0317ba4a9b8a,4dec41c8-074c-43c3-b898-0320f66f7129,cf00de31-78c2-4b37-bdd2-032409f48a6f,ae47a19a-fba3-459d-b4c7-032b20af5649,be3962f3-a4d9-49de-90db-036d3412eb3d,73062fb4-ed99-4e77-8056-03e1e4de451c,abc5a830-7b2d-4bf2-a279-0427698a975c,e6fe52b4-8ee2-47d8-b18c-042d680a2351,4c637be9-a868-4447-91b2-0431c0ee5e18,97320c2b-4cd3-4372-a9c0-04aee34ed728,d8b63ec7-d749-4606-a4cf-04d1367a7f01,9e2443ce-3be0-44d9-8084-0517c6335f33,75701f34-748d-4bdd-84e4-0530ee0c8e13,791efb39-45d6-4a06-8958-053f616e3d9a,6952d6f1-b55c-4c8a-9347-05671c7e6d6c,afe50294-7cd7-4a2c-9d7e-0617f1c4189f,9dfda338-69c5-42ea-86d4-06b30989ca5f,b8d194ed-54b0-46a9-ae1e-06c6a18ff623,9c4be030-91c0-449e-a623-06f5447a6634,f4c96c68-4115-4ab0-b557-06fd2ecb6626,6bbf60cc-443f-4881-8ed8-0710beb13f62,5048fb7c-d42c-4b15-b735-0736ccb8d2dc,65d69ecc-667a-41cb-a3df-07681ff49ce3,2783283e-4b0a-4ce9-b430-07b6f53e07d4,53d5464e-d111-4295-8383-07ef9a8cfe86,dbb06ed1-ec53-48cb-8298-088acf6187e7,24b6417f-baa9-40e4-9eb4-08d46b19bd0a,31bc083c-27da-432b-869b-08d96dc72f3c,2dd67cd2-0dfc-45dc-a747-08f47ddfeff0,1e248312-e80f-4551-bb13-09447daa462b,992d48f7-8fe5-4851-9587-0962007de285,5d317571-4b37-4f73-82f8-0968f8ad331e,570a4914-02d6-4ce3-8902-098320b3c849,77cd1b3a-9226-4221-86e8-0985d4dc09a3,29501bce-de01-4b0a-be6d-09c079ad1e76,1b3c1490-4584-4da8-b560-09f5cba6a613,0e4cf412-0f96-4d6e-bc10-0a2c24862860,33466522-10ad-4bd0-a861-0a64b3d619d5,a6293c6c-4d49-4cac-a9d2-0a78e8a01b98,075bb6ce-247a-4155-add1-0ade6e0fab63,5019e6b8-0f7a-4f9b-8ceb-0af004f7a851,b187a424-94cd-4a6e-ba49-0b4699fae15a,b04bc3eb-c339-4f9d-9402-0bbc655bae23,53aee822-57e2-49ce-adfe-0bf7d8ea13e2,9c9eabd3-ee71-4b2a-a8ac-0c0509d65686,2c69b829-54e7-4cd6-ae40-0c0aa7ad484d,e18c5ed8-6e63-44f5-8556-0c5351f55bd9,41d9d5c1-63a4-4dd1-8561-0ca556097083,923fa7cb-35db-4f84-9079-0ce82790270e,fb0ed738-085d-4736-84fb-0cec20ccef90,9d658392-cc47-4cf4-9795-0cfa6a4c2d1a,8a1a8781-e58e-4054-a011-0d10e9f28289,aba461c5-3bea-4bc3-9147-0d16f82a66e2,af185ee1-995d-4ff0-96ae-0dffff365b32,49d7c3aa-4fc4-40ae-9412-0e01aa9e9ef1,bc69978e-f2c3-4054-9e17-0e4e1be42c69,6fd3b39b-1b10-486a-a645-0e69b4dcf86f,b1e54e15-11db-4cb0-ac8c-0e866bbbce2a,778207b1-7903-441e-8356-0ed7cc15ba7a,e171c72d-de49-4559-ae2b-0ef931ebdfbf,e684290a-698f-4f3f-9faa-0f2e6a5bd23b,f73739dd-b34f-4eda-b06f-0f90f65e19a5,a6f50445-6a80-4aa3-8694-100075f3e6ba,12811e3f-aaba-4347-8d00-100e3ec550da,2e5c5d49-5bb8-40ba-a0af-108af6f1db14,6cae592a-a075-4db5-a8c4-10c641bb1b3f,5823e856-6b2d-49f8-966f-116e9d40f768,d45ca815-7815-45af-8d96-11b10e3f4bd4,1eee2a74-bcda-49b8-9de6-12a436ef205b,88c55c0b-9bd7-4258-b8bc-13534b1f4c27,f19d6a3e-eb5f-412a-85ee-1392c01d398e,68ca0473-e8f8-4b67-b4d7-13ae55bcd1d8,c6f526bf-2621-4646-9162-13fde8b67eba,bfbbf839-5511-4275-a306-144e6b3081c1,bafd9e7d-99b9-4d1d-a3f9-149810cfff01,b18f2778-4a2b-4034-b2fb-14e752b80b78,491dc6ca-41ae-4934-b7be-151331e6e3c4,9a76800c-edc5-49b6-8cd9-153231858641,5951c0dc-ac15-4fe4-a566-1559efd356ae,b272d39e-4c0f-45ee-8291-156183db7bf7".Split(',').ToList(); ;
			foreach (var table in schema.Tables)
			{
				if (listTable.Contains(table.Value.Name))
					continue;

				if (table.Value.Name == "N_FileData")
				{
					result.Clear();
					var isdo = true;
					var page = 1;

					result.AppendFormat("SET INTEGRITY FOR  {1}.{0} ALL IMMEDIATE UNCHECKED;\r\n", table.Value.Name.ToUpper(), "DBO");
					result.AppendFormat("CALL SYSPROC.ADMIN_CMD( 'REORG TABLE {1}.{0}' );\r\n", table.Value.Name.ToUpper(), "DBO");
					result.AppendFormat("DELETE FROM  {1}.{0} ;\r\n", table.Value.Name.ToUpper(), "DBO");

					while (isdo)
					{


						Console.WriteLine("Генерим скрипт для " + table.Value.Name);
						var list = ImportData(table.Value, table.Value.Columns.Any(t => t.Value.IsPrimaryKey), con, result, 10, page, ref isdo);
						Console.WriteLine("Заливаем таблицу  " + table.Value.Name);
						foreach (var nfile in list)
						{
							if (listItems.Contains(nfile.guid))
								continue;
							Console.WriteLine("Заливаем запись  " + nfile.guid);
							listItems.Add(nfile.guid);

							DB2Connection db3con = new DB2Connection("Database=servants;UserID=db2admin;Password=q121212;Server=193.233.68.82:50000");
							db3con.Open();
							DB2Command cmd2 = db3con.CreateCommand();
							cmd2.CommandType = System.Data.CommandType.StoredProcedure;
							cmd2.CommandText = "DBO.INSERTNFILE";
							cmd2.Parameters.Add("Data", nfile.data);
							cmd2.Parameters.Add("Extension", nfile.ext);
							cmd2.Parameters.Add("FileGUID", nfile.guid);
							var sss1 = string.Join(",", listItems.ToArray());
							cmd2.ExecuteReader();
							db3con.Close();

						}


						page++;
					}


					continue;
				}


				Console.WriteLine("Генерим скрипт для " + table.Value.Name);
				ImportData(table.Value, table.Value.Columns.Any(t => t.Value.IsPrimaryKey), con, result);
				Console.WriteLine("Заливаем таблицу  " + table.Value.Name);

				DB2Connection db2con = new DB2Connection("Database=servants;UserID=db2admin;Password=q121212;Server=193.233.68.82:50000");
				db2con.Open();
				DB2Command cmd = db2con.CreateCommand();
				cmd.CommandType = System.Data.CommandType.Text;
				cmd.CommandText = result.ToString();
				//Console.WriteLine("Скрипт  " + result.ToString());
				listTable.Add(table.Value.Name);
				var sss = string.Join(",", listTable.ToArray());
				cmd.ExecuteReader();
				db2con.Close();
				result.Clear();

			}

			result.Clear();
			Constraints(true, schema, result);
			DB2Connection db2con1 = new DB2Connection("Database=servants;UserID=db2admin;Password=q121212;Server=193.233.68.82:50000");
			db2con1.Open();
			DB2Command cmd1 = db2con1.CreateCommand();
			cmd1.CommandType = System.Data.CommandType.Text;
			cmd1.CommandText = result.ToString();
			Console.WriteLine("Востанавливаем констрейнт  " + result.ToString());
			cmd1.ExecuteReader();
			db2con1.Close();
			var ssss = result.ToString();

		}
		static void Constraints(bool enable, Schema schema, StringBuilder result)
		{
			foreach (var t in schema.Tables.Values)
			{
				foreach (var fk in t.ForeignKeys.Values)
				{
					if (
						(fk.RefTable.ToUpper() == "N_File".ToUpper() && fk.Columns.Any(c => c.ToUpper() == "FileID".ToUpper())))
						continue;

					if (fk.IsEnabled) //FileID
					{


						result.AppendFormat("ALTER TABLE {2}.{1} ALTER FOREIGN KEY {0} {3} ;\r\n", fk.Name.ToUpper(), t.Name.ToUpper(),
											t.Schema.Name.ToUpper(), enable ? "ENFORCED" : "NOT ENFORCED");

						//result.AppendFormat(" IF EXISTCONSTRAINT('{0}','{2}') IS NOT NULL THEN \r\n" +
						//						   "ALTER TABLE {2}.{1} ALTER FOREIGN KEY {0} NOT ENFORCED \r\n;" +
						//					"END IF; \r\n", fk.Name.ToUpper(), t.Name.ToUpper(), t.Schema.Name.ToUpper());
					}
				}
			}

		}
		public static string ImportData(Table t, bool identityInsert, SqlConnection DbConnection, StringBuilder sqlInsert)
		{

			if (DbConnection.State == System.Data.ConnectionState.Closed)
				DbConnection.Open();

			var columns = string.Join(", ", t.Columns.Values.Where(c => string.IsNullOrEmpty(c.ComputedText)).Select(c => string.Format("{0}", c.Name == "Group" ? "[Group]" : c.Name == "Index" ? "[Index]" : c.Name)).ToArray());
			SqlCommand cmd = DbConnection.CreateCommand();
			cmd.CommandType = System.Data.CommandType.Text;
			cmd.CommandText = string.Format("select {0} from [{1}] ", columns, t.Name);

			sqlInsert.AppendFormat("SET INTEGRITY FOR  {1}.{0} ALL IMMEDIATE UNCHECKED;\r\n", t.Name.ToUpper(), "DBO");
			sqlInsert.AppendFormat("CALL SYSPROC.ADMIN_CMD( 'REORG TABLE {1}.{0}' );\r\n", t.Name.ToUpper(), "DBO");
			sqlInsert.AppendFormat("DELETE FROM  {1}.{0} ;\r\n", t.Name.ToUpper(), "DBO");
			if (identityInsert && t.Identity)
			{
				sqlInsert.AppendFormat("ALTER TABLE {2}.{0} ALTER COLUMN {1} SET GENERATED BY DEFAULT;\r\n", t.Name, t.Columns.Values.FirstOrDefault(c => c.IsPrimaryKey).Name, "DBO");
			}
			using (var reader = cmd.ExecuteReader())
			{
				while (reader.Read())
				{
					StringCollection sc = new StringCollection();
					for (int i = 0; i < reader.FieldCount; i++)
					{
						sc.Add(GetStringValue(reader, i).Replace("\\", ""));
					}
					sqlInsert.AppendFormat("INSERT INTO DBO.{0} ({1})  VALUES ({2}); \r\n", t.Name, columns.Replace("[", "").Replace("]", ""), string.Join(",", sc.Cast<string>().ToArray<string>()));
				}
			}

			return sqlInsert.ToString();
		}

		public static List<nfile> ImportData(Table t, bool identityInsert, SqlConnection DbConnection, StringBuilder sqlInsert, int count, int page, ref bool isdo)
		{
			var list = new List<nfile>();
			isdo = false;
			page = page - 1;
			if (DbConnection.State == System.Data.ConnectionState.Closed)
				DbConnection.Open();

			var columnsin = string.Join(", ", t.Columns.Values.Where(c => string.IsNullOrEmpty(c.ComputedText)).Select(c => string.Format("{0}", c.Name == "Group" ? "[Group]" : c.Name == "Index" ? "[Index]" : c.Name)).ToArray());
			var columns = string.Join(", ", t.Columns.Values.Where(c => string.IsNullOrEmpty(c.ComputedText)).Select(c => string.Format("{0}", c.Name == "Group" ? "[Group]" : c.Name == "Index" ? "[Index]" : "m." + c.Name)).ToArray());
			SqlCommand cmd = DbConnection.CreateCommand();
			cmd.CommandType = System.Data.CommandType.Text;
			var sqltext = string.Format("select  {0} from (SELECT {4} ,ROW_NUMBER()  OVER (ORDER BY [FileGUID]) AS rn " +
								"FROM {1} ) as m where m.rn>{2} and m.rn<={3}", columns, t.Name, page * count, (page * count) + count, columnsin);
			cmd.CommandText = sqltext;
			//string.Format("select top {2} {0} from [{1}] ", columns, t.Name);


			if (identityInsert && t.Identity)
			{
				sqlInsert.AppendFormat("ALTER TABLE {2}.{0} ALTER COLUMN {1} SET GENERATED BY DEFAULT;\r\n", t.Name, t.Columns.Values.FirstOrDefault(c => c.IsPrimaryKey).Name, "DBO");
			}
			using (var reader = cmd.ExecuteReader())
			{
				while (reader.Read())
				{
					isdo = true;
					StringCollection sc = new StringCollection();
					var nfile = new nfile();
					nfile.data = (byte[])reader["Data"];
					nfile.ext = reader["Extension"].ToString();
					nfile.guid = reader["FileGUID"].ToString();
					list.Add(nfile);
					//sqlInsert.AppendFormat("INSERT INTO DBO.{0} ({1})  VALUES ({2}); \r\n", t.Name, columns.Replace("[", "").Replace("]", ""), string.Join(",", sc.Cast<string>().ToArray<string>()));
				}
			}

			return list;
		}
		public static string GetStringValue(SqlDataReader reader, int index)
		{
			if (reader.IsDBNull(index))
				return "null";
			else
			{
				switch (reader.GetDataTypeName(index))
				{
					case "money":
						return reader.GetSqlMoney(index).Value.ToString(CultureInfo.InvariantCulture);
					case "float":
						return reader.GetSqlDouble(index).Value.ToString(CultureInfo.InvariantCulture);
					case "int":
						return reader.GetInt32(index).ToString();
					case "smallint":
						return reader.GetInt16(index).ToString();
					case "tinyint":
						return reader.GetByte(index).ToString();
					case "bigint":
						return reader.GetInt64(index).ToString();
					case "nvarchar":
						return "N'" + reader.GetString(index).Replace("'", "''").Replace("\0", " ") + "'";
					case "varchar":
						return "N'" + reader.GetString(index).Replace("'", "''").Replace("\0", " ") + "'";
					case "bit":
						return reader.GetBoolean(index) ? "1" : "0";
					case "uniqueidentifier":
						return "N'" + reader.GetGuid(index).ToString() + "'";
					case "char":
						return "N'" + reader.GetString(index).Replace("'", "''").Replace("\0", " ") + "'";
					case "nchar":
						return "N'" + reader.GetString(index).Replace("'", "''").Replace("\0", " ") + "'";
					case "text":
						return "N'" + reader.GetString(index).Replace("'", "''") + "'";
					case "decimal":
						return reader.GetDecimal(index).ToString(CultureInfo.InvariantCulture);
					case "date":
						return String.Format("CAST('{0}' AS Date)", reader.GetDateTime(index).ToString("yyyy-MM-dd"));
					case "datetime":
						return String.Format("CAST('{0}' AS timestamp)", reader.GetSqlDateTime(index).Value.ToString("yyyy-MM-dd HH:mm:ss"), reader.GetSqlDateTime(index).TimeTicks.ToString("X8"));
					case "image":
						StringBuilder result = new StringBuilder();
						byte[] data = reader.GetSqlBytes(index).Value;
						for (int x = 0; x < data.Length; x++)
							result.Append(data[x].ToString("X2"));
						return string.Format("blob(X'{0}')", result.ToString());
					case "xml":
						return String.Format("N'{0}'", reader.GetSqlXml(index).Value.Replace("'", "''"));
					case "varbinary":
						StringBuilder result1 = new StringBuilder();
						byte[] data1 = reader.GetSqlBytes(index).Value;
						for (int x = 0; x < data1.Length; x++)
							result1.Append(data1[x].ToString("X2"));
						return string.Format("blob(X'{0}')", result1.ToString());
					default:
						throw new Exception("unknown data type: " + reader.GetDataTypeName(index));
				}
			}
		}

	}
}
