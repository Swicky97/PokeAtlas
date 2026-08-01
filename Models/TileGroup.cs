using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;

namespace PokeAtlas.Models;

public class TileGroup
{
    [Browsable(false)]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Category("General")]
    public string Name { get; set; } = "New Group";

    [Category("General")]
    public string Category { get; set; } = "Uncategorized";

    [Category("Geometry")]
    public Rectangle TileBounds { get; set; }

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
