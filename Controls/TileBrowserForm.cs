using System.Drawing.Drawing2D;
using PokeAtlas.Models;

namespace PokeAtlas.Controls;

public partial class TileBrowserForm : Form
{
    private readonly ImageList _imageList = new() { ImageSize = new Size(32, 32), ColorDepth = ColorDepth.Depth32Bit };
    private readonly int _tileSize;

    private List<TileGroup> _allGroups = new();
    private Bitmap? _tileset;

    public event Action<TileGroup>? GroupSelected;

    public TileBrowserForm(int tileSize)
    {
        InitializeComponent();

        _tileSize = tileSize;

        listViewTiles.LargeImageList = _imageList;
    }

    public void SetSource(Bitmap? tileset, IReadOnlyList<TileGroup> groups)
    {
        _tileset = tileset;
        _allGroups = groups.ToList();

        ApplyFilter();
    }

    private void txtSearch_TextChanged(object sender, EventArgs e)
    {
        ApplyFilter();
    }

    private void ApplyFilter()
    {
        string searchText = txtSearch.Text.Trim();

        IEnumerable<TileGroup> filtered = _allGroups;

        if (searchText.Length > 0)
            filtered = filtered.Where(g => MatchesSearch(g, searchText));

        PopulateList(filtered.ToList());
    }

    private static bool MatchesSearch(TileGroup group, string searchText)
    {
        return group.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase)
            || group.Category.Contains(searchText, StringComparison.OrdinalIgnoreCase)
            || group.Tags.Any(tag => tag.Contains(searchText, StringComparison.OrdinalIgnoreCase));
    }

    private void PopulateList(List<TileGroup> groups)
    {
        listViewTiles.BeginUpdate();
        listViewTiles.Items.Clear();
        _imageList.Images.Clear();

        if (_tileset is { } tileset)
        {
            foreach (TileGroup group in groups)
            {
                _imageList.Images.Add(CreateThumbnail(tileset, group));

                ListViewItem item = new(group.Name)
                {
                    ImageIndex = _imageList.Images.Count - 1,
                    Tag = group
                };

                listViewTiles.Items.Add(item);
            }
        }

        lblSummary.Text = $"{groups.Count} group(s)";

        listViewTiles.EndUpdate();
    }

    private Bitmap CreateThumbnail(Bitmap tileset, TileGroup group)
    {
        Rectangle sourceRect = new(
            group.TileBounds.X * _tileSize,
            group.TileBounds.Y * _tileSize,
            group.TileBounds.Width * _tileSize,
            group.TileBounds.Height * _tileSize);

        Bitmap thumb = new(32, 32);

        using Graphics g = Graphics.FromImage(thumb);

        g.InterpolationMode = InterpolationMode.NearestNeighbor;
        g.Clear(Color.FromArgb(45, 45, 48));

        // Preserve aspect ratio instead of stretching non-square groups into a 32x32 square.
        float scale = Math.Min(32f / sourceRect.Width, 32f / sourceRect.Height);
        int drawWidth = Math.Max(1, (int)(sourceRect.Width * scale));
        int drawHeight = Math.Max(1, (int)(sourceRect.Height * scale));
        int offsetX = (32 - drawWidth) / 2;
        int offsetY = (32 - drawHeight) / 2;

        g.DrawImage(tileset, new Rectangle(offsetX, offsetY, drawWidth, drawHeight), sourceRect, GraphicsUnit.Pixel);

        return thumb;
    }

    private void listViewTiles_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (listViewTiles.SelectedItems.Count == 0)
            return;

        if (listViewTiles.SelectedItems[0].Tag is not TileGroup group)
            return;

        GroupSelected?.Invoke(group);
    }
}
