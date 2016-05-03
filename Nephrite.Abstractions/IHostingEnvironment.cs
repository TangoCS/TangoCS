using System;
using System.IO;

namespace Nephrite
{
	public interface IHostingEnvironment
	{
		string WebRootPath { get; }
	}

	public static class HostingEnvironmentExtensions
	{
		/// <summary>
		/// Determines the physical path corresponding to the given virtual path.
		/// </summary>
		/// <param name="hostingEnvironment">An instance of <see cref="IHostingEnvironment"/>.</param>
		/// <param name="virtualPath">Path relative to the application root.</param>
		/// <returns>Physical path corresponding to the virtual path.</returns>
		public static string MapPath(
			this IHostingEnvironment hostingEnvironment,
			string virtualPath)
		{
			if (hostingEnvironment == null)
			{
				throw new ArgumentNullException(nameof(hostingEnvironment));
			}

			if (virtualPath == null)
			{
				return hostingEnvironment.WebRootPath;
			}

			// On windows replace / with \.
			virtualPath = virtualPath.Replace('/', Path.DirectorySeparatorChar);
			return Path.Combine(hostingEnvironment.WebRootPath, virtualPath);
		}
	}
}
