using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components.Extensions;
using Microsoft.FluentUI.AspNetCore.Components;

namespace FrontBlazorFluentUI.Layout;
public partial class NavigationPanel
{
    private string? _status;
    private bool _popVisible;
    private bool _ltr = true;
    private FluentDesignTheme? _theme;

    //[Inject]
    //public required CacheStorageAccessor CacheStorageAccessor { get; set; }

    [Inject]
    public required GlobalState GlobalState { get; set; }

    public bool DarkMode
    {
        get
        {
            return Mode == DesignThemeModes.Dark;
        }
        set
        {
            if (value)
                Mode = DesignThemeModes.Dark;
            else
                Mode = DesignThemeModes.Light;
        }
    }

    public DesignThemeModes Mode { get; set; }

    public OfficeColor? OfficeColor { get; set; }

    public LocalizationDirection? Direction { get; set; }

    private static IEnumerable<DesignThemeModes> AllModes => Enum.GetValues<DesignThemeModes>();

    private static IEnumerable<OfficeColor?> AllOfficeColors
    {
        get
        {
            return Enum.GetValues<OfficeColor>().Select(i => (OfficeColor?)i);
        }
    }

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            Direction = GlobalState.Dir;
            _ltr = !Direction.HasValue || Direction.Value == LocalizationDirection.LeftToRight;
        }
    }

    private static string? GetCustomColor(OfficeColor? color)
    {
        return color switch
        {
            null => OfficeColorUtilities.GetRandom(true).ToAttributeValue(),
            Microsoft.FluentUI.AspNetCore.Components.OfficeColor.Default => "#036ac4",
            _ => color.ToAttributeValue(),
        };

    }
}
