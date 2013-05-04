using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nephrite.Metamodel;

namespace Nephrite.Web
{
    public static class LangExtender
    {
        public static string Lang(this System.Web.UI.Control ctrl, string lang, string data)
        {
            if (AppMM.CurrentLanguage.LanguageCode.ToUpper() == lang.ToUpper())
                return data;
            return String.Empty;
        }

        public static string Lang(this System.Web.UI.Control ctrl, string lang, string data, params string[] otherLang)
        {
            string lc = AppMM.CurrentLanguage.LanguageCode.ToUpper();
            if (lc == lang.ToUpper())
                return data;
            for (int i = 0; i < otherLang.Length; i += 2)
                if (otherLang[i].ToUpper() == lc)
                    return otherLang[i + 1];
            return String.Empty;
        }

    }
}
