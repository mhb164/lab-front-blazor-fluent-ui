namespace Laboratory.Frontend.Interfaces;

public interface ITableViewDataProvider
{
    IEnumerable<ITableViewDataProviderColumn> Columns { get; }
    IEnumerable<ITableViewDataProviderRow> Rows { get; }
}
public interface ITableViewDataProviderColumn
{
    string Key { get; }
    string Title { get; }
}

public interface ITableViewDataProviderRow
{
    string ValueOf(string columnKey);
    string ValueOf(ITableViewDataProviderColumn column);
}
