using PokeAtlas.Models;

namespace PokeAtlas.Controls;

public partial class AutoDetectForm : Form
{
    private readonly Bitmap _tileset;
    private readonly int _tileSize;
    private readonly ImageList _imageList = new() { ImageSize = new Size(32, 32), ColorDepth = ColorDepth.Depth32Bit };

    private List<DetectedRegion> _regions;

    public event Action<Rectangle>? RegionAccepted;

    public AutoDetectForm(Bitmap tileset, List<DetectedRegion> regions, int tileSize)
    {
        InitializeComponent();

        _tileset = tileset;
        _tileSize = tileSize;
        _regions = regions;

        listViewRegions.LargeImageList = _imageList;

        PopulateList();
    }

    public void RemoveRegion(Rectangle tileBounds)
    {
        _regions = _regions.Where(r => r.TileBounds != tileBounds).ToList();

        PopulateList();
    }

    private void PopulateList()
    {
        listViewRegions.BeginUpdate();
        listViewRegions.Items.Clear();
        _imageList.Images.Clear();

        foreach (DetectedRegion region in _regions)
        {
            Rectangle sourceRect = new(
                region.TileBounds.X * _tileSize,
                region.TileBounds.Y * _tileSize,
                region.TileBounds.Width * _tileSize,
                region.TileBounds.Height * _tileSize);

            _imageList.Images.Add(TileThumbnail.Create(_tileset, sourceRect));

            ListViewItem item = new($"{region.TileBounds.Width}x{region.TileBounds.Height}")
            {
                ImageIndex = _imageList.Images.Count - 1,
                Tag = region
            };

            listViewRegions.Items.Add(item);
        }

        lblSummary.Text = _regions.Count == 0
            ? "No undetected regions remaining."
            : $"{_regions.Count} candidate region(s) found — double-click one to name and add it.";

        listViewRegions.EndUpdate();
    }

    private void listViewRegions_DoubleClick(object sender, EventArgs e)
    {
        if (listViewRegions.SelectedItems.Count == 0)
            return;

        if (listViewRegions.SelectedItems[0].Tag is not DetectedRegion region)
            return;

        RegionAccepted?.Invoke(region.TileBounds);
    }
}
