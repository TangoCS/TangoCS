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

    public class FieldsBlockCollapsibleOptions
    {
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

		public FieldsBlockCollapsibleOptions WithGrid(Grid grid) { Grid = grid; return this; }
		public FieldsBlockCollapsibleOptions SetLeftTitle(Action inner) { LeftTitle += inner; return this; }
        public FieldsBlockCollapsibleOptions SetRightTitle(Action inner) { RightTitle += inner; return this; }
        public FieldsBlockCollapsibleOptions SetContent(Action inner) { Content += inner; return this; }
        protected internal FieldsBlockCollapsibleOptions SetLeftTitle(Action<LayoutWriter> w) { LeftTitle += w; return this; }
        protected internal FieldsBlockCollapsibleOptions SetRightTitle(Action<LayoutWriter> w) { RightTitle += w; return this; }
        protected internal FieldsBlockCollapsibleOptions SetContent(Action<LayoutWriter> w) { Content += w; return this; }
        public static FieldsBlockCollapsibleOptions Make(Action<FieldsBlockCollapsibleOptions> inner = null)
        {
            var obj = new FieldsBlockCollapsibleOptions();
            inner?.Invoke(obj);
            return obj;
        }
    }

    public static class FieldsBlockCollapsibleOptionsExtension
    {
        public static FieldsBlockCollapsibleOptions SetLeftTitle(this FieldsBlockCollapsibleOptions obj, string title)
            => obj.SetLeftTitle(w => w.Write(title));

        public static FieldsBlockCollapsibleOptions SetRightTitle(this FieldsBlockCollapsibleOptions obj, Action<TitleBuilder> builder)
           => obj.SetRightTitle((Action<LayoutWriter>)TitleBuilder.Make(builder));

        public static FieldsBlockCollapsibleOptions SetContentBodyBlock(this FieldsBlockCollapsibleOptions obj, Action inner)
            => obj.SetContentBodyBlock(null, inner);
        public static FieldsBlockCollapsibleOptions SetContentBodyBlock(this FieldsBlockCollapsibleOptions obj, Action<TagAttributes> bodyBlockAttr, Action inner)
            => obj.SetContent(w => ContentBodyBlock(w, bodyBlockAttr, inner));

        public static FieldsBlockCollapsibleOptions SetContentFieldsBlock(this FieldsBlockCollapsibleOptions obj, Action inner)
            => obj.SetContentFieldsBlock(null, inner);
        public static FieldsBlockCollapsibleOptions SetContentFieldsBlock(this FieldsBlockCollapsibleOptions obj, Action<TagAttributes> fieldsBlockAttr, Action inner)
            => obj.SetContent(w => ContentFieldsBlock(w, null, fieldsBlockAttr, inner));

        public static FieldsBlockCollapsibleOptions SetContentFieldsBlockStd(this FieldsBlockCollapsibleOptions obj, Action inner)
            => obj.SetContentFieldsBlockStd(null, inner);
        public static FieldsBlockCollapsibleOptions SetContentFieldsBlockStd(this FieldsBlockCollapsibleOptions obj, Action<TagAttributes> fieldsBlockAttr, Action inner)
            => obj.SetContent(w => ContentFieldsBlock(w, a => a.Class("widthstd"), fieldsBlockAttr, inner));

        public static FieldsBlockCollapsibleOptions SetContentFieldsBlock100Percent(this FieldsBlockCollapsibleOptions obj, Action action)
            => obj.SetContentFieldsBlock100Percent(null, action);
        public static FieldsBlockCollapsibleOptions SetContentFieldsBlock100Percent(this FieldsBlockCollapsibleOptions obj, Action<TagAttributes> fieldsBlockAttr, Action inner)
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