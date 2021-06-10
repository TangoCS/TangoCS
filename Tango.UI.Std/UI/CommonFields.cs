using System;

namespace Tango.UI
{
	public static class CommonFields
	{
		public class Title : EntityField<object, string>
		{
			public override bool IsRequired => true;
		}

		public class Code : EntityField<object, string>
		{
			public override bool IsRequired => true;
		}

		public class SystemName : EntityField<object, string>
		{
			public override bool IsRequired => true;
		}

		public class Description : EntityField<object, string>
		{
		}

		public class DefaultCodifierGroup : FieldGroup
		{
			public Title Title { get; set; }
			public Code Code { get; set; }

			public DefaultCodifierGroup()
			{
				Title = AddField(new Title());
				Code = AddField(new Code());
			}
		}
	}
}
