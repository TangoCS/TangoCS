
using System;
using System.Linq;
using Nephrite.Meta;
using Nephrite.Meta.Fluent;

namespace Solution.Model
{
	public partial class MM_Package { }
	public partial class MM_ObjectType { }
	public partial class MM_ObjectProperty { }
	public partial class MM_FormField { }
	public partial class MM_Codifier { }
	public partial class MM_CodifierValue { }
	public partial class MM_FormFieldAttribute { }
	public partial class MM_FormView { }
	public partial class MM_Method { }
	public partial class MM_MethodParameter { }
	public partial class MM_FormFieldGroup { }
	public partial class MM_Predicate { }

	public class mmModule
	{
		public MetaPackage Init()
		{
			var p = new MetaPackage("mm");
			p.AddClass<MM_Package>()
				.IntKey()
				.Attribute("Guid", "Guid", MetaGuidType.NotNull())
				.Attribute("IsDataReplicated", "IsDataReplicated", MetaBooleanType.NotNull())
				.Attribute("LastModifiedDate", "Дата последнего изменения", MetaDateType.NotNull())
				.Attribute("PackageID", "Ид", MetaIntType.NotNull())
				.Attribute("SeqNo", "SeqNo", MetaIntType.NotNull())
				.Attribute("SysName", "Системное имя", MetaStringType.NotNull())
				.Attribute("Title", "Наименование", MetaStringType.NotNull())
				.Attribute("Version", "Version", MetaStringType.Null())
				.ComputedAttribute("ControlsPath", "ControlsPath", MetaStringType.Null())
				.ComputedAttribute("FullSysName", "FullSysName", MetaStringType.Null())
