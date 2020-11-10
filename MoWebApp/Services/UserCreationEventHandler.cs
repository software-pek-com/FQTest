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
    public class UserCreationEventHandler : BackgroundService
    {
        private readonly IConnectionFactory factory;
        private readonly ILogger<UserCreationEventHandler> logger;
        private readonly IOptions<AppSettings> settings;
        private readonly string queueName;

        public UserCreationEventHandler(IConnectionFactory factory, IOptions<AppSettings> settings, ILogger<UserCreationEventHandler> logger)
        {
            Guard.ArgumentNotNull(factory, nameof(factory));
            Guard.ArgumentNotNull(settings, nameof(settings));
            Guard.ArgumentNotNull(logger, nameof(logger));

            this.factory = factory;
            this.settings = settings;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stopToken)
        {
            //Do your preparation (e.g. Start code) here
            var queueName = settings.Value.EventBusQueue;

            while (!stopToken.IsCancellationRequested)
            {
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: queueName,
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);

                        var newUserData = JsonConvert.DeserializeObject<NewUserDetails>(message);
                        var welcomeMessage =
                            GetWelcomeMessage(newUserData.FirstName, newUserData.LastName, newUserData.LoginUrl);

                        // TODO: Write welcomeMessage to file.
                        
                    };
                    channel.BasicConsume(queue: queueName,
                                         autoAck: true,
                                         consumer: consumer);

                }
            }
            //Do your cleanup (e.g. Stop code) here
        }

        private static string GetWelcomeMessage(string firstName, string lastName, string env)
        {
            var welcomeMessageTemplate = @"
Hello {0} {1}, 
Welcome to Finquest candidate test platform, your registering have been approved, and now you can connect to {2}/login to use the platform.

FinQuest Team
";
            return string.Format(welcomeMessageTemplate, firstName, lastName, env);
        }
    }
}
