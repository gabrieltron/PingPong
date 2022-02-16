using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PingPong.Models
{
    [Table("Players")]
    public class Player
    {
        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }
    }
}
