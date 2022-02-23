using System.ComponentModel.DataAnnotations;

namespace PingPong.Models
{
    public class TeamIndexVM
    {
        public int Id { get; set; }

        public string Name { get; set; }

        [Display(Name = "Player One")]
        public Player PlayerOne { get; set; }

        [Display(Name = "Player Two")]
        public Player? PlayerTwo { get; set; }

        public int Wins { get; set; }

        [Display(Name = "Played Games")]
        public int PlayedGames { get; set; }
    }
}
