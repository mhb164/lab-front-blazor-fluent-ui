using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laboratory.Frontend.Model;

public class TestTable
{
    public readonly string Name;
    private readonly Dictionary<string, Column> _columns = new Dictionary<string, Column>();
    private readonly List<Row> _rows = new List<Row>();

    public TestTable(string name, params string[] columns)
    {
        Name = name;
        AddColumns(columns);
    }

    public int RowCount => _rows.Count;

    public IEnumerable<Row> Rows => _rows;

    public IEnumerable<Column> Columns => _columns.Values;

    private TestTable AddColumns(params string[] columns)
    {
        foreach (var item in columns)
        {
            var column = new Column(this, _columns.Count, item, item);
            _columns.Add(item, column);
        }

        return this;
    }

    public TestTable AddRow(params string[] items)
    {
        _rows.Add(new Row(this, _rows.Count, items));
        return this;
    }

    public Column? GetColumn(string columnKey)
    {
        if (_columns.TryGetValue(columnKey, out var column))
            return column;

        return null;
    }

    public string ValueOf(int rowIndex, string columnKey)
    {
        var column = GetColumn(columnKey);
        if (column is null)
            return $"ERROR[col_{columnKey}]!";

        return ValueOf(rowIndex, column);
    }

    public string ValueOf(int rowIndex, Column column)
    {
        if (_rows.Count <= rowIndex)
            return $"ERROR(row_{rowIndex})!";

        return _rows[rowIndex].ValueOf(column);
    }

    public class Column
    {
        public readonly TestTable Table;

        public Column(TestTable table, int index, string key, string title)
        {
            Table = table;
            Index = index;
            Key = key;
            Title = title;
        }

        public int Index { get; private set; }
        public string Key { get; private set; }
        public string Title { get; private set; }
    }

    public class Row
    {
        public readonly TestTable Table;
        public readonly int Index;
        private readonly List<string> _items;

        internal Row(TestTable table, int index, params string[] items)
        {
            Table = table;
            Index = index;
            _items = items.ToList();
        }

        public string ValueOf(string columnKey)
        {
            var column = Table.GetColumn(columnKey);
            if (column is null)
                return $"ERROR[col_{columnKey}]!";

            return ValueOf(column);
        }

        public string ValueOf(Column column)
        {
            if (column.Index == -1)
                return $"ERROR2[{column.Key}]!";

            return ValueOf(column.Index);
        }

        internal string ValueOf(int columnIndex)
        {
            if (_items.Count <= columnIndex)
                return string.Empty;

            return _items[columnIndex];
        }
    }
}