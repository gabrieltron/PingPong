using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PingPong.Models
{
    [Table("Teams")]
    public class Team
    {
        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }

        [Required]
        public Player PlayerOne { get; set; }

        public Player? PlayerTwo { get; set;  }
    }
}
