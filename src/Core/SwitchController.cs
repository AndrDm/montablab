using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Input.KeyboardAndMouse;
using Windows.Win32.UI.WindowsAndMessaging;

namespace Montab.Core;

/// <summary>
/// Переключение foreground-окон с историей: клик по уже активному окну
/// возвращает в предыдущее («туда-обратно», как Alt-Tab).
/// </summary>
internal sealed class SwitchController
{
    const byte VkMenu = 0x12; // VK_MENU (Alt)

    HWND _current;
    HWND _previous;

    public void OnForegroundChanged(HWND hwnd)
    {
        if (hwnd == _current)
            return;
        _previous = _current;
        _current = hwnd;
    }

    public void Activate(HWND target)
    {
        HWND goal = target;
        if (target == _current && _previous != default && PInvoke.IsWindow(_previous))
            goal = _previous;

        if (PInvoke.IsIconic(goal))
            PInvoke.ShowWindow(goal, SHOW_WINDOW_CMD.SW_RESTORE);

        if (!PInvoke.SetForegroundWindow(goal))
        {
            // Панель не активируется (WS_EX_NOACTIVATE), поэтому система может
            // держать foreground lock. Имитация нажатия Alt его снимает.
            PInvoke.keybd_event(VkMenu, 0, 0, 0);
            PInvoke.keybd_event(VkMenu, 0, KEYBD_EVENT_FLAGS.KEYEVENTF_KEYUP, 0);
            PInvoke.SetForegroundWindow(goal);
        }
    }
}
