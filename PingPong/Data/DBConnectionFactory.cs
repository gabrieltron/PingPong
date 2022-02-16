
using System.Data;
using System.Data.SqlClient;

namespace PingPong.Data
{
    public interface IDBConnectionFactory
    {
        IDbConnection GetConnection();
    }

    public class DBConnectionFactory : IDBConnectionFactory
    {
        private readonly IConfiguration _config;

        public DBConnectionFactory(IConfiguration config)
        {
            _config = config;
        }

        public IDbConnection GetConnection()
        {
            return new SqlConnection(_config.GetConnectionString("PingPongContext"));
        }
    }
}
