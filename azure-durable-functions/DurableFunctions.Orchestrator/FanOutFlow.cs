using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace DurableFunctions.Orchestrator
{
    public static class FanOutFlow
    {
        [FunctionName("FanOutFlow")]
        public static async Task<List<string>> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context,
            ILogger log)
        {
            var tasks = new List<Task<string>>();
            var names = new[] { "Techorama", "delaware" };

            log.LogWarning($"Started saying hello to " + names.Length +" participants");
            foreach (var name in names)
            {
                log.LogWarning($"Calling activity FanOut_Hello for "+ name);
                tasks.Add(context.CallActivityAsync<string>("FanOutFlow_Hello", name));
            }

            await Task.WhenAll(tasks);
            
            var outputs = tasks
                .Select(t => t.Result)
                .ToList();

            log.LogWarning(string.Join(',', outputs));

            return outputs;
        }

        [FunctionName("FanOutFlow_Hello")]
        public static string SayHello(
            [ActivityTrigger] string name,
            ILogger log)
        {
            log.LogWarning($"Saying hello to {name}.");
            return $"Hello {name}!";
        }

        [FunctionName("FanOutFlow_Hello_Http")]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "FanOutFlow/Hello")]HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            string instanceId = await starter.StartNewAsync("FanOutFlow", null);

            log.LogWarning($"Started orchestration with ID = '{instanceId}'.");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}