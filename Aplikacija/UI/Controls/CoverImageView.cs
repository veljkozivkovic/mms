using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Aplikacija.UI.Controls
{
    public enum ImageDisplayMode { Cover, Contain, FitWidth, FitHeight, ActualSize }
    public class CoverImageView : Control, ISupportInitialize
    {
        public ImageDisplayMode DisplayMode { get; set; } = ImageDisplayMode.Cover;

        
        public bool SmartContainForPortraits { get; set; } = true;
        private Image _image;
        public Image Image
        {
            get => _image;
            set { _image = value; Invalidate(); }
        }

        
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
                     ControlStyles.ResizeRedraw, true);   // za korektno repaint-ovanje
            DoubleBuffered = true;
            BackColor = Color.Black;
            Margin = new Padding(0);
        }

        
        public void BeginInit() { _initializing = true; }
        public void EndInit() { _initializing = false; Invalidate(); }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.Clear(BackColor);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.SmoothingMode = SmoothingMode.HighQuality;

            if (_image == null) return;
            int cw = ClientSize.Width, ch = ClientSize.Height; if (cw <= 0 || ch <= 0) return;
            int iw, ih; try { iw = _image.Width; ih = _image.Height; } catch { return; }
            if (iw <= 0 || ih <= 0) return;

            float sx = (float)cw / iw;
            float sy = (float)ch / ih;

            // izaberi mod
            var mode = DisplayMode;
            // ako je slika portret – uvek prikazi celu (Contain)
            if (SmartContainForPortraits && ih >= iw)
                mode = ImageDisplayMode.Contain;

            float scale = 1f;
            switch (mode)
            {
                case ImageDisplayMode.Cover: scale = Math.Max(sx, sy); break;
                case ImageDisplayMode.Contain: scale = Math.Min(sx, sy); break;
                case ImageDisplayMode.FitWidth: scale = sx; break;
                case ImageDisplayMode.FitHeight: scale = sy; break;
                case ImageDisplayMode.ActualSize: scale = 1f; break;
            }

            int w = Math.Max(1, (int)Math.Round(iw * scale));
            int h = Math.Max(1, (int)Math.Round(ih * scale));
            int x = (cw - w) / 2;
            int y = (ch - h) / 2;

            try { g.DrawImage(_image, new Rectangle(x, y, w, h)); } catch { /* ignore */ }

            if (_borderStyle == BorderStyle.FixedSingle)
                ControlPaint.DrawBorder(g, ClientRectangle, SystemColors.WindowFrame, ButtonBorderStyle.Solid);
            else if (_borderStyle == BorderStyle.Fixed3D)
                ControlPaint.DrawBorder3D(g, ClientRectangle, Border3DStyle.Sunken);
        }
    }
}
