using System.Drawing.Imaging;
using System.Text;
using System.Xml;
using PokeAtlas.Models;

namespace PokeAtlas.Services;

public class AtlasExportService
{
    private readonly MetadataService _metadataService = new();

    public void Export(string outputDirectory, Bitmap atlas, IReadOnlyList<AtlasPlacement> placements, int tileSize)
    {
        string pngPath = Path.Combine(outputDirectory, "MasterAtlas.png");
        string tsxPath = Path.Combine(outputDirectory, "MasterAtlas.tsx");
        string metadataPath = Path.Combine(outputDirectory, "metadata.json");

        atlas.Save(pngPath, ImageFormat.Png);

        WriteTsx(tsxPath, atlas.Width, atlas.Height, tileSize);

        List<TileGroup> exportedGroups = placements
            .Select(p => CloneWithBounds(p.Group, p.TileBounds))
            .ToList();

        _metadataService.Save(metadataPath, exportedGroups);
    }

    private static TileGroup CloneWithBounds(TileGroup group, Rectangle bounds)
    {
        return new TileGroup
        {
            Id = group.Id,
            Name = group.Name,
            Category = group.Category,
            TileBounds = bounds,
            Tags = new List<string>(group.Tags),
            Notes = group.Notes,
            SourceAtlas = "MasterAtlas.png"
        };
    }

    private static void WriteTsx(string tsxPath, int atlasWidth, int atlasHeight, int tileSize)
    {
        int columns = atlasWidth / tileSize;
        int rows = atlasHeight / tileSize;

        XmlWriterSettings settings = new()
        {
            Indent = true,
            Encoding = new UTF8Encoding(false)
        };

        using XmlWriter writer = XmlWriter.Create(tsxPath, settings);

        writer.WriteStartDocument();
        writer.WriteStartElement("tileset");
        writer.WriteAttributeString("version", "1.10");
        writer.WriteAttributeString("tiledversion", "1.11.0");
        writer.WriteAttributeString("name", "MasterAtlas");
        writer.WriteAttributeString("tilewidth", tileSize.ToString());
        writer.WriteAttributeString("tileheight", tileSize.ToString());
        writer.WriteAttributeString("tilecount", (columns * rows).ToString());
        writer.WriteAttributeString("columns", columns.ToString());

        writer.WriteStartElement("image");
        writer.WriteAttributeString("source", "MasterAtlas.png");
        writer.WriteAttributeString("width", atlasWidth.ToString());
        writer.WriteAttributeString("height", atlasHeight.ToString());
        writer.WriteEndElement();

        writer.WriteEndElement();
        writer.WriteEndDocument();
    }
}
