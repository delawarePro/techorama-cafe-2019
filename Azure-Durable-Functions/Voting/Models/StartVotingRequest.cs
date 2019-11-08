using System;
using System.Collections.Generic;

namespace Voting.Models
{
    public class StartVotingRequest
    {
        public StartVotingRequest()
        {
            Voters = new List<string>();
        }

        public int MinimumInFavor { get; set; }
        public List<string> Voters { get; set; }
        public string VotingId { get; set; }
    }
}
