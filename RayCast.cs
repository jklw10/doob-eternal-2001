using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doob_eternal_2001
{
    struct RayCast : IComparable<RayCast>
    {
        public double Length;
        public double WallHeight;
        public double Z;
        public RayCast(double length, double height, double z)
        {
            Length = length;
            WallHeight = height;
            Z = z;
        }
        public int CompareTo(RayCast other)
        {
            return other.Length.CompareTo(Length);
        }
    }
}
