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
            filtered = filtered.Where(g => TileGroupSearch.Matches(g, searchText));

        PopulateList(filtered.ToList());
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
                Rectangle sourceRect = new(
                    group.TileBounds.X * _tileSize,
                    group.TileBounds.Y * _tileSize,
                    group.TileBounds.Width * _tileSize,
                    group.TileBounds.Height * _tileSize);

                _imageList.Images.Add(TileThumbnail.Create(tileset, sourceRect));

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

    private void listViewTiles_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (listViewTiles.SelectedItems.Count == 0)
            return;

        if (listViewTiles.SelectedItems[0].Tag is not TileGroup group)
            return;

        GroupSelected?.Invoke(group);
    }
}
