namespace P03_FootballBetting.Data.Models
{
    public class PlayerStatistic  //Mapping Table
    {
        //TODO: Composite Primary Key
        public int GameId { get; set; }
        public virtual Game Game { get; set; }


        public int PlayerId { get; set; }
        public virtual Player Player { get; set; }


        public int ScoredGoals { get; set; }
        public int Assists { get; set; }
        public int MinutesPlayed { get; set; }
    }
}
