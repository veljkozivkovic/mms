using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Aplikacija.Core.Filters
{
    public interface IImageFilter
    {
        string Name { get; }
        Bitmap Apply(Bitmap src);
    }
}
