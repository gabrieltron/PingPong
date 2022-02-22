using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace PingPong.Models
{
    public class GameTeamSelectionVM
    {
        public int? GameId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public uint TeamSize { get; set; }

        public SelectList SingleTeams { get; set; }

        public SelectList DoubleTeams { get; set; }

        [Required]
        public int SelectedTeamOneId { get; set; }
        [Required]
        [Display(Name = "Team One Score")]
        public int TeamOneScore { get; set; }

        [Required]
        public int SelectedTeamTwoId { get; set; }
        [Required]
        [Display(Name = "Team Two Score")]
        public int TeamTwoScore { get; set; }
    }
}
