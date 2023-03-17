using System.IO;

namespace Tango.AspNetCore
{
    public class HostingEnvironmentProxy : IHostingEnvironment
	{
		readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment _env;

		public HostingEnvironmentProxy(Microsoft.AspNetCore.Hosting.IWebHostEnvironment env)
		{
			_env = env;

			if (string.IsNullOrWhiteSpace(_env.WebRootPath))
				_env.WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
		}

		public string WebRootPath => _env.WebRootPath;
        public string ContentRootPath => _env.ContentRootPath;
		public string EnvironmentName => _env.EnvironmentName;
	}
}
