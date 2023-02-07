using System;

namespace AzFormRecognizer.Table.ToSQL
{
    partial class DocumentTableExtension
    {
        private static List<string> TableSQLCommands(IEnumerable<Table> tables)
        {
            var commands = tables.Select(table => CreateTableSQLCommands(table)).ToList();

            foreach (var table in tables)
            {
                if (table.Title == null)
                {
                    throw new Exception("Table title is null");
                }

                var insertCommands = table.Rows.Select(row => CreateTableInsertSQLCommands(table.Title, row));
                commands.AddRange(insertCommands);
            }

            return commands;
        }

        private static string CreateTableSQLCommands(Table table)
        {
            if (table.Title == null)
            {
                throw new Exception("Table title is null");
            }

            var sql = $"CREATE TABLE {table.Title.Replace(" ", "")} (";

            if (table?.Headers == null)
            {
                throw new Exception("Table headers are null");
            }

            foreach (var header in table.Headers)
            {
                sql += GetColumn(header.Value);
            }
            sql = sql.Remove(sql.Length - 2);
            sql += ");";
            return sql;
        }

        private static string CreateTableInsertSQLCommands(string tableTitle, Dictionary<string, string> row)
        {
            var columnString = string.Empty;
            var valueString = string.Empty;

            foreach (var value in row)
            {
                if (string.IsNullOrEmpty(value.Value))
                {
                    continue;
                }

                columnString += $"{value.Key.Replace(" ", "")}, ";
                valueString += $"\'{value.Value}\', ";
            }

            columnString = columnString.Remove(columnString.Length - 2);
            valueString = valueString.Remove(valueString.Length - 2);

            var sql = $"INSERT INTO {tableTitle.Replace(" ", "")} ({columnString}) VALUES ({valueString});";

            return sql;
        }

        private static string GetType(ColumnDataTypes dataType)
        {
            switch (dataType)
            {
                case ColumnDataTypes.INT:
                    return "int";
                case ColumnDataTypes.DECIMAL:
                    return "decimal";
                case ColumnDataTypes.VARCHAR:
                default:
                    return "varchar(255)";
            }
        }

        private static string GetColumn(ColumnHeader header)
        {
            if (header == null)
            {
                throw new Exception("Column header is null");
            }

            if (string.IsNullOrEmpty(header.Name))
            {
                throw new Exception("Column header name cannot be empty or null");
            }

            string name = header.Name.Replace(" ", "");
            string typeName = GetType(header.DataType);
            string canBeNull = header.CanBeNull && (header.TableKey == null) ? "NULL" : "NOT NULL";

            string primaryKey = string.Empty;
            string foreignKey = string.Empty;

            if (header.TableKey != null && header.TableKey.Type == TableKeyType.Foreign)
            {
                canBeNull = "";
                foreignKey = $"FOREIGN KEY REFERENCES {header.TableKey.Reference}";
            }

            if (header.TableKey != null && header.TableKey.Type == TableKeyType.Primary)
            {
                primaryKey = "PRIMARY KEY";
                if (header.DataType == ColumnDataTypes.INT)
                {
                    canBeNull = "";
                    primaryKey = $"IDENTITY(1,1) {primaryKey}";
                }
                else
                {
                    canBeNull = "NOT NULL";
                }
            }

            return $"{name} {typeName} {primaryKey} {foreignKey} {canBeNull}, ";
        }
    }
}

