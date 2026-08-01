using PokeAtlas.Controls;
using PokeAtlas.Models;
using PokeAtlas.Services;

namespace PokeAtlas;

public partial class MainForm : Form
{
    private readonly TilesetCanvas _tilesetCanvas = new();

    private readonly GroupService _groupService = new();

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
            MessageBox.Show("Please select a region first.");
            return;
        }

        using AddGroupForm dialog = new();

        if (dialog.ShowDialog() != DialogResult.OK)
            return;

        TileGroup group = new()
        {
            Name = dialog.GroupName,
            Category = dialog.Category,
            TileBounds = selection.Value,
            SourceAtlas = _tilesetCanvas.TilesetPath is { } path ? Path.GetFileName(path) : string.Empty
        };

        _groupService.Add(group);

        _tilesetCanvas.ClearSelection();

        RefreshTreeView();
        SelectNodeForGroup(group);
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

    private void RefreshTreeView()
    {
        treeViewGroups.BeginUpdate();

        treeViewGroups.Nodes.Clear();

        foreach (var category in _groupService.Groups.GroupBy(g => g.Category))
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
    }
}