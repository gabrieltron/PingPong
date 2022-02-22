using Microsoft.AspNetCore.Mvc.Rendering;
using PingPong.Models.Validators;
using System.ComponentModel.DataAnnotations;

namespace PingPong.Models
{
    public class GameTeamSelectionVM
    {
        [Required]
        public DateTime Date { get; set; }

        public uint TeamsSize { get; set; }

        public IEnumerable<Team>? SingleTeams { get; set; }

        public IEnumerable<Team>? DoubleTeams { get; set; }

        [Required(ErrorMessage = "Select a team")]
        public int SelectedTeamOneId { get; set; }

        [Required]
        [Display(Name = "Team One Score")]
        public int TeamOneScore { get; set; }

        [Required(ErrorMessage = "Select a team")]
        [NotEquals(nameof(SelectedTeamOneId), ErrorMessage = "Select different teams")]
        public int SelectedTeamTwoId { get; set; }
        [Required]
        [Display(Name = "Team Two Score")]
        public int TeamTwoScore { get; set; }
    }
}
