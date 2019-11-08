using System.Threading.Tasks;
using GameDay.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace GameDay
{
    public class GameDayActivities
    {

        [FunctionName("GameDay_Activity_PlayerAvailability")]
        public async static Task<bool> PlayerAvailability(
            [OrchestrationTrigger] IDurableOrchestrationContext context,
            ILogger log)
        {
            var name = context.GetInput<string>();
            log.LogWarning($"GameDay_Activity_PlayerAvailability#{context.InstanceId}: Awaiting player availability {name}.");

            var availableEvent = context.WaitForExternalEvent("GameDay_Event_PlayerAvailable");
            var unavailableEvent = context.WaitForExternalEvent("GameDay_Event_PlayerUnavailable");

            var responseEvent = await Task.WhenAny(
                availableEvent,
                unavailableEvent);

            bool isAvailable = responseEvent == availableEvent;
            log.LogWarning($"GameDay_Activity_PlayerAvailability#{context.InstanceId}: Player {name} is {(isAvailable ? "" : "not")} available.");

            return isAvailable;
        }

        [FunctionName("GameDay_Activity_ScoreBoard")]
        [return: Table("ScoreBoard")]
        public static ScoreBoardEntity ScoreBoard(
            [ActivityTrigger] CreateScoreBoardRequest request,
            ILogger log)
        {
            if (request.IsGameOn)
            {
                log.LogWarning($"Creating scoreboard for game {request.GameId}");
                return new ScoreBoardEntity(request.GameId);
            }

            return null;
        }
    }
}