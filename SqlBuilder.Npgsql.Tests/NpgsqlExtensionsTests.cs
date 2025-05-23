using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SqlBuilder.NpgsqlSql.Tests
{
    /// <summary>
    /// 測試 SqlBuilder.NpgsqlSql 擴充方法產生 SQL 是否正確。
    /// </summary>
    public class NpgsqlSqlExtensionsTests
    {
        /// <summary>
        /// 測試用 User 實體，對應資料表 users。
        /// </summary>
        public class User
        {
            [Key]
            [Column("id")]
            public int Id { get; set; }

            [Column("user_name")]
            public string Username { get; set; }
        }

        /// <summary>
        /// 測試用 DbContext。
        /// </summary>
        public class TestDbContext : DbContext
        {
            public DbSet<User> Users { get; set; }
            public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }
        }

        /// <summary>
        /// 驗證 SqlRawFoDelete 產生的 DELETE SQL 是否包含正確語法與參數。
        /// </summary>
        [Test]
        public void SqlRawFoDelete_ShouldGenerateDeleteSql()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseNpgsql("Host=localhost;Database=test;Username=postgres;Password=pass")
                .Options;
            using var db = new TestDbContext(options);

            var builder = db.SqlRawFoDelete<User>().Where(u => u.Id == 1);
            var sql = builder.ToSql();

            Assert.That(sql, Does.Contain("DELETE FROM"));
            Assert.That(sql, Does.Contain("id={0}"));
        }

        /// <summary>
        /// 驗證 SqlRawForUpdate 產生的 UPDATE SQL 是否包含正確語法與參數。
        /// </summary>
        [Test]
        public void SqlRawForUpdate_ShouldGenerateUpdateSql()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseNpgsql("Host=localhost;Database=test;Username=postgres;Password=pass")
                .Options;
            using var db = new TestDbContext(options);

            var builder = db.SqlRawForUpdate<User>()
                            .Set(u => u.Username, "admin")
                            .Where(u => u.Id == 1);
            var sql = builder.ToSql();

            Assert.That(sql, Does.Contain("UPDATE"));
            Assert.That(sql, Does.Contain("SET user_name={0}"));
            Assert.That(sql, Does.Contain("id={1}"));
        }
    }
}