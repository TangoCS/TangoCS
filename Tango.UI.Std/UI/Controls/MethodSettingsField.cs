using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Tango;
using Tango.Data;
using Tango.Html;
using Tango.UI;

namespace Tango.UI.Controls
{
	public class MethodSettingsField : ViewComponent, IFieldValueProvider<MethodSettings>
	{
		public string TypesKey { get; set; }

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
					foreach (var par in GetMethodParameters(parts[0], parts[1]))
					{
						var name = ParameterFieldID(par);
						var r = par.GetCustomAttribute<RenderWithAttribute>();
						if (r != null)
						{
							parms.Add(par.Name.ToLower(), r.GetFormValue(Context.AllArgs, name));
						}
						else
							parms.Add(par.Name.ToLower(), Context.GetArg(name));
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

			w.FieldsBlock(() => {
				var ctx = new RenderingContext { RequestServices = Context.RequestServices };
				foreach (var par in GetMethodParameters(value.ClassName, value.MethodName))
				{
					var caption = par.GetCustomAttribute<DescriptionAttribute>()?.Description;
					if (caption == null)
						continue;

					value.Params.TryGetValue(par.Name.ToLower(), out var val);

					var r = par.GetCustomAttribute<RenderWithAttribute>();
					if (r != null)
					{
						var field = new ParameterField
						{
							ID = ParameterFieldID(par),
							Caption = caption,
							Value = val
						};
						r.Render(ctx, w, field, Grid.OneWhole);
					}
					else
						w.FormFieldTextBox(ID + "_parm_" + par.Name, caption, val);
				}
			});
		}

		string ParameterFieldID(ParameterInfo par) => ID + "_parm_" + par.Name;

		ParameterInfo[] GetMethodParameters(string className, string methodName)
		{
			var cache = new TypeCache();
			var type = cache.Get(TypesKey).Where(t => t.FullName == className).First();
			var method = type.GetMethods().Where(m => m.Name == methodName).First();
			return method.GetParameters();
		}
	}

	public class ParameterField : Tango.UI.Field, IField<object>
	{
		public object Value { get; set; }
	}

	public abstract class RenderWithAttribute : Attribute
	{
		public abstract void Render<TValue>(RenderingContext ctx, LayoutWriter w, IField<TValue> field, GridPosition grid = null);
		public abstract string GetStringValue(RenderingContext ctx, object value);
		public virtual object GetFormValue(IReadOnlyDictionary<string, object> args, string fieldID)
		{
			return args[fieldID];
		}
	}

	public class RenderingContext
	{
		public IServiceProvider RequestServices { get; set; }
		public Dictionary<Type, IEnumerable<SelectListItem>> SelectListCache { get; }
			= new Dictionary<Type, IEnumerable<SelectListItem>>();
	}

	public interface ISelectListDataSource
	{
		IEnumerable<SelectListItem> GetList();
	}

	public class EntityDataSource<T> : ISelectListDataSource
		where T : IWithTitle
	{
		IDatabase db;

		public EntityDataSource(IDatabase db)
		{
			this.db = db;
		}

		public IEnumerable<SelectListItem> GetList()
		{
			return db.Repository<T>().List().OrderBy(o => o.Title).Select(o => {
				if (o is IWithKey<int> o1)
					return new SelectListItem { Text = o.Title, Value = o1.ID.ToString() };
				else if (o is IWithKey<Guid> o2)
					return new SelectListItem { Text = o.Title, Value = o2.ID.ToString() };

				throw new Exception("Unknown key property");
			});
		}
	}

	public class EnumDataSource<T> : ISelectListDataSource
		where T : Enum
	{
		public IEnumerable<SelectListItem> GetList()
		{
			var result = new List<SelectListItem>();
			var values = Enum.GetValues(typeof(T));

			foreach (int item in values)
				result.Add(new SelectListItem
				{
					Value = item.ToString(),
					Text = Enumerations.GetEnumDescription((T)(object)item)
				});

			return result;
		}
	}
}
