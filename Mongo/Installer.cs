using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Events;

namespace Mongo
{
	public static class Installer
	{
		public static void Install(HostBuilderContext context, IServiceCollection serviceCollection)
		{
			serviceCollection.AddLogging(builder =>
			{
				builder.AddConfiguration(context.Configuration.GetSection("Logging"));
				builder.AddConsole();
			});

			serviceCollection.AddSingleton(new MongoUrl(context.Configuration.GetSection("Mongo").Get<string>()));
			serviceCollection.AddSingleton(x =>
			{
				var logger = x.GetService<ILogger<MongoClientSettings>>();
				var settings = MongoClientSettings.FromUrl(x.GetService<MongoUrl>());
				settings.ClusterConfigurator = builder =>
				{
					builder.Subscribe<CommandStartedEvent>(e =>
					{
						logger.LogDebug("Executing command {CommandName} \n {Command}", e.CommandName, e.Command.ToJson());
					});
				};
				return settings;
			});
			serviceCollection.AddSingleton<IMongoClient>(x =>
			{
				var settings = x.GetService<MongoClientSettings>();
				return new MongoClient(settings);
			});

			serviceCollection.AddSingleton(x =>
			{
				var databaseName = x.GetService<MongoUrl>()!.DatabaseName;
				return x.GetService<IMongoClient>()!.GetDatabase(databaseName);
			});

			serviceCollection.AddSingleton<IBookRepository, BookRepository>();

			serviceCollection.AddHostedService<MongoInitService>();
			serviceCollection.AddHostedService<UserInterfaceService>();
			
			BookModelSetup.Setup();
		}
	}
}