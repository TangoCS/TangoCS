using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Nephrite.Web
{
    public interface ICalendarObject : IModelObject
	{
		DateTime? StartDate { get; set; }
		DateTime? EndDate { get; set; }
        string Place { get; set; }
		string GetViewURL();
        string GetCreateURL();
	}

}
