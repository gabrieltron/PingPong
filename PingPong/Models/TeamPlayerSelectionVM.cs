using Microsoft.AspNetCore.Mvc.Rendering;
using PingPong.Models.Validators;
using System.ComponentModel.DataAnnotations;

namespace PingPong.Models
{
    public class TeamPlayerSelectionVM
    {
        public int? TeamId { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string? TeamName { get; set; }

        public IEnumerable<Player>? Players { get; set; }

        public uint NPlayers { get; set; }

        [Required(ErrorMessage = "Select a player")]
        public int SelectedPlayerOneId { get; set; }

        [RequiredIfTwoPlayers(ErrorMessage = "Select a player")]
        [NotEquals(nameof(SelectedPlayerOneId), ErrorMessage = "Select different players")]
        public int? SelectedPlayerTwoId { get; set; }

        public class RequiredIfTwoPlayers : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                var nPlayersProperty = validationContext.ObjectType.GetProperty(nameof(TeamPlayerSelectionVM.NPlayers));
                var nPlayers =(uint)nPlayersProperty.GetValue(validationContext.ObjectInstance, null);
                if (nPlayers > 1 && value == null)
                {
                    return new ValidationResult(ErrorMessage);
                }

                return ValidationResult.Success;
            }
        }
    }
}
