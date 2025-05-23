using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SqlBuilder.Npgsql;
using System.Linq;

namespace SqlBuilder.Npgsql.Tests
{
    /// <summary>
    /// 測試 AbstractNpgsqlBuilder 產生 SQL 是否正確。
    /// </summary>
    public class AbstractNpgsqlBuilderTests
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
        /// 標準化 SQL 字串，移除行尾空白、轉小寫、統一換行，避免格式差異造成測試失敗。
        /// </summary>
        static string Normalize(string s)
        {
            return string.Join('\n', s
                .Replace("\r\n", "\n")
                .Split('\n')
                .Select(line => line.TrimEnd())
            ).ToLower().Trim();
        }

        /// <summary>
        /// 驗證 DELETE SQL 產生結果。
        /// </summary>
        [Test]
        public void ToSql_ShouldGenerateExpectedDeleteSql()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseNpgsql("Host=localhost;Database=test;Username=postgres;Password=pass")
                .Options;
            using var db = new TestDbContext(options);

            var builder = db.SqlRawFoDelete<User>().Where(u => u.Id == 1);
            var sql = builder.ToSql();

            string expected =
@"-- {0} : 1
DELETE FROM users
 WHERE
(id={0})";

            Assert.That(Normalize(sql), Is.EqualTo(Normalize(expected)));
        }

        /// <summary>
        /// 驗證單欄位 UPDATE SQL 產生結果。
        /// </summary>
        [Test]
        public void ToSql_ShouldGenerateExpectedUpdateSql()
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
        /// 驗證多欄位 UPDATE SQL 產生結果。
        /// </summary>
        [Test]
        public void ToSql_ShouldGenerateExpectedMultiUpdateSql()
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
    }
}