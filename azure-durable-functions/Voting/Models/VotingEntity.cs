using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Voting.Models
{
    public interface IVotingOperations
    {
        void New(int minimumInFavor);
        void Vote(bool isYea);
    }
    public class VotingEntity : IVotingOperations
    {
        public bool EventRaised { get; set; }
        public int MinimumInFavor { get; set; }
        public int Yea { get; set; }
        public int Nay { get; set; }
        
        public void New(int minimumInFavor)
        {
            MinimumInFavor = minimumInFavor;
        }
        public void Vote(bool isYea)
        {
            if (isYea)
            {
                Yea++;
            }
            else
            {
                Nay++;
            }
        }

        [FunctionName(nameof(VotingEntity))]
        public static async Task Run(
            [EntityTrigger] IDurableEntityContext entityContext,
            [DurableClient] IDurableClient durableClient,
            ILogger log)
        {
            await entityContext.DispatchAsync<VotingEntity>();

            log.LogWarning($"Entity_Voting_Run#{entityContext.EntityId}: Executing {entityContext.OperationName}");

            var state = entityContext.GetState<VotingEntity>();

            if(state.Yea >= state.MinimumInFavor && !state.EventRaised)
            {
                log.LogWarning($"Entity_Voting_Run#{entityContext.EntityId}: Raising event Voting_Event_Accepted");

                await durableClient.RaiseEventAsync(entityContext.EntityKey, "Voting_Event_Accepted");
                state.EventRaised = true;

                entityContext.SetState(state);
            }
        }
    }
}
