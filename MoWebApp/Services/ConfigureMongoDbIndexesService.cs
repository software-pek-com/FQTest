using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MoWebApp.Core;
using MoWebApp.Models;

namespace MoWebApp.Services
{
    public class ConfigureMongoDbIndexesService : IHostedService
    {
        private readonly IMongoClient client;
        private readonly ILogger<ConfigureMongoDbIndexesService> logger;

        public ConfigureMongoDbIndexesService(IMongoClient client, ILogger<ConfigureMongoDbIndexesService> logger)
        {
            Guard.ArgumentNotNull(client, nameof(client));
            Guard.ArgumentNotNull(logger, nameof(logger));

            this.client = client;
            this.logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            /// <see cref="UserDocument.UserName"/> requires a unique constraint.
            var keys = Builders<UserDocument>.IndexKeys.Ascending(d => d.UserName);
            var indexOptions = new CreateIndexOptions { Unique = true };
            var modelIndex = new CreateIndexModel<UserDocument>(keys, indexOptions);

            var databaseName = (string) ConfigurationManager.GetSection("Database:Name");
            var database = client.GetDatabase(databaseName);

            var collection = database.GetCollection<UserDocument>(nameof(UserDocument));
            await collection.Indexes.CreateOneAsync(modelIndex);

            logger.LogInformation(
                $"Created '{nameof(UserDocument.UserName)}' index on {nameof(UserDocument)}.");
        }

        public Task StopAsync(CancellationToken cancellationToken)
            => Task.CompletedTask;
    }
}
