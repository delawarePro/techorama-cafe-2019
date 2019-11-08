using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Voting.Models;

namespace Voting
{
    public class VotingActivities
    {
        [FunctionName("Voting_Activity_Open")]
        public async static Task OpenVoting(
            [ActivityTrigger] StartVotingRequest request,
            [DurableClient] IDurableEntityClient entityClient,
            ILogger log)
        {
            log.LogWarning($"Voting_Activity_Open#{request.VotingId}: Opening vote");

            await entityClient.SignalEntityAsync<IVotingOperations>(
                request.VotingId, vote => vote.New(request.MinimumInFavor));
        }
    }
}