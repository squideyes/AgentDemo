using AgentDemo.Common;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace AgentDemo.AgentRunner
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> logger;
        private readonly string connString;
        private readonly string topicName;
        private readonly string subscriptionName;

        private bool canWork = true;
        private int millisecondsDelay = 1000;

        public Worker(ILogger<Worker> logger, IConfiguration config)
        {
            this.logger = logger;

            connString = config["ServiceBusConnString"];
            topicName = config["TopicName"];
            subscriptionName = config["SubscriptionName"];
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var serviceBusTokenSource = new CancellationTokenSource();

            var cts = CancellationTokenSource.CreateLinkedTokenSource(
                stoppingToken, serviceBusTokenSource.Token);

            await using ServiceBusClient client = new ServiceBusClient(connString);

            var processor = client.CreateProcessor(
                topicName, subscriptionName, new ServiceBusProcessorOptions());

            var options = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            };

            options.Converters.Add(new JsonStringEnumConverter());

            processor.ProcessMessageAsync += async e => 
                await ProcessMessageAsync(e, options, serviceBusTokenSource);

            processor.ProcessErrorAsync += e =>
            {
                logger.LogError(e.Exception.ToString());

                return Task.CompletedTask;
            };

            await processor.StartProcessingAsync(cts.Token);

            while (!cts.Token.IsCancellationRequested)
            {
                if (canWork)
                    logger.LogInformation($"Processing: {DateTime.Now}");

                await Task.Delay(millisecondsDelay, stoppingToken);
            }

            await processor.StopProcessingAsync();

            await processor.CloseAsync();
        }

        private async Task ProcessMessageAsync(ProcessMessageEventArgs args, 
            JsonSerializerOptions options, CancellationTokenSource cts)
        {
            var json = args.Message.Body.ToString();

            var command = JsonSerializer.Deserialize<Command>(json, options);

            switch (command.Kind)
            {
                case CommandKind.Pause:
                    canWork = false;
                    break;
                case CommandKind.Resume:
                    canWork = true;
                    break;
                case CommandKind.Stop:
                    cts.Cancel();
                    break;
                case CommandKind.Configure:
                    if (command.Settings.ContainsKey("Delay"))
                        millisecondsDelay = int.Parse(command.Settings["Delay"]);
                    break;
                default:
                    // TODO: log + continue; only throw error after x failures
                    throw new ArgumentOutOfRangeException(nameof(command));
            }

            logger.LogInformation(
                $"Received: {json} command from subscription: {subscriptionName}");

            await args.CompleteMessageAsync(args.Message);
        }
    }
}
