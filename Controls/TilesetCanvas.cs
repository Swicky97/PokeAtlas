using PokeAtlas.Models;
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

    private bool _showGrid = true;

    public const int TileSize = 16;
    private Point _hoverTile = new(-1, -1);

    // Selection
    private bool _isSelecting;

    private Point _selectionStartTile = new(-1, -1);

    private Point _selectionEndTile = new(-1, -1);

    private TileGroup? _selectedGroup;

    public string? TilesetPath { get; private set; }

    public Bitmap? Tileset => _tileset;

    public TilesetCanvas()
    {
        DoubleBuffered = true;
        ResizeRedraw = true;
        BackColor = Color.FromArgb(45, 45, 48);
    }

    public Rectangle? SelectedTileRectangle
    {
        get
        {
            if (_selectionStartTile.X < 0)
                return null;

            int left = Math.Min(_selectionStartTile.X, _selectionEndTile.X);
            int top = Math.Min(_selectionStartTile.Y, _selectionEndTile.Y);

            int right = Math.Max(_selectionStartTile.X, _selectionEndTile.X);
            int bottom = Math.Max(_selectionStartTile.Y, _selectionEndTile.Y);

            return new Rectangle(
                left,
                top,
                right - left + 1,
                bottom - top + 1);
        }
    }

    public void LoadTileset(string filePath)
    {
        _tileset?.Dispose();
        _tileset = new Bitmap(filePath);
        TilesetPath = filePath;

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

        var state = e.Graphics.Save();

        e.Graphics.TranslateTransform(_camera.X, _camera.Y);
        e.Graphics.ScaleTransform(_zoom, _zoom);

        e.Graphics.DrawImage(
            _tileset,
            new Rectangle(0, 0, _tileset.Width, _tileset.Height),
            0, 0, _tileset.Width, _tileset.Height,
            GraphicsUnit.Pixel);

        if (_showGrid)
            DrawGrid(e.Graphics);

        DrawSelection(e.Graphics);
        DrawSelectedGroup(e.Graphics);
        DrawHover(e.Graphics);

        e.Graphics.Restore(state);
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

        if (e.Button == MouseButtons.Left)
        {
            _isSelecting = true;

            _selectionStartTile = GetTileAt(e.Location);
            _selectionEndTile = _selectionStartTile;

            Invalidate();
        }
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);

        if (_isPanning)
        {
            int dx = e.X - _lastMousePosition.X;
            int dy = e.Y - _lastMousePosition.Y;

            _camera = new PointF(
                _camera.X + dx,
                _camera.Y + dy);

            _lastMousePosition = e.Location;
        }

        _hoverTile = GetTileAt(e.Location);

        if (_isSelecting)
        {
            _selectionEndTile = _hoverTile;
        }

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

        if (e.Button == MouseButtons.Left)
        {
            _isSelecting = false;

            Invalidate();
        }
    }

    private void DrawGrid(Graphics g)
    {
        if (_tileset == null)
            return;

        using Pen pen = new(Color.FromArgb(90, Color.White), 0);

        // Vertical lines
        for (int x = 0; x <= _tileset.Width; x += TileSize)
        {
            g.DrawLine(pen, x, 0, x, _tileset.Height);
        }

        // Horizontal lines
        for (int y = 0; y <= _tileset.Height; y += TileSize)
        {
            g.DrawLine(pen, 0, y, _tileset.Width, y);
        }
    }

    private void DrawHover(Graphics g)
    {
        if (_hoverTile.X < 0)
            return;

        using Pen pen = new(Color.Yellow, 0);

        g.DrawRectangle(
            pen,
            _hoverTile.X * TileSize,
            _hoverTile.Y * TileSize,
            TileSize,
            TileSize);
    }

    private Point GetTileAt(Point screenPoint)
    {
        PointF world = ScreenToWorld(screenPoint);

        return new Point(
            (int)Math.Floor(world.X / TileSize),
            (int)Math.Floor(world.Y / TileSize));
    }

    private void DrawSelection(Graphics g)
    {
        if (_selectionStartTile.X < 0)
            return;

        int left = Math.Min(_selectionStartTile.X, _selectionEndTile.X);
        int top = Math.Min(_selectionStartTile.Y, _selectionEndTile.Y);

        int right = Math.Max(_selectionStartTile.X, _selectionEndTile.X);
        int bottom = Math.Max(_selectionStartTile.Y, _selectionEndTile.Y);

        Rectangle rect = new(
            left * TileSize,
            top * TileSize,
            (right - left + 1) * TileSize,
            (bottom - top + 1) * TileSize);

        using SolidBrush brush = new(Color.FromArgb(70, Color.DeepSkyBlue));
        using Pen pen = new(Color.DeepSkyBlue, 0);

        g.FillRectangle(brush, rect);
        g.DrawRectangle(pen, rect);
    }

    public void SelectGroup(TileGroup? group)
    {
        _selectedGroup = group;
        Invalidate();
    }

    public void ClearSelection()
    {
        _selectionStartTile = new Point(-1, -1);
        _selectionEndTile = new Point(-1, -1);
        Invalidate();
    }

    private void DrawSelectedGroup(Graphics g)
    {
        if (_selectedGroup == null)
            return;

        Rectangle r = _selectedGroup.TileBounds;

        Rectangle pixels = new(
            r.X * TileSize,
            r.Y * TileSize,
            r.Width * TileSize,
            r.Height * TileSize);

        using Pen pen = new(Color.Lime, 0);

        g.DrawRectangle(pen, pixels);
    }

    public void CenterOnGroup(TileGroup group)
    {
        Rectangle r = group.TileBounds;

        float centerX = (r.Left + r.Width / 2f) * TileSize;
        float centerY = (r.Top + r.Height / 2f) * TileSize;

        _camera = new PointF(
            ClientSize.Width / 2f - centerX * _zoom,
            ClientSize.Height / 2f - centerY * _zoom);

        Invalidate();
    }
}