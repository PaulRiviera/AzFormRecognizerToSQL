using System;
namespace AzFormRecognizer.Table
{
    public enum TableKeyType
    {
        Primary,
        Foreign
    }

    public class TableKey
    {
        public TableKeyType Type { get; set; }
        public string? Reference { get; set; }
    }
}

