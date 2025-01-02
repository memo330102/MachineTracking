using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;

namespace MachineTracking.Infrastructure.Connections.Dapper
{
    public class DbConnections
    {
        private IConfiguration _configuration;

        public DbConnections(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IDbConnection CreateConnection()
        {
            string connectionString = _configuration.GetConnectionString("PostgreSQL");

            return new NpgsqlConnection(connectionString);
        }
    }
}
