using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Mongo
{
	class Program
	{
		static async Task Main(string[] args)
		{
			var hostBuilder = new HostBuilder();
			hostBuilder.ConfigureHostConfiguration(builder =>
			{
				builder.AddEnvironmentVariables("DOTNET_");
				builder.AddCommandLine(args);
			});

			hostBuilder.ConfigureAppConfiguration(builder => { builder.AddJsonFile("appsettings.json"); });

			hostBuilder.ConfigureServices(Installer.Install);

			await hostBuilder.Build().RunAsync();
		}
	}
}