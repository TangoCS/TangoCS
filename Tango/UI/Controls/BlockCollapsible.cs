using System;
using System.Collections.Generic;
using System.Text;
using Tango.Html;
using Tango.UI;

namespace Tango.UI.Controls
{
    public class ActionWrapper<T> where T : class
    {
        protected internal Action<T> _action;
        public Action GetAction(T w) => () => _action?.Invoke(w);
        public void Invoke(T w) => GetAction(w)?.Invoke();
        public static ActionWrapper<T> operator +(ActionWrapper<T> a, Action b)
        {
            if (a == null) a = new ActionWrapper<T>();
            a._action += w => b?.Invoke();
            return a;
        }
        public static ActionWrapper<T> operator +(ActionWrapper<T> a, Action<T> b)
        {
            if (a == null) a = new ActionWrapper<T>();
            a._action += b;
            return a;
        }
    }

    public class BlockCollapsibleBuilder
    {
        //private readonly Lazy<string> _id = new Lazy<string>(() => Guid.NewGuid().ToString());
        public string ID { get; set; }

		public BlockCollapsibleBuilder WithID(string id)
		{
			ID = id;
			return this;
		}

		/// <summary>
		/// Контент левого заголовока
		/// </summary>
		public ActionWrapper<LayoutWriter> LeftTitle { get; private set; }

        /// <summary>
        /// Контент правого заголовока
        /// </summary>
        public ActionWrapper<LayoutWriter> RightTitle { get; private set; }

        /// <summary>
        /// Контент тела блока
        /// </summary>
        public ActionWrapper<LayoutWriter> Content { get; private set; }

        /// <summary>
        /// Признак свернутого состояния
        /// </summary>
        public bool IsCollapsed { get; set; }
        public Grid Grid { get; private set; } = Grid.OneWhole;

		public Action<TagAttributes> ContainerAttributes { get; private set; }
		public Action<TagAttributes> BlockHeaderLeftAttributes { get; private set; }
        
        public BlockCollapsibleBuilder WithBlockHeaderLeftAttributes(Action<TagAttributes> attr) 
        { 
            BlockHeaderLeftAttributes = attr; 
            return this; 
        }
		public BlockCollapsibleBuilder WithContainerAttributes(Action<TagAttributes> attr)
		{
			ContainerAttributes = attr;
			return this;
		}
		public BlockCollapsibleBuilder WithGrid(Grid grid) { Grid = grid; return this; }
		public BlockCollapsibleBuilder SetLeftTitle(Action inner) { LeftTitle += inner; return this; }
        public BlockCollapsibleBuilder SetRightTitle(Action inner) { RightTitle += inner; return this; }
        public BlockCollapsibleBuilder SetContent(Action inner) { Content += inner; return this; }
        protected internal BlockCollapsibleBuilder SetLeftTitle(Action<LayoutWriter> w) { LeftTitle += w; return this; }
        protected internal BlockCollapsibleBuilder SetRightTitle(Action<LayoutWriter> w) { RightTitle += w; return this; }
        protected internal BlockCollapsibleBuilder SetContent(Action<LayoutWriter> w) { Content += w; return this; }
        public static BlockCollapsibleBuilder Make(Action<BlockCollapsibleBuilder> inner = null)
        {
            var obj = new BlockCollapsibleBuilder();
            inner?.Invoke(obj);
            return obj;
        }
    }

    public static class FieldsBlockCollapsibleOptionsExtension
    {
        public static BlockCollapsibleBuilder SetLeftTitle(this BlockCollapsibleBuilder obj, string title)
            => obj.SetLeftTitle(w => w.Write(title));

        public static BlockCollapsibleBuilder SetRightTitle(this BlockCollapsibleBuilder obj, Action<TitleBuilder> builder)
           => obj.SetRightTitle((Action<LayoutWriter>)TitleBuilder.Make(builder));

        public static BlockCollapsibleBuilder SetContentBodyBlock(this BlockCollapsibleBuilder obj, Action inner)
            => obj.SetContentBodyBlock(null, inner);
        public static BlockCollapsibleBuilder SetContentBodyBlock(this BlockCollapsibleBuilder obj, Action<TagAttributes> bodyBlockAttr, Action inner)
            => obj.SetContent(w => ContentBodyBlock(w, bodyBlockAttr, inner));

        public static BlockCollapsibleBuilder SetContentFieldsBlock(this BlockCollapsibleBuilder obj, Action inner)
            => obj.SetContentFieldsBlock(null, inner);
        public static BlockCollapsibleBuilder SetContentFieldsBlock(this BlockCollapsibleBuilder obj, Action<TagAttributes> fieldsBlockAttr, Action inner)
            => obj.SetContent(w => ContentFieldsBlock(w, null, fieldsBlockAttr, inner));

        public static BlockCollapsibleBuilder SetContentFieldsBlockStd(this BlockCollapsibleBuilder obj, Action inner)
            => obj.SetContentFieldsBlockStd(null, inner);
        public static BlockCollapsibleBuilder SetContentFieldsBlockStd(this BlockCollapsibleBuilder obj, Action<TagAttributes> fieldsBlockAttr, Action inner)
            => obj.SetContent(w => ContentFieldsBlock(w, a => a.Class("widthstd"), fieldsBlockAttr, inner));

        public static BlockCollapsibleBuilder SetContentFieldsBlock100Percent(this BlockCollapsibleBuilder obj, Action action)
            => obj.SetContentFieldsBlock100Percent(null, action);
        public static BlockCollapsibleBuilder SetContentFieldsBlock100Percent(this BlockCollapsibleBuilder obj, Action<TagAttributes> fieldsBlockAttr, Action inner)
            => obj.SetContent(w => ContentFieldsBlock(w, a => a.Class("width100"), fieldsBlockAttr, inner));

        private static void ContentBodyBlock(LayoutWriter w, Action<TagAttributes> bodyBlockAttr, Action inner)
        {
            bodyBlockAttr += a => a.Class("block-body");
            w.Div(bodyBlockAttr, inner);
        }

        private static void ContentFieldsBlock(LayoutWriter w, Action<TagAttributes> bodyBlockAttr, Action<TagAttributes> fieldsBlockAttr, Action inner)
            => ContentBodyBlock(w, bodyBlockAttr, () => w.FieldsBlock(fieldsBlockAttr, inner));
    }

    public class TitleBuilder
    {
        private Action<LayoutWriter> _items;
        public TitleBuilder AddItem(int width, Action inner) => AddItem(attr: a => a.Style($"width:{width}px;"), inner);
        public TitleBuilder AddItem(Action inner) => AddItem(attr: null, inner);
        public TitleBuilder AddItem(Action<TagAttributes> attr, Action inner) { _items += w => w.Div(attr, inner); return this; }
        public TitleBuilder AddItems(params Action[] inners) => AddItems(attr: null, inners);
        public TitleBuilder AddItems(Action<TagAttributes> attr, params Action[] inners) {
            foreach (var inner in inners) AddItem(attr, inner);
            return this; 
        }
        public static TitleBuilder Make(Action<TitleBuilder> builder)
        {
            var obj = new TitleBuilder();
            builder?.Invoke(obj);
            return obj;
        }
        public static explicit operator Action<LayoutWriter>(TitleBuilder bulder) => bulder._items;
    }
}