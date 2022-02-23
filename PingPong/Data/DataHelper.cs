using PingPong.Models;

namespace PingPong.Data
{
    public class DataHelper
    {
        private DataHelper() {}

        public static Team RelationshipMapper(Team team, Player playerOne, Player playerTwo)
        {
            team.PlayerOne = playerOne;
            team.PlayerTwo = playerTwo;
            return team;
        }

        public static Game RelationshipMapper(Game game, Team teamOne, Player teamOnePlayerOne, Player teamOnePlayerTwo,
            Team teamTwo, Player teamTwoPlayerOne, Player teamTwoPlayerTwo)
        {
            var updatedTeamOne = RelationshipMapper(teamOne, teamOnePlayerOne, teamOnePlayerTwo);
            var updatedTeamTwo = RelationshipMapper(teamTwo, teamTwoPlayerOne, teamTwoPlayerTwo);
            game.TeamOne = updatedTeamOne;
            game.TeamTwo = updatedTeamTwo;
            return game;
        }
    }
}
