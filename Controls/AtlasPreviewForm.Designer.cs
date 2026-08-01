namespace PokeAtlas.Controls
{
    partial class AtlasPreviewForm
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
            btnExport = new Button();
            panelScroll = new Panel();
            SuspendLayout();
            //
            // btnExport
            //
            btnExport.Dock = DockStyle.Top;
            btnExport.Height = 32;
            btnExport.Name = "btnExport";
            btnExport.Text = "Export...";
            btnExport.UseVisualStyleBackColor = true;
            btnExport.Click += btnExport_Click;
            //
            // panelScroll
            //
            panelScroll.AutoScroll = true;
            panelScroll.BackColor = Color.FromArgb(30, 30, 30);
            panelScroll.Dock = DockStyle.Fill;
            panelScroll.Name = "panelScroll";
            //
            // AtlasPreviewForm
            //
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(700, 500);
            Controls.Add(panelScroll);
            Controls.Add(btnExport);
            Name = "AtlasPreviewForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Atlas Preview";
            ResumeLayout(false);
        }

        private Button btnExport;
        private Panel panelScroll;
    }
}
