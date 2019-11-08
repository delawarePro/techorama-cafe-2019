using System;

namespace GameDay.Models
{
    public class CreateScoreBoardRequest
    {
        public Guid GameId { get; set; }

        public bool IsGameOn { get; set; }
    }
}
