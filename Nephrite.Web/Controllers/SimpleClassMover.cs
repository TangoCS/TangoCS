using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Linq;
using Nephrite.Web;

namespace Nephrite.Web.Controllers
{
	public class SimpleClassMover<T, TKey>
		where T : IEntity, IWithKey<T, TKey>, IWithSeqNo, new()
	{
		public static void Up(IQueryable<T> collection, TKey id)
		{
			T b = default(T);
			b = collection.SingleOrDefault<T>(b.KeySelector(id));
			if (b != null)
			{
				T b1 = (from br in collection where br.SeqNo < b.SeqNo orderby br.SeqNo descending select br).FirstOrDefault();
				if (b1 != null)
				{
					int s1 = b1.SeqNo;
					b1.SeqNo = b.SeqNo;
					b.SeqNo = s1;
				}
			}
		}

		public static void Down(IQueryable<T> collection, TKey id)
		{
			T b = default(T);
			b = collection.SingleOrDefault<T>(b.KeySelector(id));
			if (b != null)
			{
				T b1 = (from br in collection where br.SeqNo > b.SeqNo orderby br.SeqNo select br).FirstOrDefault();
				if (b1 != null)
				{
					int s1 = b1.SeqNo;
					b1.SeqNo = b.SeqNo;
					b.SeqNo = s1;
				}
			}
		}
	}

	
}
