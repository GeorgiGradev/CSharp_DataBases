using P03_FootballBetting.Data.Models.Enums;
using System;

namespace P03_FootballBetting.Data.Models
{
    public class Bet //Mapping Table
    {
        // No need of Composite Key (class has it's own Id)
        public int BetId { get; set; }
        public decimal Amount { get; set; }
        public Prediction Prediction { get; set; }
        public DateTime DateTime { get; set; }


        public int UserId { get; set; }
        public virtual User User { get; set; }

        public int GameId { get; set; }
        public virtual Game Game { get; set; }

    }
}
