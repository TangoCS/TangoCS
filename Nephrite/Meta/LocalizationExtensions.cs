using Nephrite.Meta;

namespace Nephrite.Localization
{
	public static class LocalizationExtensions
	{
		public static string Caption(this IMetaNamedElement el, ITextResource textResource)
		{
			return textResource.Get(el.ID, el.Name);
		}

		public static string CaptionFor(this IMetaNamedElement el, ITextResource textResource, string suffix)
		{
			return textResource.Get(el.ID + "-" + suffix);
		}

		public static string Description(this IMetaNamedElement el, ITextResource textResource)
		{
			return el.CaptionFor(textResource, "description");
		}

		public static string CaptionShort(this IMetaProperty el, ITextResource textResource)
		{
			var s = el.CaptionFor(textResource, "s");
			if (s.IsEmpty()) s = el.Caption(textResource);
			return s;
		}

		public static string CaptionPlural(this IMetaClass el, ITextResource textResource)
		{
			return el.CaptionFor(textResource, "pl");
		}
	}
}
