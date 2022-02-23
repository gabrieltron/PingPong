using System.ComponentModel.DataAnnotations;

namespace PingPong.Models
{
    public class PlayerLeaderboardVM
    {
        public string Name { get; set; }

        public int Wins {  get; set; }

        public int Loses { get; set; }

        [Display(Name = "Games Played")]
        public int GamesPlayed { get; set; }
    }
}
