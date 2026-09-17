using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Text.Json.Serialization;

namespace PokeAtlas.Models;

[JsonConverter(typeof(TileGroupJsonConverter))]
public class TileGroup
{
    [Browsable(false)]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Category("General")]
    public string Name { get; set; } = "New Group";

    [Category("General")]
    public string Category { get; set; } = "Uncategorized";

    // Tile-grid coordinates in the source tileset. Usually one contiguous rectangle (manual
    // selection, auto-detected region), but detection review (duplicates/similar/clusters) can
    // accept a scattered set of same-tile positions into a single group, hence a list rather
    // than one Rectangle.
    [Category("Geometry")]
    public List<Rectangle> Regions { get; set; } = new();

    [Browsable(false)]
    public Rectangle BoundingBox => Regions.Count == 0
        ? Rectangle.Empty
        : Regions.Skip(1).Aggregate(Regions[0], Rectangle.Union);

    [Category("Metadata")]
    public List<string> Tags { get; set; } = new();

    [Category("Metadata")]
    [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
    public string Notes { get; set; } = string.Empty;

    [Category("Metadata")]
    [ReadOnly(true)]
    public string SourceAtlas { get; set; } = string.Empty;

    public override string ToString()
    {
        return Name;
    }
}
