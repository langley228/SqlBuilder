using Microsoft.EntityFrameworkCore;

namespace SqlBuilder.Sample.Models
{
    public class SampleDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        private readonly string _connectionString;
        private readonly DbProvider _dbProvider;

        public SampleDbContext(
            DbProvider provider,
            string connectionString)
        {
            _dbProvider = provider;
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            switch (_dbProvider)
            {
                case DbProvider.SqlServer:
                    // SQL Server
                    optionsBuilder.UseSqlServer(_connectionString);
                    break;
                case DbProvider.Npgsql:
                    // PostgreSQL
                    optionsBuilder.UseNpgsql(_connectionString);
                    break;
                default:
                    break;
            }
        }
    }
}
