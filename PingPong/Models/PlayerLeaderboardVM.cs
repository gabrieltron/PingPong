using System.ComponentModel.DataAnnotations;

namespace PingPong.Models
{
    public class PlayerLeaderboardVM
    {
        public string Name { get; set; }

        [Display(Name = "Win Ratio")]
        public int WinRatio { get; set; }

        [Display(Name = "Games Played")]
        public int GamesPlayed { get; set; }
    }
}
