using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using SetiMarine.Infrastructure.Data;

namespace SetiMarine.Infrastructure.Services;

public static class AutoSchemaService
{
    public static void Sync(AppDbContext db)
    {
        try
        {
            if (!db.Database.CanConnect())
            {
                Console.WriteLine("[AutoSchemaService] Erro: Nao foi possivel conectar ao banco.");
                return;
            }

            foreach (var entity in db.Model.GetEntityTypes())
            {
                var tableName = entity.GetTableName();
                if (string.IsNullOrWhiteSpace(tableName))
                    continue;

                EnsureTable(db, tableName, entity);

                var storeObject = StoreObjectIdentifier.Table(tableName, null);

                foreach (var prop in entity.GetProperties())
                {
                    if (prop.IsShadowProperty())
                        continue;

                    var columnName = prop.GetColumnName(storeObject);
                    if (string.IsNullOrWhiteSpace(columnName))
                        continue;

                    var columnType = ResolveType(prop.ClrType);
                    EnsureColumn(db, tableName, columnName, columnType);
                }

                foreach (var index in entity.GetIndexes().Where(i => i.IsUnique))
                {
                    var cols = index.Properties
                        .Select(p => p.GetColumnName(storeObject))
                        .Where(c => !string.IsNullOrWhiteSpace(c))
                        .ToList();

                    if (cols.Count > 0)
                        EnsureUniqueIndex(db, tableName, cols!);
                }
            }

            Console.WriteLine("[AutoSchemaService] Sincronizacao concluida.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AutoSchemaService] ERRO: {ex.Message}");
        }
    }

    private static void EnsureTable(AppDbContext db, string table, IEntityType entity)
    {
        try
        {
            var safeTable = QuoteIdentifier(table);
            var pkProp    = entity.FindPrimaryKey()?.Properties.FirstOrDefault();
            var pkColumn  = pkProp?.GetColumnName() ?? "Id";
            var pkType    = pkProp?.ClrType == typeof(Guid)
                                ? "UUID DEFAULT gen_random_uuid()"
                                : "SERIAL";
            var safePk    = QuoteIdentifier(pkColumn);

            db.Database.ExecuteSqlRaw($"""
                CREATE TABLE IF NOT EXISTS {safeTable} (
                    {safePk} {pkType} PRIMARY KEY
                );
            """);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AutoSchemaService] Erro ao garantir tabela {table}: {ex.Message}");
        }
    }

    private static void EnsureColumn(AppDbContext db, string table, string column, string type)
    {
        try
        {
            if (column.Equals("Id", StringComparison.OrdinalIgnoreCase))
                return;

            var safeTable   = QuoteIdentifier(table);
            var safeColumn  = QuoteIdentifier(column);
            var tableLower  = EscapeSqlLiteral(table.ToLowerInvariant());
            var columnLower = EscapeSqlLiteral(column.ToLowerInvariant());

            db.Database.ExecuteSqlRaw($"""
                DO $$
                BEGIN
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns
                        WHERE lower(table_name)  = '{tableLower}'
                          AND lower(column_name) = '{columnLower}'
                    ) THEN
                        ALTER TABLE {safeTable}
                        ADD COLUMN {safeColumn} {type};
                    END IF;
                END
                $$;
            """);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AutoSchemaService] Erro na coluna {table}.{column}: {ex.Message}");
        }
    }

    private static void EnsureUniqueIndex(AppDbContext db, string table, List<string> columns)
    {
        try
        {
            var indexName  = $"uq_{table}_{string.Join("_", columns)}".ToLowerInvariant();
            var safeTable  = QuoteIdentifier(table);
            var safeCols   = string.Join(", ", columns.Select(QuoteIdentifier));
            var safeIndex  = QuoteIdentifier(indexName);
            var nameLiteral = EscapeSqlLiteral(indexName);

            db.Database.ExecuteSqlRaw($"""
                DO $$
                BEGIN
                    IF NOT EXISTS (
                        SELECT 1 FROM pg_indexes
                        WHERE lower(indexname) = '{nameLiteral}'
                    ) THEN
                        CREATE UNIQUE INDEX {safeIndex} ON {safeTable} ({safeCols});
                    END IF;
                END
                $$;
            """);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AutoSchemaService] Erro no indice {table}: {ex.Message}");
        }
    }

    private static string ResolveType(Type type)
    {
        var t = Nullable.GetUnderlyingType(type) ?? type;
        if (t.IsEnum) return "INTEGER";

        return t switch
        {
            _ when t == typeof(int)      => "INTEGER",
            _ when t == typeof(long)     => "BIGINT",
            _ when t == typeof(bool)     => "BOOLEAN DEFAULT FALSE",
            _ when t == typeof(Guid)     => "UUID",
            _ when t == typeof(DateTime) => "TIMESTAMP WITH TIME ZONE",
            _ when t == typeof(decimal)  => "NUMERIC(18,2)",
            _ when t == typeof(float)    => "REAL",
            _ when t == typeof(double)   => "DOUBLE PRECISION",
            _                            => "TEXT",
        };
    }

    private static string QuoteIdentifier(string name)
        => $"\"{name.Replace("\"", "\"\"")}\"";

    private static string EscapeSqlLiteral(string value)
        => value.Replace("'", "''");
}
