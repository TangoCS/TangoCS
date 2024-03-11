using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Tango
{
	public static class RouteUtils
	{
		public static StringBuilder Resolve(string template, 
			IReadOnlyDictionary<string, string> parameters,
			DynamicDictionary globalParameters,
			bool ignoreNotMachedParms = false, string notMatchedParmsBeginWith = "?")
		{
			StringBuilder sb = new StringBuilder();
			List<string> processedKeys = new List<string>();

			Regex reg = new Regex(@"({)([^}]+)(})", RegexOptions.IgnoreCase);
			MatchCollection mc = reg.Matches(template);
			int startIndex = 0;
			foreach (Match m in mc)
			{
				Group g = m.Groups[2]; //it's second in the match between { and }
				int length = g.Index - startIndex - 1;
				sb.Append(template.Substring(startIndex, length));
				string val = g.Value;
				bool isOptional = val.EndsWith("?");
				if (isOptional) val = val.Substring(0, val.Length - 1);
				int constraintPos = val.IndexOf(":");
				if (constraintPos > 0) val = val.Substring(0, constraintPos);

				if (parameters.TryGetValue(val, out string result)) //Cool, we found something
				{
					sb.Append(result);
					processedKeys.Add(val);
				}
				else if (globalParameters != null && globalParameters.TryGetValue(val, out object result2)) //Cool, we found something
				{
					sb.Append(result2);
				}
				else if (!isOptional) //didn't find a property with that name, so be gracious and put it back
				{
					sb.Append("{");
					sb.Append(val);
					sb.Append("}");
				}
				startIndex = g.Index + g.Length + 1;
			}
			if (startIndex < template.Length) //include the rest (end) of the string
			{
				sb.Append(template.Substring(startIndex));
			}

			if (ignoreNotMachedParms) return sb;

			if (parameters.Count > processedKeys.Count)
			{
				sb.Append(notMatchedParmsBeginWith);
			}
			bool first = true;
			foreach (var parm in parameters)
			{
				if (processedKeys.Contains(parm.Key)) continue;
				if (!first) sb.Append("&");
				sb.Append(parm.Key);
				if (parm.Value != null) sb.Append("=").Append(WebUtility.UrlEncode(parm.Value));
				first = false;
			}

			return sb;
		}
	}
}
