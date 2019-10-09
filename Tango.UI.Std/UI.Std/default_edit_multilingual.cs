using System;
using System.Collections.Generic;
using System.Linq;
using Tango.Meta;
using Tango.Localization;

namespace Tango.UI.Std
{
	public abstract class default_edit_multilingual<T, TData, TKey> : default_edit<T, TKey>
		where T : class, IWithKey<T, TKey>, new()
		where TData : class, IMultilingual<TData, TKey>, new()
	{
		[Inject]
		public ILanguage Language { get; set; }

		protected Lazy<List<TData>> MultilingualViewData;

		public override void OnInit()
		{
			base.OnInit();

			MultilingualViewData = new Lazy<List<TData>>(() =>
			{
				if (CreateObjectMode)
				{
					List<TData> l = new List<TData>();
					foreach (var lang in Language.List)
					{
						var obj = new TData();
						DataContext.InsertOnSubmit(obj);
						obj.LanguageCode = lang.Code;
						l.Add(obj);
					}
					return l;
				}
				else
					return DataContext.GetTable<TData>().Where(new TData().ObjectDataSelector(ViewData.ID)).ToList();
			});

		}

		public void Set<TValue>(MetaAttribute<TData, TValue> prop, ILanguage lang, TValue defaultValue = default(TValue))
		{
			foreach (var obj in MultilingualViewData.Value)
			{
				var value = Context.GetArg(prop.Name + "_" + obj.LanguageCode, defaultValue);
				prop.SetValue(obj, value);
			}
		}
	}
}
