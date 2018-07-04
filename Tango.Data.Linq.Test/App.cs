using System;

namespace Solution
{
	public static partial class App
	{
		[ThreadStatic]
		static Solution.Model.modelDataContext context = null;

		public static Solution.Model.modelDataContext DataContext
		{
			get
			{
				if (context == null)
				{
					context = new Solution.Model.modelDataContext(Nephrite.Web.ConnectionManager.Connection);
					context.CommandTimeout = 300;
				}
				return context;
			}
			set
			{
				context = value;
			}
		}
	}

	
}
