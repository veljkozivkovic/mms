namespace Aplikacija
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.MenuStrip menuMain;
        private System.Windows.Forms.ToolStripMenuItem fileMenu;
        private System.Windows.Forms.ToolStripMenuItem fileOpenMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fileSaveMenuItem;
        private System.Windows.Forms.ToolStripSeparator fileSep1;
        private System.Windows.Forms.ToolStripMenuItem fileExitMenuItem;

        private System.Windows.Forms.ToolStripMenuItem editMenu;
        private System.Windows.Forms.ToolStripMenuItem editUndoMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editRedoMenuItem;

        private System.Windows.Forms.ToolStripMenuItem filtersMenu;
        private System.Windows.Forms.ToolStripMenuItem filterGaussianMenuItem;
        private System.Windows.Forms.ToolStripMenuItem filterMeanRemovalMenuItem;
        private System.Windows.Forms.ToolStripMenuItem filterBlacklightMenuItem;
        private System.Windows.Forms.ToolStripMenuItem filterHistEqMenuItem;

        private System.Windows.Forms.ToolStrip toolMain;
        private System.Windows.Forms.ToolStripButton btnOpen;
        private System.Windows.Forms.ToolStripButton btnSave;
        private System.Windows.Forms.ToolStripSeparator toolSep1;
        private System.Windows.Forms.ToolStripButton btnUndo;
        private System.Windows.Forms.ToolStripButton btnRedo;

        private System.Windows.Forms.Panel panelViewport;
        private Aplikacija.UI.Controls.CoverImageView pbImage;

        private System.Windows.Forms.StatusStrip statusMain;
        private System.Windows.Forms.ToolStripStatusLabel lblInfo;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            menuMain = new MenuStrip();
            fileMenu = new ToolStripMenuItem();
            fileOpenMenuItem = new ToolStripMenuItem();
            fileSaveMenuItem = new ToolStripMenuItem();
            fileSep1 = new ToolStripSeparator();
            fileExitMenuItem = new ToolStripMenuItem();
            editMenu = new ToolStripMenuItem();
            editUndoMenuItem = new ToolStripMenuItem();
            editRedoMenuItem = new ToolStripMenuItem();
            filtersMenu = new ToolStripMenuItem();
            filterGaussianMenuItem = new ToolStripMenuItem();
            filterMeanRemovalMenuItem = new ToolStripMenuItem();
            filterBlacklightMenuItem = new ToolStripMenuItem();
            filterHistEqMenuItem = new ToolStripMenuItem();
            toolMain = new ToolStrip();
            btnOpen = new ToolStripButton();
            btnSave = new ToolStripButton();
            toolSep1 = new ToolStripSeparator();
            btnUndo = new ToolStripButton();
            btnRedo = new ToolStripButton();
            panelViewport = new Panel();
            pbImage = new Aplikacija.UI.Controls.CoverImageView();
            pbImage.Dock = DockStyle.Fill;
            pbImage.Margin = new Padding(0);
            pbImage.Name = "pbImage";
            pbImage.TabIndex = 0;
            pbImage.TabStop = false;
            statusMain = new StatusStrip();
            lblInfo = new ToolStripStatusLabel();
            menuMain.SuspendLayout();
            toolMain.SuspendLayout();
            panelViewport.SuspendLayout();
            statusMain.SuspendLayout();
            SuspendLayout();
            // 
            // menuMain
            // 
            menuMain.ImageScalingSize = new Size(20, 20);
            menuMain.Items.AddRange(new ToolStripItem[] { fileMenu, editMenu, filtersMenu });
            menuMain.Location = new Point(0, 0);
            menuMain.Name = "menuMain";
            menuMain.Padding = new Padding(5, 2, 0, 2);
            menuMain.Size = new Size(1035, 24);
            menuMain.TabIndex = 0;
            menuMain.Text = "menuMain";
            // 
            // fileMenu
            // 
            fileMenu.DropDownItems.AddRange(new ToolStripItem[] { fileOpenMenuItem, fileSaveMenuItem, fileSep1, fileExitMenuItem });
            fileMenu.Name = "fileMenu";
            fileMenu.Size = new Size(37, 20);
            fileMenu.Text = "File";
            // 
            // fileOpenMenuItem
            // 
            fileOpenMenuItem.Name = "fileOpenMenuItem";
            fileOpenMenuItem.ShortcutKeys = Keys.Control | Keys.O;
            fileOpenMenuItem.Size = new Size(192, 22);
            fileOpenMenuItem.Text = "Open…";
            fileOpenMenuItem.Click += fileOpenMenuItem_Click;
            // 
            // fileSaveMenuItem
            // 
            fileSaveMenuItem.Name = "fileSaveMenuItem";
            fileSaveMenuItem.ShortcutKeys = Keys.Control | Keys.S;
            fileSaveMenuItem.Size = new Size(192, 22);
            fileSaveMenuItem.Text = "Save (.yuvimg)";
            fileSaveMenuItem.Click += fileSaveMenuItem_Click;
            // 
            // fileSep1
            // 
            fileSep1.Name = "fileSep1";
            fileSep1.Size = new Size(189, 6);
            // 
            // fileExitMenuItem
            // 
            fileExitMenuItem.Name = "fileExitMenuItem";
            fileExitMenuItem.Size = new Size(192, 22);
            fileExitMenuItem.Text = "Exit";
            fileExitMenuItem.Click += fileExitMenuItem_Click;
            // 
            // editMenu
            // 
            editMenu.DropDownItems.AddRange(new ToolStripItem[] { editUndoMenuItem, editRedoMenuItem });
            editMenu.Name = "editMenu";
            editMenu.Size = new Size(39, 20);
            editMenu.Text = "Edit";
            // 
            // editUndoMenuItem
            // 
            editUndoMenuItem.Enabled = false;
            editUndoMenuItem.Name = "editUndoMenuItem";
            editUndoMenuItem.ShortcutKeys = Keys.Control | Keys.Z;
            editUndoMenuItem.Size = new Size(144, 22);
            editUndoMenuItem.Text = "Undo";
            editUndoMenuItem.Click += editUndoMenuItem_Click;
            // 
            // editRedoMenuItem
            // 
            editRedoMenuItem.Enabled = false;
            editRedoMenuItem.Name = "editRedoMenuItem";
            editRedoMenuItem.ShortcutKeys = Keys.Control | Keys.Y;
            editRedoMenuItem.Size = new Size(144, 22);
            editRedoMenuItem.Text = "Redo";
            editRedoMenuItem.Click += editRedoMenuItem_Click;
            // 
            // filtersMenu
            // 
            filtersMenu.DropDownItems.AddRange(new ToolStripItem[] { filterGaussianMenuItem, filterMeanRemovalMenuItem, filterBlacklightMenuItem, filterHistEqMenuItem });
            filtersMenu.Name = "filtersMenu";
            filtersMenu.Size = new Size(50, 20);
            filtersMenu.Text = "Filters";
            // 
            // filterGaussianMenuItem
            // 
            filterGaussianMenuItem.Name = "filterGaussianMenuItem";
            filterGaussianMenuItem.Size = new Size(197, 22);
            filterGaussianMenuItem.Text = "Gaussian Blur";
            filterGaussianMenuItem.Click += filterGaussianMenuItem_Click;
            // 
            // filterMeanRemovalMenuItem
            // 
            filterMeanRemovalMenuItem.Name = "filterMeanRemovalMenuItem";
            filterMeanRemovalMenuItem.Size = new Size(197, 22);
            filterMeanRemovalMenuItem.Text = "Mean Removal";
            filterMeanRemovalMenuItem.Click += filterMeanRemovalMenuItem_Click;
            // 
            // filterBlacklightMenuItem
            // 
            filterBlacklightMenuItem.Name = "filterBlacklightMenuItem";
            filterBlacklightMenuItem.Size = new Size(197, 22);
            filterBlacklightMenuItem.Text = "Blacklight";
            filterBlacklightMenuItem.Click += filterBlacklightMenuItem_Click;
            // 
            // filterHistEqMenuItem
            // 
            filterHistEqMenuItem.Name = "filterHistEqMenuItem";
            filterHistEqMenuItem.Size = new Size(197, 22);
            filterHistEqMenuItem.Text = "Histogram Equalization";
            filterHistEqMenuItem.Click += filterHistEqMenuItem_Click;
            // 
            // toolMain
            // 
            toolMain.ImageScalingSize = new Size(20, 20);
            toolMain.Items.AddRange(new ToolStripItem[] { btnOpen, btnSave, toolSep1, btnUndo, btnRedo });
            toolMain.Location = new Point(0, 24);
            toolMain.Name = "toolMain";
            toolMain.Size = new Size(1035, 25);
            toolMain.TabIndex = 1;
            toolMain.Text = "toolMain";
            // 
            // btnOpen
            // 
            btnOpen.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnOpen.Name = "btnOpen";
            btnOpen.Size = new Size(40, 22);
            btnOpen.Text = "Open";
            btnOpen.Click += btnOpen_Click;
            // 
            // btnSave
            // 
            btnSave.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(35, 22);
            btnSave.Text = "Save";
            btnSave.Click += btnSave_Click;
            // 
            // toolSep1
            // 
            toolSep1.Name = "toolSep1";
            toolSep1.Size = new Size(6, 25);
            // 
            // btnUndo
            // 
            btnUndo.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnUndo.Enabled = false;
            btnUndo.Name = "btnUndo";
            btnUndo.Size = new Size(40, 22);
            btnUndo.Text = "Undo";
            btnUndo.Click += btnUndo_Click;
            // 
            // btnRedo
            // 
            btnRedo.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnRedo.Enabled = false;
            btnRedo.Name = "btnRedo";
            btnRedo.Size = new Size(38, 22);
            btnRedo.Text = "Redo";
            btnRedo.Click += btnRedo_Click;
            // 
            // panelViewport
            // 
            panelViewport.AutoScroll = true;
            panelViewport.BackColor = Color.DimGray;
            panelViewport.Controls.Add(pbImage);
            panelViewport.Dock = DockStyle.Fill;
            panelViewport.Location = new Point(0, 49);
            panelViewport.Margin = new Padding(3, 2, 3, 2);
            panelViewport.Name = "panelViewport";
            panelViewport.Size = new Size(1035, 580);
            panelViewport.TabIndex = 2;
            // 
            // pbImage
            // 
            pbImage.BorderStyle = BorderStyle.FixedSingle;
            pbImage.Location = new Point(14, 12);
            pbImage.Margin = new Padding(3, 2, 3, 2);
            pbImage.Name = "pbImage";
            pbImage.Size = new Size(700, 450);
            pbImage.SizeMode = PictureBoxSizeMode.Zoom;
            pbImage.TabIndex = 0;
            pbImage.TabStop = false;
            // 
            // statusMain
            // 
            statusMain.ImageScalingSize = new Size(20, 20);
            statusMain.Items.AddRange(new ToolStripItem[] { lblInfo });
            statusMain.Location = new Point(0, 629);
            statusMain.Name = "statusMain";
            statusMain.Padding = new Padding(1, 0, 12, 0);
            statusMain.Size = new Size(1035, 22);
            statusMain.TabIndex = 3;
            statusMain.Text = "statusMain";
            // 
            // lblInfo
            // 
            lblInfo.Name = "lblInfo";
            lblInfo.Size = new Size(13, 17);
            lblInfo.Text = "–";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1035, 651);
            Controls.Add(panelViewport);
            Controls.Add(statusMain);
            Controls.Add(toolMain);
            Controls.Add(menuMain);
            MainMenuStrip = menuMain;
            Margin = new Padding(3, 2, 3, 2);
            MinimumSize = new Size(877, 535);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "MMS projekat";
            Load += MainForm_Load;
            menuMain.ResumeLayout(false);
            menuMain.PerformLayout();
            toolMain.ResumeLayout(false);
            toolMain.PerformLayout();
            panelViewport.ResumeLayout(false);
            statusMain.ResumeLayout(false);
            statusMain.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
    }
}
