namespace PingPong.Models
{
    public class TeamLeaderboardVM
    {
        public IEnumerable<LeaderboardVM> SingleTeamLeaderboards { get; set; }
        public IEnumerable<LeaderboardVM> DoubleTeamLeaderboards { get; set; }
    }
}
