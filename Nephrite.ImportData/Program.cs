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
using Nephrite.Web;

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
			ConnectionManager.SetConnectionString("Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=servants1;Data Source=TOSHIBA-TOSH\\SQL2008");
			SqlConnection con = new SqlConnection("Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=servants1;Data Source=TOSHIBA-TOSH\\SQL2008");
			var schema = new SqlServerMetadataReader().ReadSchema("DBO");
			StringBuilder result = new StringBuilder();
			Constraints(false, schema, result);
			var listTable = "MM_FormView,DocTaskOperation,DocTask,C_RFSubject,C_MassOperationState,Appendix,C_CitizenCategory,C_CourtSessionStatus,C_DocClass,C_DocName,C_DocTaskType,C_DocType,C_DocTypeAppendix,C_ExtraPayPost,C_FIAS_ActualStatus,C_FIAS_AddressObject,C_FIAS_AddressObjectType,C_FIAS_CenterStatus,C_FIAS_CurrentStatus,C_FIAS_EstateStatus,C_FIAS_House,C_FIAS_HouseInterval,C_FIAS_HouseStateStatus,C_FIAS_IntervalStatus,C_FIAS_Landmark,C_FIAS_NormativeDocument,C_FIAS_OperationStatus,C_FIAS_StructureStatus,C_Help,C_InfoDocType,C_JuridicalInstance,C_Language,C_MassCalcStatus,C_MassOperationType,C_OperationReason,C_OperationType,C_OrgUnitHie,C_OrgUnitType,C_OrgUnitTypeData,C_PaymentStatus,C_PaymentType,C_PensionerCategory,C_PensionType,C_Post,C_PostPart,C_PostSalary,C_PostSalaryIndexing,C_PostSalaryIndexingPostParts,C_PostSalaryIndexingPosts,C_PostType,C_ProvisionMode,C_RaiseRatio,C_RegionalRatio,C_RegLog,C_RequestCategory,C_RequestMethod,C_RequestResultCategory,C_RestrictionRatio,C_Scanner,C_SenderCategory,C_Seniority,C_Sign,CalendarDay,Citizen,CitizenPension,CitizenRequest,CMSFormView,Complaint,ControlTask,CourtSession,Doc,DocAsso,DocTaskComplaint,DocTaskOperationIndexing,DocTaskOperationRaiseRatios,DocTaskRequest,DocTaskTransition,DocTransition,Employee,EmployeeData,ErrorLog,HST_MM_FormView,HST_N_TimeZone,IMP_Employee,IMP_OperationReason,IMP_OrgUnit,IMP_OrgUnit_PostPart,IMP_OrgUnitAsso,IMP_PFR,IMP_PFRRFSubject,IMP_Post_Fed,IMP_Post_Fed_Salary,IMP_Post_Gov,IMP_Post_Gov_Salary,IMP_Post_Mil,IMP_Post_Mil_Salary,IMP_Post_Other,IMP_PostPart,IMP_PostSalaryIndexing,IMP_PostSalaryIndexingPostParts,IMP_RegionalRatio,IMP_RFSubject,InfoDoc,InfoDocAccess,JuridicalCase,MailMessage,MailTemplate,MassCalc,MassCalcCitizens,MassCalcOrgUnits,MassCalcPaymentStatuses,MassCalcPensionerCategories,MassCalcPostParts,MassCalcPosts,MassCalcRFSubjects,MassOperationQueue,MM_Codifier,MM_CodifierValue,MM_DataValidation,MM_DBProgrammability,MM_FormField,MM_FormFieldAttribute,MM_FormFieldGroup,MM_FormView1,MM_Group,MM_GroupAsso,MM_Method,MM_MethodGroup,MM_MethodGroupItem,MM_MethodParameter,MM_ObjectProperty,MM_ObjectType,MM_ObjectTypeStereotypes,MM_Package,MM_Predicate,MM_TaggedValueType,MMS_ChangeLog,MMS_ClassStereotype,MMS_Replication,MMS_SiteInfoobjects,MMS_Versioning,N_AppVar,N_Cache,N_DDL,N_DownloadLog,N_File".Split(',').ToList();
			var listItems = "30ca8918-5a5e-4f2a-b19b-001194d1346f,784bc6b1-c57e-4354-86a6-004c44c40984,06f8ec7b-41a2-4abb-852d-006b58d194bf,445227be-9fea-4777-8d8a-00dadc36e563,30bd2ee8-7768-44b9-9211-01440d980a53,da4a001f-b854-42c2-b0bd-014c5c580df5,b81cc9ec-aa3d-4d49-9abb-01bf9d1a0c0d,e96c3d0a-86a0-46ea-9a3c-01d6945e5e63,af6ded44-d38e-40dc-a3db-01f08c89a904,39bef69b-29f3-490a-bf9f-021104d7c992,254bd9e9-34c9-4c21-8421-0213a9a0dc82,1935d8ff-7098-495a-acc4-02967349e46c,6c9893c6-5fc0-4068-b000-02fa3f7035e7,a58e5648-9a61-447e-944b-03094f565d45,a2e217c6-c6e4-4c90-bdc8-0317ba4a9b8a,4dec41c8-074c-43c3-b898-0320f66f7129,cf00de31-78c2-4b37-bdd2-032409f48a6f,ae47a19a-fba3-459d-b4c7-032b20af5649,be3962f3-a4d9-49de-90db-036d3412eb3d,73062fb4-ed99-4e77-8056-03e1e4de451c,abc5a830-7b2d-4bf2-a279-0427698a975c,e6fe52b4-8ee2-47d8-b18c-042d680a2351,4c637be9-a868-4447-91b2-0431c0ee5e18,97320c2b-4cd3-4372-a9c0-04aee34ed728,d8b63ec7-d749-4606-a4cf-04d1367a7f01,9e2443ce-3be0-44d9-8084-0517c6335f33,75701f34-748d-4bdd-84e4-0530ee0c8e13,791efb39-45d6-4a06-8958-053f616e3d9a,6952d6f1-b55c-4c8a-9347-05671c7e6d6c,afe50294-7cd7-4a2c-9d7e-0617f1c4189f,9dfda338-69c5-42ea-86d4-06b30989ca5f,b8d194ed-54b0-46a9-ae1e-06c6a18ff623,9c4be030-91c0-449e-a623-06f5447a6634,f4c96c68-4115-4ab0-b557-06fd2ecb6626,6bbf60cc-443f-4881-8ed8-0710beb13f62,5048fb7c-d42c-4b15-b735-0736ccb8d2dc,65d69ecc-667a-41cb-a3df-07681ff49ce3,2783283e-4b0a-4ce9-b430-07b6f53e07d4,53d5464e-d111-4295-8383-07ef9a8cfe86,dbb06ed1-ec53-48cb-8298-088acf6187e7,24b6417f-baa9-40e4-9eb4-08d46b19bd0a,31bc083c-27da-432b-869b-08d96dc72f3c,2dd67cd2-0dfc-45dc-a747-08f47ddfeff0,1e248312-e80f-4551-bb13-09447daa462b,992d48f7-8fe5-4851-9587-0962007de285,5d317571-4b37-4f73-82f8-0968f8ad331e,570a4914-02d6-4ce3-8902-098320b3c849,77cd1b3a-9226-4221-86e8-0985d4dc09a3,29501bce-de01-4b0a-be6d-09c079ad1e76,1b3c1490-4584-4da8-b560-09f5cba6a613,0e4cf412-0f96-4d6e-bc10-0a2c24862860,33466522-10ad-4bd0-a861-0a64b3d619d5,a6293c6c-4d49-4cac-a9d2-0a78e8a01b98,075bb6ce-247a-4155-add1-0ade6e0fab63,5019e6b8-0f7a-4f9b-8ceb-0af004f7a851,b187a424-94cd-4a6e-ba49-0b4699fae15a,b04bc3eb-c339-4f9d-9402-0bbc655bae23,53aee822-57e2-49ce-adfe-0bf7d8ea13e2,9c9eabd3-ee71-4b2a-a8ac-0c0509d65686,2c69b829-54e7-4cd6-ae40-0c0aa7ad484d,e18c5ed8-6e63-44f5-8556-0c5351f55bd9,41d9d5c1-63a4-4dd1-8561-0ca556097083,923fa7cb-35db-4f84-9079-0ce82790270e,fb0ed738-085d-4736-84fb-0cec20ccef90,9d658392-cc47-4cf4-9795-0cfa6a4c2d1a,8a1a8781-e58e-4054-a011-0d10e9f28289,aba461c5-3bea-4bc3-9147-0d16f82a66e2,af185ee1-995d-4ff0-96ae-0dffff365b32,49d7c3aa-4fc4-40ae-9412-0e01aa9e9ef1,bc69978e-f2c3-4054-9e17-0e4e1be42c69,6fd3b39b-1b10-486a-a645-0e69b4dcf86f,b1e54e15-11db-4cb0-ac8c-0e866bbbce2a,778207b1-7903-441e-8356-0ed7cc15ba7a,e171c72d-de49-4559-ae2b-0ef931ebdfbf,e684290a-698f-4f3f-9faa-0f2e6a5bd23b,f73739dd-b34f-4eda-b06f-0f90f65e19a5,a6f50445-6a80-4aa3-8694-100075f3e6ba,12811e3f-aaba-4347-8d00-100e3ec550da,2e5c5d49-5bb8-40ba-a0af-108af6f1db14,6cae592a-a075-4db5-a8c4-10c641bb1b3f,5823e856-6b2d-49f8-966f-116e9d40f768,d45ca815-7815-45af-8d96-11b10e3f4bd4,1eee2a74-bcda-49b8-9de6-12a436ef205b,88c55c0b-9bd7-4258-b8bc-13534b1f4c27,f19d6a3e-eb5f-412a-85ee-1392c01d398e,68ca0473-e8f8-4b67-b4d7-13ae55bcd1d8,c6f526bf-2621-4646-9162-13fde8b67eba,bfbbf839-5511-4275-a306-144e6b3081c1,bafd9e7d-99b9-4d1d-a3f9-149810cfff01,b18f2778-4a2b-4034-b2fb-14e752b80b78,491dc6ca-41ae-4934-b7be-151331e6e3c4,9a76800c-edc5-49b6-8cd9-153231858641,5951c0dc-ac15-4fe4-a566-1559efd356ae,b272d39e-4c0f-45ee-8291-156183db7bf7,fe253ae0-5a91-415b-ace0-156ad1522505,15a9370b-be5a-49e6-88ff-157212af41c8,fcf922f2-a9c2-4fc7-9361-15c4173aca13,419ebe1d-a277-4c84-9c8e-15de2b2ca02c,595f8ee7-465a-4fc4-a646-16662b021302,46b5d2e8-b575-4892-9d22-166a24b437dc,d67b3a05-535e-4fc2-bf27-16758ffea188,9fb13b35-c2bc-4c89-967c-1678e84b2a0b,d6301d9d-ae2a-4746-91cd-168885443fee,59ca42ed-b528-4bca-b49c-1691e271163e,68fdf1e6-db6a-433c-8668-169649215f7d,d8841da1-18ff-4c39-972d-16bd780980bc,0ee9235f-6aa3-4361-8a6b-175c1882b22d,b4278fef-4d25-4f0d-b34d-178d731b4b00,b4682248-a6ae-4a0d-9732-17ac4d1f6d65,a0acd941-747b-47e5-9285-17dc673954ee,6bcd8b53-e435-4ba6-b34c-180d6a634260,c8b7d95e-88d2-44b2-8fd4-181b3a822d1f,b1eaef77-7c82-41cc-823f-18686c32f7b1,58ff603a-6c0e-43e7-935f-189f88fa9441,bfddb22d-ec2e-4c02-9a74-18a6c9c816ec,0ff0d239-cff8-4911-b249-18b40c480b21,2692e37f-5c7e-4f2a-ada8-18c9022a35b5,b656a1ae-1a2e-45ba-b575-1939b2fa3512,d29dbfef-5faf-4d59-b916-197bf76a9033,73aa6ed5-2427-40f3-baa6-19e89a3e0629,d09e7d4a-0de4-4c84-b52b-1a5298c15403,7dcf402a-64fc-442b-871a-1a6963fcdf16,a7378510-4c49-42c4-9ca1-1a8e83467311,0649cbb5-745b-4c48-92dc-1aa93cc5f7e6,1eab87a3-b21e-4b5c-a5a7-1b0c3baac266,02993616-15fc-445d-95fc-1b140060817b,97fee026-979e-48b2-83f5-1b3cdde00edc,a8e1d807-ee59-4920-a08c-1b5974dce108,cf81bb1b-62db-444f-ac00-1b825faac82b,c37b10c6-b27a-48fd-8ca0-1c07b580ae74,a8b9f0d3-bd9f-4f59-b4b0-1c4ff0f7c3de,955f34af-5e88-45aa-aebe-1c584b62ac72,6b5dd27c-1e74-4ea3-98a1-1cb57266c648,af529d40-a413-47f4-814c-1d2b9cf180a8,f7c66d91-c8c2-47d9-bac6-1d8f8bb988f6,adcf7c44-9b54-4433-b673-1dc5cc73e993,b0114da9-7095-4900-b050-1e4de3c46865,686e1ea8-bfe3-4e69-8ddb-1e72653bdb12,3ba893cf-8d09-4dc2-a886-1efcd3cd2ee5,8ed73b5e-cbf9-41b0-a2e1-1f192f0a06fe,5e3b2403-6184-47ac-a5d1-1f1cf69edd88,9ead4c10-ee54-4b25-b9ed-1f48d02105c1,9072d983-05d1-44f0-822c-1fc60ac0c3c1,82bbc2c1-6f6a-41b6-ba85-2044e4d064a3,7ec21132-c6f1-4000-8111-206ca958eee8,d87225b9-9e24-4830-9ec5-208dd8cc024b,617e1e08-ea27-425d-93ab-20a99f52cb8b,96111d85-fdfd-44b0-a459-20ef308d27b7,af5f8739-1fe5-4da9-bf6b-210fc61193d6,a9e04f7c-09f4-4586-97ba-212bb0214c38,619cebab-3b8d-4e25-8b2c-212decf28b8f,d0f8d89e-1b5b-4108-aee1-2152406b60b8,4829849c-7128-4bce-bfe6-21805ea7f1fa,f38a111a-acbf-4cd9-83d5-218eb72fbb13,a97387f5-c7b3-4924-b03c-21d8623a7451,34140bcd-f6e2-407e-bd7e-21d8d39d90f9,23853942-bb2a-4dd3-b415-21dab943d496,63fcba79-95a9-4f3a-85eb-224f7c031e9a,e02120f3-f969-42f2-833b-225e9ac0beee,e50b91be-dfca-476e-ac4e-23bf19ae8348,177dfae7-4df2-4804-9311-2442e31ff46c,0cc5198a-9daa-4cbf-9b7c-24546554005a,41820fca-a3c7-414f-8167-246696ae96cc,62f2911b-fb05-4449-acdd-24a9c2b2fe09,8abb3ab6-902c-4895-9b22-24ae547c72b3,06f0659b-9591-43e3-b3ee-24ae7a731060,9bc16363-d43f-48f3-8ee3-24cdd8687bc3,23c5a193-e2cd-4419-a068-24db70c31fca,674d1d49-0786-4b32-9239-251f0d141bff,2723de60-35ca-44c2-87bb-256140001af3,b8280b9b-3c7f-41d2-92ba-2570adaea86c,8212eebc-5b95-4704-9c3d-25c7bf08517a,cc990572-d88b-4245-a132-261f48a27185,26440103-5204-4474-878c-2689d02099b5,3c94d498-290b-4527-87ea-269f3732457d,685bbc2c-dd05-4793-9c1b-26c0bb07e1cd,05c55ced-64a3-40a2-a768-26c9f8bc639d,0ce77d68-334f-44a8-9bcd-26fc21d7805f,b60350c8-d403-4a5f-9778-272ebeaa6ea9,3d97df2a-f3b4-41ad-8094-273458e3f48e,8ec70b18-d38a-4c42-baeb-279709407946,5a9c03de-5c43-4763-a1b2-27b5a99222ee,4558e69d-ea4f-4ea8-bb00-28125fd0e39c,bee526bb-4ae5-4669-9b50-2890a2350729,1dcb4a5e-525f-48ce-bccb-28a4d1892355,5869cd7d-3a33-45ac-a095-28cf28378adc,82d416a2-e457-4a46-aa16-29704c2b3abe,b78c72c5-1877-43ce-b90a-29c2d1c4f0e4,6fed2c56-f9ae-4949-9d3b-2a6109517590,6e458eaa-cb4a-4226-be70-2a76e2b473ad,0b48c529-d70e-4e25-b5ea-2a8d5b56936d,4717b482-20c9-44da-8043-2adac3d07e04,b9b1dc3f-b33f-4867-bed8-2afc5c1bba2f,264c6ec8-509b-403c-bc95-2b9d993f1444,8f2154ae-eda7-4980-85df-2bdf111327c7,cfae0bf2-8147-4d59-af6c-2cb849a5263f,059369b8-15f2-44c5-a460-2d07181a22e5,fa698192-e727-412b-806b-2d48e926e986,22ce6957-fe82-437b-9efe-2d63022050e1,4d61aef0-5d83-4e48-855a-2d77637c4804,a863e94f-23e5-4c90-b5cb-2d8d1b4162e8,82c07e4c-ec71-4390-83a9-2dc054ebec2c,877d4ec8-2c78-4b54-8550-2dcdea3ec984,f02d083d-0f02-431a-abb9-2dda56afb187,83e029fc-f099-4338-8ad8-2e2368f4af00,7d5048c2-cd22-4739-9ace-2e4369d70f65,0bce7fdc-d4fc-4faa-ab75-2e64c6394ad8,b0694e21-6037-4f13-b0c3-2e9a3e3bc1da,f8f7a4a2-dfec-486b-8dcf-2ec58bd3b79b,bd62bf52-e939-4886-854e-2f05bd8bebcb,5656e3c7-ec8b-45f2-852d-2f387ec05e14,5959ac00-91bd-4825-b8c4-2f94bec4e1f8,1ed3c4a6-a0b5-4b10-9447-2f9a71277c46,c60f2400-16b2-4c1e-b3f4-2fc2e41836f1,751eba02-eae1-43d2-b001-2fe2acb8a39a,a265f627-9781-4f87-aff1-30095a59f034,d3311c60-a108-4656-8ea9-30435ddfb28a,d9c06580-ba37-47eb-b0ae-307f635686f1,78196e8b-a903-4113-8542-30e882938a96,41de69e6-f62b-43ed-9477-3132c2f55857,fd68b50e-e08e-4795-be4d-3143283cba8b,50e8ca19-0b77-41ae-841b-322e57c97b2b,d9970a22-cc21-446b-8f1f-324cfa2cc03a,0ce76073-ed46-497e-8c59-325b84ef3aaf,1e897321-b6ff-44dd-b72f-32ad62ec1e8d,f5be70bb-2e10-4d63-9086-32bd2e8c44cf,014242e2-513a-463b-af94-32dd88594796,dca83bdd-498a-4959-b7a0-33012b90d124,fd248d41-03dc-4a70-b9e2-332262761a8e,474c5e78-3356-466a-b295-3337199f5dc8,5accc4de-cd33-4982-9add-335bc2bc28a1,eeeb222c-6ac5-4f42-9ca3-335d7c98177d,2f0c450c-c201-43f9-be9b-33951f0a522b,cac178d3-d7bb-44b1-9f3d-33d3beae8e18,260b26c4-ba13-49ac-9253-343c03caa905,6e3c9eba-2439-46f2-b7f7-344f3e7936a7,a4190cf3-888c-476d-a063-348825c5e46e,5a2db1a2-715a-4fc0-8043-34b66886c34a,80e06171-8726-4961-bb42-34fe510efe16,5e4c45f6-1ce9-4624-9379-350e844ecf05,acea4578-f92e-4d26-b104-3530533827f5,f8adee37-b307-4a14-91b2-354bd3ca20c0,8f46312b-c709-4d65-9cf4-3568f7200bba,730beaeb-e304-4f08-810a-3578cb9f8971,9efab6bb-cb96-404d-9d25-358cd14a4ce9,8c5af64a-951a-4fb2-9c3b-35b737266904,5b6a3150-fcad-4e20-9f68-35db102837d4,09f31c58-178c-44aa-b6b2-3608b93a73d4,78586ba6-5327-44db-a512-363abea7945a,caeb6b1b-572e-4533-b89a-368d2682468f,76cfb03c-f829-479d-97ac-36bb0c5ddf97,9b349d1c-388d-4bac-832b-36ead4ba796b,fdd8e993-6f18-4959-8ba2-370dabe45e8d,ac9b9ac8-6ed9-4083-a563-37c576dbacee,54631c39-be29-4c67-b64e-384d212f787c,5015956c-eaba-4c93-989d-386803fc9d77,d501def0-6af7-4688-b7d5-38b18b44767e,869a2b8c-f149-48b0-a716-38dd3b9ec264,cf11459e-e8c9-428c-bb8e-38f14ef82297,669e5f8d-630a-4835-9ea9-394fceb662fa,42031943-6d9e-4eb2-b96f-398012a3ca07,700bd044-9a58-4d3c-8798-399b2749dc07,1cedb8f3-176a-49d6-b164-39b58e5f58e9,deb09359-fb2e-4a35-8cac-39d66bc5bd6f,dbb0cd97-d776-47e3-a38f-3a024789d0ee,592e6119-8080-4347-af40-3a2e345fe70e,66676347-68e9-4edd-a323-3a46db370a80,a146566f-567c-45dc-a1d8-3a77810499a4,576f9f28-7fe4-405b-aa6e-3a9c35163fa2,751ea4dd-5567-45b2-857e-3aa9f2f2d96d,53ebd3bb-f70f-4d55-80bc-3acf00cb7a1e,1caea84a-0526-4a12-8110-3ad8f9cafaa8,9053035c-c88d-45a9-b119-3aeb0f600213,a608903c-9bba-4538-95b7-3d122c336c8b,6c8ff7dd-fd9b-466c-a6cd-3d3c220d20a1,db192fd0-1e70-4032-986b-3d7fda07880a,5cc473dd-29f9-4e09-9457-3da0958ef7a0,9ed2ddb2-a084-4915-a637-3da8f9483c1b,0c140c44-4d54-4c67-80ab-3e54678f90a3,11e300cb-715e-47eb-81e2-3e70806ee3eb,7749f13a-cf07-43bb-85aa-3e8fbdf773f2,7eac64c4-7731-4d0a-b5eb-3edda8d191f5,1151fb7d-02f8-4a7a-b9d9-3f0f46026f75,dd30476e-bb14-48f6-969e-3f21c6790199,0c3ccd0a-40c4-4db7-b46a-3f671a5d5772,661d369f-38dc-43ba-a25f-40b29322b770,f3c5e5e6-21a9-476b-9fc2-40b2e4b6e2ce,55e00b2c-3f3a-4b71-b0c3-40d68f0d4e06,fb47cdb5-3efb-48a1-8e7b-412bd50a6ef3,284dc856-3767-4d69-9aa2-41a9590b0c12,158ac247-36ee-40b5-afd6-422de69e4c34,63c2e57e-f001-4e75-9fc8-42bebf122f26,3ec2c369-481d-404c-9fa0-43220efc099d,36f14f81-758d-4e43-8390-4350c645619c,94e1f501-e469-4c91-84c6-439f54f63d22,5dfe1c1a-373d-4593-b6c4-43a67819201d,3086e9c4-34bb-4adf-8870-442eb8ea5fae,a8782a7b-652a-4084-976a-446510478130,0f7bec9b-9ab6-4756-9eb4-4472c76f69d6,2f2d73dd-9dfd-43be-a07f-449de1a2c39a,217553bd-2d4a-4fb6-9c7c-44d4c39b80b9,a47eae73-80cf-41dd-baff-44f98fda9a84,828b1ce0-c201-4c76-9d68-45d73a18eab3,2e4a9beb-d0a6-4fa6-884d-45dfdc58e14f,373bd905-a8e2-4e9a-9373-46050b736d9a,e8943afb-c6b6-4328-bb70-4670064f0544,48c40e2d-170f-4888-800d-468c199c9de9,f7aed015-7b81-4342-89e4-46a6cebf96ce,99b3724a-5665-49e4-8dc1-46fdb768c8c5,7665df28-cac9-4835-a1bd-470529c0fbb5,d99adffd-e910-4c2b-9678-4708dc12b70c,572d2092-6133-4c1b-ad0e-475db8f50968,433c4ce5-cb74-4c9e-812e-47767cd9b45e,e809739f-b402-4386-8e79-47c480da8959,79081ed3-eefd-4d82-a342-48d3c04d63ef,065d95c0-3983-4b7b-8dc1-48e99d27f969,641f93dd-8619-4e88-b1d7-49037c224f85,74eb8eee-03e5-4edc-b5ec-49909bdcc1bc,269e0a64-90ea-45d4-bf55-49e96f7ab9ee,2b071467-9e96-434b-a014-49f4e1924062,f1241889-5c60-4843-83e7-4a58351f7d7d,537d2741-596d-4bc0-86c1-4bc15d1a33ef,d7442c12-a41d-479b-9c51-4be7393121e1,9e290a76-ae7a-4d3c-9425-4bebf9f7993d,0a3507df-8874-4514-833a-4bee1abed754,c20abc81-6182-432b-94fd-4c878aa1b266,6acaaf49-fd9b-4b2f-8be2-4cc684b97e95,43b6ae38-1684-4fa1-9072-4cd85948aba9,40741b6d-96e4-4905-b706-4cd8dd34f24e,bd60fea6-901e-4029-880a-4d082f0bef9c,2141219b-bb3d-4228-84a7-4d2f6214d3f4,76134b3b-cde0-413e-953a-4d8e117ed136,165b813e-393e-4adc-855a-4d9d27046439,4acb788a-26a5-4dcc-acf1-4e0db9ed72aa,bf881775-2e93-47d0-9d0d-4e17155786e1,d11f769c-69cc-4249-9695-4e37911d02f8,03d417f3-429c-4e4a-9c7f-4e5463b914c2,5db60b0d-e617-4c28-8852-4e6d8c69045f,656269e8-9cf3-4aa4-95f1-4e7cc1f3b1da,b7fdfc8f-949b-4b81-8547-4ef1d5405b7d,2332da1c-8f04-4123-81bf-4f3ad9361e10,7cd2a3ad-c9a2-4092-bf37-4f492cfb0ff7,6e3be6b0-007a-4a54-b0f8-4fa520584839,b95f85df-5d84-41db-8d53-4fc1d8b1f50a,f49f79dd-b58a-4fb3-9bf9-4ff1eeeb4ec9,c6ce01e3-41e8-4da9-8f54-50420c925f3b,fe37c627-f3bd-4d6a-9cb0-506eedaccb72,e746d1dd-387c-4dfa-bb0a-5090731c97a1,292d4ebe-e6f7-4995-8053-50b09971129e,11528d1c-23fe-4704-b294-50b97ec7f948,c873c57d-7ce6-423d-ab08-50fac0ccf990,f58a8964-89d1-4ab1-b9a7-510fbb4957f7,5ecb13fd-e2fa-40e6-9109-517aa9f4d659,ab48eb50-52e8-4d7c-b336-519c7b09ca6e,e9be72c4-d663-4aae-89bb-529b581bb273,f853db90-3dac-4549-bf45-52c5503fbaa4,65c5713e-b5c0-4f08-9947-52f575dc2b71,17021a14-4a4a-4215-ac78-533ce4cbb39c,6a687425-5174-4275-a529-53410f7ea916,eac31867-97fc-4d40-8ee6-534598fe3356,623bb93f-acff-45d0-bab0-53d94c9a32e0,9982a24c-2ef4-422d-b07c-54205a6ec745,7d5d46c6-16a7-4b63-b2d1-543377adc316,c76da0f5-a140-40aa-a80e-5578f2727c8f,605052e1-2784-4b24-a52c-55f179af7870,ed3eca33-1019-4b1b-8956-55fac086071e,cb426b10-9733-45fb-a749-56075fc1ac2f,c6499af5-5bcf-47bf-8611-5621844b5271,02b7d938-07f5-47bf-8ad6-569ff4d9999f,cb337bab-6a45-4b4d-8a18-56e45d5e45c7,fc5f3e66-a1fe-4e57-9e0e-571864afcbc1,6ed612a0-51d4-49ec-aee1-5720dcc29df8,5bc89b3d-e728-45a7-82bc-57a74ec6105d,7abd814d-308e-4f23-846e-580787305806,6273a361-5ede-4378-88d2-58ebe2eb27cc,191fd121-e710-4b67-af86-59088bf2b7eb,842234ab-ce30-4e9c-9d66-591277518a63,77385d9c-46f8-4211-9ef7-591923d106af,0911fc1d-a20c-49c2-82de-595317f05ded,0414681d-b97b-42ea-999a-5956d44774be,ee38da29-5cec-43e9-861d-598cf50567e1,766e4fad-6f20-4876-b4b6-59b16d21e45b,e8a4dec4-9906-44c9-8f5e-59cbb91e8089,f65fbb3a-5433-4bfa-bce9-5a63398a308a,60e73206-7351-4b39-b77a-5a733e83645f,e69e329b-808d-492a-b8e3-5ac6a222e745,4e68c5be-8593-4c38-8ed2-5ad9080aeb3d,8064fb82-ef51-4ed0-aab7-5b3a888607ea,6f4f4ab9-1533-46a1-abea-5bcdff744ec1,4b80f97f-a4a4-417b-a7c8-5bf307ab4fa1,3722898b-30d0-4039-94cb-5c7668e1016d,7b453ddb-a1a7-4728-8dd4-5c9e136501df,a33fdf89-c3a8-4350-bcf5-5cdd4a92d5d8,c36d8272-6bdb-4dd2-b657-5ce4f3391984,29d750d8-a4dd-47bb-85d3-5ce7e9a1a3d5,6b0f27a7-7d91-45b3-94f8-5d51d74687d9,a8aa92b8-d59b-44b9-8da4-5d76271e3578,f5ecdde1-eb20-47f4-b83e-5da6f0343d07,27604b62-cef8-420b-bd78-5da95ba8b21a,cdcf920d-80c1-4360-9d23-5df077af0caf,acef2703-62a4-49c7-ace8-5ef311241f19,1b95d1eb-6cdb-4005-895e-5f12bc091705,55044c89-c202-469b-83b8-5faf5cd77850,4c9cf9c4-3909-4f37-b070-5fc714252489,ac289e07-ecbd-466a-8a67-6049692cf60c,e1395d9e-97c2-45b1-8a4d-609da3c1858a,a69cfaa5-8692-4e7b-a777-60ba017382a5,506c287b-f838-4c5d-9348-60cb1b1b6369,35ecec2b-59dd-4661-a9a4-6149585a0e16,94c5b76e-5193-4254-9764-6223d017c45e,904afdac-a106-4dee-bb49-622e62302c88,63704597-d5a5-4eaa-ab67-629011f4bb80,a9eaf3e7-513d-455e-99c1-62951f8de4aa,f9ac7c84-4125-4ffa-9fe7-62f5b4dcecb4,de475d1f-b277-443d-857b-632a36ad4916,9ab881f5-b81d-4ae4-84e1-636a19fdd9bf,2d1b2748-b137-4117-92f2-637cf9d4caf3,6a62393a-4fc4-45ab-acd5-63b23b2f3ca2,b9f201d0-af8a-47e3-a534-641efd1c141d,a3b2f923-0499-45c5-abfc-643580590094,8e62cd40-b5b3-4688-82e8-6459d12fce69,af1c4052-e6bd-4fe2-adf7-6473527d1076,67397674-f1ff-4882-b2fc-648f2d8c25a7,670c704b-e906-42c3-9aad-64c34f18d905,1b447c97-34e1-4926-99f3-64f37cf48988,c03753dd-ffaf-416e-bbaa-6514abcf2678,2365c9bc-7323-4068-8322-658af4d55184,710cdbdc-57da-4a72-b62a-65a9345c521d,706e3598-00cc-47fc-be26-65b388b90b4d,d3a9c3cf-ee34-443a-9c52-65c66156caff,fc207147-e298-4b81-b815-660123205229,96b5ab84-749d-43ef-b89c-6623873d07cf,70661b70-99db-4bed-9c48-66930f29da8b,551d1e12-210e-42a8-8db7-66e198162932,c84e7d14-52f0-453b-b7bc-66f513944332,368e9e27-7406-4231-8bf2-673fd268b5e7,18820897-1562-4407-9aff-677441d6bbb4,fb3dca86-32cf-41f6-8ee5-679a73776c28,aba85d0c-0644-41d9-a57a-67badfeb213b,bfd39604-671e-4ba5-8dc1-6801f752d725,ad3ad6ae-d8b5-433c-8bef-68269de0fae9,9f048853-1359-4419-81d8-6845714f371b,6bb04861-0bf0-4b64-a070-685828acf4c5,933bc683-f2f2-4a3f-86c5-68589ab522bb,00166fed-ef2a-42c8-b242-68c509c4ef24,65f19100-a248-42f6-b92f-68dc76e69596,03f1679f-234b-40e2-a8e6-68e886540028,25d6ca57-ebb0-4154-a549-68fee1b81ab5,1618d46a-7fbc-4da9-aec4-695e35b0afa0,f5af4b3d-c56a-4aa8-b227-69b4d50123a8,fef77720-5df2-4d0f-a6cd-6a3e004f1ea5,0ea69fd4-1bb5-46e3-84c3-6ab7ea198555,45d62be3-9c96-4678-8efd-6ae8625ffb43,21ea20fc-6e85-4467-8096-6afa73ef0bfb,c5026b4a-04f9-4017-8611-6b400f27ccc4,22c5f0ba-a6a3-41df-891e-6b4a67ea4b9c,566b85cb-403f-4263-a424-6b674f80d16e,62658fe1-55c8-4480-a5b6-6b69d170f4ef,608193d4-98e3-409e-ac94-6b6f9968b8ae,a5e39900-1141-4b8e-87a5-6bd2ddc70bb1,aefb25e7-bf1d-45fd-9b7c-6be3407f89c3,b7b2a97e-63b5-4e92-b49b-6bed6d67167a,0c658f8a-888d-430d-a7bb-6c10e3532ccc,b6555969-d86f-4e69-94fc-6c27121e0874,000fccce-115f-457c-869c-6c631d848809,97617334-2a52-42d4-8798-6cbcde860030,97701600-78f0-406f-9c55-6cdc1af11822,ff91b764-3121-4199-8b62-6d4eb890e049,a14a4e31-22b7-4a96-ac52-6d5c7f13fee7,3f15a7b8-c5cf-4659-bc96-6d6c16d159fd,3d1e9942-ac0a-4cfb-aeb9-6dae509e6021,c795c37c-d4e5-4b54-ad68-6db28cfaff94,4020ac19-9d76-480a-ada0-6e9f3ddffe56,82ddec83-fbbb-45bc-99e4-6ed7668ede0f,691f45b6-7f60-4ab7-8efe-6edc577fdf6c,80c2b2c1-6736-471f-8b3d-6f1078f9e591,4e418118-712a-4dbf-871a-6f4e866bacc1,09cde59b-695c-4a22-8fb1-6f85149afb52,cdbf2454-1631-47c1-8814-6f9310d0ab50,c8a94433-06c9-4766-a0d1-6fd4353586fe,ee93471d-49bd-4454-adc8-6fffe8d776f5,ae15fd66-ec32-4028-927b-70394d4afe27,381c1258-f42d-4cd2-85c8-70686326999d,d6538732-e429-41ec-974b-70acb35f4328,de178599-6908-4aa6-9fc2-711f6941ca7f,68a5b51d-a105-4d55-91b8-7126ab9758f0,857a465f-aaa3-41bf-a975-7142fb8f4761,f9335e74-e9f0-4500-bb7f-714dffa88175,8effb41a-7636-481a-a6c8-717d2ee3acac,334651da-1687-46ef-a369-7195ee7ca3c8,d72244b4-cb0f-4879-a6a5-71dfecaf56f2,0425cf7d-70fd-488f-956f-7241f4fa238a,a1ee96c4-0e58-46c1-b2a5-724d9cf912da,7fee6b92-4b66-40f0-9ee7-726496037fe2,028789e5-35cf-4066-949d-726892db0a11,f5fb14f0-09bf-44a7-9caf-72d5054cbac4,3ba9dfc2-207e-4166-b57c-72f3ea40d2a8,6ecb54ca-c714-4c1c-81c7-7327740364c1,437c6279-c864-47d0-b8a8-734b8d3762c7,98f66782-6677-4469-90ce-73673ba81822,92a08fb1-4bba-4eba-b44b-739033379249,c90068bb-bcb7-4c23-a201-7395ffb673b3,9d936996-f191-4578-989e-73bfd5c8ea3e,9cbca8d3-9bf7-4e74-a6bd-7405cb9950d0,114dacd3-53c9-4338-bf44-744f9e0b5e80,e225f458-04ee-4c51-a7cd-7452838f9672,bc0183fe-af71-4f6f-ae21-74bfb3e887cd,77172ead-248b-46ee-8121-74d765e22e6e,158aabd3-6ec8-498b-9db8-758b131c40d9,54eac6a6-e372-4db1-8edd-75ba9232c7b5,6e061837-dbd8-479b-8e4a-75d0213dc093,a3c1be71-946d-4f9f-9298-75d20cd0b971,d889ee7b-3225-4aaf-846b-75e4bdb3c26b,98ff553c-ac41-49cd-b20b-75ea0d81a41c,e143049d-e07a-4c45-af6b-760f453fb91e,518c68cb-2e3a-483d-8176-76136532a0fe,af8c983e-76d0-4035-a163-761afe57306a,1a1e4af0-34f6-4c35-b80b-762701be957e,03f2a75f-471c-453f-a39b-768237f7158d,29750513-72b3-43bd-a8e9-7695a97e5bee,7955012e-67a8-4006-b935-76fc1b34aced,cfe0e1c8-fe43-423c-af2f-77460ecf7537,d2d85ac3-e2b4-45f6-acea-77501aab6953,af44f563-78c2-41f9-9ff1-7779e6d6bd8b,0a3374ad-63c2-4edd-9ff6-7872fc2dc6b6,1387d55b-d24d-47b9-8ac3-78bb647e200e,baeb588d-0118-4748-9ad9-78e36cb18877,cb9e0cc6-9b66-4532-99b8-78eadd24992c,bd53f335-727a-408e-a9cb-793eef86658e,9b53b7c2-f23c-44a8-ad57-7984405e5ae4,0262397c-e65c-45a0-9be4-79ae7d446498,650681a2-8925-4dfa-a70f-79b4b49d5439,9b5313db-3779-486f-88a0-79b7d5cd5f99,559fa45f-e5f3-4840-8828-79ed018d14bb,9308b44d-c150-4d69-b460-7a285cc10202,d15e804e-ca18-41e9-8d1f-7a535f3f18b5,972d42f2-e18b-48cc-8bcf-7a64fe963b05,1080be89-8825-44bb-a674-7a93a7cccfbb,bd77ed3f-29f7-49d0-be45-7acf681a9a9b,267340ae-c71f-439c-9520-7bc6bf9a7748,1fb235b1-254d-4830-9f12-7bfbbc37d148,969d0bc0-8dd6-43db-99c5-7cf207c82333,7efdb7e7-c356-4465-bbff-7d0300032ae7,bad6a95b-bcf0-4b7f-9ddc-7d224f512552,f05f83d2-3386-41ad-8bb5-7d368b63e4ae,643b965c-cc20-495a-9815-7d5e6f3c5a6d,7ebc3241-c26b-4920-bbe2-7e47b903a0c1,4184ffec-46b2-44ec-9d6d-7eacf10c5e0a,2352013c-8a89-4ba7-93f5-7f1094f2fbc8,5b3a9fcd-abbc-4f5c-88f1-7f4dabc609f2,fad572c9-6de8-44e3-bddb-7f5f71ec2d62,d9b47971-3804-4b08-a95f-7f7df888a363,78b4b240-a5e6-4371-8896-7f8be3386b0d,cf0bb879-1a52-44f1-b6fe-7f9295c2700d,632097cc-c105-4688-8b37-8045b471521a,dba8eaf9-660b-4db5-86c5-80a6033aca95,4bbf3380-7064-4e8d-863d-80bb55d17f18,f3fd32de-b238-48d7-98ec-80d58257103f,aa9eb49d-c355-4450-88a1-80fd0aae75bf,068e182c-62f4-48d0-bbd5-80ffd82d7cea,bee30ef4-a7b7-4f77-b9d7-8108d2e5de4b,b2691e84-047a-431b-bdc9-8108f6649e47,b93dd376-4102-449d-8c03-81694c2aa3a8,61cda266-acf6-45bd-aaea-823b61a7118d,ef1a3e24-45f1-41cc-a900-82b3b7741223,0359bfd6-68b2-4f70-adbd-82ece9e98b25,74b5285b-6acc-4825-95de-831110f1ff63,120142ca-1007-462f-a64d-8386195d5c20,a8b3c7b5-a01a-4f13-92bf-83a55f3d08bb,ebf32721-4dd4-4189-9ee0-83ba4f82b4b7,7e5575ad-119f-43b3-9702-83e0c2282664,aec79885-f5e0-4a0e-b1d6-83f83bfebc2b,18e70638-3583-4198-a956-842780dc126f,9c781ba0-31fe-4ee5-8b91-843a62ef1cf1,5a3a853c-b5c2-416c-ba0c-843e7c28fe6f,8158bffb-6a30-4193-84ae-846a1a899386,80a813ea-da0c-4997-aa7b-84740010e922,4f2ddf9d-876a-4379-885f-847651875c42,96d61ee6-cb70-479a-80e0-84d857919517,c5f0fecf-0d86-4035-834f-859f1328bef2,1eb6754c-2e2a-4f7b-94d3-85e44c9850af,b33e461c-01a2-4e71-bba7-85ea870187f4,43dc471f-7f3b-4689-b542-85f41c5ab59a,68279ac8-e797-4d60-9c9e-8618f5f26259,9de0ec89-4f94-4492-8607-864cf745760c,fc70443b-1f1a-45ea-a9b4-86695882e56b,21fdd2ff-190a-4254-8cf7-86788b7bf2f2,ef0406e4-0316-41c8-b908-867a8b4c3200,c69e809a-7a1e-4e84-9d80-86f0880b3272,5a302815-99ba-414f-9d5f-86f6af2f7f42,16201348-679a-411c-a632-86fb54309e8d,f3da48d5-5b2e-47c3-a9d1-871950b755bc,6311b1ba-e937-47b5-a956-87545895ec9f,4f82f2ef-8eaa-45d7-a9aa-8762457b1e48,571e8449-09e9-4acf-8c0f-87c686822f26,3c24c0f6-c8d3-492b-b6a5-882cbed0f33b,621872dc-f2ff-45cb-8d72-8862134f3dd9,c099b90e-2f67-45a3-bf10-88753afe4b11,e469a64d-89a3-4cf1-a3af-895d102e5964,5a42a857-37e9-4f30-a019-8964a1fb0e78,c54849b4-e094-49ab-9f08-897b4deec5f7,30e1df94-662a-405e-9307-898820a751c9,62bdcb1a-8203-46ed-8ec7-8a7be8829459,bb72ddec-4618-45da-84a4-8aea938b3225,70ca1de5-a81c-437e-98f9-8b5087c1f6ca,39ea352a-36e6-4e4d-b133-8b6b0d8e2b44,0444d875-1d31-46bb-b7e6-8b86f076b9dc,2e5fa0b7-2234-43c3-8e6d-8b99fc9f1f39,d4ccd71e-5fc7-4cde-88bb-8be078b0bcd6,210ffd18-24d4-4d48-8a4d-8c2e6c994e00,cbe4cffc-613a-404a-842c-8c52289ac308,4dfbb634-1999-41bc-8902-8cd6e02cc2fa,baa93314-2a50-487f-a3cf-8d6a0362b7dc,3f187f18-6c8d-4f51-a3d9-8db218f5e6aa,949cb1d4-12c3-4257-95ce-8e988e98f285,3f8707a3-b184-40ad-89a1-8ea6a5b9151c,3342c640-4c99-4a0d-9b61-8f10ab145e91,73c944f2-6926-48b4-8dc5-8f139f1b2415,9fd8e52a-a074-4571-9cfd-8f49fba05c2d,adf02ade-4382-48f3-a52a-8f743d869e74,bb301afc-bd91-44b8-afe3-8fe6b0a535b8,826e7f2d-df64-4cf0-bfe2-8feb983d1d17,844906c4-e4a3-4b4a-b518-9048f3c8d312,f6f4cb97-fc6c-45b0-bc24-909579a4b0aa,5e40bd29-3e87-4ae5-955a-90f8fd9ebaf3,c595c2d4-2a04-4e69-9cda-910bb8e4ea54,8d3c6d00-e4d2-4a7b-bfc0-911035b2f834,b52e8f80-302e-4e16-a5a0-912ca2a1d580,2d2e9ee5-f8b4-432b-abbd-915d50d25ab8,5a295477-70f3-486c-abf0-9186ec33179e,63a8cdb2-6300-4c68-9b93-91b44e1b1b13,ba9f1a39-c2e9-4241-948a-91ec5fd05566,a4c984a7-7fed-49aa-aff9-91f647ee4764,a11a8af1-0770-41a5-8814-9224113ee198,35a7e756-d6fe-4a11-bb77-9270356af783,b6fa7ae2-d816-41bd-95c1-927bb4c33090,c69e3f64-30fb-4bbc-a596-92dddf376697,1f825dc5-6eaa-4a5a-bca5-93013770d6d4,6d10d592-a0db-4313-b9f1-93338bfd2128,eabac452-7ea3-48b3-a644-934c68940ddd,72e97798-0014-443d-9e0c-93523c0275a9,731a0264-b1fe-423e-9413-938cd3f1c64c,07a5db94-118c-478c-8a44-93a2df789345,d8e33061-a704-4910-8147-93f551590fd9,d7b530ba-87d3-4568-b26a-941a72e82ee8,ce81ae82-1d30-488c-b71f-94356508f561,2b50a28d-4b7b-4de4-884a-94a53f1b2d75,12460279-53d6-40e7-9217-94cdd044ce9b,c40f49a5-4a93-481e-a97e-94e32eee956c,cbbb26ba-e978-4f2c-840f-952da907eaca,b31bd780-a252-486f-880d-95531a53d98d,aa14ef26-cf47-410c-ab3f-95813d8a8bfd,c4e66f24-82f9-46da-98a5-958a6aa76c31,0671b591-b249-49cf-85bd-96118d3b5a9d,1b0fe99a-ea59-4dd7-9362-9666f62ee2f3,4a293344-50e9-45f0-9295-96bc19a30613,6554f8ec-8d57-40cc-8d4b-96dcc33557d4,92b960e9-c21f-4116-a49c-975f90d48d4e,7461580d-c65e-4c4e-966e-97a6ba57d2ec,10ef2f00-ec33-4923-8429-97f9f770c1f4,7b196468-5c06-4441-9e03-982f4b00bc1d,2877f11c-0d9b-4ffe-8de3-9840311adb5d,811517c5-1870-4125-9648-9853d64e1466,7381ad9e-4a0d-4829-8170-987446a288e1,41d72204-2632-4fc0-aecb-989049b9ab8c,49eb89c4-5390-428f-87a9-98a3f4653023,4595c653-edc9-4e57-8539-98c4504a856c,947f8538-0c3d-4500-b60b-98caccae6485,2069cbe5-80b7-44ce-b7d7-98e6bb920d60,eba0fdb2-3ce1-4e21-9e77-98e8d97c0267,b171ae41-5f26-44f3-bae2-98f22e93e06d,93a104d4-b516-4ece-86d0-993b06fc388f,c7635c03-cc81-46fb-af80-9958be7bc5a5,a1702616-d642-4930-b379-99c73a438603,4d98ed42-bc3f-4275-9198-99d8b188d424,ff29685a-bbfa-465f-8474-9aa80b86c0fd,52e31c82-1e2f-4661-961b-9aaeac7fe25c,40f577cc-7e26-4a56-8d87-9ad154efec4b,3eeb1cb1-c7eb-4d6b-8cde-9ad63cb32e30,7bdbf2da-5dfb-4996-aded-9adf8100f0cd,6dd72641-1955-476c-84e1-9b384d834e64,447681eb-3ccf-4d28-b23e-9b4641fdb762,94f42997-905f-4bbd-aba1-9bad5b19c381,3f022a09-bee5-401f-973d-9bccb8a7fcd4,dc13a59c-e5bc-44ea-b9b5-9c49186de260,1fe5e772-ca93-436a-b6c1-9c6af9acea1e,99b9fada-6b08-4847-8388-9c8e0cefe9c5,b84a1124-a95f-4dd8-a549-9c8fa00befaa,0e73ff5d-8dd7-4905-a0a9-9c9380988a90,0f1225ab-a205-4823-808c-9cc344272b1a,4cd4b09b-2c44-4f3d-99e8-9cc97164e3cc,59240f63-887b-40ba-b51a-9ce339bde52f,874726c3-53f7-424c-99f7-9d218a7ca89d,c2536b59-091b-44ae-ab7d-9d6c4a47b0d3,8aff2819-e3a7-4452-a55d-9d7abedda5e4,3d7c6b95-dc64-4286-aa06-9d824ea5f89c,c327ddab-2ded-413f-acb5-9da3a92c5bb3,8f0466c7-16ef-4099-801c-9da7a9903152,39142e32-626f-46f1-8007-9dacd0880e9c,764c20a0-3009-47e1-bb7e-9daeb9d1a56d,93857bea-b957-4ca8-8b06-9dbf63d63a9c,27c14cbf-734c-4c39-a979-9e74ae499189,5d31f96d-837a-48f3-9ade-9e7e4594c33e,868f831d-0b58-4fe8-834e-9e89d2966bfb,aaec45f5-526b-43a9-92d8-9e9ec2520ce4,32728108-3a7b-4c27-8489-9f0b032729eb,580a6b1a-6712-44be-8803-9f1d77e14642,b031d7c1-e69b-46ee-8330-9f70acc6c721,31126060-f32f-4f9b-b3cb-9f9f46ec28f3,1326253d-7b0a-4535-ad69-9fd97ac1bad6,b45b23a7-b049-4d14-b7cc-9ff5c51e1fb2,f0e51bd1-a7fe-47e3-b5a1-a00bf5c3bc6f,290a33bb-cafa-4ac2-af26-a035619449a3,1d08f035-5dde-4f5f-9383-a0415ddc6151,9879fe6e-4dc5-4410-af69-a0577c7408b5,f429c300-19dc-4448-908d-a0694b388600,06603e76-3a5e-44f6-abaa-a095066eecf8,ac099617-313a-47b5-b72c-a0b2e268eb9b,819eb75c-78a5-4a86-95f7-a0ba414fa9b4,742ba346-ec5d-4c2c-9354-a0cff65603b0,744780ee-f21d-4a47-9aa7-a0fa97adb9ee,24479c29-e082-4ff5-90a9-a1b3c2b7dd1a,eb4dc800-2341-4a36-915b-a1d94d8cab1a,d88fa1ed-5ff9-459c-85a1-a1dfa61847f1,3aff63d1-5edd-455a-ba47-a1f6f5ff6e70,7240fa26-bd13-4ad6-9119-a2244df68b52,c35a9fae-62f7-4ee7-a524-a23b1c97054e,10fee859-c3ab-4949-9969-a24f67006ed2,b6c1c687-0a61-44a5-bbf0-a2799ada2ceb,1dcd5eae-b71a-47ee-803f-a29baa091301,e4f57a93-d0ca-4dd2-bb01-a2c29667b483,d755f549-544f-4ec3-93ed-a338607eaba7,9cfd1005-d483-4cf4-bacf-a33be68d2298,549c9b14-5aaa-4c07-a98a-a3c51f518017,7b6acee9-233c-4698-9fe9-a40a82357fed,0023b7f0-757c-40be-b136-a419a4ed93ce,77b7cf47-e1c5-41fc-a331-a4335855d09c,80ea5dae-9dea-4c85-b4d9-a4615565b06d,919042e8-2e5d-476e-89af-a4a7377746f4,f5257b08-1d23-4f8f-a4b2-a56401ec8477,1c752f44-90e4-4b99-b8b5-a5de0af9c853,f038ad27-d0f4-4241-b1ee-a6009066312b,5d8b5cde-71bd-4fac-8b6a-a63667e6f149,a9d33b47-87f9-4409-88ec-a63c3207a381,81d88657-5d82-4528-a56b-a641858c72f9,28a32ba6-7706-4639-88e0-a653979e358a,0ef388e2-efe2-46fa-8b31-a653c46b5dc7,a405589d-8b9a-4c20-8667-a6d4c68427b2,b362b15e-0ca8-40bb-aef2-a704db73fbfe,29515594-dd6c-4bb2-8312-a71ee5673586,7c84a9ba-aea8-4e02-a9e8-a74dd8a0bbbc,0ba95a6e-df2e-444a-a433-a7be370602e2,6a128e06-d3a8-4791-875e-a7d2eccfbe04,81b2ab9b-daf3-4546-b5c1-a7db647ccc7c,0bb24709-1448-46fa-bc36-a7dc3f17080d,b4d846a5-0801-4def-b5f8-a7e628015c44,d353b387-9bfc-4165-9582-a7fa36fe8d37,821eded0-8d0a-41d7-864d-a8b7933e13bf,c823d94b-11fc-4a13-b2b2-a8cd70314e9f,181764b4-4ead-43cf-aa7b-a8ebcacb6ecd,d8656c92-b416-4ae5-a6a7-a8ed7854e1e5,057d09d7-f501-4281-8101-a965ca2c8b2b,a936dcd0-562a-4c59-bea3-a9974ab2031c,8e886821-df76-4b8b-a754-a99f3e162d6c,f3ba1224-5007-4e4c-b199-a9b0c0c8d878,99e57369-d186-4803-895a-aa1f9b426ba9,68c7863d-3c59-4ca4-9a53-aa1fbda6cb67,d52db298-9900-41ee-b188-aa40559f81af,bf28ca1d-cea8-47a5-88bb-aa45add9003a,713bb600-8de6-4e01-9fb8-aa470cbf6967,0dc9b2a5-11bf-4a15-bfea-aa6d6aad1d77,3d24cc7c-c067-4ae6-9317-aa904e240066,efc3d53e-aabc-4ed5-bb29-ab276bdeb62a,439bffc9-d33e-4351-9f71-ab319dd928ec,a2485600-9308-4482-af5a-ab375c76a2b7,a1204f94-3aa8-4b20-a5d9-ab4d359aa657,ae4f29d5-a0f2-4515-b400-ab67287b480f,7fde33e3-be4f-4348-a0f6-ab88c8010859,c680fee2-fe3b-4e4c-8092-aba1e5a0049a,02485f7e-3084-47ac-84d8-ac01a1c7b574,60c7abbd-be54-48f9-93ca-ace5742d2228,88911e61-c8f2-4fad-a088-acef99fea97d,57d991d5-1b04-4db2-a1eb-ad0e73da33f6,0fc04568-481b-4322-a3ee-ad16e1690210,ba081648-b63f-41e8-945b-ad1b91466f4e,8b3c06ef-908e-4dce-b263-ad3a6ae701e3,4b456e18-02c6-4fd1-9fd0-ad579f6d4735,d64e0b92-e376-4423-9935-ad7fda22c4ee,473bf46f-b770-4e41-a2da-adef9f78d697,a27769d2-074a-4a93-bb44-adfed75490d2,52022ac7-3240-4f4f-90bf-ae1ba5dc647f,05e39c16-120d-4f80-9f55-ae7456368639".Split(',').ToList(); ;
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
							if(listItems.Exists(t=>t == nfile.guid))
								continue; ;
							DB2Connection db3con = new DB2Connection("Database=servants;UserID=db2admin;Password=q121212;Server=193.233.68.82:50000");
							db3con.Open();
							DB2Command cmd2 = db3con.CreateCommand();
							cmd2.CommandType = System.Data.CommandType.StoredProcedure;
							cmd2.CommandText = "DBO.INSERTNFILEd";
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
				var sss = string.Join(",", listTable.ToArray());
				listTable.Add(table.Value.Name);
			
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

					if (fk.Name.ToUpper() == "FK_MASSOPERATIONQUEUE_STATE")
					{

					}


					if (
						(fk.RefTable.ToUpper() == "N_File".ToUpper() && fk.Columns.Any(c => c.ToUpper() == "FILEID".ToUpper() || c.ToUpper() == "FILEGUID".ToUpper() || c.ToUpper() == "PHOTOGUID".ToUpper())))
						continue;

					//if (fk.IsEnabled) //FileID
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
