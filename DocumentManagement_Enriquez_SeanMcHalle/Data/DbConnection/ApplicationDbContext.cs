using Microsoft.Data.SqlClient;
using System.Data;

namespace DocumentManagement_Enriquez_SeanMcHalle.Data.DbConnection
{
    public class ApplicationDbContext
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public ApplicationDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection")
            ?? throw new ArgumentNullException(nameof(configuration), "Connection string not found."); ;
        }

        public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
    }
}
