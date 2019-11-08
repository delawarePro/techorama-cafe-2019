using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace GameDay.Models
{
    public class ScoreBoardEntity : TableEntity
    {
        public ScoreBoardEntity()
        {

        }
        public ScoreBoardEntity(Guid gameId)
        {
            PartitionKey = "ScoreBoard";
            RowKey = gameId.ToString();
        }

        public int HomeScore { get; set; }

        public int VisitorsScore { get; set; }
    }
}
