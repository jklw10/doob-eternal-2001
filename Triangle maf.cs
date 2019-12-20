using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Doob_eternal_2001
{
    class Shape
    {
        public Vector[] Corners;
        public Color Color;
        public Shape(params Vector[] corners)
        {
            Corners = corners;
        }
        public static implicit operator PointF[](Shape v) 
        {
            List<PointF> returned = new List<PointF>();
            foreach(Vector place in v.Corners)
            {
                returned.Add(new PointF((float)place.X,(float)place.Y));
            }
            return returned.ToArray();
        }
    }
}
