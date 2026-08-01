namespace PokeAtlas.Models;

public class DuplicateTileGroup
{
    public required int TileSize { get; init; }

    public required List<Point> Positions { get; init; }
}
