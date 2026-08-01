using System.Text.Json;
using PokeAtlas.Models;

namespace PokeAtlas.Services;

public class MetadataService
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    public void Save(string metadataPath, IEnumerable<TileGroup> groups)
    {
        AtlasMetadata metadata = new() { Groups = groups.ToList() };

        string json = JsonSerializer.Serialize(metadata, Options);

        File.WriteAllText(metadataPath, json);
    }

    public List<TileGroup> Load(string metadataPath)
    {
        string json = File.ReadAllText(metadataPath);

        AtlasMetadata? metadata = JsonSerializer.Deserialize<AtlasMetadata>(json, Options);

        return metadata?.Groups ?? new List<TileGroup>();
    }
}
