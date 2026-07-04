using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

namespace Montab.Core;

/// <summary>Отслеживаемое окно верхнего уровня.</summary>
internal sealed class WindowItem
{
    public required HWND Hwnd { get; init; }
    public string Title = "";
    public HICON Icon;
    public bool OwnsIcon;
    public bool IsMinimized;

    /// <summary>Погашено пользователем: окно живо, но превью отключено (полоска).</summary>
    public bool IsCollapsed;
}
