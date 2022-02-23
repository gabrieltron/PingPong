using System.ComponentModel.DataAnnotations.Schema;

namespace PingPong.Models
{
    [Table("Games")]
    public class Game
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int TeamOneScore { get; set; }
        public Team TeamOne { get; set; }
        public int TeamTwoScore { get; set; }
        public Team TeamTwo { get; set; }
    }
}
