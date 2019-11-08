using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace Voting.Models
{
    public class VoterEntity
    {
        [FunctionName(nameof(VoterEntity))]
        public static void Run(
            [EntityTrigger] IDurableEntityContext entityContext,
            ILogger log)
        {
            log.LogWarning($"Entity_Voting_Run#{entityContext.EntityId}: Executing {entityContext.OperationName}");

            switch (entityContext.OperationName)
            {
                case "Vote":
                    var state = entityContext.GetInput<VoterEntityVoteParameters>();
                    if (entityContext.IsNewlyConstructed)
                    {
                        log.LogWarning($"Entity_Voting_Run#{entityContext.EntityId}: New vote. Regestering vote on VotingEntity");

                        entityContext.SignalEntity<IVotingOperations>(
                            new EntityId(nameof(VotingEntity), state.VotingId),
                            vote => vote.Vote(state.Vote));
                    }
                    else
                    {
                        log.LogWarning($"Entity_Voting_Run#{entityContext.EntityId}: Existing vote.");
                    }
                    break;
                case "Function-1":
                    break;
            }
        }
    }
}
