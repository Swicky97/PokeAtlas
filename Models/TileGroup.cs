namespace PokeAtlas.Models;

public class TileGroup
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = "New Group";

    public string Category { get; set; } = "Uncategorized";

    public Rectangle TileBounds { get; set; }

    public override string ToString()
    {
        return Name;
    }
}
