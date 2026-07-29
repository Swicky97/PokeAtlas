using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtlasDex.Models;
public class TileGroup
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = "";

    public string Category { get; set; } = "";

    public Rectangle Bounds { get; set; }
}
