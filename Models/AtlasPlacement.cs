namespace PokeAtlas.Models;

public class AtlasPlacement
{
    public required TileGroup Group { get; init; }

    public required Rectangle TileBounds { get; init; }
}
