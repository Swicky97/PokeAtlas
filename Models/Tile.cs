using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace PokeAtlas.Models;

// One entry in the shared tile database. Deliberately stores raw pixel bytes rather than a
// live Bitmap per tile: a large atlas can have tens of thousands of tiles, and Windows caps a
// process at roughly 10,000 GDI handles -- eagerly creating one Bitmap object per tile would
// risk hitting that ceiling. ToBitmap() builds one on demand for the (comparatively rare)
// cases that need to actually display a tile.
public class Tile
{
    public required Point Position { get; init; }

    public required int TileSize { get; init; }

    // Tightly-packed BGRA, TileSize*TileSize*4 bytes, no row padding.
    public required byte[] PixelData { get; init; }

    public required string ExactHash { get; init; }

    public required PerceptualHash PerceptualHash { get; init; }

    public required IReadOnlyList<Color> Palette { get; init; }

    public bool IsTransparent
    {
        get
        {
            for (int i = 3; i < PixelData.Length; i += 4)
            {
                if (PixelData[i] != 0)
                    return false;
            }

            return true;
        }
    }

    public Bitmap ToBitmap()
    {
        Bitmap bitmap = new(TileSize, TileSize, PixelFormat.Format32bppArgb);

        BitmapData data = bitmap.LockBits(
            new Rectangle(0, 0, TileSize, TileSize),
            ImageLockMode.WriteOnly,
            PixelFormat.Format32bppArgb);

        Marshal.Copy(PixelData, 0, data.Scan0, PixelData.Length);

        bitmap.UnlockBits(data);

        return bitmap;
    }
}
