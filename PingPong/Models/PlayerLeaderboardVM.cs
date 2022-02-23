using System.ComponentModel.DataAnnotations;

namespace PingPong.Models
{
    public class PlayerLeaderboardVM
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int Wins {  get; set; }

        public int Loses { get; set; }
    }
}
