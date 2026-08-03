namespace PokeAtlas.Models;

// Enables compound queries like "Large Red Roof": every space-separated term must match
// somewhere on the group (name, category, or any tag) for the group to match overall.
public static class TileGroupSearch
{
    public static bool Matches(TileGroup group, string searchText)
    {
        string[] terms = searchText.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        return terms.All(term => MatchesTerm(group, term));
    }

    private static bool MatchesTerm(TileGroup group, string term)
    {
        return group.Name.Contains(term, StringComparison.OrdinalIgnoreCase)
            || group.Category.Contains(term, StringComparison.OrdinalIgnoreCase)
            || group.Tags.Any(tag => tag.Contains(term, StringComparison.OrdinalIgnoreCase));
    }
}
