# SqlBuilder

SqlBuilder 是一套以 C# 撰寫、支援 Entity Framework Core 的 SQL 語句建構器，能以強型別、鏈式語法快速產生 DELETE、UPDATE 等 SQL 指令，並支援多語句組合與參數化查詢。

## 特色

- 強型別 Lambda 表達式，避免魔法字串
- 支援多語句組合（可同時產生多個 UPDATE/DELETE）
- 參數化查詢，防止 SQL Injection
- 可與 EF Core DbContext 無縫整合
- 支援 PostgreSQL（需搭配 SqlBuilder.NpgsqlSql 專案）
- 支援 SQL Server（需搭配 SqlBuilder.SqlServerSql 專案）

## 安裝

請將 `SqlBuilder` 及 `SqlBuilder.NpgsqlSql` 或 `SqlBuilder.SqlServerSql` 專案加入你的解決方案，並參考範例專案 `SqlBuilder.Sample`。

## 快速範例

```csharp
using SqlBuilder.NpgsqlSql;
using SqlBuilder.SqlServerSql;
using SqlBuilder.Sample.Models;

// PostgreSQL 範例
var pgDb = new SampleDbContext(DbProvider.Npgsql, "Host=localhost;Port=5432;Database=sampledb;Username=postgres;Password=yourpassword");

// 執行 DELETE
pgDb.SqlRawFoDelete<User>()
    .Where(u => u.Id == 1)
    .ExecuteSqlRaw();
// 產生 SQL：
// -- {0} : 1
// DELETE FROM users
//  WHERE
// (id={0})

// 執行 UPDATE
pgDb.SqlRawForUpdate<User>()
    .Set(u => u.Username, "admin")
    .Where(u => u.Id == 1)
    .ExecuteSqlRaw();
// 產生 SQL：
// -- {0} : admin
// -- {1} : 1
// UPDATE users
//  SET username={0}
//  WHERE
// (id={1})

// 執行多欄位 UPDATE
pgDb.SqlRawForUpdate<User>()
    .Set(new { Username = "user2", Password = "pwd2" })
    .Where(u => u.Id == 2)
    .ExecuteSqlRaw();
// 產生 SQL：
// -- {0} : user2
// -- {1} : pwd2
// -- {2} : 2
// UPDATE users
//  SET username={0}, password={1}
//  WHERE
// (id={2})

// 多語句組合（Update + Delete）
pgDb.SqlRawForUpdate<User>()
    .Set(u => u.Username, "admin2")
    .Where(u => u.Id == 5)
    .SqlRawFoDelete()
    .Where(u => u.Id == 6)
    .ExecuteSqlRaw();
// 產生 SQL：
// -- {0} : admin2
// -- {1} : 5
// -- {2} : 6
// UPDATE users
//  SET username={0}
//  WHERE
// (id={1})
// ;
// DELETE FROM users
//  WHERE
// (id={2})

// SQL Server 範例
var sqlDb = new SampleDbContext(DbProvider.SqlServer, "Server=localhost;Database=sampledb;User Id=sa;Password=yourpassword;TrustServerCertificate=True;");

// 執行 DELETE
sqlDb.SqlServerRawFoDelete<User>()
      .Where(u => u.Id == 1)
      .ExecuteSqlRaw();
// 產生 SQL：
// -- {0} : 1
// DELETE FROM users
//  WHERE
// (id=@P_0)

// 執行 UPDATE
sqlDb.SqlServerRawForUpdate<User>()
      .Set(u => u.Username, "admin")
      .Where(u => u.Id == 1)
      .ExecuteSqlRaw();
// 產生 SQL：
// -- {0} : admin
// -- {1} : 1
// UPDATE users
//  SET username=@P_0
//  WHERE
// (id=@P_1)

// 執行多欄位 UPDATE
sqlDb.SqlServerRawForUpdate<User>()
      .Set(new { Username = "user2", Password = "pwd2" })
      .Where(u => u.Id == 2)
      .ExecuteSqlRaw();
// 產生 SQL：
// -- {0} : user2
// -- {1} : pwd2
// -- {2} : 2
// UPDATE users
//  SET username=@P_0, password=@P_1
//  WHERE
// (id=@P_2)

// 多語句組合（Update + Delete）
sqlDb.SqlServerRawForUpdate<User>()
      .Set(u => u.Username, "admin2")
      .Where(u => u.Id == 5)
      .SqlRawFoDelete()
      .Where(u => u.Id == 6)
      .ExecuteSqlRaw();
// 產生 SQL：
// -- {0} : admin2
// -- {1} : 5
// -- {2} : 6
// UPDATE users
//  SET username=@P_0
//  WHERE
// (id=@P_1)
// ;
// DELETE FROM users
//  WHERE
// (id=@P_2)
```

## 執行 SQL

你可以直接呼叫 `ExecuteSqlRaw()` 或 `ExecuteSqlRawAsync()` 執行 SQL，無需手動取得 SQL 字串。

> 注意：多語句執行需資料庫支援，請確認資料庫設定允許多語句。

## 參數化查詢

SqlBuilder 會自動將參數以 `{0}`、`{1}`... 或 `@P_0`、`@P_1` 方式產生，並對應參數陣列，防止 SQL Injection。

## 範例專案

請參考 `SqlBuilder.Sample` 目錄下的 [`Program.cs`](SqlBuilder.Sample/Program.cs) 及 `Samples` 目錄，有完整用法展示。