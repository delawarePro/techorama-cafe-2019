using System;
using System.Collections.Generic;

namespace GameDay.Models
{
    public class StartGameRequest
    {
        public StartGameRequest()
        {
            Players = new List<string>();
        }

        public int MinimumOfPlayers { get; set; }
        public Guid GameId { get; set; }
        public List<string> Players { get; set; }
        public DateTime ExpectResponseBefore { get; set; }
    }
}
