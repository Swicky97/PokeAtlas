using PokeAtlas.Models;

namespace PokeAtlas.Services;

// Difference hash (dHash): a well-known, dependency-free perceptual hash. Downsamples a tile
// to a small grayscale grid and records whether brightness increases moving right across each
// row, packing those comparisons into a bitmask, then pairs that with the tile's average color
// (see PerceptualHash.DistanceTo for why color has to be included too).
public static class PerceptualHashService
{
    private const int HashWidth = 9;
    private const int HashHeight = 8;

    public static PerceptualHash Compute(byte[] pixelData, int tileSize)
    {
        float[,] gray = Downsample(pixelData, tileSize, HashWidth, HashHeight);

        ulong structure = 0;
        int bit = 0;

        for (int y = 0; y < HashHeight; y++)
        {
            for (int x = 0; x < HashWidth - 1; x++)
            {
                if (gray[x, y] < gray[x + 1, y])
                    structure |= 1UL << bit;

                bit++;
            }
        }

        (byte r, byte g, byte b) = AverageColor(pixelData);

        return new PerceptualHash(structure, r, g, b);
    }

    private static (byte R, byte G, byte B) AverageColor(byte[] pixelData)
    {
        long sumR = 0, sumG = 0, sumB = 0;
        int count = 0;

        for (int i = 0; i < pixelData.Length; i += 4)
        {
            if (pixelData[i + 3] == 0)
                continue;

            sumB += pixelData[i];
            sumG += pixelData[i + 1];
            sumR += pixelData[i + 2];
            count++;
        }

        if (count == 0)
            return (0, 0, 0);

        return ((byte)(sumR / count), (byte)(sumG / count), (byte)(sumB / count));
    }

    private static float[,] Downsample(byte[] pixelData, int tileSize, int targetWidth, int targetHeight)
    {
        float[,] result = new float[targetWidth, targetHeight];

        for (int ty = 0; ty < targetHeight; ty++)
        {
            int srcYStart = ty * tileSize / targetHeight;
            int srcYEnd = Math.Max(srcYStart + 1, (ty + 1) * tileSize / targetHeight);

            for (int tx = 0; tx < targetWidth; tx++)
            {
                int srcXStart = tx * tileSize / targetWidth;
                int srcXEnd = Math.Max(srcXStart + 1, (tx + 1) * tileSize / targetWidth);

                float sum = 0;
                int count = 0;

                for (int sy = srcYStart; sy < srcYEnd && sy < tileSize; sy++)
                {
                    for (int sx = srcXStart; sx < srcXEnd && sx < tileSize; sx++)
                    {
                        int i = (sy * tileSize + sx) * 4;

                        byte b = pixelData[i];
                        byte g = pixelData[i + 1];
                        byte r = pixelData[i + 2];

                        sum += 0.299f * r + 0.587f * g + 0.114f * b;
                        count++;
                    }
                }

                result[tx, ty] = count > 0 ? sum / count : 0f;
            }
        }

        return result;
    }
}
