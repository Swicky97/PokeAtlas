namespace PokeAtlas.Controls;

public partial class AtlasPreviewForm : Form
{
    public AtlasPreviewForm(Bitmap atlas)
    {
        InitializeComponent();

        pictureBoxAtlas.Image = atlas;
        pictureBoxAtlas.Size = atlas.Size;
    }
}
