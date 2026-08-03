using CeramiQ.Web.Data;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Text.RegularExpressions;

namespace CeramiQ.Web.Services
{
    public class SafeSqlQueryService
    {
        private readonly ApplicationDbContext _context;

        private static readonly string[] ForbiddenKeywords =
        {
            "INSERT",
            "UPDATE",
            "DELETE",
            "DROP",
            "ALTER",
            "CREATE",
            "TRUNCATE",
            "EXEC",
            "EXECUTE",
            "MERGE",
            "GRANT",
            "REVOKE",
            "DENY",
            "BACKUP",
            "RESTORE",
            "DBCC",
            "WAITFOR",
            "OPENROWSET",
            "OPENDATASOURCE",
            "BULK",
            "INTO"
        };

        public SafeSqlQueryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool IsSafeQuery(
            string sql,
            out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(sql))
            {
                errorMessage = "SQL sorgusu boş olamaz.";
                return false;
            }

            string normalizedSql = sql.Trim();

            if (normalizedSql.Length > 5000)
            {
                errorMessage = "SQL sorgusu izin verilen uzunluğu aşıyor.";
                return false;
            }

            if (!normalizedSql.StartsWith(
                    "SELECT",
                    StringComparison.OrdinalIgnoreCase))
            {
                errorMessage =
                    "Güvenlik nedeniyle yalnızca SELECT sorgularına izin verilir.";

                return false;
            }

            if (normalizedSql.Contains(";") ||
                normalizedSql.Contains("--") ||
                normalizedSql.Contains("/*") ||
                normalizedSql.Contains("*/"))
            {
                errorMessage =
                    "Sorguda güvenli olmayan karakterler bulundu.";

                return false;
            }

            foreach (string keyword in ForbiddenKeywords)
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
            ExecuteSelectQueryAsync(
                string sql,
                CancellationToken cancellationToken = default)
        {
            if (!IsSafeQuery(sql, out string errorMessage))
            {
                throw new InvalidOperationException(errorMessage);
            }

            List<string> columns = new();
            List<Dictionary<string, string>> rows = new();

            var connection =
                _context.Database.GetDbConnection();

            bool shouldCloseConnection =
                connection.State != ConnectionState.Open;

            if (shouldCloseConnection)
            {
                await connection.OpenAsync(cancellationToken);
            }

            try
            {
                await using var command =
                    connection.CreateCommand();

                command.CommandText = sql;
                command.CommandType = CommandType.Text;
                command.CommandTimeout = 10;

                await using var reader =
                    await command.ExecuteReaderAsync(
                        cancellationToken);

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    string columnName = reader.GetName(i);

                    if (string.IsNullOrWhiteSpace(columnName))
                    {
                        columnName = $"Column{i + 1}";
                    }

                    string originalName = columnName;
                    int number = 2;

                    while (columns.Contains(
                        columnName,
                        StringComparer.OrdinalIgnoreCase))
                    {
                        columnName =
                            $"{originalName}_{number}";

                        number++;
                    }

                    columns.Add(columnName);
                }

                while (await reader.ReadAsync(
                    cancellationToken))
                {
                    Dictionary<string, string> row = new();

                    for (int i = 0;
                         i < reader.FieldCount;
                         i++)
                    {
                        string value;

                        if (reader.IsDBNull(i))
                        {
                            value = "-";
                        }
                        else if (reader.GetValue(i) is DateTime date)
                        {
                            value = date.ToString(
                                "dd.MM.yyyy HH:mm:ss");
                        }
                        else
                        {
                            value =
                                Convert.ToString(
                                    reader.GetValue(i)) ?? "-";
                        }

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