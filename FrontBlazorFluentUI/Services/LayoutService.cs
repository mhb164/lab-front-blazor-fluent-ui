namespace FrontBlazorFluentUI.Services;

public class LayoutService
{
    public event Action? OnChange;
    private string _title = "Front Blazor FluentUI";

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