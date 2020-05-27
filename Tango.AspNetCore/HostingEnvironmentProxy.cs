using System.IO;

namespace Tango.AspNetCore
{
    public class HostingEnvironmentProxy : IHostingEnvironment
	{
		Microsoft.AspNetCore.Hosting.IHostingEnvironment _env;

		public HostingEnvironmentProxy(Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
		{
			_env = env;

			if (string.IsNullOrWhiteSpace(_env.WebRootPath))
				_env.WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
		}

		public string WebRootPath => _env.WebRootPath;
        public string ContentRootPath => _env.ContentRootPath;
    }
}
