namespace PokeAtlas.Controls
{
    partial class TileBrowserForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            txtSearch = new TextBox();
            lblSummary = new Label();
            listViewTiles = new ListView();
            SuspendLayout();
            //
            // txtSearch
            //
            txtSearch.Dock = DockStyle.Top;
            txtSearch.Name = "txtSearch";
            txtSearch.PlaceholderText = "name, category, tag...";
            txtSearch.TextChanged += txtSearch_TextChanged;
            //
            // lblSummary
            //
            lblSummary.Dock = DockStyle.Top;
            lblSummary.Height = 24;
            lblSummary.Name = "lblSummary";
            lblSummary.Padding = new Padding(6, 6, 6, 0);
            lblSummary.Text = "0 group(s)";
            //
            // listViewTiles
            //
            listViewTiles.Dock = DockStyle.Fill;
            listViewTiles.MultiSelect = false;
            listViewTiles.Name = "listViewTiles";
            listViewTiles.View = View.LargeIcon;
            listViewTiles.SelectedIndexChanged += listViewTiles_SelectedIndexChanged;
            //
            // TileBrowserForm
            //
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(500, 450);
            Controls.Add(listViewTiles);
            Controls.Add(lblSummary);
            Controls.Add(txtSearch);
            Name = "TileBrowserForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Tile Browser";
            ResumeLayout(false);
            PerformLayout();
        }

        private TextBox txtSearch;
        private Label lblSummary;
        private ListView listViewTiles;
    }
}
