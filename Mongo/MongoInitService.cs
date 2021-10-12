using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Mongo
{
    public class MongoInitService : IHostedService
    {
        private readonly ILogger<MongoInitService> _logger;
        private readonly IMongoClient _mongoClient;

        public MongoInitService(ILogger<MongoInitService> logger, IMongoClient mongoClient)
        {
            _logger = logger;
            _mongoClient = mongoClient;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var databaseNames = await _mongoClient.ListDatabaseNames().ToListAsync(cancellationToken);
            _logger.LogInformation("{Names}", string.Join(", ", databaseNames));
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}