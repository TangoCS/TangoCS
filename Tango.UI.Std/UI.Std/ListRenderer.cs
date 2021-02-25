using System;
using System.Collections.Generic;
using System.Linq;
using Tango.Html;
using Tango.UI.Controls;
using Tango.UI.Std.ListMassOperations;

namespace Tango.UI.Std
{
	public abstract class ListRendererAbstract<TResult>
	{
		public abstract void Render(LayoutWriter w, IEnumerable<TResult> result, IFieldCollection<TResult> fields);
	}

	public class ListRenderer<TResult> : ListRendererAbstract<TResult>
	{
		protected string _id;

		public ListRenderer()
		{
			
		}

		public ListRenderer(string id)
		{
			_id = id?.ToLower();
		}

		protected virtual void RenderRows(LayoutWriter w, IEnumerable<TResult> result, IFieldCollection<TResult> fields)
		{
			var gvalue = new string[fields.Groups.Count];
			var i = 0;
			var curLevel = 1;

			foreach (var o in result)
			{
				bool newGroupItem = false;
				int j = 0;
				int lev = 0;
				foreach (var g in fields.Groups)
				{
					var val = g.ValueFunc(o);
					if (val.IsEmpty()) val = g.DefaultValue;
					if (val != gvalue[j] || newGroupItem)
					{
						if (!val.IsEmpty())
						{
							w.Tr(a => fields.GroupRowAttributes?.Invoke(a, o, new RowInfo { RowNum = i, Level = lev }), () =>
							{
								RenderGroupRow(w, fields, g, o);
							});
						}
						gvalue[j] = val;
						newGroupItem = true;
					}
					if (!val.IsEmpty()) lev++;
					j++;
				}
				if (newGroupItem) curLevel = lev;

				var r = new RowInfo { RowNum = i, Level = curLevel };
				fields.BeforeRowContent?.Invoke(w, o, r);

				w.Tr(a => fields.RowAttributes?.Invoke(a, o, r), () => {
					foreach (var cols in fields.Cells)
						foreach (var c in cols.AsEnumerable())
							w.Td(a => c.Attributes?.Invoke(a, o, r), () => c.Content(w, o, r));
				});

				fields.AfterRowContent?.Invoke(w, o, r);

				i++;
			}
		}

		protected virtual void RenderGroupRow(LayoutWriter w, IFieldCollection<TResult> fields, ListGroup<TResult> g, TResult o)
		{
			var colspan = fields.Cells.Count;
			if (fields.EnableSelect)
			{
				w.Td(() => { });
				colspan--;
			}
			
			
			if (g.Cells != null)
			{
				var r = new GroupRowDescription<TResult>();
				g.Cells(o, r);
				foreach (var (attr, lw) in r.Cells)
				{
					w.Td(a => { a.Class("gr").Set(attr); }, () => lw?.Invoke(w));
				}
			}
			else
			{
				w.Td(a => a.Class("gr").ColSpan(colspan), () => g.Cell(w, o));
			}
		}

		public override void Render(LayoutWriter w, IEnumerable<TResult> result, IFieldCollection<TResult> fields)
		{
			var rendererIsControl = !_id.IsEmpty() && w.IDPrefix != _id && !w.IDPrefix.EndsWith("_" + _id);

			if (rendererIsControl) w.PushPrefix(_id);

			fields.ListAttributes += a => a.ID();
            
            if (fields.EnableSelect || fields.EnableFixedHeader) fields.ListAttributes += a => a.DataCtrl("listview");
            if (fields.EnableFixedHeader) fields.ListAttributes += a => a.Class("fixedheader");
			
            fields.ListAttributes = (a => a.Class("listviewtable")) + fields.ListAttributes;

			if (fields.EnableSelect && fields.HeaderRows.Count > 0)
				fields.AddCheckBoxCell();

			if (fields.EnableSelect && !fields.RowAttributes.GetInvocationList().Any(o => o.Method.Name.Contains("SetRowID")))
				fields.SetRowID(o => (o as dynamic).ID);

			w.Table(fields.ListAttributes, () => {
				fields.BeforeHeader?.Invoke(w, result);

				var i = 0;

				foreach (var hr in fields.HeaderRows)
				{
					w.Tr(a => fields.HeaderRowAttributes?.Invoke(a, i), () => {
						foreach (var h in hr)
							foreach (var hdr in h.AsEnumerable())
								w.Th(hdr.Attributes, () => hdr.Content(w));
					});
					i++;
				}

				if (fields.EnableSelect && fields.AllowSelectAllPages)
					w.InfoRow(fields.Cells.Count);

				RenderRows(w, result, fields);
			});

			if (rendererIsControl) w.PopPrefix();
		}
	}

	public class BlocksListRenderer<TResult> : ListRendererAbstract<TResult>
	{
		public override void Render(LayoutWriter w, IEnumerable<TResult> result, IFieldCollection<TResult> fields)
		{
			fields.ListAttributes = (a => a.Class("blockslist")) + fields.ListAttributes;
			w.Div(fields.ListAttributes, () => {
				fields.BeforeHeader?.Invoke(w, result);

				int i = 1;
				string[] gvalue = new string[fields.Groups.Count];

				foreach (var o in result)
				{
					bool newGroupItem = false;
					int j = 0;
					foreach (var g in fields.Groups)
					{
						var val = g.ValueFunc(o);
						if (val.IsEmpty()) val = g.DefaultValue;
						if (val != gvalue[j] && i != 1)
							for (int k = j; k < fields.Groups.Count; k++)
								w.DivEnd();
						if (val != gvalue[j] || newGroupItem)
						{
							w.DivBegin();
							w.Div(a => a.Class("gr"), () => g.Cell(w, o));
							gvalue[j] = val;
							newGroupItem = true;
						}
						j++;
					}

					var r = new RowInfo { RowNum = i, Level = j };
					w.Div(a => fields.RowAttributes?.Invoke(a, o, r), () => {
						foreach (var cols in fields.Cells)
							foreach (var c in cols.AsEnumerable())
								c.Content(w, o, r);
					});

					i++;
				}

				foreach (var g in fields.Groups)
					w.DivEnd();
			});
		}
	}

	public class TreeListRenderer<TResult> : ListRenderer<TResult>
	{
		Paging _paging;
		int _level;

		public TreeListRenderer(string id, Paging paging, int level) : base(id)
		{
			_paging = paging;
			_level = level;
		}

		public void SetLevel(int level)
		{
			_level = level;
		}

		void PagingRow(LayoutWriter w, int colSpan)
		{
			w.Tr(a => a.ID(w.Context.Sender + "_" + _paging.ID + "_row").Class("pagingrow").Data("level", _level), () => {
				w.Td(a => a.ColSpan(colSpan), () => {
					w.Div(a => a.Class($"treerow l{_level}"), () => {
						for (int i = 0; i < _level; i++)
							w.Div(a => a.Class("level-padding" + (i == _level ? " last" : "")), "");

						w.Div(a => a.Class("leaf"), () => w.Span("&nbsp;"));

						w.Div(() => {
							w.Span(a => a.ID(w.Context.Sender + "_" + _paging.ID));
						});
					});

				});
			});
		}

		public void SelectedBlock(LayoutWriter w, IFieldCollection<TResult> fields)
		{
			Action<TagAttributes> selectedListAttrs = a => a.ID("selected").Class("listviewtable width100").DataCtrl("listview");
			selectedListAttrs += fields.ListAttributes;

			w.Table(selectedListAttrs, () => {
				var i = 0;

				foreach (var hr in fields.HeaderRows)
				{
					w.Tr(a => fields.HeaderRowAttributes?.Invoke(a, i), () => {
						foreach (var h in hr)
							foreach (var hdr in h.AsEnumerable())
								w.Th(hdr.Attributes, () => hdr.Content(w));
					});
					i++;
				}
			});
		}

		void MainBlock(LayoutWriter w, IEnumerable<TResult> result, IFieldCollection<TResult> fields)
		{
			Action<TagAttributes> listAttrs = a => a.ID("tree").Class("listviewtable width100");
			//listAttrs += a => a.DataCtrl("listview");
			listAttrs += fields.ListAttributes;

			w.Div(a => a.ID().DataCtrl("listview"), () => {
				w.Table(listAttrs, () => {
					fields.BeforeHeader?.Invoke(w, result);
					var i = 0;

					foreach (var hr in fields.HeaderRows)
					{
						w.Tr(a => fields.HeaderRowAttributes?.Invoke(a, i), () => {
							foreach (var h in hr)
								foreach (var hdr in h.AsEnumerable())
									w.Th(hdr.Attributes, () => hdr.Content(w));
						});
						i++;
					}

					RenderRows(w, result, fields);
				});

				if (fields.EnableSelect)
					w.Hidden("selectedvalues", null, a => a.DataHasClientState(ClientStateType.Array, _id));
			});
		}

		public override void Render(LayoutWriter w, IEnumerable<TResult> result, IFieldCollection<TResult> fields)
		{
			var rendererIsControl = !_id.IsEmpty() && w.IDPrefix != _id && !w.IDPrefix.EndsWith("_" + _id);

			if (rendererIsControl) w.PushPrefix(_id);

			if (_level > 0)
			{
				RenderRows(w, result, fields);
				if (result.Count() >= _paging.PageSize || _paging.PageIndex > 1)
					PagingRow(w, fields.Cells.Count);
			}
			else
			{
				MainBlock(w, result, fields);
			}

			if (rendererIsControl) w.PopPrefix();
		}
	}

}
