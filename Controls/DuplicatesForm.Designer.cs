namespace PokeAtlas.Controls
{
    partial class DuplicatesForm
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
            lblSummary = new Label();
            listViewDuplicates = new ListView();
            SuspendLayout();
            //
            // lblSummary
            //
            lblSummary.Dock = DockStyle.Top;
            lblSummary.Height = 28;
            lblSummary.Name = "lblSummary";
            lblSummary.Padding = new Padding(6, 6, 6, 0);
            lblSummary.Text = "Scanning...";
            //
            // listViewDuplicates
            //
            listViewDuplicates.Dock = DockStyle.Fill;
            listViewDuplicates.MultiSelect = false;
            listViewDuplicates.Name = "listViewDuplicates";
            listViewDuplicates.View = View.LargeIcon;
            listViewDuplicates.SelectedIndexChanged += listViewDuplicates_SelectedIndexChanged;
            //
            // DuplicatesForm
            //
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(500, 400);
            Controls.Add(listViewDuplicates);
            Controls.Add(lblSummary);
            Name = "DuplicatesForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Duplicate Tiles";
            ResumeLayout(false);
        }

        private Label lblSummary;
        private ListView listViewDuplicates;
    }
}
