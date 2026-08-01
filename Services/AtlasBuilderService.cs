using System.Drawing.Drawing2D;
using PokeAtlas.Models;

namespace PokeAtlas.Services;

public class AtlasBuilderService
{
    // 512px at a 16px tile size; grows automatically if a single group is wider than this.
    private const int MinAtlasWidthTiles = 32;

    // Every group reserves one blank tile row above it for the category header the preview draws.
    private const int HeaderTiles = 1;

    public AtlasBuildResult Build(Bitmap sourceTileset, IEnumerable<TileGroup> groups, int tileSize)
    {
        List<TileGroup> ordered = groups
            .OrderByDescending(g => g.TileBounds.Height)
            .ThenByDescending(g => g.TileBounds.Width)
            .ToList();

        int atlasWidthTiles = ordered.Count == 0
            ? MinAtlasWidthTiles
            : Math.Max(MinAtlasWidthTiles, ordered.Max(g => g.TileBounds.Width));

        SkylinePacker packer = new(atlasWidthTiles);

        var rawPlacements = new List<(TileGroup Group, Rectangle SourcePixels, Point ArtTile)>();

        foreach (TileGroup group in ordered)
        {
            Point slotTile = packer.Place(group.TileBounds.Width, group.TileBounds.Height + HeaderTiles);
            Point artTile = new(slotTile.X, slotTile.Y + HeaderTiles);

            Rectangle sourcePixels = new(
                group.TileBounds.X * tileSize,
                group.TileBounds.Y * tileSize,
                group.TileBounds.Width * tileSize,
                group.TileBounds.Height * tileSize);

            rawPlacements.Add((group, sourcePixels, artTile));
        }

        Bitmap atlas = new(atlasWidthTiles * tileSize, Math.Max(packer.UsedHeight, 1) * tileSize);

        using (Graphics g = Graphics.FromImage(atlas))
        {
            g.Clear(Color.FromArgb(45, 45, 48));
            g.InterpolationMode = InterpolationMode.NearestNeighbor;

            foreach (var (group, sourcePixels, artTile) in rawPlacements)
            {
                Rectangle destinationPixels = new(
                    artTile.X * tileSize,
                    artTile.Y * tileSize,
                    sourcePixels.Width,
                    sourcePixels.Height);

                g.DrawImage(sourceTileset, destinationPixels, sourcePixels, GraphicsUnit.Pixel);
            }
        }

        List<AtlasPlacement> placements = rawPlacements
            .Select(p => new AtlasPlacement
            {
                Group = p.Group,
                TileBounds = new Rectangle(
                    p.ArtTile.X,
                    p.ArtTile.Y,
                    p.Group.TileBounds.Width,
                    p.Group.TileBounds.Height)
            })
            .ToList();

        return new AtlasBuildResult
        {
            Atlas = atlas,
            Placements = placements
        };
    }
}
