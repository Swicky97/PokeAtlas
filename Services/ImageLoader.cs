using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtlasDex.Services;

public class ImageLoader
{
    public Bitmap? Image { get; private set; }

    public void Load(string path)
    {
        Image?.Dispose();
        Image = new Bitmap(path);
    }
}
