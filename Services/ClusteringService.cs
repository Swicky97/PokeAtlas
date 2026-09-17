using PokeAtlas.Models;

namespace PokeAtlas.Services;

// Density-based clustering (DBSCAN) over perceptual-hash distance. Unlike SimilarityService's
// union-find, which chains transitively (A~B~C get merged even if A and C alone aren't alike),
// DBSCAN only grows a cluster from core points that have at least minPts neighbors, and leaves
// sparse/one-off tiles unclustered as noise instead of forcing them into a group.
public class ClusteringService
{
    private const int DefaultEps = 6;
    private const int DefaultMinPts = 3;

    public List<List<Tile>> ClusterTiles(IReadOnlyList<Tile> tiles, int eps = DefaultEps, int minPts = DefaultMinPts)
    {
        // A blank tile isn't a meaningful "similar structure" -- and since blank tiles all
        // look alike, including them would merge unrelated clusters into one giant blob.
        List<Tile> candidates = tiles.Where(t => !t.IsTransparent).ToList();

        List<int>[] neighbors = new List<int>[candidates.Count];

        for (int i = 0; i < candidates.Count; i++)
        {
            neighbors[i] = new List<int>();

            for (int j = 0; j < candidates.Count; j++)
            {
                if (i != j && candidates[i].PerceptualHash.DistanceTo(candidates[j].PerceptualHash) <= eps)
                    neighbors[i].Add(j);
            }
        }

        // 0 = unvisited, -1 = noise, >0 = cluster id.
        int[] label = new int[candidates.Count];
        int clusterId = 0;

        for (int i = 0; i < candidates.Count; i++)
        {
            if (label[i] != 0)
                continue;

            if (neighbors[i].Count + 1 < minPts)
            {
                label[i] = -1;
                continue;
            }

            clusterId++;
            label[i] = clusterId;

            Queue<int> seeds = new(neighbors[i]);

            while (seeds.Count > 0)
            {
                int q = seeds.Dequeue();

                if (label[q] == -1)
                    label[q] = clusterId;

                if (label[q] != 0)
                    continue;

                label[q] = clusterId;

                if (neighbors[q].Count + 1 >= minPts)
                {
                    foreach (int n in neighbors[q])
                        seeds.Enqueue(n);
                }
            }
        }

        return Enumerable.Range(1, clusterId)
            .Select(id => Enumerable.Range(0, candidates.Count)
                .Where(i => label[i] == id)
                .Select(i => candidates[i])
                .ToList())
            .OrderByDescending(g => g.Count)
            .ToList();
    }
}
