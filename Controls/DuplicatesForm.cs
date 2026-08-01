using System.Drawing.Drawing2D;
using PokeAtlas.Models;

namespace PokeAtlas.Controls;

public partial class DuplicatesForm : Form
{
    private readonly int _tileSize;

    public event Action<Rectangle>? TileSelected;

    public DuplicatesForm(Bitmap tileset, List<DuplicateTileGroup> duplicates, int tileSize)
    {
        InitializeComponent();

        _tileSize = tileSize;

        PopulateList(tileset, duplicates);
    }

    private void PopulateList(Bitmap tileset, List<DuplicateTileGroup> duplicates)
    {
        int totalRedundant = duplicates.Sum(d => d.Positions.Count - 1);

        lblSummary.Text = duplicates.Count == 0
            ? "No duplicate tiles found."
            : $"{duplicates.Count} duplicate tile group(s) found ({totalRedundant} redundant tile(s)).";

        ImageList imageList = new() { ImageSize = new Size(32, 32), ColorDepth = ColorDepth.Depth32Bit };
        listViewDuplicates.LargeImageList = imageList;

        foreach (DuplicateTileGroup group in duplicates)
        {
            Point first = group.Positions[0];
            Rectangle sourceRect = new(first.X * _tileSize, first.Y * _tileSize, _tileSize, _tileSize);

            Bitmap thumb = new(32, 32);

            using (Graphics g = Graphics.FromImage(thumb))
            {
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.DrawImage(tileset, new Rectangle(0, 0, 32, 32), sourceRect, GraphicsUnit.Pixel);
            }

            imageList.Images.Add(thumb);

            ListViewItem item = new($"{group.Positions.Count}x")
            {
                ImageIndex = imageList.Images.Count - 1,
                Tag = group
            };

            listViewDuplicates.Items.Add(item);
        }
    }

    private void listViewDuplicates_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (listViewDuplicates.SelectedItems.Count == 0)
            return;

        if (listViewDuplicates.SelectedItems[0].Tag is not DuplicateTileGroup group)
            return;

        Point first = group.Positions[0];

        TileSelected?.Invoke(new Rectangle(first.X, first.Y, 1, 1));
    }
}
