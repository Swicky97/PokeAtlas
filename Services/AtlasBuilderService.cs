using System.Drawing.Drawing2D;
using PokeAtlas.Models;

namespace PokeAtlas.Services;

public class AtlasBuilderService
{
    // 512px at a 16px tile size; grows automatically if a single group is wider than this.
    private const int MinAtlasWidthTiles = 32;

    // Every group reserves one blank tile row above it for the category header the preview draws.
    private const int HeaderTiles = 1;

    private readonly record struct RegionLayout(Rectangle Source, Point OffsetTiles);

    private readonly record struct GroupLayout(TileGroup Group, Size BlockSize, List<RegionLayout> Regions);

    public AtlasBuildResult Build(Bitmap sourceTileset, IEnumerable<TileGroup> groups, int tileSize)
    {
        List<GroupLayout> ordered = groups
            .Select(LayoutGroup)
            .OrderByDescending(g => g.BlockSize.Height)
            .ThenByDescending(g => g.BlockSize.Width)
            .ToList();

        int atlasWidthTiles = ordered.Count == 0
            ? MinAtlasWidthTiles
            : Math.Max(MinAtlasWidthTiles, ordered.Max(g => g.BlockSize.Width));

        SkylinePacker packer = new(atlasWidthTiles);

        var rawPlacements = new List<(GroupLayout Layout, Point ArtTile)>();

        foreach (GroupLayout layout in ordered)
        {
            Point slotTile = packer.Place(layout.BlockSize.Width, layout.BlockSize.Height + HeaderTiles);
            Point artTile = new(slotTile.X, slotTile.Y + HeaderTiles);

            rawPlacements.Add((layout, artTile));
        }

        Bitmap atlas = new(atlasWidthTiles * tileSize, Math.Max(packer.UsedHeight, 1) * tileSize);

        using (Graphics g = Graphics.FromImage(atlas))
        {
            g.Clear(Color.FromArgb(45, 45, 48));
            g.InterpolationMode = InterpolationMode.NearestNeighbor;

            foreach (var (layout, artTile) in rawPlacements)
            {
                foreach (RegionLayout region in layout.Regions)
                {
                    Rectangle sourcePixels = new(
                        region.Source.X * tileSize,
                        region.Source.Y * tileSize,
                        region.Source.Width * tileSize,
                        region.Source.Height * tileSize);

                    Rectangle destinationPixels = new(
                        (artTile.X + region.OffsetTiles.X) * tileSize,
                        (artTile.Y + region.OffsetTiles.Y) * tileSize,
                        sourcePixels.Width,
                        sourcePixels.Height);

                    g.DrawImage(sourceTileset, destinationPixels, sourcePixels, GraphicsUnit.Pixel);
                }
            }
        }

        List<AtlasPlacement> placements = rawPlacements
            .Select(p => new AtlasPlacement
            {
                Group = p.Layout.Group,
                TileBounds = new Rectangle(p.ArtTile.X, p.ArtTile.Y, p.Layout.BlockSize.Width, p.Layout.BlockSize.Height)
            })
            .ToList();

        return new AtlasBuildResult
        {
            Atlas = atlas,
            Placements = placements
        };
    }

    // A single region lays out as-is. Multiple regions (from accepting a scattered
    // duplicate/similar/cluster suggestion, always one tile each) are grid-packed into one
    // contiguous block so the group still reads as a single clean chunk in the built atlas.
    private static GroupLayout LayoutGroup(TileGroup group)
    {
        List<Rectangle> regions = group.Regions;

        if (regions.Count <= 1)
        {
            Rectangle region = regions.Count == 1 ? regions[0] : Rectangle.Empty;

            List<RegionLayout> singleLayout = regions.Count == 1
                ? new List<RegionLayout> { new(region, Point.Empty) }
                : new List<RegionLayout>();

            return new GroupLayout(group, new Size(region.Width, region.Height), singleLayout);
        }

        int cols = (int)Math.Ceiling(Math.Sqrt(regions.Count));
        int rows = (int)Math.Ceiling(regions.Count / (double)cols);

        List<RegionLayout> layouts = regions
            .Select((region, i) => new RegionLayout(region, new Point(i % cols, i / cols)))
            .ToList();

        return new GroupLayout(group, new Size(cols, rows), layouts);
    }
}
