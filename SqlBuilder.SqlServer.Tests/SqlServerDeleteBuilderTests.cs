using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace SqlBuilder.SqlServer.Tests
{
    /// <summary>
    /// 測試 SqlServerDeleteBuilder 產生 DELETE SQL 是否正確。
    /// </summary>
    public class SqlServerDeleteBuilderTests
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
        /// 驗證透過 DbContext 擴充方法產生的 DELETE SQL 是否正確。
        /// </summary>
        [Test]
        public void Where_ShouldGenerateExpectedDeleteSql_ByExtension()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseSqlServer("Server=localhost;Database=test;User Id=sa;Password=pass;TrustServerCertificate=True;")
                .Options;
            using var db = new TestDbContext(options);

            // 透過 DbContext 擴充方法取得 builder
            var execBuilder = db.SqlRawFoDelete<User>().Where(u => u.Id == 1);
            var sql = execBuilder.ToSql();

            string expected =
@"-- @P_0 : 1
DELETE FROM users
 WHERE
(id=@P_0)";

            Assert.That(Normalize(sql), Is.EqualTo(Normalize(expected)));
        }

        /// <summary>
        /// 驗證多條件 DELETE SQL 是否正確。
        /// </summary>
        [Test]
        public void Where_ShouldGenerateExpectedDeleteSql_WithMultipleConditions()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseSqlServer("Server=localhost;Database=test;User Id=sa;Password=pass;TrustServerCertificate=True;")
                .Options;
            using var db = new TestDbContext(options);

            var execBuilder = db.SqlRawFoDelete<User>().Where(u => u.Id > 1 && u.Username == "test");
            var sql = execBuilder.ToSql();

            string expected =
@"-- @P_0 : 1
-- @P_1 : test
DELETE FROM users
 WHERE
((id>@P_0) AND (user_name=@P_1))";

            Assert.That(Normalize(sql), Is.EqualTo(Normalize(expected)));
        }

        /// <summary>
        /// 驗證沒有 Where 條件時產生的 DELETE SQL。
        /// </summary>
        [Test]
        public void Where_ShouldGenerateDeleteSql_WithoutCondition()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseSqlServer("Server=localhost;Database=test;User Id=sa;Password=pass;TrustServerCertificate=True;")
                .Options;
            using var db = new TestDbContext(options);

            var builder = db.SqlRawFoDelete<User>();
            var sql = builder.ToSql();

            string expected =
@"DELETE FROM users";

            Assert.That(Normalize(sql), Does.Contain(Normalize(expected)));
        }
    }
}