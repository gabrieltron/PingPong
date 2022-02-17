using Dapper;
using PingPong.Models;

namespace PingPong.Data
{
    public class GameRepository
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
                const string sql = "SELECT * FROM Games";
                teams = await connection.QueryAsync<Game>(sql);
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
                    game.Date, 
                    game.TeamOneId, 
                    game.TeamOneScore, 
                    game.TeamTwoId, 
                    game.TeamTwoScore 
                });
            }
        }

        public async Task<Game> FindOne(int id)
        {
            Game game;
            using (var connection = _connectionFactory.GetConnection())
            {
                const string sql = "SELECT * FROM Games WHERE Id = @Id";
                game = await connection.QueryFirstOrDefaultAsync<Game>(sql, new { id });
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
    }
}
