using System;
using System.Net.Http;
using System.Threading.Tasks;
using Voting.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Voting
{
    public class VotingTriggers
    {
        [FunctionName("Voting_HTTP_Start")]
        public static async Task<object> Start(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Voting")]HttpRequestMessage requestMessage,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            var request = await requestMessage.Content.Deserialize<StartVotingRequest>();
            request.VotingId = Guid.NewGuid().ToString();

            await starter.StartNewAsync("Voting_Orchestration", request.VotingId, request);

            log.LogInformation($"Voting initiated with id = '{request.VotingId}'.");

            return new
            {
                VotingId = request.VotingId
            };
        }

        [FunctionName("Voting_HTTP_Satus")]
        public static async Task<object> Status(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Voting/{instanceId}")]HttpRequestMessage requestMessage,
            [DurableClient] IDurableOrchestrationClient durableClient,
            [DurableClient] IDurableEntityClient entityClient,
            string instanceId)
        {
            var status = await durableClient.GetStatusAsync(instanceId);
            var state = await entityClient.ReadEntityStateAsync<VotingEntity>(
                new EntityId(nameof(VotingEntity), instanceId));

            return new
            {
                status.InstanceId,
                status.RuntimeStatus,
                status.Output,
                State = state.EntityState
            };
        }

        [FunctionName("Voting_HTTP_Yea")]
        public static async Task VoteYea(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Voting/{votingId}/Voters/{voter}/yea")]HttpRequestMessage requestMessage,
            [DurableClient] IDurableEntityClient entityClient,
            string votingId,
            string voter)
        {
            await entityClient.SignalEntityAsync(
                new EntityId(nameof(VoterEntity), $"{votingId}:{voter}"),
                "Vote",
                new VoterEntityVoteParameters { VotingId = votingId, Vote = true });
        }

        [FunctionName("Voting_HTTP_Nay")]
        public static async Task VoteNay(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Voting/{votingId}/Voters/{voter}/nay")]HttpRequestMessage requestMessage,
            [DurableClient] IDurableEntityClient entityClient,
            string votingId,
            string voter)
        {
            await entityClient.SignalEntityAsync(
                new EntityId(nameof(VoterEntity), $"{votingId}:{voter}"),
                "Vote",
                new VoterEntityVoteParameters { VotingId = votingId, Vote = false });
        }
    }
}