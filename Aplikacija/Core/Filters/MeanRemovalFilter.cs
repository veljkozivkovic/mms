using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Aplikacija.Core.Filters
{
    public class MeanRemovalFilter : IImageFilter
    {
        public string Name => "Mean Removal";
        public Bitmap Apply(Bitmap src)
            => new GaussianBlurFilterProxy(new int[,] {
                 {-1,-1,-1},
                 {-1, 9,-1},
                 {-1,-1,-1}
               }, 1).Apply(src);

        // mali proxy da ne dupliramo kod
        private class GaussianBlurFilterProxy : IImageFilter
        {
            private readonly int[,] _k; private readonly int _div;
            public GaussianBlurFilterProxy(int[,] k, int div) { _k = k; _div = div; }
            public string Name => "3x3 Conv";
            public Bitmap Apply(Bitmap src) => GaussianBlurFilter_Util.Convolve3x3(src, _k, _div);
        }

        private static class GaussianBlurFilter_Util
        {
            public static Bitmap Convolve3x3(Bitmap src, int[,] k, int div)
                => (new GaussianBlurFilter()).GetType()
                    .GetMethod("Convolve3x3", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
                    .Invoke(null, new object[] { src, k, div }) as Bitmap;
        }
    }
}
