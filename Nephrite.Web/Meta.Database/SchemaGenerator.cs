using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nephrite.Meta.Database
{
	public partial class Schema
	{
		public void Generate(MetaClass cls)
		{
			if (!cls.IsPersistent) return;

			Table t = new Table();
			t.Name = cls.Name;
			t.Description = cls.Caption;
			t.Identity = cls.Key is MetaAttribute && (cls.Key as MetaAttribute).IsIdentity;

			PrimaryKey pk = new PrimaryKey();
			pk.Name = "PK_" + cls.Name;
			pk.Columns = cls.CompositeKey.Select(o => o.Name).ToArray();
			t.PrimaryKey = pk;

			Tables.Add(t.Name, t);

			Table tdata = null;
			if (cls.IsMultilingual)
			{
				tdata = new Table();
				tdata.Name = cls.Name + "Data";
				Tables.Add(tdata.Name, tdata);
			}

			foreach (var prop in cls.Properties)
			{
				
			}
		}
	}
}