using Montab.Core;
using Windows.Win32.Foundation;

namespace Montab.UI;

internal readonly record struct LayoutItem(WindowItem Window, RECT Bounds, bool IsStrip);

/// <summary>
/// Раскладка ленты: живые тайлы (высота по аспекту окна-источника) и полоски
/// (свёрнутые/погашенные). Все размеры — логические px, масштабируются по DPI.
/// </summary>
internal sealed class LayoutEngine
{
    const int StripHeightLogical = 28;
    const int GapLogical = 6;
    const int PaddingLogical = 8;

    public int TotalHeight { get; private set; }

    public List<LayoutItem> Compute(IReadOnlyList<WindowItem> items, RECT client, uint dpi, int scrollOffset)
    {
        var result = new List<LayoutItem>(items.Count);

        int gap = Scale(GapLogical, dpi);
        int padding = Scale(PaddingLogical, dpi);
        int stripHeight = Scale(StripHeightLogical, dpi);

        int width = client.right - client.left - 2 * padding;
        if (width <= 0)
            return result;

        int y = padding - scrollOffset;
        foreach (var item in items)
        {
            // M2: все элементы — полоски; живые тайлы по аспекту появятся в M3.
            bool isStrip = true;
            int height = stripHeight;

            result.Add(new LayoutItem(item, new RECT
            {
                left = client.left + padding,
                top = y,
                right = client.left + padding + width,
                bottom = y + height,
            }, isStrip));

            y += height + gap;
        }

        TotalHeight = y + scrollOffset + padding - (items.Count > 0 ? gap : 0);
        return result;
    }

    public static int Scale(int logical, uint dpi) => (int)(logical * dpi / 96.0);
}
