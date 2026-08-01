using PokeAtlas.Models;
using PokeAtlas.Services;

namespace PokeAtlas.Controls;

public partial class AtlasPreviewForm : Form
{
    private readonly AtlasBuildResult _result;
    private readonly int _tileSize;
    private readonly AtlasExportService _exportService = new();
    private readonly AtlasPreviewCanvas _canvas;

    public event Action<TileGroup>? GroupSelected;

    public AtlasPreviewForm(AtlasBuildResult result, int tileSize)
    {
        InitializeComponent();

        _result = result;
        _tileSize = tileSize;

        _canvas = new AtlasPreviewCanvas(result.Atlas, result.Placements, tileSize);
        _canvas.GroupClicked += group => GroupSelected?.Invoke(group);

        panelScroll.Controls.Add(_canvas);

        FormClosed += (_, _) => _result.Atlas.Dispose();
    }

    public void HighlightGroup(TileGroup group)
    {
        _canvas.SelectGroup(group);

        if (_canvas.GetGroupPixelBounds(group) is not { } bounds)
            return;

        int targetX = Math.Max(0, bounds.X + bounds.Width / 2 - panelScroll.ClientSize.Width / 2);
        int targetY = Math.Max(0, bounds.Y + bounds.Height / 2 - panelScroll.ClientSize.Height / 2);

        panelScroll.AutoScrollPosition = new Point(targetX, targetY);
    }

    private void btnExport_Click(object sender, EventArgs e)
    {
        using FolderBrowserDialog dialog = new()
        {
            Description = "Choose an export folder"
        };

        if (dialog.ShowDialog(this) != DialogResult.OK)
            return;

        try
        {
            _exportService.Export(dialog.SelectedPath, _result.Atlas, _result.Placements, _tileSize);

            MessageBox.Show(
                $"Exported MasterAtlas.png, MasterAtlas.tsx, and metadata.json to:\n{dialog.SelectedPath}",
                "Export Complete",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Export failed:\n{ex.Message}", "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
