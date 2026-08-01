using PokeAtlas.Models;
using PokeAtlas.Services;

namespace PokeAtlas.Controls;

public partial class AtlasPreviewForm : Form
{
    private readonly AtlasBuildResult _result;
    private readonly int _tileSize;
    private readonly AtlasExportService _exportService = new();

    public AtlasPreviewForm(AtlasBuildResult result, int tileSize)
    {
        InitializeComponent();

        _result = result;
        _tileSize = tileSize;

        AtlasPreviewCanvas canvas = new(result.Atlas, result.Placements, tileSize);

        panelScroll.Controls.Add(canvas);
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
