namespace PokeAtlas.Controls
{
    partial class AutoDetectForm
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
            listViewRegions = new ListView();
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
            // listViewRegions
            //
            listViewRegions.Dock = DockStyle.Fill;
            listViewRegions.MultiSelect = false;
            listViewRegions.Name = "listViewRegions";
            listViewRegions.View = View.LargeIcon;
            listViewRegions.DoubleClick += listViewRegions_DoubleClick;
            //
            // AutoDetectForm
            //
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(500, 400);
            Controls.Add(listViewRegions);
            Controls.Add(lblSummary);
            Name = "AutoDetectForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Auto-Detected Regions";
            ResumeLayout(false);
        }

        private Label lblSummary;
        private ListView listViewRegions;
    }
}
