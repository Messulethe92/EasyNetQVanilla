using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WebSubscriber
{
    public class ConsumeRabbitMQHostedService : BackgroundService
    {
        private readonly ILogger logger;
        private IConnection connection;
        private IModel channel;

        public ConsumeRabbitMQHostedService(ILoggerFactory loggerFactory)
        {
            this.logger = loggerFactory.CreateLogger<ConsumeRabbitMQHostedService>();
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
            channel.BasicQos(0, 1, false);

            connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (ch, ea) =>
            {
                // received message
                var content = System.Text.Encoding.UTF8.GetString(ea.Body);

                // handle the message
                HandleMessage(content);
                channel.BasicAck(ea.DeliveryTag, false);
            };

            consumer.Shutdown += OnConsumerShutdown;
            consumer.Registered += OnConsumerRegistered;
            consumer.Unregistered += OnConsumerUnregistered;
            consumer.ConsumerCancelled += OnConsumerConsumerCancelled;

            channel.BasicConsume("Messages.TextMessage, Messages_test", false, consumer);
            return Task.CompletedTask;
        }

        private void HandleMessage(string content)
        {
            // do something
            var textMessage = JsonConvert.DeserializeObject<Messages.TextMessage>(content);
            logger.LogInformation($"Consumer received: {textMessage.Text ?? ""}");
        }

        private void OnConsumerConsumerCancelled(object sender, ConsumerEventArgs e)
        {
        }

        private void OnConsumerUnregistered(object sender, ConsumerEventArgs e)
        {
        }

        private void OnConsumerRegistered(object sender, ConsumerEventArgs e)
        {
        }

        private void OnConsumerShutdown(object sender, ShutdownEventArgs e)
        {
        }

        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
        }
    }
}
