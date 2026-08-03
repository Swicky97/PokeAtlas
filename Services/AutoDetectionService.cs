using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using PokeAtlas.Models;

namespace PokeAtlas.Services;

// Proposes candidate TileGroup regions by flood-filling connected clusters of non-transparent
// tiles. This finds "there is a distinct blob of art here" geometrically -- it has no idea
// whether that blob is a roof, a tree, or anything else, so the caller still has to name and
// categorize each region. That split (machine finds boundaries, human assigns meaning) is
// deliberate: real semantic recognition is out of scope for a heuristic like this.
public class AutoDetectionService
{
    public List<DetectedRegion> DetectRegions(Bitmap tileset, int tileSize, IEnumerable<TileGroup> existingGroups)
    {
        int columns = tileset.Width / tileSize;
        int rows = tileset.Height / tileSize;

        bool[,] occupied = new bool[columns, rows];
        bool[,] claimed = new bool[columns, rows];

        MarkOccupied(tileset, tileSize, columns, rows, occupied);
        MarkClaimed(existingGroups, columns, rows, claimed);

        bool[,] visited = new bool[columns, rows];
        List<DetectedRegion> regions = new();

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                if (visited[x, y] || !occupied[x, y] || claimed[x, y])
                    continue;

                Rectangle bounds = FloodFill(occupied, claimed, visited, columns, rows, x, y);

                regions.Add(new DetectedRegion { TileBounds = bounds });
            }
        }

        return regions
            .OrderByDescending(r => r.TileBounds.Width * r.TileBounds.Height)
            .ToList();
    }

    private static Rectangle FloodFill(bool[,] occupied, bool[,] claimed, bool[,] visited, int columns, int rows, int startX, int startY)
    {
        Queue<Point> queue = new();
        queue.Enqueue(new Point(startX, startY));
        visited[startX, startY] = true;

        int minX = startX, maxX = startX, minY = startY, maxY = startY;

        while (queue.Count > 0)
        {
            Point p = queue.Dequeue();

            minX = Math.Min(minX, p.X);
            maxX = Math.Max(maxX, p.X);
            minY = Math.Min(minY, p.Y);
            maxY = Math.Max(maxY, p.Y);

            Span<Point> neighbors =
            [
                new Point(p.X - 1, p.Y),
                new Point(p.X + 1, p.Y),
                new Point(p.X, p.Y - 1),
                new Point(p.X, p.Y + 1)
            ];

            foreach (Point neighbor in neighbors)
            {
                if (neighbor.X < 0 || neighbor.X >= columns || neighbor.Y < 0 || neighbor.Y >= rows)
                    continue;

                if (visited[neighbor.X, neighbor.Y] || !occupied[neighbor.X, neighbor.Y] || claimed[neighbor.X, neighbor.Y])
                    continue;

                visited[neighbor.X, neighbor.Y] = true;
                queue.Enqueue(neighbor);
            }
        }

        return new Rectangle(minX, minY, maxX - minX + 1, maxY - minY + 1);
    }

    private static void MarkClaimed(IEnumerable<TileGroup> existingGroups, int columns, int rows, bool[,] claimed)
    {
        foreach (TileGroup group in existingGroups)
        {
            Rectangle bounds = group.TileBounds;

            for (int y = Math.Max(0, bounds.Top); y < Math.Min(rows, bounds.Bottom); y++)
            {
                for (int x = Math.Max(0, bounds.Left); x < Math.Min(columns, bounds.Right); x++)
                {
                    claimed[x, y] = true;
                }
            }
        }
    }

    private static void MarkOccupied(Bitmap tileset, int tileSize, int columns, int rows, bool[,] occupied)
    {
        BitmapData data = tileset.LockBits(
            new Rectangle(0, 0, tileset.Width, tileset.Height),
            ImageLockMode.ReadOnly,
            PixelFormat.Format32bppArgb);

        try
        {
            int stride = data.Stride;

            byte[] pixels = new byte[stride * tileset.Height];
            Marshal.Copy(data.Scan0, pixels, 0, pixels.Length);

            int? backgroundColor = DetectBackgroundColor(pixels, stride, tileset.Width, tileset.Height);

            for (int ty = 0; ty < rows; ty++)
            {
                for (int tx = 0; tx < columns; tx++)
                {
                    bool hasContent = false;

                    for (int py = 0; py < tileSize && !hasContent; py++)
                    {
                        int rowStart = (ty * tileSize + py) * stride + tx * tileSize * 4;

                        for (int px = 0; px < tileSize; px++)
                        {
                            int i = rowStart + px * 4;

                            if (pixels[i + 3] == 0)
                                continue; // fully transparent -- doesn't count as content

                            if (backgroundColor is { } bg && PackColor(pixels, i) == bg)
                                continue; // matches the detected solid background color -- doesn't count as content

                            hasContent = true;
                            break;
                        }
                    }

                    occupied[tx, ty] = hasContent;
                }
            }
        }
        finally
        {
            tileset.UnlockBits(data);
        }
    }

    // Some tilesets mark empty space with a solid opaque color (e.g. magenta or white) instead
    // of real alpha transparency. Sample the four corners: if at least three agree on one
    // fully-opaque color, treat that as "background" too. If the corners disagree (e.g. art
    // touches a corner), fall back to alpha-only detection so this never makes things worse.
    private static int? DetectBackgroundColor(byte[] pixels, int stride, int width, int height)
    {
        int[] corners =
        [
            PackColor(pixels, PixelIndex(0, 0, stride)),
            PackColor(pixels, PixelIndex(width - 1, 0, stride)),
            PackColor(pixels, PixelIndex(0, height - 1, stride)),
            PackColor(pixels, PixelIndex(width - 1, height - 1, stride))
        ];

        var mostCommon = corners
            .GroupBy(c => c)
            .OrderByDescending(g => g.Count())
            .First();

        if (mostCommon.Count() < 3)
            return null;

        int color = mostCommon.Key;
        bool isOpaque = ((color >> 24) & 0xFF) == 255;

        return isOpaque ? color : null;
    }

    private static int PixelIndex(int x, int y, int stride) => y * stride + x * 4;

    private static int PackColor(byte[] pixels, int index)
    {
        byte b = pixels[index];
        byte g = pixels[index + 1];
        byte r = pixels[index + 2];
        byte a = pixels[index + 3];

        return (a << 24) | (r << 16) | (g << 8) | b;
    }
}
