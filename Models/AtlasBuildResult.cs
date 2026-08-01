namespace PokeAtlas.Models;

public class AtlasBuildResult
{
    public required Bitmap Atlas { get; init; }

    public required IReadOnlyList<AtlasPlacement> Placements { get; init; }
}
