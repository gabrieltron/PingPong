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

        public async Task<IEnumerable<Player>> GetAll()
        {
            IEnumerable<Player> players;
            using (var connection = _connectionFactory.GetConnection())
            {
                const string sql = "SELECT * FROM Players";
                players = await connection.QueryAsync<Player>(sql);
            }
            return players;
        }

        public async Task Add(Player player)
        {
            IEnumerable<Player> players;
            using (var connection = _connectionFactory.GetConnection())
            {
                const string sql = "INSERT INTO Players(Name) values (@Name)";
                await connection.QueryAsync(sql, new { player.Name });
            }
        }
    }
}
