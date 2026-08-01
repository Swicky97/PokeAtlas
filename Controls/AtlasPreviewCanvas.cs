using System.Drawing.Drawing2D;
using PokeAtlas.Models;

namespace PokeAtlas.Controls;

internal class AtlasPreviewCanvas : Control
{
    private readonly Bitmap _atlas;
    private readonly IReadOnlyList<AtlasPlacement> _placements;
    private readonly int _tileSize;

    public AtlasPreviewCanvas(Bitmap atlas, IReadOnlyList<AtlasPlacement> placements, int tileSize)
    {
        _atlas = atlas;
        _placements = placements;
        _tileSize = tileSize;

        DoubleBuffered = true;
        Size = atlas.Size;
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
    }
}
