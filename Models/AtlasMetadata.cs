using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtlasDex.Models;

public class AtlasMetadata
{
    public List<TileGroup> Groups { get; set; } = new();
}
