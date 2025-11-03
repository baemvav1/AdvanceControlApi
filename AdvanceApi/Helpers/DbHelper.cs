using System;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace AdvanceApi.Helpers
{
    public class DbHelper
    {

        private readonly string _connectionString;

        public DbHelper(IConfiguration configuration)
        {
            _connectionString = Environment.GetEnvironmentVariable("DefaultConnection") ??
                                configuration.GetConnectionString("DefaultConnection") ??
                                throw new ArgumentNullException(nameof(configuration));
        }

        public SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
