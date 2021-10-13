using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

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
				var settings = MongoClientSettings.FromUrl(x.GetService<MongoUrl>());
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