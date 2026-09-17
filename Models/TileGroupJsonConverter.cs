using System.Text.Json;
using System.Text.Json.Serialization;

namespace PokeAtlas.Models;

// Hand-written rather than attribute-driven because Regions needs to accept two shapes on
// read: today's "regions" array, or an older single-rectangle "bounds" object from
// metadata.json files saved before groups could hold scattered tile positions.
public class TileGroupJsonConverter : JsonConverter<TileGroup>
{
    public override TileGroup Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        TileGroup group = new() { Regions = new List<Rectangle>() };

        while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
        {
            string propertyName = reader.GetString()!;
            reader.Read();

            switch (propertyName.ToLowerInvariant())
            {
                case "id":
                    group.Id = reader.GetGuid();
                    break;
                case "name":
                    group.Name = reader.GetString() ?? group.Name;
                    break;
                case "category":
                    group.Category = reader.GetString() ?? group.Category;
                    break;
                case "regions":
                    List<Rectangle> regions = new();

                    while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                        regions.Add(RectangleJsonConverter.ReadRectangle(ref reader));

                    group.Regions = regions;
                    break;
                case "bounds":
                    group.Regions = new List<Rectangle> { RectangleJsonConverter.ReadRectangle(ref reader) };
                    break;
                case "tags":
                    group.Tags = JsonSerializer.Deserialize<List<string>>(ref reader, options) ?? group.Tags;
                    break;
                case "notes":
                    group.Notes = reader.GetString() ?? group.Notes;
                    break;
                case "sourceatlas":
                    group.SourceAtlas = reader.GetString() ?? group.SourceAtlas;
                    break;
                default:
                    reader.Skip();
                    break;
            }
        }

        return group;
    }

    public override void Write(Utf8JsonWriter writer, TileGroup value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WriteString("id", value.Id);
        writer.WriteString("name", value.Name);
        writer.WriteString("category", value.Category);

        writer.WriteStartArray("regions");
        foreach (Rectangle region in value.Regions)
            RectangleJsonConverter.WriteRectangle(writer, region);
        writer.WriteEndArray();

        writer.WritePropertyName("tags");
        JsonSerializer.Serialize(writer, value.Tags, options);

        writer.WriteString("notes", value.Notes);
        writer.WriteString("sourceAtlas", value.SourceAtlas);

        writer.WriteEndObject();
    }
}
