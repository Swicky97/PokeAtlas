using System.Drawing.Drawing2D;
using PokeAtlas.Models;

namespace PokeAtlas.Services;

public class AtlasBuilderService
{
    private const int Padding = 4;
    private const int MaxRowWidth = 512;
    private const int CategoryHeaderHeight = 18;

    public Bitmap Build(Bitmap sourceTileset, IEnumerable<TileGroup> groups, int tileSize)
    {
        var placements = new List<(TileGroup Group, Rectangle SourcePixels, Point Destination)>();

        int y = 0;
        int atlasWidth = 0;

        foreach (var category in groups.GroupBy(g => g.Category).OrderBy(c => c.Key))
        {
            y += CategoryHeaderHeight;

            int x = 0;
            int rowHeight = 0;

            foreach (TileGroup group in category)
            {
                int pixelWidth = group.TileBounds.Width * tileSize;
                int pixelHeight = group.TileBounds.Height * tileSize;

                if (x > 0 && x + pixelWidth > MaxRowWidth)
                {
                    x = 0;
                    y += rowHeight + Padding;
                    rowHeight = 0;
                }

                Rectangle sourcePixels = new(
                    group.TileBounds.X * tileSize,
                    group.TileBounds.Y * tileSize,
                    pixelWidth,
                    pixelHeight);

                placements.Add((group, sourcePixels, new Point(x, y)));

                atlasWidth = Math.Max(atlasWidth, x + pixelWidth);
                rowHeight = Math.Max(rowHeight, pixelHeight);

                x += pixelWidth + Padding;
            }

            y += rowHeight + Padding * 3;
        }

        Bitmap atlas = new(Math.Max(atlasWidth, 1), Math.Max(y, 1));

        using Graphics g = Graphics.FromImage(atlas);

        g.Clear(Color.FromArgb(45, 45, 48));
        g.InterpolationMode = InterpolationMode.NearestNeighbor;

        using Font font = new("Segoe UI", 8f);
        using Brush textBrush = new SolidBrush(Color.White);

        string? currentCategory = null;

        foreach (var (group, sourcePixels, destination) in placements)
        {
            if (group.Category != currentCategory)
            {
                currentCategory = group.Category;
                g.DrawString(currentCategory, font, textBrush, new PointF(0, destination.Y - CategoryHeaderHeight));
            }

            g.DrawImage(
                sourceTileset,
                new Rectangle(destination, sourcePixels.Size),
                sourcePixels,
                GraphicsUnit.Pixel);
        }

        return atlas;
    }
}
