using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Tango.Html;
using Tango.Localization;

namespace Tango.UI
{
    public class LayoutWriter : HtmlWriter
    {
        public ActionContext Context { get; }
        public IResourceManager Resources => Context.Resources;

        public List<ClientAction> ClientActions { get; private set; } = new List<ClientAction>();
        public HashSet<string> Includes { get; private set; } = new HashSet<string>();
        public IFieldBlockRenderer FieldBlockRenderer { get; private set; }

        //LayoutWriter(ActionContext context, StringBuilder sb) : base(sb) 
        //{
        //	Context = context;
        //}

        public LayoutWriter(ActionContext context, string idPrefix = null) : base(idPrefix)
        {
            Context = context;

            var reqEnv = context.RequestServices.GetService(typeof(IRequestEnvironment)) as IRequestEnvironment;
            FieldBlockRenderer = context.RequestServices.GetService(typeof(IFieldBlockRenderer)) as IFieldBlockRenderer ??
                                 (reqEnv.IsIE() ? (IFieldBlockRenderer) new TableFieldBlockRenderer() : new GridFieldBlockRenderer());
        }

        LayoutWriter(ActionContext context, string idPrefix, StringBuilder sb) : base(idPrefix, sb)
        {
            Context = context;

            var reqEnv = context.RequestServices.GetService(typeof(IRequestEnvironment)) as IRequestEnvironment;
            FieldBlockRenderer = context.RequestServices.GetService(typeof(IFieldBlockRenderer)) as IFieldBlockRenderer ??
                                 (reqEnv.IsIE() ? (IFieldBlockRenderer) new TableFieldBlockRenderer() : new GridFieldBlockRenderer());
        }

        //public LayoutWriter(ActionContext context)
        //{
        //	Context = context;
        //}

        public LayoutWriter Clone(string newIdPrefix)
        {
            if (newIdPrefix != null && newIdPrefix.StartsWith("#"))
                newIdPrefix = newIdPrefix.Substring(1);

            return new LayoutWriter(Context, newIdPrefix, GetStringBuilder())
            {
                ClientActions = ClientActions,
                Includes = Includes
            };
        }

        public void FieldsBlock(Action<TagAttributes> attributes, Action content)
        {
            FieldBlockRenderer.FieldsBlock(this, attributes, content);
        }

        public void FormField(string name, Action caption, Action content, GridPosition grid, bool isRequired = false, Action description = null, bool isVisible = true,
            string hint = null, bool withCheck = false, bool isDisabled = false, bool disableCheck = false)
        {
            FieldBlockRenderer.FormField(this, name, caption, content, grid, isRequired, description, isVisible, hint, withCheck, isDisabled, disableCheck);
        }

        public void FormFieldDescription(string name, Action description = null)
        {
            FieldBlockRenderer.FormFieldDescription(this, name, description);
        }
    }

    public interface IFieldBlockRenderer
    {
        void FieldsBlock(LayoutWriter w, Action<TagAttributes> attributes, Action content);

        void FormField(LayoutWriter w, string name, Action caption, Action content, GridPosition grid, bool isRequired = false, Action description = null, bool isVisible = true,
            string hint = null, bool withCheck = false, bool isDisabled = false, bool disableCheck = false);

        void FormFieldDescription(LayoutWriter w, string name, Action description = null);
        void FormFieldCaption(LayoutWriter w, string name, Action caption, bool isRequired = false, string hint = null);
    }

    public class TableFieldBlockRenderer : IFieldBlockRenderer
    {
        public void FormField(LayoutWriter w, string name, Action caption, Action content, GridPosition grid, bool isRequired = false, Action description = null,
            bool isVisible = true, string hint = null, bool withCheck = false, bool isDisabled = false, bool disableCheck = false)
        {
            w.Tr(a => a.ID(name + "_field").Style(isVisible ? "" : "display:none"), () => {
                w.Td(a => a.ID(name + "_fieldlabel").Class("formlabel"), () => {
                    FormFieldCaption(w, name, caption, isRequired, hint);
                    FormFieldDescription(w, name, description);
                });
                w.Td(a => a.ID(name + "_fieldbody").Class("formbody"), content);
            });
        }

        public void FormFieldDescription(LayoutWriter w, string name, Action description = null)
        {
            if (description != null)
                w.Div(a => a.ID(name + "_fielddescription").Class("descriptiontext"), description);
        }

        public void FieldsBlock(LayoutWriter w, Action<TagAttributes> attributes, Action content)
        {
            w.Table(a => a.Class("formtable").Set(attributes), content);
        }

        public void FormFieldCaption(LayoutWriter w, string name, Action caption, bool isRequired = false, string hint = null)
        {
            w.Span(a => a.ID(name + "_fieldcaption"), caption);
            if (!string.IsNullOrEmpty(hint))
                w.Sup(a => a.Style("margin-left:2px").Title(hint), "?");
            if (isRequired)
                w.Span(a => a.ID(name + "_fieldrequired").Class("formvalidation"), "&nbsp;*");
        }
    }

    public class GridFieldBlockRenderer : IFieldBlockRenderer
    {
        public void FormField(LayoutWriter w, string name, Action caption, Action content, GridPosition grid, bool isRequired = false, Action description = null,
            bool isVisible = true, string hint = null, bool withCheck = false, bool isDisabled = false, bool disableCheck = false)
        {
            if (grid == null) grid = Grid.OneWhole;

            string width = "grid-column-end: span " + (int) grid.Field;
            string br = grid.BreakRow ? "grid-column-start: 1" : "";
            string vis = isVisible ? "" : "display:none";

            string style = new string[] {width, br, vis}.Where(s => s != "").Join(";");

            int labelWidth = (int) Math.Round(100 / (60 / (double) grid.Caption), 0, MidpointRounding.AwayFromZero);
            int bodyWidth = 100 - labelWidth;
            int checkBoxWidth = 10;

            if (withCheck)
                bodyWidth = bodyWidth - checkBoxWidth;

            w.Div(a => a.ID(name + "_field").Class("field").Style(style), () => {
                w.Div(a => a.ID(name + "_fieldlabel").Class("field-label").Style($"width:{labelWidth}%"), () => {
                    FormFieldCaption(w, name, caption, isRequired, hint);
                    FormFieldDescription(w, name, description);
                });

                w.Div(a => {
                    a.ID(name + "_fieldbody").Class("field-body").Style($"width:{bodyWidth}%");
                    if (isDisabled)
                        a.Class("disabled");
                }, content);

                if (withCheck)
                    w.Div(a => a.ID(name + "_field_check").Style($"width:{checkBoxWidth}%"), () => {
                        w.CheckBox(name + "_check", isChecked: !isDisabled, attributes: a =>
                            a.ID(name + "_check")
                                .Disabled(disableCheck)
                                .Data("p-field_id", name)
                                .Data("e", "OnFieldCheckBoxChange")
                                .OnChange("ajaxUtils.postEventFromElementWithApiResponse(this)"));
                    });
            });
        }

        public void FormFieldDescription(LayoutWriter w, string name, Action description = null)
        {
            if (description != null)
                w.Div(a => a.ID(name + "_fielddescription").Class("field-description"), description);
        }

        public void FieldsBlock(LayoutWriter w, Action<TagAttributes> attributes, Action content)
        {
            w.Div(a => a.Class("fieldsblock-grid").Set(attributes), content);
        }

        public void FormFieldCaption(LayoutWriter w, string name, Action caption, bool isRequired = false, string hint = null)
        {
            w.Span(a => a.ID(name + "_fieldcaption"), caption);
            if (isRequired)
                w.Span(a => a.ID(name + "_fieldrequired").Class("field-validation"), "&nbsp;*");
            if (!string.IsNullOrEmpty(hint))
                w.Icon("hint", a => a.Style("margin-left:2px").Title(hint));
        }
    }

    public class GridPosition
    {
        public Grid Field { get; set; }
        public Grid Caption { get; set; }
        public bool BreakRow { get; set; }

        public static implicit operator GridPosition((Grid field, Grid caption, bool br) rec)
        {
            return new GridPosition {Field = rec.field, Caption = rec.caption, BreakRow = rec.br};
        }

        public static implicit operator GridPosition((Grid field, Grid caption) rec)
        {
            return new GridPosition {Field = rec.field, Caption = rec.caption};
        }

        public static implicit operator GridPosition(Grid field)
        {
            return new GridPosition {Field = field, Caption = Grid.ThreeTenth};
        }
    }

    public enum Grid
    {
        OneWhole = 60,

        OneHalf = 30,

        OneThird = 20,
        TwoThirds = 40,

        OneQuater = 15,
        ThreeQuaters = 45,

        OneFifth = 12,
        TwoFifths = 24,
        ThreeFiths = 36,
        FourFifths = 48,

        OneSixth = 10,
        FiveSixth = 50,

        OneTenth = 6,
        ThreeTenth = 18,
        SevenTenth = 42,
        NineTenth = 54
    }

    public class FieldsBlockCollapsibleOptions
    {
        public Action<TagAttributes> Attributes { get; set; }

        /// <summary>
        /// Признак свернутого состояния
        /// </summary>
        public bool IsCollapsed { get; set; }

        public Grid? Grid { get; set; }
    }

	public static class LayoutWriterMainExtensions
	{
		public static LayoutWriter Clone(this LayoutWriter w, IViewElement el)
		{
			return w.Clone(el.ClientID);
		}

		public static void AjaxForm(this LayoutWriter w, string name, Action content)
		{
			w.AjaxForm(name, false, null, content);
		}

		public static void AjaxForm(this LayoutWriter w, string name, bool submitOnEnter, Action content)
		{
			w.AjaxForm(name, submitOnEnter, null, content);
		}

		public static void AjaxForm(this LayoutWriter w, string name, Action<FormTagAttributes> attributes, Action content)
		{
			w.AjaxForm(name, false, attributes, content);
		}

		public static void AjaxForm(this LayoutWriter w, string name, bool submitOnEnter, Action<FormTagAttributes> attributes, Action content)
		{
			w.Form(a => a.ID(name).Set(attributes), () => {
				content?.Invoke();
				// Workaround to avoid corrupted XHR2 request body in IE10 / IE11
				w.Hidden(Constants.IEFormFix, null);
			});
			w.AddClientAction("ajaxUtils", "initForm", f => new { ID = f(name), SubmitOnEnter = submitOnEnter });
		}

		//public static void FormTable100Percent(this LayoutWriter w, Action content)
		//{
		//	w.FieldsBlock(a => a.Class("width100"), content);
		//}

		public static void FieldsBlockStd(this LayoutWriter w, Action content)
		{
			FieldsBlockStd(w, null, content);
		}

		public static void FieldsBlock100Percent(this LayoutWriter w, Action content)
		{
			FieldsBlock100Percent(w, null, content);
		}

		public static void FieldsBlock(this LayoutWriter w, Action content)
		{
			w.FieldsBlock(null, content);
		}

		//public static void FieldsBlock(this LayoutWriter w, Action<TagAttributes> attributes, Action content)
		//{
		//	w.FormTable(a => a.ID().Set(attributes), content);
		//}

		public static void FieldsBlockStd(this LayoutWriter w, Action<TagAttributes> attributes, Action content)
		{
			w.Div(a => a.Class("widthstd"), () => w.FieldsBlock(attributes, content));
		}

		public static void FieldsBlock100Percent(this LayoutWriter w, Action<TagAttributes> attributes, Action content)
		{
			w.Div(a => a.Class("width100"), () => w.FieldsBlock(attributes, content));
		}

		static void BlockCollapsibleInt(this LayoutWriter w, Action title, Action content, FieldsBlockCollapsibleOptions options = null)
		{
			var id = Guid.NewGuid().ToString();
			var js = "domActions.toggleClass({id: '" + w.GetID(id) + "', clsName: 'collapsed' })";

			var grid = options?.Grid;
			if (grid == null) grid = Grid.OneWhole;
			var width = $"grid-column-end: span {(int)grid}";

			w.Div(a => {
				a.ID(id).Class("block block-collapsible").Style(width);
				if (options?.IsCollapsed ?? false)
					a.Class("collapsed");
			}, () => {
				w.Div(a => a.Class("block-header").OnClick(js), () => {
					w.Div(a => a.Class("block-btn"), () => w.Icon("right"));
					w.Div(a => a.Class("block-title"), title);
				});
				content();
			});
		}

		public static T GridColumn<T>(this TagAttributes<T> a, Grid? value)
			where T : TagAttributes<T>
		{
			if (value == null) value = Grid.OneWhole;
			var width = $"grid-column-end: span {(int)value};";
			return a.Style(width);
		}

		public static void Block(this LayoutWriter w, Action content, Grid? grid = null)
		{
			w.Div(a => a.Class("grid60").GridColumn(grid), content);
		}

		public static void BlockCollapsible(this LayoutWriter w, string title, Action content, FieldsBlockCollapsibleOptions options = null)
		{
			w.BlockCollapsible(() => w.Write(title), content, options);
		}
		public static void BlockCollapsible(this LayoutWriter w, Action title, Action content, FieldsBlockCollapsibleOptions options = null)
		{
			w.BlockCollapsibleInt(title, () => w.Div(a => a.Class("block-body").Set(options?.Attributes), content), options);
		}

		public static void FieldsBlockCollapsible(this LayoutWriter w, string title, Action content, FieldsBlockCollapsibleOptions options = null)
		{
			w.FieldsBlockCollapsible(() => w.Write(title), content, options);
		}

		public static void FieldsBlockCollapsible(this LayoutWriter w, Action title, Action content, FieldsBlockCollapsibleOptions options = null)
		{
			w.BlockCollapsibleInt(title, () =>
					w.Div(a => a.Class("block-body"), () =>
						w.FieldsBlock(a => a.Set(options?.Attributes),
							content)
					), options
			);
		}


		public static void GroupTitle(this LayoutWriter w, Action<TagAttributes> attributes, Action content)
		{
			w.Div(a => a.Class("tabletitle").Set(attributes), content);
		}

		public static void FormMargin(this LayoutWriter w, Action inner)
		{
			w.Div(a => a.Style("padding:8px"), inner);
		}

		public static void ButtonsBar(this LayoutWriter w, Action content)
		{
			w.ButtonsBar(null, content);
		}

		public static void ButtonsBar(this LayoutWriter w, Action<TagAttributes> attributes, Action content)
		{
			w.Div(a => a.ID("buttonsbar").Class("buttonsbar").Set(attributes), content);
		}

		public static void ButtonsBarRight(this LayoutWriter w, Action content)
		{
			w.Div(a => a.Class("right"), content);
		}

		public static void ButtonsBarRight(this LayoutWriter w, Action<TagAttributes> attributes, Action content)
		{
			w.Div(a => a.Class("right").Set(attributes), content);
		}

		public static void ButtonsBarLeft(this LayoutWriter w, Action content)
		{
			w.Div(a => a.Class("left"), content);
		}

		public static void ButtonsBarLeft(this LayoutWriter w, Action<TagAttributes> attributes, Action content)
		{
			w.Div(a => a.Class("left").Set(attributes), content);
		}

		
    }
}