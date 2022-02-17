using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace PingPong.Models
{
    public class TeamPlayerSelectionVM
    {
        public int? TeamId { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string? TeamName { get; set; }

        public SelectList? Players { get; set; }

        [Required]
        public int SelectedPlayerOneId { get; set; }
        public int? SelectedPlayerTwoId { get; set; }
    }
}
