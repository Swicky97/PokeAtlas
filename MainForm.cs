using PokeAtlas.Controls;
using PokeAtlas.Models;

namespace PokeAtlas;

public partial class MainForm : Form
{
    private readonly TilesetCanvas _tilesetCanvas = new();

    private readonly List<TileGroup> _groups = new();

    public MainForm()
    {
        InitializeComponent();

        InitializeCanvas();
    }

    private void InitializeCanvas()
    {
        _tilesetCanvas.Dock = DockStyle.Fill;

        splitContainerInner.Panel1.Controls.Add(_tilesetCanvas);
    }

    private void OpenTileset()
    {
        using OpenFileDialog dialog = new()
        {
            Filter = "PNG Files (*.png)|*.png"
        };

        if (dialog.ShowDialog() == DialogResult.OK)
        {
            _tilesetCanvas.LoadTileset(dialog.FileName);
        }
    }

    private void openToolStripMenuItem_Click(object sender, EventArgs e)
    {
        OpenTileset();
    }

    private void openToolStripButton_Click(object sender, EventArgs e)
    {
        OpenTileset();
    }

    private void addGroupToolStripButton_Click(object sender, EventArgs e)
    {
        Rectangle? selection = _tilesetCanvas.SelectedTileRectangle;

        if (selection == null)
        {
            MessageBox.Show(
                "Please select a region first.",
                "No Selection",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            return;
        }

        MessageBox.Show(selection.Value.ToString());
    }


}