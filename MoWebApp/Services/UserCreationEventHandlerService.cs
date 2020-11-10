using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MoWebApp.Core;
using MoWebApp.Documents;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MoWebApp.Services
{
    /// <summary>
    /// Represents a service which handles new user creation events by formatting
    /// a welcome message.
    /// </summary>
    public class UserCreationEventHandlerService : IHostedService
    {
        private readonly IConnectionFactory factory;
        private readonly ILogger<UserCreationEventHandlerService> logger;
        private readonly AppSettings settings;
        private readonly IWebHostEnvironment host;
        private IConnection connection;
        private IModel channel;

        public UserCreationEventHandlerService(IConnectionFactory factory, IOptions<AppSettings> settings, ILogger<UserCreationEventHandlerService> logger, IWebHostEnvironment host)
        {
            Guard.ArgumentNotNull(factory, nameof(factory));
            Guard.ArgumentNotNull(settings, nameof(settings));
            Guard.ArgumentNotNull(logger, nameof(logger));
            Guard.ArgumentNotNull(host, nameof(host));

            this.factory = factory;
            this.settings = settings.Value;
            this.logger = logger;
            this.host = host;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation($"Checking for '{EventBusNames.NewUsers}' events.");

            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            channel.QueueDeclare(
                queue: settings.EventBusQueue,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += EventHandler;

            channel.BasicConsume(
                queue: settings.EventBusQueue,
                autoAck: true,
                consumer: consumer);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation(nameof(UserCreationEventHandlerService) + " is stopping.");

            channel.Dispose();
            connection.Dispose();

            return Task.CompletedTask;
        }

        #region Private

        private void TryHandleNewUserEvent(object state)
        {
            logger.LogInformation($"Checking for '{EventBusNames.NewUsers}' events.");

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(
                    queue: settings.EventBusQueue,
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += EventHandler;

                channel.BasicConsume(
                    queue: settings.EventBusQueue,
                    autoAck: true,
                    consumer: consumer);
            }
        }

#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        private void EventHandler(object? model, BasicDeliverEventArgs args)
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        {
            var body = args.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            var newUserData = JsonConvert.DeserializeObject<NewUserDetails>(message);

            logger.LogInformation(
                $"Handling new user '{newUserData.FirstName} {newUserData.LastName}'.");

            var welcomeMessage =
                GetWelcomeMessage(newUserData.FirstName, newUserData.LastName, newUserData.LoginUrl);

            var folderPath = Path.Combine(host.ContentRootPath, settings.WelcomeMessageFolder);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // Access denied exceptions can be resolved as per:
            // https://www.c-sharpcorner.com/blogs/access-to-path-is-denied-permissions-error
            var filePath = Path.Combine(folderPath, newUserData.Id + ".txt");
            using (var writer = File.CreateText(filePath))
            {
                writer.Write(welcomeMessage);
            }

            logger.LogInformation(
                $"Welcome message written to '{filePath}' for user '{newUserData.FirstName} {newUserData.LastName}'.");
        }

        private static string GetWelcomeMessage(string firstName, string lastName, string url)
        {
            var welcomeMessageTemplate = @"Hello {0} {1}, 
Welcome to Finquest candidate test platform, your registering have been approved, and now you can connect to {2} to use the platform.

FinQuest Team";
            return string.Format(welcomeMessageTemplate, firstName, lastName, url);
        }

        #endregion
    }
}
