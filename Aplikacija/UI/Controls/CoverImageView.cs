using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Aplikacija.UI.Controls
{
    public class CoverImageView : Control, ISupportInitialize
    {
        private Image _image;
        public Image Image
        {
            get => _image;
            set { _image = value; Invalidate(); }
        }

        // shim-ovi zbog Designer-a
        private PictureBoxSizeMode _sizeMode = PictureBoxSizeMode.Zoom;
        public PictureBoxSizeMode SizeMode { get => _sizeMode; set { _sizeMode = value; Invalidate(); } }

        private BorderStyle _borderStyle = BorderStyle.None;
        public BorderStyle BorderStyle { get => _borderStyle; set { _borderStyle = value; Invalidate(); } }

        private bool _initializing;

        public CoverImageView()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw, true);   // <— bitno za korektno repaint-ovanje
            DoubleBuffered = true;
            BackColor = Color.Black;
            Margin = new Padding(0);
        }

        // ISupportInitialize no-op
        public void BeginInit() { _initializing = true; }
        public void EndInit() { _initializing = false; Invalidate(); }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // priprema
            var g = e.Graphics;
            g.Clear(BackColor);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.SmoothingMode = SmoothingMode.HighQuality;

            // ako još uvek inicijalizuje Designer ili nema slike — nema crtanja
            if (_initializing || _image == null) return;

            // dimenzije klijenta
            int cw = ClientSize.Width, ch = ClientSize.Height;
            if (cw <= 0 || ch <= 0) return;

            // dimenzije slike (mogu da bace ObjectDisposedException)
            int iw, ih;
            try { iw = _image.Width; ih = _image.Height; }
            catch { return; }
            if (iw <= 0 || ih <= 0) return;

            // cover skala – čuvaj se NaN/Infinity
            float sx = (float)cw / iw;
            float sy = (float)ch / ih;
            if (float.IsNaN(sx) || float.IsInfinity(sx) || float.IsNaN(sy) || float.IsInfinity(sy)) return;

            float scale = Math.Max(sx, sy);
            int w = Math.Max(1, (int)Math.Round(iw * scale));
            int h = Math.Max(1, (int)Math.Round(ih * scale));
            int x = (cw - w) / 2;
            int y = (ch - h) / 2;

            var dest = new Rectangle(x, y, w, h);
            if (dest.Width <= 0 || dest.Height <= 0) return;

            try
            {
                g.DrawImage(_image, dest);
            }
            catch (ArgumentException)
            {
                // ako GDI+ ipak poblesavi, samo preskoči frame (izbegni rušenje)
            }

            // Border po potrebi
            if (_borderStyle == BorderStyle.FixedSingle)
                ControlPaint.DrawBorder(g, ClientRectangle, SystemColors.WindowFrame, ButtonBorderStyle.Solid);
            else if (_borderStyle == BorderStyle.Fixed3D)
                ControlPaint.DrawBorder3D(g, ClientRectangle, Border3DStyle.Sunken);
        }
    }
}
