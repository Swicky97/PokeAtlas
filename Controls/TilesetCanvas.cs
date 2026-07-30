using System.Drawing.Drawing2D;

namespace PokeAtlas.Controls;

public class TilesetCanvas : ScrollableControl
{
    private Bitmap? _tileset;

    // Camera zoom and position
    private PointF _camera = PointF.Empty;
    private float _zoom = 1.0f;
    private const float MinZoom = 0.25f;
    private const float MaxZoom = 32.0f;
    private const float ZoomStep = 1.15f;

    // Camera panning
    private bool _isPanning;
    private Point _lastMousePosition;

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

        _camera = PointF.Empty;
        _zoom = 1.0f;

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

        e.Graphics.TranslateTransform(_camera.X, _camera.Y);
        e.Graphics.ScaleTransform(_zoom, _zoom);

        e.Graphics.DrawImage(
            _tileset,
            new Rectangle(
                0,
                0,
                _tileset.Width,
                _tileset.Height));
    }

    protected override void OnMouseWheel(MouseEventArgs e)
    {
        base.OnMouseWheel(e);

        if (e.Delta > 0)
            ZoomIn(e.Location);
        else
            ZoomOut(e.Location);
    }

    protected override void OnMouseEnter(EventArgs e)
    {
        base.OnMouseEnter(e);

        Focus();
    }

    private void ZoomIn(Point mouse)
    {
        SetZoom(_zoom * ZoomStep, mouse);
    }

    private void ZoomOut(Point mouse)
    {
        SetZoom(_zoom / ZoomStep, mouse);
    }

    private void SetZoom(float newZoom, Point mousePosition)
    {
        newZoom = Math.Clamp(newZoom, MinZoom, MaxZoom);

        if (Math.Abs(newZoom - _zoom) < 0.001f)
            return;

        PointF worldBefore = ScreenToWorld(mousePosition);

        _zoom = newZoom;

        _camera = new PointF(
            mousePosition.X - worldBefore.X * _zoom,
            mousePosition.Y - worldBefore.Y * _zoom);

        Invalidate();
    }

    private PointF ScreenToWorld(PointF screen)
    {
        return new PointF(
            (screen.X - _camera.X) / _zoom,
            (screen.Y - _camera.Y) / _zoom);
    }

    private PointF WorldToScreen(PointF world)
    {
        return new PointF(
            world.X * _zoom + _camera.X,
            world.Y * _zoom + _camera.Y);
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);

        if (e.Button == MouseButtons.Middle)
        {
            _isPanning = true;
            _lastMousePosition = e.Location;
            Cursor = Cursors.SizeAll;
        }
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);

        if (!_isPanning)
            return;

        int dx = e.X - _lastMousePosition.X;
        int dy = e.Y - _lastMousePosition.Y;

        _camera = new PointF(
            _camera.X + dx,
            _camera.Y + dy);

        _lastMousePosition = e.Location;

        Invalidate();
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);

        if (e.Button == MouseButtons.Middle)
        {
            _isPanning = false;
            Cursor = Cursors.Default;
        }
    }
}