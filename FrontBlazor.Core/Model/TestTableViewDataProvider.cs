namespace Laboratory.Frontend.Model;

public class TestTableViewDataProvider : ITableViewDataProvider
{
    private readonly TestTable _table;
    public TestTableViewDataProvider(TestTable table)
    {
        _table = table;
    }

    public IEnumerable<ITableViewDataProviderColumn> Columns => _table.Columns.Select(x => new Column(x));

    public IEnumerable<ITableViewDataProviderRow> Rows => _table.Rows.Select(x => new Row(x));

    public class Column : ITableViewDataProviderColumn
    {
        private readonly TestTable.Column _column;

        public Column(TestTable.Column column)
        {
            _column = column;
        }

        public string Key => _column.Key;
        public string Title => _column.Title;
    }

    public class Row : ITableViewDataProviderRow
    {
        private readonly TestTable.Row _row;

        public Row(TestTable.Row row)
        {
            _row = row;
        }

        public string ValueOf(string columnKey) => _row.ValueOf(columnKey);

        public string ValueOf(ITableViewDataProviderColumn column) => _row.ValueOf(column.Key);
    }
}