using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Tango.TaskManager
{
    public class Program
    {
        public static void Main(string[] args)
        {
            bool IsService = args.Contains("--windows-service");
            var pathToExe = IsService ? Process.GetCurrentProcess().MainModule.FileName : Assembly.GetEntryAssembly().Location;
            var curPath = Path.GetDirectoryName(pathToExe);
            Directory.SetCurrentDirectory(curPath);

            var builder = CreateHostBuilder(args.Where(arg => arg != "--windows-service").ToArray());
            if (IsService)
                builder = builder.UseWindowsService();

            using (var host = builder.UseContentRoot(curPath).Build())
            {
                host.Run();
            }
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddHostedService<TaskHostedService>();
            });
    }
}