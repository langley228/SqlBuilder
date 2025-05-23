using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SqlBuilder.NpgsqlSql;
using System.Linq;

namespace SqlBuilder.NpgsqlSql.Tests
{
    /// <summary>
    /// 測試 NpgsqlUpdateBuilder 產生 UPDATE SQL 是否正確。
    /// </summary>
    public class NpgsqlUpdateBuilderTests
    {
        public class User
        {
            [Key]
            [Column("id")]
            public int Id { get; set; }

            [Column("user_name")]
            public string Username { get; set; }
        }

        public class TestDbContext : DbContext
        {
            public DbSet<User> Users { get; set; }
            public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }
        }

        static string Normalize(string s)
        {
            return string.Join('\n', s
                .Replace("\r\n", "\n")
                .Split('\n')
                .Select(line => line.TrimEnd())
            ).ToLower().Trim();
        }

        /// <summary>
        /// 驗證單一欄位 UPDATE SQL 是否正確。
        /// </summary>
        [Test]
        public void Set_ShouldGenerateExpectedUpdateSql()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseNpgsql("Host=localhost;Database=test;Username=postgres;Password=pass")
                .Options;
            using var db = new TestDbContext(options);

            var builder = db.SqlRawForUpdate<User>()
                            .Set(u => u.Username, "abc")
                            .Where(u => u.Id == 2);
            var sql = builder.ToSql();

            string expected =
@"-- {0} : abc
-- {1} : 2
UPDATE users
 SET user_name={0}
 WHERE
(id={1})";

            Assert.That(Normalize(sql), Is.EqualTo(Normalize(expected)));
        }

        /// <summary>
        /// 驗證多欄位 UPDATE SQL 是否正確。
        /// </summary>
        [Test]
        public void SetMulti_ShouldGenerateExpectedUpdateSql()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseNpgsql("Host=localhost;Database=test;Username=postgres;Password=pass")
                .Options;
            using var db = new TestDbContext(options);

            var builder = db.SqlRawForUpdate<User>()
                            .Set(new { Username = "user2", Id = 5 })
                            .Where(u => u.Id == 3);
            var sql = builder.ToSql();

            string expected =
@"-- {0} : user2
-- {1} : 5
-- {2} : 3
UPDATE users
 SET user_name={0}, id={1}
 WHERE
(id={2})";

            Assert.That(Normalize(sql), Is.EqualTo(Normalize(expected)));
        }

        /// <summary>
        /// 驗證欄位累加 UPDATE SQL 是否正確。
        /// </summary>
        [Test]
        public void Inc_ShouldGenerateExpectedUpdateSql()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseNpgsql("Host=localhost;Database=test;Username=postgres;Password=pass")
                .Options;
            using var db = new TestDbContext(options);

            var builder = db.SqlRawForUpdate<User>()
                            .Inc(u => u.Id, 10)
                            .Where(u => u.Username == "test");
            var sql = builder.ToSql();

            string expected =
@"-- {0} : 10
-- {1} : test
UPDATE users
 SET id+({0})
 WHERE
(user_name={1})";

            Assert.That(Normalize(sql), Is.EqualTo(Normalize(expected)));
        }
    }
}