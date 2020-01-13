using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Doob_eternal_2001
{

    struct Vector3D : IComparable<Vector3D>
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public Vector XY
        {
            get { return (X, Y); }
            set { X = value.X; Y = value.Y; }
        }
        public Vector XZ
        {
            get { return (X, Z); }
            set { X = value.X; Z = value.Y; }
        }
        public Vector YZ
        {
            get { return (Y, Z); }
            set { Y = value.X; Z = value.Y; }
        }


        public double Magnitude
        {
            get { return Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2)); }
            private set { }
        }

        public Vector Angle
        {
            get { return VectorToRadians(this); }
            set { }
        }

        public override string ToString()
        {
            return "(" + X + "," + Y + "," + Z + ")";
        }

        /// <summary>
        /// (X, Y)
        /// </summary>
        /// <param name="coordinate"></param>
        public Vector3D(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Vector3D(Vector xy, double z)
        {
            X = xy.X;
            Y = xy.Y;
            Z = z;
        }
        /// <summary>
        /// creates a vector of (size, size, size)
        /// </summary>
        /// <param name="size"></param>
        public Vector3D(double size)
        {
            X = size;
            Y = size;
            Z = size;
        }
        public int CompareTo(Vector3D other)
        {
            // The magnitude comparison depends on the comparison of 
            // the underlying Double values. 
            return Magnitude.CompareTo(other.Magnitude);
        }

        public static bool operator >(Vector3D operand1, Vector3D operand2)
        {
            return operand1.CompareTo(operand2) == 1;
        }

        // Define the is less than operator.
        public static bool operator <(Vector3D operand1, Vector3D operand2)
        {
            return operand1.CompareTo(operand2) == -1;
        }

        // Define the is greater than or equal to operator.
        public static bool operator >=(Vector3D operand1, Vector3D operand2)
        {
            return operand1.CompareTo(operand2) >= 0;
        }

        // Define the is less than or equal to operator.
        public static bool operator <=(Vector3D operand1, Vector3D operand2)
        {
            return operand1.CompareTo(operand2) <= 0;
        }
        public static bool operator ==(Vector3D operand1, Vector3D operand2)
        {
            return operand1.CompareTo(operand2) == 0;
        }
        public static bool operator !=(Vector3D operand1, Vector3D operand2)
        {
            return operand1.CompareTo(operand2) != 0;
        }
        public static Vector3D operator +(Vector3D a, Vector b)
        {
            return new Vector3D(a.X + b.X, a.Y + b.Y, a.Z);
        }
        public static Vector3D operator -(Vector3D a, Vector b)
        {
            return new Vector3D(a.X - b.X, a.Y - b.Y, a.Z);
        }
        public static Vector3D operator +(Vector3D a, Vector3D b)
        {
            return new Vector3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Vector3D operator -(Vector3D a, Vector3D b)
        {
            return new Vector3D(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static Vector3D operator *(Vector3D a, double b)
        {
            return new Vector3D((int)(a.X * b), (int)(a.Y * b), (int)(a.Z * b));
        }

        public static Vector3D operator *(Vector3D a, Vector3D b)
        {
            return new Vector3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Vector3D operator /(Vector3D a, double b)
        {
            return new Vector3D((int)Math.Round(a.X / b), (int)Math.Round(a.Y / b), (int)Math.Round(a.Z / b));
        }

        public static Vector3D operator /(Vector3D a, Vector3D b)
        {
            return new Vector3D(a.X / b.X, a.Y / b.Y, a.Z / b.Z);
        }

        public static Vector3D operator %(Vector3D a, Vector3D b)
        {
            return a - Floor(a / b) * b;
        }

        public static Vector3D Floor(Vector3D a)
        {
            return new Vector3D((int)Math.Floor(a.X), (int)Math.Floor(a.Y), (int)Math.Floor(a.Z));
        }
        /// <summary>
        /// converts from a Vector to radians;
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector VectorToRadians(Vector3D v)
        {
            return new Vector(Math.Atan2(v.Y, v.X), Math.Atan2(v.Z, v.XY.Magnitude));
        }

        public static explicit operator string(Vector3D v)
        {
            return "(" + v.X + ", " + v.Y + ", " + v.Z + ")";
        }

        public static implicit operator (double,double,double)(Vector3D v)
        {
            return (v.X, v.Y, v.Z);
        }

        public static implicit operator Vector3D((int x, int y, int z) position)
        {
            return new Vector3D(position.x, position.y, position.z);
        }//*/
        public static implicit operator Vector3D((IntVector v, int z) position)
        {
            return new Vector3D(position.v.X, position.v.Y, position.z);
        }
    }
}
