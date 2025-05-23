using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace SqlBuilder.SqlServer.Tests
{
    /// <summary>
    /// 測試 SqlServerlUpdateBuilder 產生 UPDATE SQL 是否正確。
    /// </summary>
    public class SqlServerUpdateBuilderTests
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
                .UseSqlServer("Server=localhost;Database=test;User Id=sa;Password=pass;TrustServerCertificate=True;")
                .Options;
            using var db = new TestDbContext(options);

            var builder = db.SqlRawForUpdate<User>()
                            .Set(u => u.Username, "abc")
                            .Where(u => u.Id == 2);
            var sql = builder.ToSql();

            string expected =
@"-- @P_0 : abc
-- @P_1 : 2
UPDATE users
 SET user_name=@P_0
 WHERE
(id=@P_1)";

            Assert.That(Normalize(sql), Is.EqualTo(Normalize(expected)));
        }

        /// <summary>
        /// 驗證多欄位 UPDATE SQL 是否正確。
        /// </summary>
        [Test]
        public void SetMulti_ShouldGenerateExpectedUpdateSql()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseSqlServer("Server=localhost;Database=test;User Id=sa;Password=pass;TrustServerCertificate=True;")
                .Options;
            using var db = new TestDbContext(options);

            var builder = db.SqlRawForUpdate<User>()
                            .Set(new { Username = "user2", Id = 5 })
                            .Where(u => u.Id == 3);
            var sql = builder.ToSql();

            string expected =
@"-- @P_0 : user2
-- @P_1 : 5
-- @P_2 : 3
UPDATE users
 SET user_name=@P_0, id=@P_1
 WHERE
(id=@P_2)";

            Assert.That(Normalize(sql), Is.EqualTo(Normalize(expected)));
        }
    }
}