using Nephrite.Meta;

namespace Nephrite.Localization
{
	public static class LocalizationExtensions
	{
		public static string Caption(this ITextResource textResource, IMetaNamedElement el)
		{
			return textResource.Get(el.ID, el.Name);
		}

		public static string CaptionFor(this ITextResource textResource, IMetaNamedElement el, string suffix)
		{
			return textResource.Get(el.ID + "-" + suffix);
		}

		public static string Description(this ITextResource textResource, IMetaNamedElement el)
		{
			return textResource.CaptionFor(el, "description");
		}

		public static string CaptionShort(this ITextResource textResource, IMetaProperty el)
		{
			var s = textResource.CaptionFor(el, "s");
			if (s.IsEmpty()) s = textResource.Caption(el);
			return s;
		}

		public static string CaptionPlural(this ITextResource textResource, IMetaClass el)
		{
			return textResource.CaptionFor(el, "pl");
		}
	}
}
