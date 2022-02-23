namespace PingPong.Models
{
    public class PlayerDetailsVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<Game> LastGames { get; set; }
    }
}
