namespace Laboratory.Front.Components;

public partial class TableView
{
    [Parameter] public ITableViewDataProvider DataProvider { get; set; }


    [Parameter] public string? ContainerStyle { get; set; }
    //height: calc(100dvh - 160px);min-height: 8rem;overflow-x: auto;overflow-y: hidden;

    PaginationState pagination = new PaginationState { ItemsPerPage = 10 };

    public GridSort<ITableViewDataProviderRow> GetSortBy(ITableViewDataProviderColumn column)
    {
        return GridSort<ITableViewDataProviderRow>
            .ByDescending(p => p.ValueOf(column));
    }

    public IEnumerable<ITableViewDataProviderColumn> Columns => DataProvider.Columns;
    public IQueryable<ITableViewDataProviderRow> Rows
    {
        get
        {
            return DataProvider.Rows.Where(x => IsIncludedInSearch(x)).AsQueryable();
        }
    }

    private bool IsIncludedInSearch(ITableViewDataProviderRow row)
    {
        if (!ColumnFilters.Any())
            return true;

        foreach (var columnFilter in ColumnFilters)
        {
            if (string.IsNullOrWhiteSpace(columnFilter.Value))
                continue;

            if (!row.ValueOf(columnFilter.Key).Contains(columnFilter.Value, StringComparison.OrdinalIgnoreCase))
                return false;
        }

        return true;
    }

    private Dictionary<string, string> ColumnFilters = new();

    private bool HasColumnFilter(ITableViewDataProviderColumn column)
        => !string.IsNullOrWhiteSpace(GetColumnFilter(column));

    private string GetColumnFilter(ITableViewDataProviderColumn column)
        => ColumnFilters.TryGetValue(column.Key, out var val) ? val : string.Empty;

    private void SetColumnFilter(ITableViewDataProviderColumn column, string value)
    {
        ColumnFilters[column.Key] = value;
    }
}
