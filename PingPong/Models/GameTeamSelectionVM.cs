using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace PingPong.Models
{
    public class GameTeamSelectionVM
    {
        public int? GameId { get; set; }

        [Required]
        public DateTime Date{ get; set; }

        public SelectList? Teams{ get; set; }

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
