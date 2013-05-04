using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.Linq;

namespace Nephrite.Web.Controls
{
	public enum TableDnDSortDirection
	{
		Asc,
		Desc
	}

	[ParseChildren(true)]
	[PersistChildren(false)]
	public class TableDnD<T> : TableDnD
		where T : class, IMovableObject, IModelObject, new()
	{
		public IQueryable<T> ObjectList { get; set; }

		HiddenField hCurID = new HiddenField { ID = "hCurID" };
		HiddenField hNewID = new HiddenField { ID = "hNewID" };
		LinkButton go = new LinkButton { ID = "Go" };

		
		string descPart = "if (i < rows.length - 1) newid = rows[i + 1].id";
		string ascPart = "if (i > 1) newid = rows[i - 1].id";

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			Controls.Add(hCurID);
			Controls.Add(hNewID);
			Controls.Add(go);

			go.Click += new EventHandler(go_Click);
		}

		protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
			Page.ClientScript.RegisterClientScriptInclude("tablednd", Settings.JSPath + "jquery.tablednd_0_5.js");
			
			
		}

		protected override void Render(HtmlTextWriter writer)
		{
			string script = @"
<script type='text/javascript' language='javascript'>
$(document).ready( function() {
	var index = -1;
    $('#" + TableID + @"').tableDnD({
        dragHandle: 'dragHandle',
        onDragStart: function(table, row) {
			var rows = table.tBodies[0].rows;
			for (var i = 1; i < rows.length; i++) {
				if (row.parentNode.id == rows[i].id) {
					index = i;
					break;
				}
			}
        },
        onDrop: function(table, row) {
			var rows = table.tBodies[0].rows;
			var newid = -1;
			for (var i = 1; i < rows.length; i++) {
				
				if (row.id == rows[i].id) {
						if (i == index) { index = -1; break; }
						" + (SortDirection == TableDnDSortDirection.Asc ? ascPart : descPart) + @";
						document.getElementById('" + hNewID.ClientID + @"').value = newid;
						document.getElementById('" + hCurID.ClientID + @"').value = row.id;
						" + Page.ClientScript.GetPostBackEventReference(go, "", true) + @";
						index = -1;
						break;
				}
			}
        }
    });
    
    $(""#" + TableID + @" td[id='" + TableID + @"DragTD']"").hover(function() {
          $(this).addClass('showDragHandle');
    }, function() {
          $(this).removeClass('showDragHandle');
    });
});
</script>
";
			//Page.ClientScript.RegisterClientScriptBlock(typeof(Page), "", script, true);

			base.Render(writer);
			writer.Write(script);


		}

		void go_Click(object sender, EventArgs e)
		{
			int newID = hNewID.Value.ToInt32(-1);
			int curID = hCurID.Value.ToInt32(-1);
			T obj = new T();

			if (curID > 0)
			{
				T q1 = ObjectList.Single(obj.FindByID<T>(curID));
				IEnumerable<T> q3 = null;

				if (SortDirection == TableDnDSortDirection.Desc)
				{
					if (newID > 0)
					{
						T q2 = ObjectList.Single(obj.FindByID<T>(newID));
						q3 = ObjectList.Where(o => o.SeqNo <= q2.SeqNo).Where(obj.NotID<T>(q1.ObjectID));
						q1.SeqNo = q2.SeqNo;
					}
					else
					{
						int min = ObjectList.Min(o => o.SeqNo);
						q3 = ObjectList.Where(o => o.SeqNo < min).Where(obj.NotID<T>(q1.ObjectID));
						q1.SeqNo = min - 1;
					}
					foreach (T q in q3)
						q.SeqNo--;
				}
				else
				{
					if (newID > 0)
					{
						T q2 = ObjectList.Single(obj.FindByID<T>(newID));
						q3 = ObjectList.Where(o => o.SeqNo > q2.SeqNo).Where(obj.NotID<T>(q1.ObjectID));
						q1.SeqNo = q2.SeqNo + 1;
					}
					else
					{
						q3 = ObjectList.Where(obj.NotID<T>(q1.ObjectID));
						q1.SeqNo = 1;
					}
					foreach (T q in q3)
						q.SeqNo++;
				}

				DataContext.SubmitChanges();
				Query.Redirect();
			}
		}
	}

	[ControlBuilder(typeof(TableDnDControlBuilder))]
	[ParseChildren(true)]
	[PersistChildren(false)]
	public class TableDnD : Control, INamingContainer
	{
		public string Type { get; set; }
		public string TableID { get; set; }
		public DataContext DataContext { get; set; }
		public TableDnDSortDirection SortDirection { get; set; }
	}

	public class TableDnDControlBuilder : ControlBuilder
	{
		public override void Init(TemplateParser parser, ControlBuilder parentBuilder, Type type, string tagName, string id, System.Collections.IDictionary attribs)
		{
			string typeName = (string)attribs["Type"];
			Type t = Type.GetType(typeName);
			Type genericType = typeof(TableDnD<>);

			base.Init(parser, parentBuilder, genericType.MakeGenericType(t), tagName, id, attribs);
		}
	}
}
