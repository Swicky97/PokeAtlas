using System.Drawing.Drawing2D;

namespace PokeAtlas.Controls;

internal static class TileThumbnail
{
    public static Bitmap Create(Bitmap tileset, Rectangle sourcePixelRect, int size = 32)
    {
        Bitmap thumb = new(size, size);

        using Graphics g = Graphics.FromImage(thumb);

        g.InterpolationMode = InterpolationMode.NearestNeighbor;
        g.Clear(Color.FromArgb(45, 45, 48));

        // Preserve aspect ratio instead of stretching non-square regions into a square thumbnail.
        float scale = Math.Min((float)size / sourcePixelRect.Width, (float)size / sourcePixelRect.Height);
        int drawWidth = Math.Max(1, (int)(sourcePixelRect.Width * scale));
        int drawHeight = Math.Max(1, (int)(sourcePixelRect.Height * scale));
        int offsetX = (size - drawWidth) / 2;
        int offsetY = (size - drawHeight) / 2;

        g.DrawImage(tileset, new Rectangle(offsetX, offsetY, drawWidth, drawHeight), sourcePixelRect, GraphicsUnit.Pixel);

        return thumb;
    }
}
