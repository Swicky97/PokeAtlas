using PokeAtlas.Controls;

namespace PokeAtlas;

public partial class MainForm : Form
{
    private readonly TilesetCanvas _tilesetCanvas = new();

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

    private void openToolStripMenuItem_Click(object sender, EventArgs e)
    {
        using OpenFileDialog dialog = new();

        dialog.Filter = "PNG Files (*.png)|*.png";

        if (dialog.ShowDialog() != DialogResult.OK)
            return;

        _tilesetCanvas.LoadTileset(dialog.FileName);
    }
}