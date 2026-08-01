namespace PokeAtlas.Controls;

public partial class AddGroupForm : Form
{
    public string GroupName => txtName.Text.Trim();

    public string Category => cmbCategory.Text;

    public AddGroupForm()
    {
        InitializeComponent();

        cmbCategory.Items.AddRange(new object[]
        {
            "Terrain",
            "Water",
            "Cliffs",
            "Trees",
            "Buildings",
            "Town",
            "Special"
        });

        cmbCategory.SelectedIndex = 0;
    }
}
