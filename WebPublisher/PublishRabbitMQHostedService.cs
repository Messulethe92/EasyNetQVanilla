using Messages;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebPublisher
{
    public class PublishRabbitMQHostedService : BackgroundService
    {

        private readonly ILogger logger;
        private IConnection connection;
        private IModel channel;
        public PublishRabbitMQHostedService(ILoggerFactory loggerFactory)
        {

            this.logger = loggerFactory.CreateLogger<PublishRabbitMQHostedService>();
            InitRabbitMQ();
        }

        private void InitRabbitMQ()
        {
            var factory = new ConnectionFactory { HostName = "localhost" };

            // create connection
            connection = factory.CreateConnection();

            // create channel
            channel = connection.CreateModel();

            channel.ExchangeDeclare("Messages.TextMessage, Messages", ExchangeType.Topic, true);
            channel.QueueDeclare("Messages.TextMessage, Messages_test", true, false, false, null);
            channel.QueueBind("Messages.TextMessage, Messages_test", "Messages.TextMessage, Messages", "#", null);
            
            connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }

        public Task SendMessage(TextMessage textMessage) {

            logger.LogInformation($"Publishing message: {textMessage.Text}");
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(textMessage));

            channel.BasicPublish(exchange: "Messages.TextMessage, Messages",
                                 routingKey: "#",                                 
                                 basicProperties: null,
                                 body: body);

            logger.LogInformation($"Published message");

            return Task.CompletedTask;
        }

        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
        }
    }
}
