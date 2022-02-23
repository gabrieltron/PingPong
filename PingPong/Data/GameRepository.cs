using Dapper;
using PingPong.Models;

namespace PingPong.Data
{
    public interface IGameRepository : ICrudRepository<Game, int> {
        public Task<IEnumerable<Game>> FindLastGamesFromPlayers(int playerId, int nLastGames);
    }

    public class GameRepository : IGameRepository
    {
        private readonly IDBConnectionFactory _connectionFactory;

        public GameRepository(IDBConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }


        public async Task<IEnumerable<Game>> FindAll()
        {
            IEnumerable<Game> teams;
            using (var connection = _connectionFactory.GetConnection())
            {
                const string sql = @"SELECT * FROM Games g
                    LEFT JOIN Teams t1 ON g.TeamOneId = t1.Id
                    LEFT JOIN Players t1p1 ON t1.PlayerOneId = t1p1.Id
                    LEFT JOIN Players t1p2 ON t1.PlayerTwoId = t1p2.Id
                    LEFT JOIN Teams t2 ON g.TeamTwoId = t2.Id
                    LEFT JOIN Players t2p1 ON t2.PlayerOneId = t2p1.Id
                    LEFT JOIN Players t2p2 ON t2.PlayerTwoId = t2p2.Id";
                teams = await connection.QueryAsync<Game, Team, Player, Player, Team, Player, Player, Game>(sql, DataHelper.RelationshipMapper);
            }
            return teams;
        }

        public async Task Create(Game game)
        {
            using (var connection = _connectionFactory.GetConnection())
            {
                const string sql = @"INSERT INTO Games(Date, TeamOneId, TeamOneScore, TeamTwoId, TeamTwoScore)
                    values (@Date, @TeamOneId, @TeamOneScore, @TeamTwoId, @TeamTwoScore)";
                await connection.QueryAsync(sql, new {
                    Date = game.Date, 
                    TeamOneId = game.TeamOne.Id, 
                    TeamOneScore = game.TeamOneScore, 
                    TeamTwoId = game.TeamTwo.Id, 
                    TeamTwoScore = game.TeamTwoScore 
                });
            }
        }

        public async Task<Game> FindOne(int id)
        {
            Game game;
            using (var connection = _connectionFactory.GetConnection())
            {
                const string sql = @"SELECT * FROM Games g
                    LEFT JOIN Teams t1 ON g.TeamOneId = t1.Id
                    LEFT JOIN Players t1p1 ON t1.PlayerOneId = t1p1.Id
                    LEFT JOIN Players t1p2 ON t1.PlayerTwoId = t1p2.Id
                    LEFT JOIN Teams t2 ON g.TeamTwoId = t2.Id
                    LEFT JOIN Players t2p1 ON t2.PlayerOneId = t2p1.Id
                    LEFT JOIN Players t2p2 ON t2.PlayerTwoId = t2p2.Id
                    WHERE g.Id = @Id";
                game = (await connection.QueryAsync<Game, Team, Player, Player, Team, Player, Player, Game>(sql, DataHelper.RelationshipMapper,
                    new { id }))?.FirstOrDefault();
            }
            return game;
        }

        public async Task Delete(int id)
        {
            using (var connection = _connectionFactory.GetConnection())
            {
                const string sql = "DELETE FROM Games WHERE Id = @Id";
                await connection.QueryAsync(sql, new { id });
            }
        }

        public async Task<IEnumerable<Game>> FindLastGamesFromPlayers(int playerId, int nLastGames)
        {
            IEnumerable<Game> teams;
            using (var connection = _connectionFactory.GetConnection())
            {
                const string sql = @"SELECT TOP @NLastGames * FROM Games g
                    LEFT JOIN Teams t1 ON g.TeamOneId = t1.Id
                    LEFT JOIN Players t1p1 ON t1.PlayerOneId = t1p1.Id
                    LEFT JOIN Players t1p2 ON t1.PlayerTwoId = t1p2.Id
                    LEFT JOIN Teams t2 ON g.TeamTwoId = t2.Id
                    LEFT JOIN Players t2p1 ON t2.PlayerOneId = t2p1.Id
                    LEFT JOIN Players t2p2 ON t2.PlayerTwoId = t2p2.Id
                    WHERE @PlayerId IN (t1p1.Id, t1p2.Id, t2p1.Id, t2p2.Id)
                    ORDER BY g.Date DESC";
                teams = await connection.QueryAsync<Game, Team, Player, Player, Team, Player, Player, Game>(sql, DataHelper.RelationshipMapper,
                    new { playerId, nLastGames });
            }
            return teams;
        }


        public Task Update(Game model)
        {
            throw new NotImplementedException();
        }
    }
}
