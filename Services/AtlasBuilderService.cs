using System.Drawing.Drawing2D;
using PokeAtlas.Models;

namespace PokeAtlas.Services;

public class AtlasBuilderService
{
    private const int MaxRowWidth = 512;

    public AtlasBuildResult Build(Bitmap sourceTileset, IEnumerable<TileGroup> groups, int tileSize)
    {
        int padding = tileSize;
        int categoryHeaderHeight = tileSize;

        var rawPlacements = new List<(TileGroup Group, Rectangle SourcePixels, Point Destination)>();

        int y = 0;
        int atlasWidth = 0;

        foreach (var category in groups.GroupBy(g => g.Category).OrderBy(c => c.Key))
        {
            y += categoryHeaderHeight;

            int x = 0;
            int rowHeight = 0;

            foreach (TileGroup group in category)
            {
                int pixelWidth = group.TileBounds.Width * tileSize;
                int pixelHeight = group.TileBounds.Height * tileSize;

                if (x > 0 && x + pixelWidth > MaxRowWidth)
                {
                    x = 0;
                    y += rowHeight + padding;
                    rowHeight = 0;
                }

                Rectangle sourcePixels = new(
                    group.TileBounds.X * tileSize,
                    group.TileBounds.Y * tileSize,
                    pixelWidth,
                    pixelHeight);

                rawPlacements.Add((group, sourcePixels, new Point(x, y)));

                atlasWidth = Math.Max(atlasWidth, x + pixelWidth);
                rowHeight = Math.Max(rowHeight, pixelHeight);

                x += pixelWidth + padding;
            }

            y += rowHeight + padding;
        }

        Bitmap atlas = new(Math.Max(atlasWidth, tileSize), Math.Max(y, tileSize));

        using (Graphics g = Graphics.FromImage(atlas))
        {
            g.Clear(Color.FromArgb(45, 45, 48));
            g.InterpolationMode = InterpolationMode.NearestNeighbor;

            using Font font = new("Segoe UI", 8f);
            using Brush textBrush = new SolidBrush(Color.White);

            string? currentCategory = null;

            foreach (var (group, sourcePixels, destination) in rawPlacements)
            {
                if (group.Category != currentCategory)
                {
                    currentCategory = group.Category;
                    g.DrawString(currentCategory, font, textBrush, new PointF(0, destination.Y - categoryHeaderHeight));
                }

                g.DrawImage(
                    sourceTileset,
                    new Rectangle(destination, sourcePixels.Size),
                    sourcePixels,
                    GraphicsUnit.Pixel);
            }
        }

        List<AtlasPlacement> placements = rawPlacements
            .Select(p => new AtlasPlacement
            {
                Group = p.Group,
                TileBounds = new Rectangle(
                    p.Destination.X / tileSize,
                    p.Destination.Y / tileSize,
                    p.Group.TileBounds.Width,
                    p.Group.TileBounds.Height)
            })
            .ToList();

        return new AtlasBuildResult
        {
            Atlas = atlas,
            Placements = placements
        };
    }
}
