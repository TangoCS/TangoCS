﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Nephrite
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