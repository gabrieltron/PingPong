using Dapper;
using PingPong.Models;

namespace PingPong.Data
{
    public class PlayerRepository
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
    }
}
