namespace PokeAtlas.Controls;

internal static class CategoryColor
{
    public static Color For(string category)
    {
        float hue = Math.Abs(StableHash(category)) % 360;

        return FromHsv(hue, 0.65f, 0.95f);
    }

    private static int StableHash(string value)
    {
        unchecked
        {
            int hash = 23;

            foreach (char c in value)
                hash = hash * 31 + c;

            return hash;
        }
    }

    private static Color FromHsv(float hue, float saturation, float value)
    {
        int hi = (int)(hue / 60) % 6;
        float f = hue / 60 - (int)(hue / 60);

        int v = (int)(value * 255);
        int p = (int)(value * (1 - saturation) * 255);
        int q = (int)(value * (1 - f * saturation) * 255);
        int t = (int)(value * (1 - (1 - f) * saturation) * 255);

        return hi switch
        {
            0 => Color.FromArgb(255, v, t, p),
            1 => Color.FromArgb(255, q, v, p),
            2 => Color.FromArgb(255, p, v, t),
            3 => Color.FromArgb(255, p, q, v),
            4 => Color.FromArgb(255, t, p, v),
            _ => Color.FromArgb(255, v, p, q),
        };
    }
}
