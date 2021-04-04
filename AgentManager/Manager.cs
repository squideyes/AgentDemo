using AgentDemo.Common;
using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AgentDemo.AgentManager
{
    public class Manager
    {
        private readonly string connString;

        public Manager(IConfiguration config)
        {
            if(config == null)
                throw new ArgumentNullException(nameof(config));

            connString = config["ServiceBusConnString"];
        }

        [FunctionName("Manager")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var json = await new StreamReader(req.Body).ReadToEndAsync();

            var command = JsonConvert.DeserializeObject<Command>(json);

            // TODO: validate command

            await using ServiceBusClient client = new ServiceBusClient(connString);

            var sender = client.CreateSender(command.GetTopicName());

            var message = new ServiceBusMessage(json);

            await sender.SendMessageAsync(message);

            return new OkObjectResult($"Sent a \"{command}\" command to {client.FullyQualifiedNamespace}");
        }
    }
}

