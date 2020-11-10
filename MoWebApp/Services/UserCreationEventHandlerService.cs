using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
    public class UserCreationEventHandlerService : IHostedService, IDisposable
    {
        private readonly IConnectionFactory factory;
        private readonly ILogger<UserCreationEventHandlerService> logger;
        private readonly AppSettings settings;
        private Timer timer;

        public UserCreationEventHandlerService(IConnectionFactory factory, IOptions<AppSettings> settings, ILogger<UserCreationEventHandlerService> logger)
        {
            Guard.ArgumentNotNull(factory, nameof(factory));
            Guard.ArgumentNotNull(settings, nameof(settings));
            Guard.ArgumentNotNull(logger, nameof(logger));

            this.factory = factory;
            this.settings = settings.Value;
            this.logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Timed Background Service is starting.");

            timer = new Timer(TryHandleNewUserEvent, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Timed Background Service is stopping.");

            timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            timer?.Dispose();
        }

        #region Private

        private void TryHandleNewUserEvent(object state)
        {
            logger.LogInformation($"Checking for '{EventBusNames.NewUsers}' events.");

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                //channel.ExchangeDeclare(exchange: EventBusNames.NewUsers, type: ExchangeType.Fanout);

                //var queueName = channel.QueueDeclare().QueueName;
                //channel.QueueBind(queue: queueName, exchange: EventBusNames.NewUsers, routingKey: "");

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

            // TODO: Add path to configuration.
            var path = Environment.CurrentDirectory;
            using (var file = System.IO.File.Create(path))
            using (var writer = new System.IO.StreamWriter(file))
            {
                writer.Write(welcomeMessage);
            }

            logger.LogInformation(
                $"Welcome message written to file for user '{newUserData.FirstName} {newUserData.LastName}'.");
        }

        private static string GetWelcomeMessage(string firstName, string lastName, string url)
        {
            var welcomeMessageTemplate = @"
Hello {0} {1}, 
Welcome to Finquest candidate test platform, your registering have been approved, and now you can connect to {2} to use the platform.

FinQuest Team
";
            return string.Format(welcomeMessageTemplate, firstName, lastName, url);
        }

        #endregion
    }
}
