using Tango.Meta;

namespace Tango.Localization
{
	public static class LocalizationExtensions
	{
		public static string Caption(this IResourceManager textResource, IMetaNamedElement el)
		{
			return textResource.Get(el.ID);
		}

		public static string CaptionFor(this IResourceManager textResource, IMetaNamedElement el, string suffix)
		{
			return textResource.Get(el.ID + "-" + suffix);
		}

		public static string Description(this IResourceManager textResource, IMetaNamedElement el)
		{
			var res = "";
			if (textResource.TryGet($"{el.ID}-description", out res)) return res;
			return "";
		}

		public static string CaptionShort(this IResourceManager textResource, IMetaProperty el)
		{
			var s = textResource.CaptionFor(el, "s");
			if (s.IsEmpty()) s = textResource.Caption(el);
			return s;
		}

		public static string CaptionPlural(this IResourceManager textResource, IMetaClass el)
		{
			return textResource.CaptionFor(el, "pl");
		}
	}
}
