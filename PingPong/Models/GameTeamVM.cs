using System.ComponentModel.DataAnnotations;

namespace PingPong.Models
{
    public class GameTeamVM
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }

        [Display(Name = "Team One")]
        public Team TeamOne { get; set; }
        [Display(Name = "Team One Score")]
        public int TeamOneScore { get; set; }

        [Display(Name = "Team Two")]
        public Team TeamTwo { get; set; }
        [Display(Name = "Team Two Score")]
        public int TeamTwoScore { get; set; } 
    }
}
