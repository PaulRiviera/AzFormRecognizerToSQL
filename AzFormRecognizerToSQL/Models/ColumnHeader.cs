using System;
namespace AzFormRecognizer.Table
{
    public enum ColumnDataTypes
    {
        VARCHAR,
        INT,
        FLOAT,
        DECIMAL,
    }

    public class ColumnHeader
    {
        public string? Name { get; set; }

        public ColumnDataTypes DataType { get; set; } = ColumnDataTypes.VARCHAR;

        public bool CanBeNull { get; set; } = true;

        public TableKey? TableKey { get; set; }
    }
}

