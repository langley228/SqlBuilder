using Microsoft.EntityFrameworkCore;

namespace SqlBuilder.Sample.Models
{
    public class SampleDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        private readonly string _connectionString;

        public SampleDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_connectionString.Contains("Host=") || _connectionString.Contains("Port="))
            {
                // PostgreSQL
                optionsBuilder.UseNpgsql(_connectionString);
            }
            else
            {
                // SQL Server
                optionsBuilder.UseSqlServer(_connectionString);
            }
        }
    }
}
