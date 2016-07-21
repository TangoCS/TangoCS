using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Tango
{
	public static class Json
	{
		public static JsonSerializerSettings CamelCase
		{
			get
			{
				return new JsonSerializerSettings
				{
					ContractResolver = new CamelCasePropertyNamesContractResolver()
				};
			}
		}
	}
}
