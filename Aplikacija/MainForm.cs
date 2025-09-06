using System;
using System.Drawing;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace Aplikacija
{
    public partial class MainForm : Form
    {
        private Bitmap _currentBitmap;

        public MainForm()
        {
            InitializeComponent();
        }

        // ===== lifecycle =====
        private void MainForm_Load(object sender, EventArgs e)
        {
            lblInfo.Text = "Ready";
        }

        // ===== File =====
        private void fileOpenMenuItem_Click(object sender, EventArgs e) => BtnOpen();
        private void fileSaveMenuItem_Click(object sender, EventArgs e) => BtnSave();
        private void fileExitMenuItem_Click(object sender, EventArgs e) => Close();

        private void btnOpen_Click(object sender, EventArgs e) => BtnOpen();
        private void btnSave_Click(object sender, EventArgs e) => BtnSave();

        // ===== Edit =====
        private void editUndoMenuItem_Click(object sender, EventArgs e) => BtnUndo();
        private void editRedoMenuItem_Click(object sender, EventArgs e) => BtnRedo();

        private void btnUndo_Click(object sender, EventArgs e) => BtnUndo();
        private void btnRedo_Click(object sender, EventArgs e) => BtnRedo();

        // ===== Filters =====
        private void filterGaussianMenuItem_Click(object sender, EventArgs e) => ApplyFilterPlaceholder("Gaussian Blur");
        private void filterMeanRemovalMenuItem_Click(object sender, EventArgs e) => ApplyFilterPlaceholder("Mean Removal");
        private void filterBlacklightMenuItem_Click(object sender, EventArgs e) => ApplyFilterPlaceholder("Blacklight");
        private void filterHistEqMenuItem_Click(object sender, EventArgs e) => ApplyFilterPlaceholder("Histogram Equalization");

        // ===== helpers (privremeno – TODO: zameni pravom logikom) =====
        private void BtnOpen()
        {
            using var ofd = new OpenFileDialog
            {
                Title = "Open image",
                Filter = "Images|*.bmp;*.png;*.jpg;*.jpeg;*.yuvimg|All files|*.*"
            };
            if (ofd.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    // TODO: ako je .yuvimg pozovi svoj loader
                    _currentBitmap?.Dispose();
                    _currentBitmap = new Bitmap(ofd.FileName);
                    pbImage.Image = _currentBitmap;
                    lblInfo.Text = $"{_currentBitmap.Width}×{_currentBitmap.Height}";
                    EnableUndoRedo(false, false);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, "Open failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnSave()
        {
            using var sfd = new SaveFileDialog
            {
                Title = "Save custom format",
                Filter = "YUV Image (*.yuvimg)|*.yuvimg",
                DefaultExt = "yuvimg"
            };
            if (sfd.ShowDialog(this) == DialogResult.OK)
            {
                // TODO: konverzija Bitmap→YUV + downsampling + snimanje
                MessageBox.Show(this, "TODO: implement Save (.yuvimg)", "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ShowBitmapAt1x(Bitmap bmp)
        {
            if (bmp == null) return;

            // 1:1 prikaz
            pbImage.SizeMode = PictureBoxSizeMode.Normal;
            pbImage.Image = bmp;

            // postavi tačnu veličinu picturebox-a da bude koliko i slika
            pbImage.Size = bmp.Size;
            pbImage.Location = new Point(0, 0);

            // obezbedi skrolbarove
            panelViewport.AutoScroll = true;
            panelViewport.AutoScrollMinSize = bmp.Size;

            // status
            lblInfo.Text = $"{bmp.Width}×{bmp.Height} px";
        }

        private void BtnUndo()
        {
            // TODO: pozovi UndoRedoManager.Undo
            MessageBox.Show(this, "TODO: Undo", "Edit", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnRedo()
        {
            // TODO: pozovi UndoRedoManager.Redo
            MessageBox.Show(this, "TODO: Redo", "Edit", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ApplyFilterPlaceholder(string name)
        {
            // TODO: zameni pozivom pravog filtera + push u Undo
            MessageBox.Show(this, $"TODO: Apply '{name}'", "Filter", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void EnableUndoRedo(bool undo, bool redo)
        {
            editUndoMenuItem.Enabled = btnUndo.Enabled = undo;
            editRedoMenuItem.Enabled = btnRedo.Enabled = redo;
        }
    }
}
