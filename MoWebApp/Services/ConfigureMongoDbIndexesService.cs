using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MoWebApp.Core;
using MoWebApp.Data;

namespace MoWebApp.Services
{
    /// <summary>
    /// Represents configuration of an index on <see cref="User.UserName"/> in the MongoDB database.
    /// </summary>
    public class ConfigureMongoDbIndexesService : IHostedService
    {
        private readonly IMongoClient client;
        private readonly ILogger<ConfigureMongoDbIndexesService> logger;

        /// <summary>
        /// Creates an instance of this class.
        /// </summary>
        public ConfigureMongoDbIndexesService(IMongoClient client, ILogger<ConfigureMongoDbIndexesService> logger)
        {
            Guard.ArgumentNotNull(client, nameof(client));
            Guard.ArgumentNotNull(logger, nameof(logger));

            this.client = client;
            this.logger = logger;
        }

        /// <summary>
        /// Starts the task to create an index on <see cref="User.UserName"/> in the MongoDB database.
        /// </summary>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            /// <see cref="User.UserName"/> requires a unique constraint.
            var keys = Builders<User>.IndexKeys.Ascending(d => d.UserName);
            var indexOptions = new CreateIndexOptions { Unique = true };
            var modelIndex = new CreateIndexModel<User>(keys, indexOptions);

            var databaseName = (string)ConfigurationManager.GetSection("Database:Name");
            var database = client.GetDatabase(databaseName);

            var collection = database.GetCollection<User>(nameof(User));
            await collection.Indexes.CreateOneAsync(modelIndex);

            logger.LogInformation(
                $"Created '{nameof(User.UserName)}' index on {nameof(User)}.");
        }

        /// <summary>
        /// Starts the task to create an index on <see cref="User.UserName"/> in the MongoDB database.
        /// </summary>
        public Task StopAsync(CancellationToken cancellationToken)
            => Task.CompletedTask;
    }
}
