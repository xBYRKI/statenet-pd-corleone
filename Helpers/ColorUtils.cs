namespace statenet_lspd.Helpers;

public static class ColorUtils
{
    public static string GetContrastingTextColor(string hexColor)
    {
        if (string.IsNullOrWhiteSpace(hexColor)) return "#000000"; // Fallback: Schwarz

        hexColor = hexColor.Replace("#", "");

        if (hexColor.Length != 6)
            return "#000000"; // ungültige Farbe → Schwarz

        var r = int.Parse(hexColor.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        var g = int.Parse(hexColor.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        var b = int.Parse(hexColor.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

        // W3C-Empfehlung: https://www.w3.org/TR/AERT/#color-contrast
        var brightness = ((r * 299) + (g * 587) + (b * 114)) / 1000;
        return brightness > 128 ? "#000000" : "#ffffff";
    }
}
