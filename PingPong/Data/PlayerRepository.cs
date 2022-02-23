using Dapper;
using PingPong.Models;

namespace PingPong.Data
{
    public interface IPlayerRepository : ICrudRepository<Player, int> {
        public Task<IEnumerable<PlayerLeaderboardVM>> FindLeaderboard(int nPlayers);
    }

    public class PlayerRepository : IPlayerRepository
    {
        private readonly IDBConnectionFactory _connectionFactory;

        public PlayerRepository(IDBConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Player>> FindAll()
        {
            IEnumerable<Player> players;
            using (var connection = _connectionFactory.GetConnection())
            {
                const string sql = "SELECT * FROM Players";
                players = await connection.QueryAsync<Player>(sql);
            }
            return players;
        }

        public async Task Create(Player player)
        {
            using (var connection = _connectionFactory.GetConnection())
            {
                const string sql = "INSERT INTO Players(Name) values (@Name)";
                await connection.QueryAsync(sql, new { player.Name });
            }
        }

        public async Task<Player> FindOne(int id)
        {
            Player player;
            using (var connection = _connectionFactory.GetConnection())
            {
                const string sql = "SELECT * FROM Players WHERE Id = @Id";
                player = await connection.QueryFirstOrDefaultAsync<Player>(sql, new { id });
            }
            return player;
        }

        public async Task Update(Player player)
        {
            using (var connection = _connectionFactory.GetConnection())
            {
                const string sql = "UPDATE Players SET Name = @Name WHERE Id = @Id";
                await connection.QueryAsync(sql, new { player.Name, player.Id });
            }
        }

        public async Task Delete(int id)
        {
            using (var connection = _connectionFactory.GetConnection())
            {
                const string sql = "DELETE FROM Players WHERE Id = @Id";
                await connection.QueryAsync(sql, new { id });
            }
        }

        public async Task<IEnumerable<PlayerLeaderboardVM>> FindLeaderboard(int nPlayers)
        {
            IEnumerable<PlayerLeaderboardVM> leaderboard;
            using (var connection = _connectionFactory.GetConnection())
            {
                const string sql = @"SELECT leaderboard.*, p.Name
                    FROM (
	                    SELECT player.Id AS Id, SUM(player.Wins) AS Wins, SUM(player.Loses) AS Loses
	                    FROM (
		                    SELECT p.Id,
			                    CASE WHEN (t.Id = g.TeamOneId AND g.TeamOneScore > g.TeamTwoScore)
				                    OR (t.Id = g.TeamTwoId AND g.TeamTwoScore > g.TeamOneScore)
				                    THEN 1
			                    ELSE 0
			                    END AS Wins,
			                    CASE WHEN (t.Id = g.TeamOneId AND g.TeamOneScore < g.TeamTwoScore)
				                    OR (t.Id = g.TeamTwoId AND g.TeamTwoScore < g.TeamOneScore)
				                    THEN 1
			                    ELSE 0
			                    END AS Loses
		                    FROM Players p
		                    INNER JOIN Teams t ON p.Id IN (t.PlayerOneId, t.PlayerTwoId)
		                    INNER JOIN Games g ON t.Id IN (g.TeamOneId, g.TeamTwoId)
	                    ) AS player
						GROUP BY Id
						ORDER BY CAST(SUM(player.Wins) AS float)/(SUM(player.Wins)+SUM(player.Loses)) DESC
                        OFFSET 0 ROWS FETCH NEXT @NPlayers ROWS ONLY
                    ) AS leaderboard
                    INNER JOIN Players p ON p.Id = leaderboard.Id";
                leaderboard = await connection.QueryAsync<PlayerLeaderboardVM>(sql, new { nPlayers });
            }
            return leaderboard;
        }
    }
}
