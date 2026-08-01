using PokeAtlas.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokeAtlas.Services;

public class GroupService
{
    private readonly List<TileGroup> _groups = new();

    public IReadOnlyList<TileGroup> Groups => _groups;

    public void Add(TileGroup group)
    {
        _groups.Add(group);
    }

    public void Remove(TileGroup group)
    {
        _groups.Remove(group);
    }

    public void Clear()
    {
        _groups.Clear();
    }
}
