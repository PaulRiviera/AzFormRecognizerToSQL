using System;
using AzFormRecognizer.Table;
using Azure.AI.FormRecognizer.DocumentAnalysis;

namespace AzFormRecognizer.Table.ToSQL
{
    public static partial class DocumentTableExtension
    {
        public delegate void ConfigureTables(List<Table> tables, DocumentDetails documentDetails);

        public static List<string> ToSQL(this IReadOnlyList<DocumentTable> tables, DocumentDetails documentDetails, ConfigureTables handler)
        {
            List<Table> parsedTable = tables.Select(table => ParseTable(table)).ToList();

            handler(parsedTable, documentDetails);

            var commands = TableSQLCommands(parsedTable);

            return commands;
        }
    }
}

