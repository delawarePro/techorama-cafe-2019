using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GameDay.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace GameDay
{
    public static class GameDayOrchestration
    {
        [FunctionName("GameDay_Orchestration")]
        public static async Task<bool> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context,
            ILogger log)
        {
            var request = context.GetInput<StartGameRequest>();
            log.LogWarning($"GameDay_Orchestration#{context.InstanceId}: starting game " + request.GameId);

            var tasks = new List<Task<bool>>();
            foreach (var player in request.Players)
            {
                log.LogWarning($"GameDay_Orchestration#{context.InstanceId}: checking player availability " + player);

                tasks.Add(
                    context.CallSubOrchestratorAsync<bool>(
                        "GameDay_Activity_PlayerAvailability",
                        request.GameId.ToString() + ":" + player,
                        player));
            }

            var timeoutTask = context.CreateTimer(
                context.CurrentUtcDateTime.AddHours(5),
                CancellationToken.None);

            var resultEvent = await Task.WhenAny(
                Task.WhenAll(tasks),
                timeoutTask);

            if (resultEvent == timeoutTask)
            {
                log.LogWarning($"GameDay_Orchestration#{context.InstanceId}: not all players responded");
            }
            else
            {
                log.LogWarning($"GameDay_Orchestration#{context.InstanceId}: all players responded");
            }

            var availablePlayers = tasks
                .Where(t => t.IsCompletedSuccessfully)
                .Where(t => t.Result == true)
                .Count();

            var minimumOfPlayersAvailalbe = availablePlayers >= request.MinimumOfPlayers;

            log.LogWarning($"GameDay_Orchestration#{context.InstanceId}: {availablePlayers} players are available, {request.MinimumOfPlayers} player(s) needed.");
            log.LogWarning($"GameDay_Orchestration#{context.InstanceId}: {(minimumOfPlayersAvailalbe ? "Game on!" : "Game over....")}");

            await context.CallActivityAsync(
                "GameDay_Activity_ScoreBoard",
                new CreateScoreBoardRequest()
                {
                    GameId = request.GameId,
                    IsGameOn = minimumOfPlayersAvailalbe
                });

            return minimumOfPlayersAvailalbe;
        }
    }
}