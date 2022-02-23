using Dapper;
using PingPong.Models;

namespace PingPong.Data
{
    public interface ITeamRepository : ICrudRepository<Team, int> {
        public Task<IEnumerable<Team>> FindSingleTeams();
        public Task<IEnumerable<Team>> FindDoubleTeams();

        public Task<Team> FindByPlayers(int firstPlayerId, int? secondPlayerId);
    }

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
                const string sql = @"SELECT * FROM Teams t
                    LEFT JOIN Players p1 ON t.PlayerOneId = p1.id
                    LEFT JOIN Players p2 ON t.PlayerTwoId = p2.id";
                teams = await connection.QueryAsync<Team, Player, Player, Team>(sql, DataHelper.RelationshipMapper);
            }
            return teams;
        }

        public async Task<IEnumerable<Team>> FindSingleTeams()
        {
            IEnumerable<Team> teams;
            using (var connection = _connectionFactory.GetConnection())
            {
                const string sql = @"SELECT * FROM Teams t
                    LEFT JOIN Players p1 ON t.PlayerOneId = p1.id
                    LEFT JOIN Players p2 ON t.PlayerTwoId = p2.id
                    WHERE PlayerTwoId IS NULL";
                teams = await connection.QueryAsync<Team, Player, Player, Team>(sql, DataHelper.RelationshipMapper);
            }
            return teams;
        }

        public async Task<IEnumerable<Team>> FindDoubleTeams()
        {
            IEnumerable<Team> teams;
            using (var connection = _connectionFactory.GetConnection())
            {
                const string sql = @"SELECT * FROM Teams t
                    LEFT JOIN Players p1 ON t.PlayerOneId = p1.id
                    LEFT JOIN Players p2 ON t.PlayerTwoId = p2.id
                    WHERE PlayerTwoId IS NOT NULL";
                teams = await connection.QueryAsync<Team, Player, Player, Team>(sql, DataHelper.RelationshipMapper);
            }
            return teams;
        }

        public async Task Create(Team team)
        {
            using (var connection = _connectionFactory.GetConnection())
            {
                const string sql = @"INSERT INTO Teams(Name, PlayerOneId, PlayerTwoId)
                    values (@Name, @PlayerOneId, @PlayerTwoId)";
                await connection.QueryAsync(sql, new { 
                    Name = team.Name, 
                    PlayerOneId = team.PlayerOne.Id, 
                    PlayerTwoId = team.PlayerTwo!.Id 
                });
            }
        }

        public async Task<Team> FindOne(int id)
        {
            Team team;
            using (var connection = _connectionFactory.GetConnection())
            {
                const string sql = @"SELECT * FROM Teams t
                    LEFT JOIN Players p1 ON t.PlayerOneId = p1.id
                    LEFT JOIN Players p2 ON t.PlayerTwoId = p2.id
                    WHERE t.Id = @Id";
                team = (await connection.QueryAsync<Team, Player, Player, Team>(sql, DataHelper.RelationshipMapper,
                    new { id }))?.FirstOrDefault();
            }
            return team;
        }

        public async Task Update(Team team)
        {
            using (var connection = _connectionFactory.GetConnection())
            {
                const string sql = @"UPDATE Teams SET Name = @Name, PlayerOneId = @PlayerOneId, PlayerTwoId = @PlayerTwoId
                    WHERE Id = @Id";
                await connection.QueryAsync(sql, new
                {
                    Name = team.Name,
                    PlayerOneId = team.PlayerOne.Id,
                    PlayerTwoId = team.PlayerTwo!.Id,
                    Id = team.Id
                });
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

        public async Task<Team> FindByPlayers(int firstPlayerId, int? secondPlayerId)
        {
            using (var connection = _connectionFactory.GetConnection())
            {
                string sql;
                if (secondPlayerId == null)
                {
                    sql = @"SELECT * FROM Teams t
                        LEFT JOIN Players p1 ON t.PlayerOneId = p1.id
                        LEFT JOIN Players p2 ON t.PlayerTwoId = p2.id
                        WHERE @FirstPlayerId = PlayerOneId
                        AND PlayerTwoId IS NULL";
                } else
                {
                    sql = @"SELECT * FROM Teams t
                        LEFT JOIN Players p1 ON t.PlayerOneId = p1.id
                        LEFT JOIN Players p2 ON t.PlayerTwoId = p2.id
                        WHERE @FirstPlayerId IN (PlayerOneId, PlayerTwoId) 
                        AND @SecondPlayerId IN (PlayerOneId, PlayerTwoId)";
                }
                return (await connection.QueryAsync<Team, Player, Player, Team>(sql, DataHelper.RelationshipMapper,
                    new { firstPlayerId, secondPlayerId }))?.FirstOrDefault();
            }
        }
    }
}
