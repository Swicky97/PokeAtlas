using System.Drawing.Drawing2D;

namespace PokeAtlas.Controls;

public class TilesetCanvas : ScrollableControl
{
    private Bitmap? _tileset;

    public float Zoom { get; private set; } = 1.0f;

    public TilesetCanvas()
    {
        DoubleBuffered = true;
        ResizeRedraw = true;
        BackColor = Color.FromArgb(45, 45, 48);
    }

    public void LoadTileset(string filePath)
    {
        _tileset?.Dispose();
        _tileset = new Bitmap(filePath);

        Zoom = 1.0f;

        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        e.Graphics.Clear(BackColor);

        if (_tileset == null)
            return;

        e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
        e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;

        e.Graphics.DrawImage(
            _tileset,
            0,
            0,
            _tileset.Width * Zoom,
            _tileset.Height * Zoom);
    }
}