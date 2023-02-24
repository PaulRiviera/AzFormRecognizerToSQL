using System;
using System.Collections.Generic;
using Xunit;
using AzFormRecognizer.Table;
using AzFormRecognizer.Table.ToSQL;

public class SQLCommandBuilderTests
{
    [Fact]
    public void ConvertTableToCommandsWithDefaultOptions()
    {
        var table = new Table();
        table.Title = "Test Data";

        table.Headers = new Dictionary<int, ColumnHeader>();
        table.Headers.Add(0, new ColumnHeader() { Name = "Column 1", DataType = ColumnDataTypes.INT, TableKey = new TableKey() { Type = TableKeyType.Primary } });
        table.Headers.Add(1, new ColumnHeader() { Name = "Column 2", DataType = ColumnDataTypes.INT });
        table.Headers.Add(2, new ColumnHeader() { Name = "Column 3", DataType = ColumnDataTypes.INT });

        table.Rows = new List<Dictionary<string, string>>();
        table.Rows.Add(new Dictionary<string, string>() { { "Column 2", "2" }, { "Column 3", "3" } });
        table.Rows.Add(new Dictionary<string, string>() { { "Column 2", "5" }, { "Column 3", "6" } });
        
        var sqlCmd = SQLCommandBuilder.TableSQLCommands(new List<Table>() { table });

        Assert.Equal(3, sqlCmd.Count);
        Assert.Equal("IF (OBJECT_ID(N'dbo.TestData', N'U') IS NULL) BEGIN CREATE TABLE dbo.TestData (Column1 int  IDENTITY(1,1) PRIMARY KEY NOT NULL, Column2 int NULL, Column3 int NULL) END", sqlCmd[0]);
        Assert.Equal("INSERT INTO TestData (Column2, Column3) VALUES ('2', '3');", sqlCmd[1]);
        Assert.Equal("INSERT INTO TestData (Column2, Column3) VALUES ('5', '6');", sqlCmd[2]);
    }

    [Fact]
    public void ConvertTableToCommandsWithFalseIgnoreExisting()
    {
        var table = new Table();
        table.Title = "Test Data Two";

        table.Headers = new Dictionary<int, ColumnHeader>();
        table.Headers.Add(0, new ColumnHeader() { Name = "Column 1", DataType = ColumnDataTypes.INT, TableKey = new TableKey() { Type = TableKeyType.Primary } });
        table.Headers.Add(1, new ColumnHeader() { Name = "Column 2", DataType = ColumnDataTypes.INT });
        table.Headers.Add(2, new ColumnHeader() { Name = "Column 3", DataType = ColumnDataTypes.INT });

        table.Rows = new List<Dictionary<string, string>>();
        table.Rows.Add(new Dictionary<string, string>() { { "Column 2", "2" }, { "Column 3", "3" } });
        table.Rows.Add(new Dictionary<string, string>() { { "Column 2", "5" }, { "Column 3", "6" } });
        
        var sqlCmd = SQLCommandBuilder.TableSQLCommands(new List<Table>() { table }, new SQLCommandBuilderOptions() { IgnoreCreateIfTableExists = false });

        Assert.Equal(3, sqlCmd.Count);
        Assert.Equal("CREATE TABLE TestDataTwo (Column1 int  IDENTITY(1,1) PRIMARY KEY NOT NULL, Column2 int NULL, Column3 int NULL)", sqlCmd[0]);
        Assert.Equal("INSERT INTO TestDataTwo (Column2, Column3) VALUES ('2', '3');", sqlCmd[1]);
        Assert.Equal("INSERT INTO TestDataTwo (Column2, Column3) VALUES ('5', '6');", sqlCmd[2]);
    }
}