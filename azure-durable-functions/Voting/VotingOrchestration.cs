using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Voting.Models;

namespace Voting
{
    public static class VotingOrchestration
    {
        [FunctionName("Voting_Orchestration")]
        public static async Task<bool> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context,
            ILogger log)
        {
            var request = context.GetInput<StartVotingRequest>();
            log.LogWarning($"Voting_Orchestration#{context.InstanceId}: opening vote " + request.VotingId);

            await context.CallActivityAsync("Voting_Activity_Open", request);

            await context.WaitForExternalEvent("Voting_Event_Accepted");

            log.LogWarning($"Voting_Orchestration#{context.InstanceId}: accepting vote " + request.VotingId);

            return true;
        }
    }
}