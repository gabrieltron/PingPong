using Dapper;
using PingPong.Models;

namespace PingPong.Data
{
    public interface ITeamRepository : ICrudRepository<Team, int> {}

    public class TeamRepository : ITeamRepository
    {
        private readonly IDBConnectionFactory _connectionFactory;

        public TeamRepository(IDBConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Team>> FindAll()
        {
            IEnumerable<Team> teams;
            using (var connection = _connectionFactory.GetConnection())
            {
                const string sql = "SELECT * FROM Teams";
                teams = await connection.QueryAsync<Team>(sql);
            }
            return teams;
        }

        public async Task Create(Team team)
        {
            using (var connection = _connectionFactory.GetConnection())
            {
                const string sql = @"INSERT INTO Teams(Name, PlayerOneId, PlayerTwoId)
                    values (@Name, @PlayerOneId, @PlayerTwoId)";
                await connection.QueryAsync(sql, new { team.Name, team.PlayerOneId, team.PlayerTwoId });
            }
        }

        public async Task<Team> FindOne(int id)
        {
            Team team;
            using (var connection = _connectionFactory.GetConnection())
            {
                const string sql = "SELECT * FROM Teams WHERE Id = @Id";
                team = await connection.QueryFirstOrDefaultAsync<Team>(sql, new { id });
            }
            return team;
        }

        public async Task Update(Team team)
        {
            using (var connection = _connectionFactory.GetConnection())
            {
                const string sql = @"UPDATE Teams SET Name = @Name, PlayerOneId = @PlayerOneId, PlayerTwoId = @PlayerTwoId
                    WHERE Id = @Id";
                await connection.QueryAsync(sql, new { team.Name, team.PlayerOneId, team.PlayerTwoId, team.Id });
            }
        }

        public async Task Delete(int id)
        {
            using (var connection = _connectionFactory.GetConnection())
            {
                const string sql = "DELETE FROM Teams WHERE Id = @Id";
                await connection.QueryAsync(sql, new { id });
            }
        }

    }
}
