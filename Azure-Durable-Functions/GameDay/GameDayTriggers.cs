using System;
using System.Net.Http;
using System.Threading.Tasks;
using GameDay.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace GameDay
{
    public class GameDayTriggers
    {
        [FunctionName("GameDay_HTTP_Start")]
        public static async Task<object> Start(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Games")]HttpRequestMessage requestMessage,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            var request = await requestMessage.Content.Deserialize<StartGameRequest>();
            request.GameId = Guid.NewGuid();

            await starter.StartNewAsync("GameDay_Orchestration", request.GameId.ToString(), request);

            log.LogInformation($"StartGame initiated with id = '{request.GameId}'.");

            return new
            {
                GameId = request.GameId
            };
        }

        [FunctionName("GameDay_HTTP_Satus")]
        public static async Task<object> Status(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Games/{instanceId}")]HttpRequestMessage requestMessage,
            [DurableClient] IDurableOrchestrationClient starter,
            string instanceId,
            ILogger log)
        {
            var status = await starter.GetStatusAsync(instanceId);

            return new
            {
                status.InstanceId,
                status.RuntimeStatus,
                status.Output
            };
        }

        [FunctionName("GameDay_HTTP_PlayerAvailable")]
        public static async Task RaiseGameDayPlayerAvailable(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Games/{gameId}/Players/{player}/Available")]HttpRequestMessage requestMessage,
            [DurableClient] IDurableOrchestrationClient starter,
            string gameId,
            string player,
            ILogger log)
        {
            await starter.RaiseEventAsync($"{gameId}:{player}", "GameDay_Event_PlayerAvailable");
        }

        [FunctionName("GameDay_HTTP_PlayerUnavailable")]
        public static async Task RaiseGameDayPlayerUnavailable(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Games/{gameId}/Players/{player}/Unavailable")]HttpRequestMessage requestMessage,
            [DurableClient] IDurableOrchestrationClient starter,
            string gameId,
            string player,
            ILogger log)
        {
            await starter.RaiseEventAsync($"{gameId}:{player}", "GameDay_Event_PlayerUnavailable");
        }
    }
}