using System.Numerics;

namespace PokeAtlas.Models;

// Combines a structural (edge/gradient) hash with average color into one comparable value.
// Structural distance alone treats any two flat, single-color tiles as identical -- there's no
// internal gradient to compare -- which is wrong for pixel art, where large flat-colored areas
// are the norm, not the exception. Color has to factor into "similar" too, not just edges.
public readonly record struct PerceptualHash(ulong Structure, byte AverageR, byte AverageG, byte AverageB)
{
    public int DistanceTo(PerceptualHash other)
    {
        int structuralDistance = BitOperations.PopCount(Structure ^ other.Structure);

        int colorDistance =
            Math.Abs(AverageR - other.AverageR) +
            Math.Abs(AverageG - other.AverageG) +
            Math.Abs(AverageB - other.AverageB);

        // Structural distance maxes at 64 (bits); color distance maxes at 765 (3 channels x
        // 255). Scale color down to a comparable range so neither term dominates the other.
        return structuralDistance + colorDistance / 12;
    }
}
