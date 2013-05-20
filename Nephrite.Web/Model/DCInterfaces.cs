using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Web;

namespace Nephrite.Web.Model
{



	public interface IDC_N_Cache<T, TE>
		where T : IQueryable<TE>, ITable
		where TE : IN_Cache
	{ T N_Cache { get; } }

	

	
}