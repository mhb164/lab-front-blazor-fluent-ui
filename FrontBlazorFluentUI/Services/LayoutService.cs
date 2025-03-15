namespace Laboratory.Front.Services;

public class LayoutService
{
    public event Action? OnChange;
    private string _title = "Loading....";

    public string Title
    {
        get => _title;
        set
        {
            _title = value;
            OnChange?.Invoke(); // Notify components to update UI
        }
    }
}