namespace PokeAtlas.Services;

// Classic skyline bin-packing (Bottom-Left heuristic), operating entirely in
// grid units so callers can trivially keep results aligned to a tile grid.
internal class SkylinePacker
{
    private class Node
    {
        public int X;
        public int Y;
        public int Width;
    }

    private readonly int _atlasWidth;
    private readonly List<Node> _skyline;

    public int UsedHeight { get; private set; }

    public SkylinePacker(int atlasWidth)
    {
        _atlasWidth = atlasWidth;
        _skyline = new List<Node> { new() { X = 0, Y = 0, Width = atlasWidth } };
    }

    public Point Place(int width, int height)
    {
        (int x, int y, int index) = FindBestPosition(width);

        UsedHeight = Math.Max(UsedHeight, y + height);

        AddSkylineLevel(index, x, y + height, width);

        return new Point(x, y);
    }

    private (int X, int Y, int Index) FindBestPosition(int width)
    {
        int bestY = int.MaxValue;
        int bestX = 0;
        int bestIndex = 0;

        for (int i = 0; i < _skyline.Count; i++)
        {
            if (_skyline[i].X + width > _atlasWidth)
                continue;

            int y = GetLandingHeight(i, width);

            if (y < bestY)
            {
                bestY = y;
                bestX = _skyline[i].X;
                bestIndex = i;
            }
        }

        return (bestX, bestY, bestIndex);
    }

    private int GetLandingHeight(int startIndex, int width)
    {
        int remaining = width;
        int y = 0;
        int i = startIndex;

        while (remaining > 0 && i < _skyline.Count)
        {
            y = Math.Max(y, _skyline[i].Y);
            remaining -= _skyline[i].Width;
            i++;
        }

        return y;
    }

    private void AddSkylineLevel(int startIndex, int x, int y, int width)
    {
        Node newNode = new() { X = x, Y = y, Width = width };

        _skyline.Insert(startIndex, newNode);

        int i = startIndex + 1;

        while (i < _skyline.Count)
        {
            Node node = _skyline[i];

            int shrink = (newNode.X + newNode.Width) - node.X;

            if (shrink <= 0)
                break;

            if (shrink >= node.Width)
            {
                _skyline.RemoveAt(i);
                continue;
            }

            node.X += shrink;
            node.Width -= shrink;
            break;
        }

        MergeAdjacent();
    }

    private void MergeAdjacent()
    {
        for (int i = 0; i < _skyline.Count - 1; i++)
        {
            if (_skyline[i].Y == _skyline[i + 1].Y)
            {
                _skyline[i].Width += _skyline[i + 1].Width;
                _skyline.RemoveAt(i + 1);
                i--;
            }
        }
    }
}
