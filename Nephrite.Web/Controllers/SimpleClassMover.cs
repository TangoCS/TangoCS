using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Linq;
using Nephrite.Web;

namespace Nephrite.Web.Controllers
{
	public class SimpleClassMover<T>
		where T : class, IMovableObject, new()
	{
		public static void Up(EntitySet<T> collection, int id)
		{
			Up(collection.AsQueryable(), id, true);
		}

		public static void Up(EntitySet<T> collection, Guid guid)
		{
			Up(collection.AsQueryable(), guid, true);
		}

		public static void Up(IQueryable<T> collection, int id)
		{
			Up(collection, id, true);
		}

		public static void Up(IQueryable<T> collection, Guid guid)
		{
			Up(collection, guid, true);
		}

		public static void Up(IQueryable<T> collection, int id, bool submitChanges)
		{
			T b = new T();
			b = collection.SingleOrDefault<T>(b.FindByID<T>(id));
			if (b != null)
			{
				T b1 = (from br in collection where br.SeqNo < b.SeqNo orderby br.SeqNo descending select br).FirstOrDefault();
				if (b1 != null)
				{
					int s1 = b1.SeqNo;
					b1.SeqNo = b.SeqNo;
					b.SeqNo = s1;
					if (submitChanges) Base.Model.SubmitChanges();
				}
			}
		}

		public static void Up(IQueryable<T> collection, Guid guid, bool submitChanges)
		{
			T b = new T();
			b = collection.SingleOrDefault<T>(b.FindByGUID<T>(guid));
			if (b != null)
			{
				T b1 = (from br in collection where br.SeqNo < b.SeqNo orderby br.SeqNo descending select br).FirstOrDefault();
				if (b1 != null)
				{
					int s1 = b1.SeqNo;
					b1.SeqNo = b.SeqNo;
					b.SeqNo = s1;
					if (submitChanges) Base.Model.SubmitChanges();
				}
			}
		}

		public static void Down(EntitySet<T> collection, int id)
		{
			Down(collection.AsQueryable(), id, true);
		}

		public static void Down(EntitySet<T> collection, Guid guid)
		{
			Down(collection.AsQueryable(), guid, true);
		}

		public static void Down(IQueryable<T> collection, int id)
		{
			Down(collection, id, true);
		}

		public static void Down(IQueryable<T> collection, Guid guid)
		{
			Down(collection, guid, true);
		}

		public static void Down(IQueryable<T> collection, int id, bool submitChanges)
		{
			T b = new T();
			b = collection.SingleOrDefault<T>(b.FindByID<T>(id));
			if (b != null)
			{
				T b1 = (from br in collection where br.SeqNo > b.SeqNo orderby br.SeqNo select br).FirstOrDefault();
				if (b1 != null)
				{
					int s1 = b1.SeqNo;
					b1.SeqNo = b.SeqNo;
					b.SeqNo = s1;
					if (submitChanges) Base.Model.SubmitChanges();
				}
			}
		}

		public static void Down(IQueryable<T> collection, Guid guid, bool submitChanges)
		{
			T b = new T();
			b = collection.SingleOrDefault<T>(b.FindByGUID<T>(guid));
			if (b != null)
			{
				T b1 = (from br in collection where br.SeqNo > b.SeqNo orderby br.SeqNo select br).FirstOrDefault();
				if (b1 != null)
				{
					int s1 = b1.SeqNo;
					b1.SeqNo = b.SeqNo;
					b.SeqNo = s1;
					if (submitChanges) Base.Model.SubmitChanges();
				}
			}
		}
	}

	
}
