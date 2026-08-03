using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using PokeAtlas.Models;

namespace PokeAtlas.Services;

// Builds the shared Tile database (sub-phase 1 of the Phase 12 detection pipeline): one Tile
// per grid cell, with its exact hash, perceptual hash, and dominant-color palette precomputed
// so later detection algorithms don't each have to re-scan raw pixels themselves.
public class TileDatabaseService
{
    private const int PaletteSize = 3;

    public List<Tile> BuildDatabase(Bitmap tileset, int tileSize)
    {
        int columns = tileset.Width / tileSize;
        int rows = tileset.Height / tileSize;

        BitmapData data = tileset.LockBits(
            new Rectangle(0, 0, tileset.Width, tileset.Height),
            ImageLockMode.ReadOnly,
            PixelFormat.Format32bppArgb);

        List<Tile> tiles = new(columns * rows);

        try
        {
            int stride = data.Stride;

            byte[] allPixels = new byte[stride * tileset.Height];
            Marshal.Copy(data.Scan0, allPixels, 0, allPixels.Length);

            for (int ty = 0; ty < rows; ty++)
            {
                for (int tx = 0; tx < columns; tx++)
                {
                    byte[] tilePixels = ExtractTilePixels(allPixels, stride, tx, ty, tileSize);

                    tiles.Add(new Tile
                    {
                        Position = new Point(tx, ty),
                        TileSize = tileSize,
                        PixelData = tilePixels,
                        ExactHash = Convert.ToHexString(MD5.HashData(tilePixels)),
                        PerceptualHash = PerceptualHashService.Compute(tilePixels, tileSize),
                        Palette = ExtractPalette(tilePixels)
                    });
                }
            }
        }
        finally
        {
            tileset.UnlockBits(data);
        }

        return tiles;
    }

    private static byte[] ExtractTilePixels(byte[] source, int stride, int tx, int ty, int tileSize)
    {
        byte[] result = new byte[tileSize * tileSize * 4];

        for (int py = 0; py < tileSize; py++)
        {
            int srcRowStart = (ty * tileSize + py) * stride + tx * tileSize * 4;
            int dstRowStart = py * tileSize * 4;

            Array.Copy(source, srcRowStart, result, dstRowStart, tileSize * 4);
        }

        return result;
    }

    private static List<Color> ExtractPalette(byte[] pixelData)
    {
        Dictionary<int, int> counts = new();

        for (int i = 0; i < pixelData.Length; i += 4)
        {
            byte b = pixelData[i];
            byte g = pixelData[i + 1];
            byte r = pixelData[i + 2];
            byte a = pixelData[i + 3];

            if (a == 0)
                continue;

            // Quantize so near-identical shades (anti-aliasing, gradients) bucket together
            // instead of each being counted as its own distinct color.
            int key = ((r / 16 * 16) << 16) | ((g / 16 * 16) << 8) | (b / 16 * 16);

            counts[key] = counts.GetValueOrDefault(key) + 1;
        }

        return counts
            .OrderByDescending(kv => kv.Value)
            .Take(PaletteSize)
            .Select(kv => Color.FromArgb(255, (kv.Key >> 16) & 0xFF, (kv.Key >> 8) & 0xFF, kv.Key & 0xFF))
            .ToList();
    }
}
