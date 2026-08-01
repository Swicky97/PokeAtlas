using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using PokeAtlas.Models;

namespace PokeAtlas.Services;

public class DuplicateDetectionService
{
    public List<DuplicateTileGroup> FindDuplicates(Bitmap tileset, int tileSize)
    {
        int columns = tileset.Width / tileSize;
        int rows = tileset.Height / tileSize;

        BitmapData data = tileset.LockBits(
            new Rectangle(0, 0, tileset.Width, tileset.Height),
            ImageLockMode.ReadOnly,
            PixelFormat.Format32bppArgb);

        Dictionary<string, List<Point>> byHash = new();

        try
        {
            int stride = data.Stride;

            byte[] pixels = new byte[stride * tileset.Height];
            Marshal.Copy(data.Scan0, pixels, 0, pixels.Length);

            byte[] tileBuffer = new byte[tileSize * tileSize * 4];

            using MD5 md5 = MD5.Create();

            for (int ty = 0; ty < rows; ty++)
            {
                for (int tx = 0; tx < columns; tx++)
                {
                    int bufferIndex = 0;
                    bool isTransparent = true;

                    for (int py = 0; py < tileSize; py++)
                    {
                        int rowStart = (ty * tileSize + py) * stride + tx * tileSize * 4;

                        for (int px = 0; px < tileSize; px++)
                        {
                            int sourceIndex = rowStart + px * 4;

                            byte b = pixels[sourceIndex];
                            byte g = pixels[sourceIndex + 1];
                            byte r = pixels[sourceIndex + 2];
                            byte a = pixels[sourceIndex + 3];

                            if (a != 0)
                                isTransparent = false;

                            tileBuffer[bufferIndex++] = b;
                            tileBuffer[bufferIndex++] = g;
                            tileBuffer[bufferIndex++] = r;
                            tileBuffer[bufferIndex++] = a;
                        }
                    }

                    if (isTransparent)
                        continue;

                    string hash = Convert.ToHexString(md5.ComputeHash(tileBuffer));

                    if (!byHash.TryGetValue(hash, out List<Point>? positions))
                    {
                        positions = new List<Point>();
                        byHash[hash] = positions;
                    }

                    positions.Add(new Point(tx, ty));
                }
            }
        }
        finally
        {
            tileset.UnlockBits(data);
        }

        return byHash.Values
            .Where(positions => positions.Count > 1)
            .Select(positions => new DuplicateTileGroup
            {
                TileSize = tileSize,
                Positions = positions
            })
            .OrderByDescending(group => group.Positions.Count)
            .ToList();
    }
}
