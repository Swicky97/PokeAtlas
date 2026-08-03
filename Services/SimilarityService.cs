using PokeAtlas.Models;

namespace PokeAtlas.Services;

// Sub-phase 3 of the Phase 12 pipeline: groups tiles that look visually similar (not
// necessarily byte-identical) using perceptual-hash Hamming distance. Unlike exact-hash
// grouping, "similar" isn't a clean equality bucket, so tiles are unioned transitively --
// A similar to B and B similar to C puts all three in one cluster, even if A and C alone
// wouldn't have matched.
public class SimilarityService
{
    private const int DefaultMaxDistance = 6;

    public List<List<Tile>> FindSimilarGroups(IReadOnlyList<Tile> tiles, int maxDistance = DefaultMaxDistance)
    {
        // A blank tile isn't a meaningful "similar structure" -- and since blank tiles all
        // look alike, including them would merge unrelated clusters into one giant blob.
        List<Tile> candidates = tiles.Where(t => !t.IsTransparent).ToList();

        int[] parent = Enumerable.Range(0, candidates.Count).ToArray();

        int Find(int i)
        {
            while (parent[i] != i)
            {
                parent[i] = parent[parent[i]];
                i = parent[i];
            }

            return i;
        }

        void Union(int a, int b)
        {
            int rootA = Find(a);
            int rootB = Find(b);

            if (rootA != rootB)
                parent[rootA] = rootB;
        }

        for (int i = 0; i < candidates.Count; i++)
        {
            for (int j = i + 1; j < candidates.Count; j++)
            {
                if (candidates[i].PerceptualHash.DistanceTo(candidates[j].PerceptualHash) <= maxDistance)
                    Union(i, j);
            }
        }

        return Enumerable.Range(0, candidates.Count)
            .GroupBy(Find)
            .Where(g => g.Count() > 1)
            .Select(g => g.Select(i => candidates[i]).ToList())
            .OrderByDescending(g => g.Count)
            .ToList();
    }
}
