using PokeAtlas.Controls;
using PokeAtlas.Models;
using PokeAtlas.Services;

namespace PokeAtlas;

public partial class MainForm : Form
{
    private readonly TilesetCanvas _tilesetCanvas = new();

    private readonly GroupService _groupService = new();

    private readonly MetadataService _metadataService = new();

    private readonly AtlasBuilderService _atlasBuilderService = new();

    private readonly DuplicateDetectionService _duplicateDetectionService = new();

    private readonly AutoDetectionService _autoDetectionService = new();

    private readonly TileDatabaseService _tileDatabaseService = new();

    private readonly SimilarityService _similarityService = new();

    private DuplicatesForm? _duplicatesForm;

    private DuplicatesForm? _similarTilesForm;

    private AtlasPreviewForm? _atlasPreviewForm;

    private TileBrowserForm? _tileBrowserForm;

    private AutoDetectForm? _autoDetectForm;

    public MainForm()
    {
        InitializeComponent();

        InitializeCanvas();
    }

    private void InitializeCanvas()
    {
        _tilesetCanvas.Dock = DockStyle.Fill;

        innerSplitContainer.Panel1.Controls.Add(_tilesetCanvas);
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

            LoadMetadataIfPresent();
        }
    }

    private string? GetMetadataPath()
    {
        if (_tilesetCanvas.TilesetPath is not { } tilesetPath)
            return null;

        string? directory = Path.GetDirectoryName(tilesetPath);

        return directory is null ? null : Path.Combine(directory, "metadata.json");
    }

    private void LoadMetadataIfPresent()
    {
        string? metadataPath = GetMetadataPath();

        if (metadataPath is null || !File.Exists(metadataPath))
            return;

        try
        {
            List<TileGroup> groups = _metadataService.Load(metadataPath);

            _groupService.Clear();

            foreach (TileGroup group in groups)
                _groupService.Add(group);

            RefreshTreeView();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to load metadata.json:\n{ex.Message}", "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void saveToolStripButton_Click(object sender, EventArgs e)
    {
        string? metadataPath = GetMetadataPath();

        if (metadataPath is null)
        {
            MessageBox.Show("Please open a tileset first.");
            return;
        }

        try
        {
            _metadataService.Save(metadataPath, _groupService.Groups);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to save metadata.json:\n{ex.Message}", "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void buildToolStripButton_Click(object sender, EventArgs e)
    {
        if (_tilesetCanvas.Tileset is not { } tileset)
        {
            MessageBox.Show("Please open a tileset first.");
            return;
        }

        if (_groupService.Groups.Count == 0)
        {
            MessageBox.Show("Please add at least one group first.");
            return;
        }

        _atlasPreviewForm?.Close();

        AtlasBuildResult result = _atlasBuilderService.Build(tileset, _groupService.Groups, TilesetCanvas.TileSize);

        _atlasPreviewForm = new AtlasPreviewForm(result, TilesetCanvas.TileSize);
        _atlasPreviewForm.FormClosed += (_, _) => _atlasPreviewForm = null;
        _atlasPreviewForm.GroupSelected += group => SelectNodeForGroup(group);

        _atlasPreviewForm.Show(this);
    }

    private void duplicatesToolStripButton_Click(object sender, EventArgs e)
    {
        if (_tilesetCanvas.Tileset is not { } tileset)
        {
            MessageBox.Show("Please open a tileset first.");
            return;
        }

        if (_duplicatesForm is { IsDisposed: false })
        {
            _duplicatesForm.Activate();
            return;
        }

        Cursor = Cursors.WaitCursor;
        List<DuplicateTileGroup> duplicates;

        try
        {
            duplicates = _duplicateDetectionService.FindDuplicates(tileset, TilesetCanvas.TileSize);
        }
        finally
        {
            Cursor = Cursors.Default;
        }

        _duplicatesForm = new DuplicatesForm(tileset, duplicates, TilesetCanvas.TileSize);
        _duplicatesForm.TileSelected += bounds => _tilesetCanvas.CenterOnBounds(bounds);
        _duplicatesForm.Show(this);
    }

    private void browserToolStripButton_Click(object sender, EventArgs e)
    {
        if (_tileBrowserForm is { IsDisposed: false })
        {
            _tileBrowserForm.Activate();
            return;
        }

        _tileBrowserForm = new TileBrowserForm(TilesetCanvas.TileSize);
        _tileBrowserForm.FormClosed += (_, _) => _tileBrowserForm = null;
        _tileBrowserForm.GroupSelected += group => SelectNodeForGroup(group);
        _tileBrowserForm.SetSource(_tilesetCanvas.Tileset, _groupService.Groups);

        _tileBrowserForm.Show(this);
    }

    private void autoDetectToolStripButton_Click(object sender, EventArgs e)
    {
        if (_tilesetCanvas.Tileset is not { } tileset)
        {
            MessageBox.Show("Please open a tileset first.");
            return;
        }

        if (_autoDetectForm is { IsDisposed: false })
        {
            _autoDetectForm.Activate();
            return;
        }

        Cursor = Cursors.WaitCursor;
        List<DetectedRegion> regions;

        try
        {
            regions = _autoDetectionService.DetectRegions(tileset, TilesetCanvas.TileSize, _groupService.Groups);
        }
        finally
        {
            Cursor = Cursors.Default;
        }

        _autoDetectForm = new AutoDetectForm(tileset, regions, TilesetCanvas.TileSize);
        _autoDetectForm.FormClosed += (_, _) => _autoDetectForm = null;
        _autoDetectForm.RegionAccepted += bounds =>
        {
            if (CreateGroupFromDialog(bounds) is not null)
                _autoDetectForm?.RemoveRegion(bounds);
        };

        _autoDetectForm.Show(this);
    }

    private void similarToolStripButton_Click(object sender, EventArgs e)
    {
        if (_tilesetCanvas.Tileset is not { } tileset)
        {
            MessageBox.Show("Please open a tileset first.");
            return;
        }

        if (_similarTilesForm is { IsDisposed: false })
        {
            _similarTilesForm.Activate();
            return;
        }

        Cursor = Cursors.WaitCursor;
        List<DuplicateTileGroup> similarGroups;

        try
        {
            List<Tile> tiles = _tileDatabaseService.BuildDatabase(tileset, TilesetCanvas.TileSize);

            similarGroups = _similarityService
                .FindSimilarGroups(tiles)
                .Select(cluster => new DuplicateTileGroup
                {
                    TileSize = TilesetCanvas.TileSize,
                    Positions = cluster.Select(t => t.Position).ToList()
                })
                .ToList();
        }
        finally
        {
            Cursor = Cursors.Default;
        }

        _similarTilesForm = new DuplicatesForm(tileset, similarGroups, TilesetCanvas.TileSize, "Similar Tiles", "similar");
        _similarTilesForm.TileSelected += bounds => _tilesetCanvas.CenterOnBounds(bounds);

        _similarTilesForm.Show(this);
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
            MessageBox.Show("Please select a region first.");
            return;
        }

        if (CreateGroupFromDialog(selection.Value) is not null)
            _tilesetCanvas.ClearSelection();
    }

    private TileGroup? CreateGroupFromDialog(Rectangle tileBounds)
    {
        using AddGroupForm dialog = new();

        if (dialog.ShowDialog(this) != DialogResult.OK)
            return null;

        TileGroup group = new()
        {
            Name = dialog.GroupName,
            Category = dialog.Category,
            TileBounds = tileBounds,
            SourceAtlas = _tilesetCanvas.TilesetPath is { } path ? Path.GetFileName(path) : string.Empty
        };

        _groupService.Add(group);

        RefreshTreeView();
        SelectNodeForGroup(group);

        return group;
    }

    private void deleteToolStripButton_Click(object sender, EventArgs e)
    {
        if (treeViewGroups.SelectedNode?.Tag is not TileGroup group)
        {
            MessageBox.Show("Please select a group to delete.");
            return;
        }

        _groupService.Remove(group);

        propertiesGrid.SelectedObject = null;
        _tilesetCanvas.SelectGroup(null);

        RefreshTreeView();
    }

    private void propertiesGrid_PropertyValueChanged(object sender, PropertyValueChangedEventArgs e)
    {
        if (propertiesGrid.SelectedObject is not TileGroup group)
            return;

        RefreshTreeView();
        SelectNodeForGroup(group);
    }

    private void SelectNodeForGroup(TileGroup group)
    {
        foreach (TreeNode categoryNode in treeViewGroups.Nodes)
        {
            foreach (TreeNode node in categoryNode.Nodes)
            {
                if (node.Tag == group)
                {
                    treeViewGroups.SelectedNode = node;
                    return;
                }
            }
        }
    }

    private void searchToolStripTextBox_TextChanged(object sender, EventArgs e)
    {
        RefreshTreeView();
    }

    private void RefreshTreeView()
    {
        string searchText = searchToolStripTextBox.Text.Trim();

        IEnumerable<TileGroup> groups = _groupService.Groups;

        if (searchText.Length > 0)
            groups = groups.Where(g => TileGroupSearch.Matches(g, searchText));

        treeViewGroups.BeginUpdate();

        treeViewGroups.Nodes.Clear();

        foreach (var category in groups.GroupBy(g => g.Category))
        {
            TreeNode categoryNode = new(category.Key);

            foreach (TileGroup group in category)
            {
                TreeNode node = new(group.Name)
                {
                    Tag = group
                };

                categoryNode.Nodes.Add(node);
            }

            treeViewGroups.Nodes.Add(categoryNode);
        }

        treeViewGroups.ExpandAll();

        treeViewGroups.EndUpdate();

        _tileBrowserForm?.SetSource(_tilesetCanvas.Tileset, _groupService.Groups);
    }

    private void treeViewGroups_AfterSelect(object sender, TreeViewEventArgs e)
    {
        if (e.Node?.Tag is not TileGroup group)
        {
            propertiesGrid.SelectedObject = null;
            return;
        }

        propertiesGrid.SelectedObject = group;

        _tilesetCanvas.SelectGroup(group);
        _tilesetCanvas.CenterOnGroup(group);

        _atlasPreviewForm?.HighlightGroup(group);
    }
}