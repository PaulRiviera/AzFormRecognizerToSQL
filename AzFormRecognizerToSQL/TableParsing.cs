using Azure.AI.FormRecognizer.DocumentAnalysis;

namespace AzFormRecognizer.Table.ToSQL
{
    partial class DocumentTableExtension
    {
        /// <summary>
        /// Parses the DocumentTable from Azure Form Recognizer and returns a Table object.
        /// </summary>
        /// <param name="documentTable">Table from Azure Form Recognizer output.</param>
        /// <returns>Table</returns>
        public static Table ParseTable(DocumentTable documentTable)
        {
            var table = new Table();
            var rows = documentTable.Cells.GroupBy(cell => cell.RowIndex).ToDictionary(group => group.Key, group => group.ToList());

            RemoveEmptyRows(rows);
            ExtractTableTitle(rows, table);
            ExtractTableHeaders(rows, table);
            ExtractRowsData(rows, table);

            return table;
        }

        /// <summary>
        /// Removes any empty rows from the DocumentTable.
        /// </summary>
        private static void RemoveEmptyRows(Dictionary<int, List<DocumentTableCell>> rows)
        {
            foreach (var row in rows)
            {
                if (row.Value.All(cell => cell.Content == ""))
                {
                    rows.Remove(row.Key);
                }
            }
        }

        /// <summary>
        /// Extracts the Title by looking for the first row with a single non-empty cell. 
        /// </summary>
        private static void ExtractTableTitle(Dictionary<int, List<DocumentTableCell>> rows, Table table)
        {
            var rowWithOneValue = rows.Where(row => row.Value.Where(cell => cell.Content != "").Count() == 1).FirstOrDefault();
            if (rowWithOneValue.Value != null)
            {
                table.Title = rowWithOneValue.Value.FirstOrDefault(cell => cell.Content != "")?.Content ?? string.Empty;
                rows.Remove(rowWithOneValue.Key);
            }
        }

        /// <summary>
        /// Extracts the Headers by looking for the first row with all cells of type DocumentTableCellKind.ColumnHeader.
        /// </summary>
        private static void ExtractTableHeaders(Dictionary<int, List<DocumentTableCell>> rows, Table table)
        {
            var headerRow = rows.Where(row => row.Value.All(cell => cell.Kind == DocumentTableCellKind.ColumnHeader)).FirstOrDefault().Value;
            if (headerRow != null)
            {
                var headerRowValues = headerRow.ToDictionary(cell => cell.ColumnIndex, cell => new ColumnHeader() { Name = (cell.Content == "" ? $"Column_{cell.ColumnIndex}" : cell.Content) });
                table.Headers = headerRowValues;
            }
        }

        /// <summary>
        /// Extracts the Rows by looking for all rows with all cells of type DocumentTableCellKind.Content.
        /// </summary>
        private static void ExtractRowsData(Dictionary<int, List<DocumentTableCell>> rows, Table table)
        {
            var contentRows = rows.Where(row => row.Value.All(cell => cell.Kind == DocumentTableCellKind.Content)).Select(row => row.Value).ToList();
            foreach (var contentRow in contentRows)
            {
                if (table.Headers == null)
                {
                    throw new Exception("Table headers cannot be null");
                }

                var contentRowValues = contentRow.ToDictionary(cell => table.Headers[cell.ColumnIndex].Name ?? $"Column_{cell.ColumnIndex}", cell => cell.Content);
                table.Rows.Add(contentRowValues);
            }
        }
    }
}

