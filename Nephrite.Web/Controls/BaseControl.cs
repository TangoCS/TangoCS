using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using Nephrite.MVC;
using System.Web.UI.WebControls;

namespace Nephrite.Web.Controls
{
	public abstract class BaseControl : Control
	{
		[Inject]
		public AbstractQueryString Query { get; set; }

		protected override void OnInit(EventArgs e)
		{
			var props = GetType().GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(InjectAttribute)));
			foreach (var prop in props)
				prop.SetValue(this, DI.RequestServices.GetService(prop.PropertyType));

			base.OnInit(e);
		}
	}

	public abstract class BaseUserControl : UserControl
	{
		[Inject]
		public AbstractQueryString Query { get; set; }

		protected override void OnInit(EventArgs e)
		{
			var props = GetType().GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(InjectAttribute)));
			foreach (var prop in props)
				prop.SetValue(this, DI.RequestServices.GetService(prop.PropertyType));

			base.OnInit(e);
		}
	}

	public abstract class BaseCompositeControl : CompositeControl
	{
		[Inject]
		public AbstractQueryString Query { get; set; }

		protected override void OnInit(EventArgs e)
		{
			var props = GetType().GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(InjectAttribute)));
			foreach (var prop in props)
				prop.SetValue(this, DI.RequestServices.GetService(prop.PropertyType));

			base.OnInit(e);
		}
	}
}