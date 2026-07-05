using Windows.Win32;

namespace Montab.App;

/// <summary>
/// Строки интерфейса: русский при русском языке системы, иначе английский.
/// (InvariantGlobalization=true — CultureInfo недоступна, язык берём у Win32.)
/// </summary>
internal static class Strings
{
    const int LangRussian = 0x19; // PRIMARYLANGID русского языка

    static readonly bool Russian = (PInvoke.GetUserDefaultUILanguage() & 0x3FF) == LangRussian;

    public static string DockLeft => Russian ? "Слева" : "Dock left";
    public static string DockRight => Russian ? "Справа" : "Dock right";
    public static string ScrollBar => Russian ? "Скролл Бар" : "Scroll Bar";
    public static string Autostart => Russian ? "Автозапуск" : "Start with Windows";
    public static string Exit => Russian ? "Выход" : "Exit";
}
