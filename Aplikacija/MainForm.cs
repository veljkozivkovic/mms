using Aplikacija.Core.Filters;
using Aplikacija.Core.Imaging;
using Aplikacija.Core.IO;
using Aplikacija.UI.Controls;
using System;
using System.Drawing;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using System.Diagnostics;

namespace Aplikacija
{
    public partial class MainForm : Form
    {
        private Bitmap _currentBitmap;
        private readonly ImageDocument _doc = new();
        public MainForm()
        {
            InitializeComponent();
            pbImage.DisplayMode = ImageDisplayMode.Contain;  // uvek cela slika
            pbImage.SmartContainForPortraits = false;        
            pbImage.Dock = DockStyle.Fill;
            panelViewport.AutoScroll = false;
        }

        // ===== lifecycle =====
        private void MainForm_Load(object sender, EventArgs e)
        {
            lblInfo.Text = "Ready";
            UpdateUi();
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
        private void filterGaussianMenuItem_Click(object sender, EventArgs e)
            => ApplyFilter(new GaussianBlurFilter());

        private void filterMeanRemovalMenuItem_Click(object sender, EventArgs e)
            => ApplyFilter(new MeanRemovalFilter());

        private void filterBlacklightMenuItem_Click(object sender, EventArgs e)
            => ApplyFilter(new BlacklightFilter());

        private void filterHistEqMenuItem_Click(object sender, EventArgs e)
            => ApplyFilter(new HistogramEqualizationFilter());

        // ===== helpers =====
        private void SetBusy(bool busy)
        {
            UseWaitCursor = busy;
            menuMain.Enabled = !busy;
            toolMain.Enabled = !busy;
            Cursor.Current = busy ? Cursors.WaitCursor : Cursors.Default;
        }

        private static string FormatDuration(TimeSpan t)
        {
            if (t.TotalSeconds >= 1.0) return $"{t.TotalSeconds:F3} s";
            return $"{t.TotalMilliseconds:F1} ms";
        }

        private void BtnOpen()
        {
            using var ofd = new OpenFileDialog
            {
                Title = "Open image",
                Filter = "Images|*.bmp;*.png;*.jpg;*.jpeg;*.yuvimg|All files|*.*"
            };
            if (ofd.ShowDialog(this) != DialogResult.OK) return;

            try
            {
                Bitmap bmp;
                if (ofd.FileName.EndsWith(".yuvimg", StringComparison.OrdinalIgnoreCase))
                    bmp = YuvImgCodec.Load(ofd.FileName);
                else
                    bmp = new Bitmap(ofd.FileName);

                _doc.LoadBitmap(bmp);
                bmp.Dispose();
                pbImage.Image = _doc.Bitmap; // cover prikaz
                lblInfo.Text = $"{_doc.Bitmap.Width}×{_doc.Bitmap.Height}";
                UpdateUi();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Open failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSave()
        {
            if (_doc.Bitmap == null) return;
            using var sfd = new SaveFileDialog
            {
                Title = "Save custom format",
                Filter = "YUV Image (*.yuvimg)|*.yuvimg",
                DefaultExt = "yuvimg"
            };
            if (sfd.ShowDialog(this) != DialogResult.OK) return;

            try
            {
                YuvImgCodec.Save(sfd.FileName, _doc.Bitmap, Subsampling.YUV420);
                lblInfo.Text = $"Saved .yuvimg ({_doc.Bitmap.Width}×{_doc.Bitmap.Height})";
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Save failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowBitmapAt1x(Bitmap bmp)
        {
            if (bmp == null) return;

            
            pbImage.DisplayMode = ImageDisplayMode.ActualSize;

            
            pbImage.Dock = DockStyle.None;
            pbImage.Image = bmp;
            pbImage.Size = bmp.Size;                 
            pbImage.Location = new Point(0, 0);

            panelViewport.AutoScroll = true;
            panelViewport.AutoScrollMinSize = bmp.Size;

            lblInfo.Text = $"{bmp.Width}×{bmp.Height} px (1:1)";
        }

        private void BtnUndo()
        {
            _doc.Undo();
            pbImage.Image = _doc.Bitmap;
            UpdateUi();
        }

        private void BtnRedo()
        {
            _doc.Redo();
            pbImage.Image = _doc.Bitmap;
            UpdateUi();
        }

        private void ApplyFilter(IImageFilter filter)
        {
            if (_doc.Bitmap == null) return;

            SetBusy(true);
            var sw = Stopwatch.StartNew();
            try
            {
                _doc.Apply(src => filter.Apply(src));
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, $"Filter failed: {filter.Name}",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            finally
            {
                sw.Stop();
                SetBusy(false);
            }

            pbImage.Image = _doc.Bitmap;
            lblInfo.Text = $"Applied: {filter.Name}  •  {FormatDuration(sw.Elapsed)}  •  {_doc.Bitmap.Width}×{_doc.Bitmap.Height}";
            UpdateUi();
        }



        private void UpdateUi()
        {
            bool has = _doc.Bitmap != null;
            btnSave.Enabled = fileSaveMenuItem.Enabled = has;
            btnUndo.Enabled = editUndoMenuItem.Enabled = _doc.CanUndo;
            btnRedo.Enabled = editRedoMenuItem.Enabled = _doc.CanRedo;
        }

        private void EnableUndoRedo(bool undo, bool redo)
        {
            editUndoMenuItem.Enabled = btnUndo.Enabled = undo;
            editRedoMenuItem.Enabled = btnRedo.Enabled = redo;
        }
    }
}
