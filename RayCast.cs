using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Doob_eternal_2001
{
    struct RayCast : IComparable<RayCast>
    {
        public double Length;
        public double WallHeight;
        public double Z;
        public Shader Color;
        public RayCast(double length, double height, double z, Shader color)
        {
            Length = length;
            WallHeight = height;
            Z = z;
            Color = color;
        }
        public int CompareTo(RayCast other)
        {
            return other.Length.CompareTo(Length);
        }
    }
}
