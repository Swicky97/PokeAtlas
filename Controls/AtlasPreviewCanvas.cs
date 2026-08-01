using System.Drawing.Drawing2D;
using PokeAtlas.Models;

namespace PokeAtlas.Controls;

internal class AtlasPreviewCanvas : Control
{
    private readonly Bitmap _atlas;
    private readonly IReadOnlyList<AtlasPlacement> _placements;
    private readonly int _tileSize;

    private TileGroup? _selectedGroup;

    public event Action<TileGroup>? GroupClicked;

    public AtlasPreviewCanvas(Bitmap atlas, IReadOnlyList<AtlasPlacement> placements, int tileSize)
    {
        _atlas = atlas;
        _placements = placements;
        _tileSize = tileSize;

        DoubleBuffered = true;
        Size = atlas.Size;
    }

    public void SelectGroup(TileGroup? group)
    {
        _selectedGroup = group;
        Invalidate();
    }

    public Rectangle? GetGroupPixelBounds(TileGroup group)
    {
        AtlasPlacement? placement = _placements.FirstOrDefault(p => p.Group == group);

        if (placement is null)
            return null;

        return new Rectangle(
            placement.TileBounds.X * _tileSize,
            (placement.TileBounds.Y - 1) * _tileSize,
            placement.TileBounds.Width * _tileSize,
            (placement.TileBounds.Height + 1) * _tileSize);
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);

        Point tile = new(e.X / _tileSize, e.Y / _tileSize);

        AtlasPlacement? hit = _placements.FirstOrDefault(p => p.TileBounds.Contains(tile));

        if (hit is not null)
            GroupClicked?.Invoke(hit.Group);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
        e.Graphics.DrawImageUnscaled(_atlas, Point.Empty);

        using Font headerFont = new("Segoe UI", 6f);
        using Brush headerTextBrush = new SolidBrush(Color.White);

        foreach (AtlasPlacement placement in _placements)
        {
            Rectangle headerPixels = new(
                placement.TileBounds.X * _tileSize,
                (placement.TileBounds.Y - 1) * _tileSize,
                placement.TileBounds.Width * _tileSize,
                _tileSize);

            using (SolidBrush headerBackBrush = new(CategoryColor.For(placement.Group.Category)))
            {
                e.Graphics.FillRectangle(headerBackBrush, headerPixels);
            }

            GraphicsState state = e.Graphics.Save();

            e.Graphics.SetClip(headerPixels);
            e.Graphics.DrawString(placement.Group.Category, headerFont, headerTextBrush, headerPixels.Location);

            e.Graphics.Restore(state);
        }

        if (_selectedGroup is not null && GetGroupPixelBounds(_selectedGroup) is { } selectedBounds)
        {
            using Pen selectionPen = new(Color.Lime, 2);

            e.Graphics.DrawRectangle(
                selectionPen,
                selectedBounds.X + 1,
                selectedBounds.Y + 1,
                selectedBounds.Width - 2,
                selectedBounds.Height - 2);
        }
    }
}
