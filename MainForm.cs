using PokeAtlas.Controls;

namespace PokeAtlas
{
    public partial class MainForm : Form
    {
        private readonly TilesetCanvas _canvas = new();

        public MainForm()
        {
            InitializeComponent();

            _canvas.Dock = DockStyle.Fill;

            splitContainer2.Panel1.Controls.Add(_canvas);
        }
    }
}
