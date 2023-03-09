using System;
using System.Collections.Generic;
using System.Text;
using Tango.Html;
using Tango.UI;

namespace Tango.UI.Controls
{
    public class FieldsBlockCollapsibleOptions
    {
        private Action _leftTitle;
        private Action _rightTitle;
        private Action _content;
        private readonly LayoutWriter _w;
        public FieldsBlockCollapsibleOptions(LayoutWriter w)
        {
            _w = w;
        }
        /// <summary>
        /// Признак свернутого состояния
        /// </summary>
        public bool IsCollapsed { get; set; }
        public Grid Grid { get; set; } = Grid.OneWhole;

        public FieldsBlockCollapsibleOptions SetLeftTitle(Action action)
        {
            _leftTitle += action;
            return this;
        }

        public FieldsBlockCollapsibleOptions SetLeftTitle(string title)
        {
            _leftTitle += () => LeftTitle(title);
            return this;
        }

        public FieldsBlockCollapsibleOptions SetRightTitle(Action action)
        {
            _rightTitle += action;
            return this;
        }

        public FieldsBlockCollapsibleOptions SetContent(Action action)
        {
            _content += action;
            return this;
        }

        public FieldsBlockCollapsibleOptions SetContentFieldsBlock(Action action, Action<TagAttributes> attr = null)
        {
            _content += () => ContentFieldsBlock(action, attr);
            return this;
        }

        public FieldsBlockCollapsibleOptions SetContentFieldsBlockStd(Action action, Action<TagAttributes> attr = null)
        {
            _content += () => ContentFieldsBlockStd(action, attr);
            return this;
        }

        public FieldsBlockCollapsibleOptions SetContentFieldsBlock100Percent(Action action, Action<TagAttributes> attr = null)
        {
            _content += () => ContentFieldsBlock100Percent(action, attr);
            return this;
        }

        public FieldsBlockCollapsibleOptions SetContentBodyBlock(Action action, Action<TagAttributes> attr = null)
        {
            _content += () => ContentBodyBlock(action, attr);
            return this;
        }

        public void Render()
        {
            var width = $"grid-column-end: span {(int)Grid}";
            var id = Guid.NewGuid().ToString();
            _w.Div(a => {
                a.ID(id).Class("block block-collapsible").Style(width);
                if (IsCollapsed) a.Class("collapsed");
            }, () => {
                _w.Div(a => a.Class("block-header"), () => {
                    RenderBlockHeaderLeft(id);
                    RenderBlockHeaderRight();
                });
                _content?.Invoke();
            });
        }

        private void LeftTitle(string title) => _w.Write(title);
        private void ContentFieldsBlock(Action action, Action<TagAttributes> attr = null) =>
            _w.Div(a => a.Class("block-body"), () => _w.FieldsBlock(attr, action));
        private void ContentFieldsBlockStd(Action action, Action<TagAttributes> attr = null) =>
            _w.Div(a => a.Class("block-body").Class("widthstd"), () => _w.FieldsBlock(attr, action));
        private void ContentFieldsBlock100Percent(Action action, Action<TagAttributes> attr = null) =>
            _w.Div(a => a.Class("block-body").Class("width100"), () => _w.FieldsBlock(attr, action));
        private void ContentBodyBlock(Action action, Action<TagAttributes> attr = null) =>
            _w.Div(a => a.Class("block-body").Set(attr), action);

        private void RenderBlockHeaderLeft(string id)
        {
            var js = "domActions.toggleClass({id: '" + _w.GetID(id) + "', clsName: 'collapsed' })";
            _w.Div(a => a.Class("block-header-left").OnClick(js), () =>
            {
                _w.Div(a => a.Class("block-btn"), () => _w.Icon("right"));
                _w.Div(a => a.Class("block-title"), _leftTitle);
            });
        }

        private void RenderBlockHeaderRight()
        {
            if (_rightTitle != null)
            {
                _w.Div(a => a.Class("block-header-right"), () =>
                {
                    _w.Div(a => a.Class("block-title"), _rightTitle);
                });
            }
        }

        public static FieldsBlockCollapsibleOptions Make(LayoutWriter w, Action<FieldsBlockCollapsibleOptions> action = null)
        {
            var obj = new FieldsBlockCollapsibleOptions(w);
            action?.Invoke(obj);
            return obj;
        }
    }
}