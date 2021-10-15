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

			serviceCollection.AddSingleton<EventSubscriber>();
			serviceCollection.AddSingleton(new MongoUrl(context.Configuration.GetSection("Mongo").Get<string>()));
			serviceCollection.AddSingleton(x =>
			{
				var logger = x.GetService<ILogger<MongoClientSettings>>();
				var settings = MongoClientSettings.FromUrl(x.GetService<MongoUrl>());
				settings.SdamLogFilename = @"sdam.log";
				settings.ClusterConfigurator = builder => builder.Subscribe(x.GetService<EventSubscriber>());
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
			serviceCollection.AddSingleton<IUserRepository, UserRepository>();

			serviceCollection.AddHostedService<MongoInitService>();
			serviceCollection.AddHostedService<UserInterfaceService>();

			serviceCollection.AddSingleton(context.Configuration.GetSection("BookReservationService").Get<BookReservationServiceConfig>());
			serviceCollection.AddSingleton<BookReservationService>();
			
			MongoModelsSetup.Setup();
		}
	}
}