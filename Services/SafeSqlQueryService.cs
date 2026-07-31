using CeramiQ.Web.Data;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Text.RegularExpressions;

namespace CeramiQ.Web.Services
{
    public class SafeSqlQueryService
    {
        private readonly ApplicationDbContext _context;

        public SafeSqlQueryService(ApplicationDbContext context)
        {
            _context = context;
        }
        private readonly string[] forbiddenKeywords =
        {
            "INSERT",
            "UPDATE",
            "DELETE",
            "DROP",
            "ALTER",
            "CREATE",
            "TRUNCATE",
            "EXEC",
            "MERGE"
        };

        public bool IsSafeQuery(string sql, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(sql))
            {
                errorMessage = "SQL sorgusu boş olamaz.";
                return false;
            }

            string normalizedSql = sql.Trim();

            if (!normalizedSql.StartsWith(
                    "SELECT",
                    StringComparison.OrdinalIgnoreCase))
            {
                errorMessage = "Yalnızca SELECT sorgularına izin verilir.";
                return false;
            }

            if (normalizedSql.Contains(";") ||
                normalizedSql.Contains("--") ||
                normalizedSql.Contains("/*"))
            {
                errorMessage = "Sorguda güvenli olmayan karakterler bulundu.";
                return false;
            }

            foreach (string keyword in forbiddenKeywords)
            {
                bool containsKeyword = Regex.IsMatch(
                    normalizedSql,
                    $@"\b{keyword}\b",
                    RegexOptions.IgnoreCase);

                if (containsKeyword)
                {
                    errorMessage =
                        $"{keyword} komutunun kullanılmasına izin verilmez.";

                    return false;
                }
            }

            return true;
        }
        public async Task<(
    List<string> Columns,
    List<Dictionary<string, string>> Rows)>
    ExecuteSelectQueryAsync(string sql)
        {
            if (!IsSafeQuery(sql, out string errorMessage))
            {
                throw new InvalidOperationException(errorMessage);
            }

            List<string> columns = new();
            List<Dictionary<string, string>> rows = new();

            var connection = _context.Database.GetDbConnection();

            bool shouldCloseConnection =
                connection.State != ConnectionState.Open;

            if (shouldCloseConnection)
            {
                await connection.OpenAsync();
            }

            try
            {
                await using var command = connection.CreateCommand();

                command.CommandText = sql;
                command.CommandTimeout = 10;

                await using var reader =
                    await command.ExecuteReaderAsync();

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    columns.Add(reader.GetName(i));
                }

                while (await reader.ReadAsync())
                {
                    Dictionary<string, string> row = new();

                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        string value = reader.IsDBNull(i)
                            ? "-"
                            : Convert.ToString(reader.GetValue(i)) ?? "-";

                        row[columns[i]] = value;
                    }

                    rows.Add(row);
                }
            }
            finally
            {
                if (shouldCloseConnection)
                {
                    await connection.CloseAsync();
                }
            }

            return (columns, rows);
        }
    }
}