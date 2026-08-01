namespace PokeAtlas
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            mainMenuStrip = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            openToolStripMenuItem = new ToolStripMenuItem();
            mainToolStrip = new ToolStrip();
            openToolStripButton = new ToolStripButton();
            toolStripSeparator1 = new ToolStripSeparator();
            saveToolStripButton = new ToolStripButton();
            toolStripSeparator2 = new ToolStripSeparator();
            addGroupToolStripButton = new ToolStripButton();
            toolStripSeparator3 = new ToolStripSeparator();
            deleteToolStripButton = new ToolStripButton();
            toolStripSeparator4 = new ToolStripSeparator();
            gridToolStripButton = new ToolStripButton();
            statusStrip = new StatusStrip();
            splitContainerOuter = new SplitContainer();
            treeViewGroups = new TreeView();
            innerSplitContainer = new SplitContainer();
            propertiesGrid = new PropertyGrid();
            mainMenuStrip.SuspendLayout();
            mainToolStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainerOuter).BeginInit();
            splitContainerOuter.Panel1.SuspendLayout();
            splitContainerOuter.Panel2.SuspendLayout();
            splitContainerOuter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)innerSplitContainer).BeginInit();
            innerSplitContainer.Panel2.SuspendLayout();
            innerSplitContainer.SuspendLayout();
            SuspendLayout();
            // 
            // mainMenuStrip
            // 
            mainMenuStrip.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem });
            mainMenuStrip.Location = new Point(0, 0);
            mainMenuStrip.Name = "mainMenuStrip";
            mainMenuStrip.Size = new Size(800, 24);
            mainMenuStrip.TabIndex = 0;
            mainMenuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { openToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(37, 20);
            fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            openToolStripMenuItem.Name = "openToolStripMenuItem";
            openToolStripMenuItem.Size = new Size(112, 22);
            openToolStripMenuItem.Text = "Open...";
            openToolStripMenuItem.Click += openToolStripMenuItem_Click;
            // 
            // mainToolStrip
            // 
            mainToolStrip.Items.AddRange(new ToolStripItem[] { openToolStripButton, toolStripSeparator1, saveToolStripButton, toolStripSeparator2, addGroupToolStripButton, toolStripSeparator3, deleteToolStripButton, toolStripSeparator4, gridToolStripButton });
            mainToolStrip.Location = new Point(0, 24);
            mainToolStrip.Name = "mainToolStrip";
            mainToolStrip.Size = new Size(800, 25);
            mainToolStrip.TabIndex = 1;
            mainToolStrip.Text = "toolStrip1";
            // 
            // openToolStripButton
            // 
            openToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            openToolStripButton.Image = (Image)resources.GetObject("openToolStripButton.Image");
            openToolStripButton.ImageTransparentColor = Color.Magenta;
            openToolStripButton.Name = "openToolStripButton";
            openToolStripButton.Size = new Size(40, 22);
            openToolStripButton.Text = "Open";
            openToolStripButton.Click += openToolStripButton_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(6, 25);
            // 
            // saveToolStripButton
            // 
            saveToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            saveToolStripButton.Image = (Image)resources.GetObject("saveToolStripButton.Image");
            saveToolStripButton.ImageTransparentColor = Color.Magenta;
            saveToolStripButton.Name = "saveToolStripButton";
            saveToolStripButton.Size = new Size(35, 22);
            saveToolStripButton.Text = "Save";
            saveToolStripButton.Click += saveToolStripButton_Click;
            //
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(6, 25);
            // 
            // addGroupToolStripButton
            // 
            addGroupToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            addGroupToolStripButton.Image = (Image)resources.GetObject("addGroupToolStripButton.Image");
            addGroupToolStripButton.ImageTransparentColor = Color.Magenta;
            addGroupToolStripButton.Name = "addGroupToolStripButton";
            addGroupToolStripButton.Size = new Size(69, 22);
            addGroupToolStripButton.Text = "Add Group";
            addGroupToolStripButton.Click += addGroupToolStripButton_Click;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new Size(6, 25);
            // 
            // deleteToolStripButton
            // 
            deleteToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            deleteToolStripButton.Image = (Image)resources.GetObject("deleteToolStripButton.Image");
            deleteToolStripButton.ImageTransparentColor = Color.Magenta;
            deleteToolStripButton.Name = "deleteToolStripButton";
            deleteToolStripButton.Size = new Size(44, 22);
            deleteToolStripButton.Text = "Delete";
            deleteToolStripButton.Click += deleteToolStripButton_Click;
            //
            // toolStripSeparator4
            // 
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new Size(6, 25);
            // 
            // gridToolStripButton
            // 
            gridToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            gridToolStripButton.Image = (Image)resources.GetObject("gridToolStripButton.Image");
            gridToolStripButton.ImageTransparentColor = Color.Magenta;
            gridToolStripButton.Name = "gridToolStripButton";
            gridToolStripButton.Size = new Size(33, 22);
            gridToolStripButton.Text = "Grid";
            // 
            // statusStrip
            // 
            statusStrip.Location = new Point(0, 428);
            statusStrip.Name = "statusStrip";
            statusStrip.Size = new Size(800, 22);
            statusStrip.TabIndex = 2;
            statusStrip.Text = "statusStrip1";
            // 
            // splitContainerOuter
            // 
            splitContainerOuter.Dock = DockStyle.Fill;
            splitContainerOuter.FixedPanel = FixedPanel.Panel1;
            splitContainerOuter.Location = new Point(0, 49);
            splitContainerOuter.Name = "splitContainerOuter";
            // 
            // splitContainerOuter.Panel1
            // 
            splitContainerOuter.Panel1.Controls.Add(treeViewGroups);
            // 
            // splitContainerOuter.Panel2
            // 
            splitContainerOuter.Panel2.Controls.Add(innerSplitContainer);
            splitContainerOuter.Size = new Size(800, 379);
            splitContainerOuter.SplitterDistance = 250;
            splitContainerOuter.TabIndex = 3;
            // 
            // treeViewGroups
            // 
            treeViewGroups.Dock = DockStyle.Fill;
            treeViewGroups.Location = new Point(0, 0);
            treeViewGroups.Name = "treeViewGroups";
            treeViewGroups.Size = new Size(250, 379);
            treeViewGroups.TabIndex = 0;
            treeViewGroups.AfterSelect += treeViewGroups_AfterSelect;
            // 
            // innerSplitContainer
            // 
            innerSplitContainer.Dock = DockStyle.Fill;
            innerSplitContainer.FixedPanel = FixedPanel.Panel2;
            innerSplitContainer.Location = new Point(0, 0);
            innerSplitContainer.Name = "innerSplitContainer";
            // 
            // innerSplitContainer.Panel1
            // 
            innerSplitContainer.Panel1.BackColor = Color.DimGray;
            // 
            // innerSplitContainer.Panel2
            // 
            innerSplitContainer.Panel2.Controls.Add(propertiesGrid);
            innerSplitContainer.Size = new Size(546, 379);
            innerSplitContainer.SplitterDistance = 300;
            innerSplitContainer.TabIndex = 0;
            // 
            // propertiesGrid
            // 
            propertiesGrid.Dock = DockStyle.Fill;
            propertiesGrid.Location = new Point(0, 0);
            propertiesGrid.Name = "propertiesGrid";
            propertiesGrid.Size = new Size(242, 379);
            propertiesGrid.TabIndex = 0;
            propertiesGrid.PropertyValueChanged += propertiesGrid_PropertyValueChanged;
            //
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(splitContainerOuter);
            Controls.Add(statusStrip);
            Controls.Add(mainToolStrip);
            Controls.Add(mainMenuStrip);
            MainMenuStrip = mainMenuStrip;
            Name = "MainForm";
            Text = "Form1";
            mainMenuStrip.ResumeLayout(false);
            mainMenuStrip.PerformLayout();
            mainToolStrip.ResumeLayout(false);
            mainToolStrip.PerformLayout();
            splitContainerOuter.Panel1.ResumeLayout(false);
            splitContainerOuter.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerOuter).EndInit();
            splitContainerOuter.ResumeLayout(false);
            innerSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)innerSplitContainer).EndInit();
            innerSplitContainer.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip mainMenuStrip;
        private ToolStrip mainToolStrip;
        private StatusStrip statusStrip;
        private SplitContainer splitContainerOuter;
        private TreeView treeViewGroups;
        private SplitContainer innerSplitContainer;
        private PropertyGrid propertiesGrid;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem openToolStripMenuItem;
        private ToolStripButton openToolStripButton;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripButton saveToolStripButton;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripButton addGroupToolStripButton;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripButton deleteToolStripButton;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripButton gridToolStripButton;
    }
}
