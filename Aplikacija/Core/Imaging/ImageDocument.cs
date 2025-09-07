using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Aplikacija.Core.Imaging
{
    public class ImageDocument : IDisposable
    {
        public Bitmap Bitmap { get; private set; }
        public readonly UndoRedoManager<Bitmap> History = new()
        {
            
            OnDrop = (bmp) => { try { bmp?.Dispose(); } catch { } }
        };

        public void LoadBitmap(Bitmap bmp)
        {
            Bitmap?.Dispose();
            Bitmap = DeepClone(bmp);
            History.Reset(DeepClone(Bitmap)); 
        }

        public void Apply(Func<Bitmap, Bitmap> transform)
        {
            if (Bitmap == null) return;

            
            var next = transform(Bitmap);
            if (next == null) return; 

            
            History.Push(DeepClone(next));

            
            var old = Bitmap;
            Bitmap = next;
            old?.Dispose();
        }

        public bool CanUndo => History.CanUndo;
        public bool CanRedo => History.CanRedo;

        public void Undo()
        {
            if (!CanUndo) return;

            
            var snapshot = History.Undo();
            if (snapshot != null)
            {
                var old = Bitmap;
                Bitmap = DeepClone(snapshot);
                old?.Dispose();
            }
        }

        public void Redo()
        {
            if (!CanRedo) return;

            var snapshot = History.Redo();
            if (snapshot != null)
            {
                var old = Bitmap;
                Bitmap = DeepClone(snapshot);
                old?.Dispose();
            }
        }

        public void Dispose() => Bitmap?.Dispose();

        private static Bitmap DeepClone(Bitmap src)
        {
            if (src == null) return null;
            return src.Clone(new Rectangle(0, 0, src.Width, src.Height),
                             PixelFormat.Format32bppArgb);
        }
    }
}
