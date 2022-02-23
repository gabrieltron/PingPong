using System.ComponentModel.DataAnnotations;

namespace PingPong.Models
{
    public class TeamDetailsVM
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [Display(Name = "Player One")]
        public Player PlayerOne { get; set; }
        [Display(Name = "Player Two")]
        public Player PlayerTwo { get; set; }

        [Display(Name = "Last Games")]
        public IEnumerable<Game> LastGames { get; set; }
    }
}
