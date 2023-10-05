using System;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Principal;
using Tango.AccessControl;
using Tango.Html;
using Tango.Identity.Std;
using Tango.Logger;
using Tango.Meta.Database;
using Tango.UI.Controls;

namespace Tango.UI.Std
{
	public interface IWithChangeEvent : IViewPagePart
	{
		event Action<ApiResponse> Changed;
	}

	public interface IWithChangeEventHandler : IViewPagePart
	{
		void OnChange(ApiResponse response);
	}

    public interface IRigthViewPagePart : IWithChangeEventHandler { }

    public class ViewPagePart_sidebar_2col_collapsible<TLeft, TRight> : ViewPagePart_sidebar_2col<TLeft, TRight>
		where TLeft : IWithChangeEvent, new()
		where TRight : IWithChangeEventHandler, new()
	{
		protected virtual string LeftSideTitle => "";

		protected virtual string RightSideTitle => "";

		protected override Action<LayoutWriter> RenderPlaceHolderLeftSide => w => w.CollapsibleSidebar(LeftSideTitle, () => w.Div(a => a.ID("container")));
		
		protected override Action<LayoutWriter> RenderPlaceHolderRightSide => w => w.CollapsibleSidebar(RightSideTitle, () => w.Div(a => a.ID("container")));

		protected override string ContentBodyClass => "layout1 withwrap";
		
	}

    public abstract class ViewPagePart_sidebar_2col_collapsible<TLeft> : ViewPagePart_sidebar_2col_base
        where TLeft : IWithChangeEvent, new()
    {
        protected new TLeft left
        {
            get { return (TLeft)base.left; }
            set { base.left = value; }
        }

        protected virtual string LeftSideTitle => "";
        protected virtual string RightSideTitle => "";
        protected override Action<LayoutWriter> RenderPlaceHolderLeftSide => w => w.CollapsibleSidebar(LeftSideTitle, () => w.Div(a => a.ID("container")));
        protected override Action<LayoutWriter> RenderPlaceHolderRightSide => w => w.CollapsibleSidebar(RightSideTitle, () => w.Div(a => a.ID("container")));
        protected override string ContentBodyClass => "layout1 withwrap";

        public override void OnInit()
        {
            left = CreateLeft();
            right = CreateRight("right");

            left.Changed += response => RenderContainer(response, right);
            left.Changed += right.OnChange;
        }

        protected virtual TLeft CreateLeft() => CreateControl<TLeft>("left", SetPropertiesLeft);
        protected abstract IRigthViewPagePart CreateRight(string idControl);
        protected virtual void SetPropertiesLeft(TLeft c) { }
    }

    public abstract class ViewPagePart_sidebar_2col_base : ViewPagePart
	{
		protected IWithChangeEvent left { get; set; }
		protected IWithChangeEventHandler right { get; set; }

		public virtual bool EnableToolbar => false;
		protected virtual void Toolbar(LayoutWriter w) { }
		protected virtual string FormTitle => "";

		protected virtual string ContentBodyClass => "grid_sidebar_2col";

		protected virtual Action<LayoutWriter> RenderPlaceHolderLeftSide => w => w.Div(a => a.ID("container"));

		protected virtual Action<LayoutWriter> RenderPlaceHolderRightSide => w => w.Div(a => a.ID("container"));

		public void RenderContainer(ApiResponse response, IViewPagePart el)
		{
			response.WithNamesAndWritersFor(el);
			var c2 = el.GetContainer();
			c2.ToRemove.Add("contentheader");
			c2.Render(response);
		}

		public override void OnLoad(ApiResponse response)
		{
			response.WithWritersFor(this);
			if (EnableToolbar) response.AddWidget("contenttoolbar", Toolbar);
			response.AddWidget("contenttitle", FormTitle);
			response.AddWidget("contentbody", w => {
				w.Div(a => a.Class(ContentBodyClass), () => {
					w.PushPrefix(left.ID);
					RenderPlaceHolderLeftSide(w);
					w.PopPrefix();

					w.PushPrefix(right.ID);
					RenderPlaceHolderRightSide(w);
					w.PopPrefix();
				});
			});
			response.AddWidget("#title", FormTitle);

			RenderContainer(response, left);
			left.OnLoad(response);

			RenderContainer(response, right);
			right.OnLoad(response);
			right.SetArgGroup("right", response);
		}
	}

	public abstract class ViewPagePart_sidebar_2col<TLeft, TRight> : ViewPagePart_sidebar_2col_base
		where TLeft : IWithChangeEvent, new()
		where TRight : IWithChangeEventHandler, new()
	{
		protected new TLeft left
		{
			get { return (TLeft)base.left; }
			set { base.left = value; }
		}

		protected new TRight right
		{
			get { return (TRight)base.right; }
			set { base.right = value; }
		}

		public override void OnInit()
		{
			left = CreateLeft();
			right = CreateRight();

			left.Changed += response => RenderContainer(response, right);
			left.Changed += right.OnChange;
		}

		protected virtual TLeft CreateLeft() => CreateControl<TLeft>("left", SetPropertiesLeft);
		protected virtual TRight CreateRight() => CreateControl<TRight>("right", SetPropertiesRight);

		protected virtual void SetPropertiesLeft(TLeft c) { }
		protected virtual void SetPropertiesRight(TRight c) { }
	}

	public abstract class ViewPagePart_sidebar_2col<TLeft> : ViewPagePart_sidebar_2col_base
		where TLeft : IWithChangeEvent, new()
	{
		protected new TLeft left
		{
			get { return (TLeft)base.left; }
			set { base.left = value; }
		}

		public override void OnInit()
		{
			left = CreateLeft();
			right = CreateRight("right");

			left.Changed += response => RenderContainer(response, right);
			left.Changed += right.OnChange;
		}

		protected abstract IWithChangeEventHandler CreateRight(string id);

		protected virtual TLeft CreateLeft() => CreateControl<TLeft>("left", SetPropertiesLeft);
		protected virtual void SetPropertiesLeft(TLeft c) { }
	}

	public class ViewPagePart_3col<TLeft, TCenter, TRight> : ViewPagePart
		where TLeft : IWithChangeEvent, new()
		where TCenter : IWithChangeEvent, IWithChangeEventHandler, new()
		where TRight : IWithChangeEventHandler, new()
	{
		protected TLeft left;
		protected TCenter center;
		protected TRight right;

		public override void OnInit()
		{
			left = CreateControl<TLeft>("left", SetPropertiesLeft);
			center = CreateControl<TCenter>("center", SetPropertiesCenter);
			right = CreateControl<TRight>("right", SetPropertiesRight);

			left.Changed += center.OnChange;
			left.Changed += right.OnChange;
			center.Changed += right.OnChange;
		}

		protected virtual void SetPropertiesLeft(TLeft c) { }
		protected virtual void SetPropertiesCenter(TCenter c) { }
		protected virtual void SetPropertiesRight(TRight c) { }

		protected virtual string FormTitle => null;

		protected virtual Grid LeftGrid => Grid.OneThird;
		protected virtual Grid CenterGrid => Grid.OneThird;
		protected virtual Grid RightGrid => Grid.OneThird;

		public override void OnLoad(ApiResponse response)
		{
			response.WithWritersFor(this);
			response.AddWidget("contentbody", w => {
				w.Block(() => {
					w.PushPrefix(left.ID);
					w.Div(a => a.ID("container").Class("grid60").GridColumn(LeftGrid));
					w.PopPrefix();

					w.PushPrefix(center.ID);
					w.Div(a => a.ID("container").Class("grid60").GridColumn(CenterGrid));
					w.PopPrefix();

					w.PushPrefix(right.ID);
					w.Div(a => a.ID("container").Class("grid60").GridColumn(RightGrid));
					w.PopPrefix();
				});
			});

			if (FormTitle != null)
				response.AddWidget("contenttitle", FormTitle);

			response.WithNamesAndWritersFor(left);
			var c1 = left.GetContainer();
			c1.ToRemove.Add("contentheader");
			c1.Render(response);
			left.OnLoad(response);

			response.WithNamesAndWritersFor(center);
			var c2 = center.GetContainer();
			c2.ToRemove.Add("contentheader");
			c2.Render(response);
			center.OnLoad(response);

			response.WithNamesAndWritersFor(right);
			var c3 = right.GetContainer();
			c3.ToRemove.Add("contentheader");
			c3.Render(response);
			right.OnLoad(response);
		}
	}

    public abstract class ViewPagePart_2lines_base : ViewPagePart
    {
        protected IWithChangeEvent top { get; set; }
        protected IWithChangeEventHandler bottom { get; set; }

        public virtual bool EnableToolbar => false;
        protected virtual void Toolbar(LayoutWriter w) { }
        protected virtual string FormTitle => "";

        protected virtual string ContentBodyClass => "grid_sidebar_2line";

        protected virtual Action<LayoutWriter> RenderPlaceHolderTopSide => w => w.Div(a => a.ID("container"));

        protected virtual Action<LayoutWriter> RenderPlaceHolderBottomSide => w => w.Div(a => a.ID("container"));

        public void RenderContainer(ApiResponse response, IViewPagePart el)
        {
            response.WithNamesAndWritersFor(el);
            var c2 = el.GetContainer();
            c2.Render(response);
        }

        public override void OnLoad(ApiResponse response)
        {
            response.WithWritersFor(this);
            if (EnableToolbar) response.AddWidget("contenttoolbar", Toolbar);
            response.AddWidget("contenttitle", FormTitle);
            response.AddWidget("contentbody", w =>
            {
                w.Div(a => a.Class(ContentBodyClass), () =>
                {
                    w.PushPrefix(top.ID);
                    RenderPlaceHolderTopSide(w);
                    w.PopPrefix();

                    w.PushPrefix(bottom.ID);
                    RenderPlaceHolderBottomSide(w);
                    w.PopPrefix();
                });
            });
            response.AddWidget("#title", FormTitle);

            RenderContainer(response, top);
            top.OnLoad(response);

            RenderContainer(response, bottom);
            bottom.OnLoad(response);
        }
    }

    public abstract class ViewPagePart_2lines<TTop, TBottom> : ViewPagePart_2lines_base
        where TTop : IWithChangeEvent, new()
        where TBottom : IWithChangeEventHandler, new()
    {
        protected virtual string TopSideTitle => "";

        protected virtual string BottomSideTitle => "";
        protected new TTop top
        {
            get { return (TTop)base.top; }
            set { base.top = value; }
        }

        protected new TBottom bottom
        {
            get { return (TBottom)base.bottom; }
            set { base.bottom = value; }
        }

        public override void OnInit()
        {
            top = CreateTop();
            bottom = CreateBottom();

            top.Changed += response => RenderContainer(response, bottom);
            top.Changed += bottom.OnChange;
        }

        protected virtual TTop CreateTop() => CreateControl<TTop>("top", SetPropertiesTop);
        protected virtual TBottom CreateBottom() => CreateControl<TBottom>("bottom", SetPropertiesBottom);

        protected virtual void SetPropertiesTop(TTop c) { }
        protected virtual void SetPropertiesBottom(TBottom c) { }
    }

    /// <summary>
    /// Базовый класс для формы с верхней формой для параметром и нижней частью, разделенной на 2 части
    /// </summary>
    /// <typeparam name="TTop"></typeparam>
    /// <typeparam name="TBottomLeft"></typeparam>
    /// <typeparam name="TBottomRight"></typeparam>
    //public abstract class ViewPagePart_top_2col_bottom<TTop, TBottomLeft, TBottomRight> : ViewPagePart
    //	where TTop : IViewPagePart, new()
    //	where TBottomLeft : IViewPagePart, new()
    //	where TBottomRight : IViewPagePart, new()
    //{
    //	private TTop _top;
    //	private TBottomLeft _bottomLeft;
    //	private TBottomRight _bottomRight;

    //	protected virtual bool ObjectNotExists => false;

    //	protected TTop top {
    //		get { return (TTop)this._top; }
    //		set { this._top = value; }
    //	}

    //	protected TBottomLeft bottomLeft {
    //		get { return (TBottomLeft)this._bottomLeft; }
    //		set { this._bottomLeft = value; }
    //	}

    //	protected TBottomRight bottomRight {
    //		get { return (TBottomRight)this._bottomRight; }
    //		set { this._bottomRight = value; }
    //	}

    //	protected virtual Grid BottomLeftGrid => Grid.OneHalf;
    //	protected virtual Grid BottomRightGrid => Grid.OneHalf;

    //	protected virtual Action<TagAttributes> BottomBlockAttributes => null;

    //	public override void OnInit()
    //	{
    //		_top = CreateControl<TTop>("top", SetPropertiesTop);
    //		_bottomLeft = CreateControl<TBottomLeft>("bottomLeft", SetPropertiesBottomLeft);
    //		_bottomRight = CreateControl<TBottomRight>("bottomRight", SetPropertiesBottomRight);
    //	}

    //	protected virtual void SetPropertiesTop(TTop c) { }
    //	protected virtual void SetPropertiesBottomLeft(TBottomLeft c) { }
    //	protected virtual void SetPropertiesBottomRight(TBottomRight c) { }

    //       public override void OnLoad(ApiResponse response)
    //       {
    //		response.WithWritersFor(this);
    //		response.AddWidget("contentbody", w => {
    //			w.Block(() => {
    //				w.PushPrefix(top.ID);
    //				w.Div(a => a.ID("container"));
    //				w.PopPrefix();
    //			});

    //			w.Block(attrs => attrs.Set(BottomBlockAttributes), () => {
    //				w.PushPrefix(bottomLeft.ID);
    //				w.Div(a => a.ID("container").GridColumn(BottomLeftGrid));
    //				w.PopPrefix();

    //				w.PushPrefix(bottomRight.ID);
    //				w.Div(a => a.ID("container").GridColumn(BottomRightGrid));
    //				w.PopPrefix();
    //			});
    //		});

    //		if (FormTitle != null)
    //			response.AddWidget("contenttitle", FormTitle);

    //		void prepare<T>(T c) where T: IViewPagePart
    //		{
    //			response.WithNamesAndWritersFor(c);
    //			var c1 = c.GetContainer();
    //			c1.ToRemove.Add("contentheader");
    //			c1.Render(response);
    //			c.OnLoad(response);
    //		}

    //		prepare(top);
    //		prepare(bottomLeft);
    //		prepare(bottomRight);
    //	}



    //	protected virtual string FormTitle => null;
    //}

}