using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Tango;
using Tango.Html;
using Tango.UI;

namespace Tango.UI.Controls
{
	public class MethodSettingsField : ViewComponent, IFieldValueProvider<MethodSettings>
	{
		public string TypesKey { get; set; }

		FieldGroup gr = new FieldGroup();

		public MethodSettings DefaultValue { get; set; }
		public MethodSettings Value
		{
			get
			{
				var ddlVal = Context.GetArg(ID + "_ddl");
				if (ddlVal.IsEmpty())
					return DefaultValue;
				else
				{
					var parts = ddlVal.Split('|');
					var parms = new Dictionary<string, object>();
					foreach (var fv in Context.AllArgs.Where(x => x.Key.StartsWith(ID + "_parm_")))
					{
						var name = fv.Key.Replace(ID + "_parm_", "");
						parms.Add(name, fv.Value);
					}
					return new MethodSettings
					{
						ClassName = parts[0],
						MethodName = parts[1],
						Params = parms
					};
				}
			}
		}

		public void Render(LayoutWriter w, MethodSettings value)
		{
			var cache = new TypeCache();
			var list = cache.Get(TypesKey).SelectMany(t => {
				var clsName = t.GetCustomAttribute<DescriptionAttribute>()?.Description ?? t.Name;
				return t.GetMethods()
				.Where(m => m.GetCustomAttribute<DescriptionAttribute>() != null)
				.Select(m => {
					var mName = m.GetCustomAttribute<DescriptionAttribute>().Description;
					return new SelectListItem { Value = t.FullName + "|" + m.Name, Text = clsName + "." + mName };
				});
			});
			w.Div(a => a.ID(ID + "_fld"), () =>
			{
				w.DropDownList(ID + "_ddl", $"{value.ClassName}|{value.MethodName}", list.AddEmptyItem(), a => a.OnChangePostEvent(OnChangeMethod));

				w.Div(a => a.ID(ID + "_parms"), () => Parms(w, value));
			});
		}

		public void OnChangeMethod(ApiResponse response)
		{
			var ddlVal = Context.GetArg(ID + "_ddl");

			response.WithNamesAndWritersFor(ParentElement);

			if (ddlVal.IsEmpty())
				response.AddWidget(ID + "_parms", "");
			else
			{
				var parts = ddlVal.Split('|');
				var m = new MethodSettings
				{
					ClassName = parts[0],
					MethodName = parts[1],
					Params = new Dictionary<string, object>()
				};

				response.AddWidget(ID + "_parms", w => Parms(w, m));
			}
		}

		void Parms(LayoutWriter w, MethodSettings value)
		{
			if (value.ClassName.IsEmpty() || value.MethodName.IsEmpty())
				return;

			var cache = new TypeCache();
			var type = cache.Get(TypesKey).Where(t => t.FullName == value.ClassName).First();
			var method = type.GetMethods().Where(m => m.Name == value.MethodName).First();

			w.FieldsBlock(() => {
				foreach (var par in method.GetParameters())
				{
					var caption = par.GetCustomAttribute<DescriptionAttribute>()?.Description;
					if (caption == null)
						continue;

					value.Params.TryGetValue(par.Name.ToLower(), out var val);

					if (par.ParameterType == typeof(DateTime) || par.ParameterType == typeof(DateTime?))
						w.FormFieldCalendar(ID + "_parm_" + par.Name, caption, val != null ? (DateTime)val : (DateTime?)null);
					else
						w.FormFieldTextBox(ID + "_parm_" + par.Name, caption, val);
				}
			});
		}
	}

	public class ParameterField<TValue> : Tango.UI.Field, IField<TValue>
	{
		public override string Caption { get; set; }

		public TValue Value => throw new NotImplementedException();
	}
}
