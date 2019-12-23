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
        public void Move(Vector movement)
        {
            for (int i = 0; i < Corners.Length; i++)
            {
                Corners[i] += movement;
            }
        }
        public void Scale(Vector scale)
        {
            Vector tempVal = (0,0);
            for (int i = 0; i < Corners.Length; i++)
            {
                tempVal += Corners[i];
            }
            Vector middle = tempVal / Corners.Length;
            for (int i = 0; i < Corners.Length; i++)
            {
                Corners[i] = (middle - Corners[i]) *scale +middle;
            }
        }
    }
}
