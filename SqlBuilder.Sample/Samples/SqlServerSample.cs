using SqlBuilder.Sample.Models;
using System;
namespace SqlBuilder.Sample.Samples
{
    using SqlBuilder.SqlServer;

    class SqlServerSample
    {
        public static void Sample()
        {
            string connectionString = "Server=localhost;Database=sampledb;User Id=sa;Password=yourpassword;TrustServerCertificate=True;";
            using var db = new SampleDbContext(DbProvider.Npgsql, connectionString);

            // 建立 DELETE SQL
            var deleteSql = db.SqlRawFoDelete<User>()
                              .Where(u => u.Id == 1)
                              .ToSql();
            Console.WriteLine("Delete SQL:");
            Console.WriteLine(deleteSql);

            // 建立單欄位 UPDATE SQL
            var updateSql = db.SqlRawForUpdate<User>()
                              .Set(u => u.Username, "admin")
                              .Where(u => u.Id == 1)
                              .ToSql();
            Console.WriteLine("Update SQL:");
            Console.WriteLine(updateSql);

            // 建立多欄位 UPDATE SQL
            var updateMultiSql = db.SqlRawForUpdate<User>()
                                   .Set(new { Username = "user2", Password = "pwd2" })
                                   .Where(u => u.Id == 2)
                                   .ToSql();
            Console.WriteLine("Update Multi SQL:");
            Console.WriteLine(updateMultiSql);

            // 建立條件 UPDATE SQL
            var updateWhereSql = db.SqlRawForUpdate<User>()
                                   .Set(u => u.Password, "newpass")
                                   .Where(u => u.Username == "admin" && u.Id > 10)
                                   .ToSql();
            Console.WriteLine("Update Where SQL:");
            Console.WriteLine(updateWhereSql);

            // 多組合：多次 Set、Inc 串接
            var updateComboSql = db.SqlRawForUpdate<User>()
                                   .Set(u => u.Username, "multi")
                                   .Set(u => u.Password, "multiPwd")
                                   .Inc(u => u.Id, 5)
                                   .Where(u => u.Username != "admin")
                                   .Where(u => u.Id > 100)
                                   .ToSql();
            Console.WriteLine("Update Combo SQL:");
            Console.WriteLine(updateComboSql);

            // 多語句組合：同一 Builder 產生多個 SQL
            var multiSql = db.SqlRawForUpdate<User>()
                             .Set(u => u.Username, "admin2")
                             .Where(u => u.Id == 5)
                             .SqlRawFoDelete()
                             .Where(u => u.Id == 6)
                             .ToSql();
            Console.WriteLine("Multi SQL (Update + Delete):");
            Console.WriteLine(multiSql);
            var executeSql = db.SqlRawForUpdate<User>()
                              .Set(u => u.Username, "admin1")
                              .Where(u => u.Id == 1);
            executeSql.ExecuteSqlRaw();
            executeSql = db.SqlRawForUpdate<User>()
                              .Set(u => u.Username, "admin2")
                              .Where(u => u.Id == 1);
            executeSql.ExecuteSqlRaw();
        }
    }
}