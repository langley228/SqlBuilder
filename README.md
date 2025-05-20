# SqlBuilder

SqlBuilder 是一套以 C# 撰寫、支援 Entity Framework Core 的 SQL 語句建構器，能以強型別、鏈式語法快速產生 DELETE、UPDATE 等 SQL 指令，並支援多語句組合與參數化查詢。

## 特色

- 強型別 Lambda 表達式，避免魔法字串
- 支援多語句組合（可同時產生多個 UPDATE/DELETE）
- 參數化查詢，防止 SQL Injection
- 可與 EF Core DbContext 無縫整合
- 支援 PostgreSQL（Npgsql）

## 安裝

請將 `SqlBuilder` 及 `SqlBuilder.NpgsqlSql` 專案加入你的解決方案，並參考範例專案 `SqlBuilder.Sample`。

## 快速範例

```csharp
using SqlBuilder.NpgsqlSql;
using SqlBuilder.Sample.Models;

string connectionString = "Host=localhost;Port=5432;Database=sampledb;Username=postgres;Password=yourpassword";
using var db = new SampleDbContext(connectionString);

// 單一 DELETE
var deleteSql = db.SqlRawFoDelete<User>()
                  .Where(u => u.Id == 1)
                  .ToSql();

// 單一 UPDATE
var updateSql = db.SqlRawForUpdate<User>()
                  .Set(u => u.Username, "admin")
                  .Where(u => u.Id == 1)
                  .ToSql();

// 多欄位 UPDATE
var updateMultiSql = db.SqlRawForUpdate<User>()
                       .Set(new { Username = "user2", Password = "pwd2" })
                       .Where(u => u.Id == 2)
                       .ToSql();

// 多語句組合（Update + Delete）
var multiSql = db.SqlRawForUpdate<User>()
                 .Set(u => u.Username, "admin2")
                 .Where(u => u.Id == 5)
                 .SqlRawFoDelete()
                 .Where(u => u.Id == 6)
                 .ToSql();
```

## 執行 SQL

你可以將產生的 SQL 交給 EF Core 執行：

```csharp
db.Database.ExecuteSqlRaw(multiSql);
```

> 注意：多語句執行需資料庫支援，請確認資料庫設定允許多語句。

## 參數化查詢

SqlBuilder 會自動將參數以 `{0}`、`{1}`... 方式產生，並對應參數陣列，防止 SQL Injection。

## 範例專案

請參考 `SqlBuilder.Sample` 目錄下的 `Program.cs`，有完整用法展示。