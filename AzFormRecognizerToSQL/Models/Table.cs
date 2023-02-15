using System;
using System.Collections.Generic;

namespace AzFormRecognizer.Table
{
    public class Table
    {
        public string? Title { get; set; }

        public Dictionary<TableKeyType, string>? Keys;

        public Dictionary<int, ColumnHeader>? Headers;

        public List<Dictionary<string, string>> Rows { get; set; } = new List<Dictionary<string, string>>();

    }
}

