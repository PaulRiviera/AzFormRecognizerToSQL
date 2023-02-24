using System;
using System.Linq;
using System.Collections.Generic;


namespace AzFormRecognizer.Table.ToSQL
{
    public class SQLCommandBuilderOptions
    {
        private static SQLCommandBuilderOptions _default = new SQLCommandBuilderOptions();

        /// <summary>
        /// Default options
        /// </summary>
        public static SQLCommandBuilderOptions Default { get { return _default; } }

        /// <summary>
        /// If true, the table will be created if it does not exist. If false, the command will fail if the table already exists.
        /// </summary>
        public bool IgnoreCreateIfTableExists { get; set; } = true;
    }
}