using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
namespace Doob_eternal_2001
{
    
    class Shader
    {
        Color color;
        public int A, R, G, B;
        public Shader(Color c)
        {
            A = c.A;
            R = c.R;
            G = c.G;
            B = c.B;
            color = c;
        }
        public Shader(int a,int r, int g, int b)
        {
            A = Clamp(a, 0, 255);
            R = Clamp(r, 0, 255);
            G = Clamp(g, 0, 255);
            B = Clamp(b, 0, 255);
            color = Color.FromArgb(A,R,G,B);
        }
        public static int Clamp(int value, int min, int max)
        {
            if (value < min)
            {
                value = min;
            }
            else if (value > max)
            {
                value = max;
            }
            return value;
        }

        public static implicit operator Color(Shader w)
        {
            return w.color;
        }
        public static implicit operator Shader(Color w)
        {
            return new Shader(w);
        }
        public static Shader operator +(Shader a, Shader b)
        {
            return new Shader(a.A + b.A, a.R + b.R, a.G + b.G, a.B+ b.B);
        }

        public static Shader operator -(Shader a, Shader b)
        {
            return new Shader(a.A - b.A, a.R - b.R, a.G - b.G, a.B - b.B);
        }

        public static Shader operator *(Shader a, double b)
        {
            return new Shader(a.A, (int)(a.R * b), (int)(a.G * b), (int)(a.B * b));
        }

        public static Shader operator *(Shader a, Shader b)
        {
            return new Shader(a.A * b.A, a.R * b.R, a.G * b.G, a.B * b.B);
        }

        public static Shader operator /(Shader a, double b)
        {
            return new Shader(a.A, (int)(a.R / b), (int)(a.G / b), (int)(a.B / b));
        }

        public static Shader operator /(Shader a, Shader b)
        {
            return new Shader(a.A / b.A, a.R / b.R, a.G / b.G, a.B / b.B);
        }
    }
}
