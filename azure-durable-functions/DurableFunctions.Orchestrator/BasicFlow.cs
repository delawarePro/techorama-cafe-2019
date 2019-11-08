using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace DurableFunctions.Orchestrator
{
    public static class BasicFlow
    {
        [FunctionName("BasicFlow")]
        public static async Task<List<string>> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context,
            ILogger log)
        {
            var outputs = new List<string>();

            log.LogWarning($"Calling activity BasicFlow_Hello for Techorama");
            outputs.Add(await context.CallActivityAsync<string>
                ("BasicFlow_Hello", "Techorama"));

            log.LogWarning($"Calling activity BasicFlow_Hello for delaware");
            outputs.Add(await context.CallActivityAsync<string>
                ("BasicFlow_Hello", "delaware"));

            log.LogWarning(string.Join(',', outputs));

            return outputs;
        }

        [FunctionName("BasicFlow_Hello")]
        public static string SayHello(
            [ActivityTrigger] string name,
            ILogger log)
        {
            log.LogWarning($"Saying hello to {name}.");
            return $"Hello {name}!";
        }

        [FunctionName("BasicFlow_Hello_Http")]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "BasicFlow/Hello")]HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            string instanceId = await starter.StartNewAsync("BasicFlow", null);

            log.LogWarning($"Started orchestration with ID = '{instanceId}'.");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}