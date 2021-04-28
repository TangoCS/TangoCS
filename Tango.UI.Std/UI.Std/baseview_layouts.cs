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

namespace Tango.UI.Std
{
	public interface IWithChangeEvent
	{
		event Action<ApiResponse> Changed;
	}

	public interface IWithChangeEventHandler
	{
		void OnChange(ApiResponse response);
	}

	public class ViewPagePart_sidebar_2col<TLeft, TRight> : ViewPagePart
		where TLeft : ViewPagePart, IWithChangeEvent, new()
		where TRight : ViewPagePart, IWithChangeEventHandler, new()
	{
		protected TLeft left;
		protected TRight right;

		public override void OnInit()
		{
			left = CreateControl<TLeft>("left", SetPropertiesLeft);
			right = CreateControl<TRight>("right", SetPropertiesRight);

			left.Changed += right.OnChange;
		}

		protected virtual void SetPropertiesLeft(TLeft c) { }
		protected virtual void SetPropertiesRight(TRight c) { }

		public virtual bool EnableToolbar => false;
		protected virtual void Toolbar(LayoutWriter w) { }

		public override void OnLoad(ApiResponse response)
		{
			response.WithWritersFor(this);
			if (EnableToolbar) response.AddWidget("contenttoolbar", Toolbar);
			response.AddWidget("contentbody", w => {
				w.Div(a => a.Class("grid_sidebar_2col"), () => {
					w.PushPrefix(left.ID);
					w.Div(a => a.ID("container"));
					w.PopPrefix();

					w.PushPrefix(right.ID);
					w.Div(a => a.ID("container"));
					w.PopPrefix();
				});
			});

			response.WithNamesAndWritersFor(left);
			var c1 = left.GetContainer();
			c1.ToRemove.Add("contentheader");
			c1.Render(response);
			left.OnLoad(response);

			response.WithNamesAndWritersFor(right);
			var c2 = right.GetContainer();
			c2.ToRemove.Add("contentheader");
			c2.Render(response);
			right.OnLoad(response);
		}
	}


	public class ViewPagePart_3col<TLeft, TCenter, TRight> : ViewPagePart
		where TLeft : ViewPagePart, IWithChangeEvent, new()
		where TCenter : ViewPagePart, IWithChangeEvent, IWithChangeEventHandler, new()
		where TRight : ViewPagePart, IWithChangeEventHandler, new()
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

}