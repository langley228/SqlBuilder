using Microsoft.EntityFrameworkCore;
using SqlBuilder.Sample.Samples;

namespace SqlBuilder.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            // PostgreSQL 範例
            NpgsqlSample.Sample();

            // SQL Server 範例
            SqlServerSample.Sample();
        }
    }
}