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
        public double Z;
        public double WallHeight;
        public Color Color;
        public Shape(double z,double wallHeight, Color color,params Vector[] corners)
        {
            Corners = corners;
            Z = z;
            WallHeight = wallHeight;
            Color = color;
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
        public void Move(Vector movement,double z)
        {
            for (int i = 0; i < Corners.Length; i++)
            {
                Corners[i] += movement;
            }
            Z = z;
        }
        public void SetPos(Vector pos, double z)
        {
            Vector tempVal = (0, 0);
            for (int i = 0; i < Corners.Length; i++)
            {
                tempVal += Corners[i];
            }
            Vector middle = tempVal / Corners.Length;
            for (int i = 0; i < Corners.Length; i++)
            {
                Corners[i] = ((0,0)-(middle - Corners[i])) + pos;
            }
            Z = z;
        }
        public void Scale(Vector scale, double wallHeight)
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
            WallHeight = wallHeight;
        }
        public void Rotate(double angle, double wallHeight)
        {
            Vector tempVal = (0, 0);
            for (int i = 0; i < Corners.Length; i++)
            {
                tempVal += Corners[i];
            }
            Vector middle = tempVal / Corners.Length;
            for (int i = 0; i < Corners.Length; i++)
            {
                Corners[i] = Vector.RotateVector(middle - Corners[i],angle) + middle;
            }
            WallHeight = wallHeight;
        }
    }
}
