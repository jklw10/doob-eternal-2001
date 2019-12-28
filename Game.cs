using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

namespace Doob_eternal_2001
{
    public partial class Game : Form
    {
        double deg2rad = 1f / 180f * Math.PI;
        private Brush brush = new SolidBrush(Color.FromArgb(255, 0, 125, 0));
        static List<Shape> ToDraw = new List<Shape>();
        Player player = new Player(new Vector(0, 0), 1, (float)Math.PI);
        int fov = 90;
        int screenWidth = 400;
        int screenHeight = 400;
        Vector mouseDelta;
        Vector prevMousePos;
        public Game()
        {
            InitializeComponent();
            DoubleBuffered = true;

        }
        internal static void UpdateToDraw(List<Shape> newShapeList)
        {
            ToDraw = newShapeList;
        }
        
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            for (int x = 0; x < screenWidth; x++)
            {
                int lineHeight;
                Shape line;
                foreach (RayCast cast in ShootTowards(x))
                {
                    lineHeight = Clamp((int)((screenWidth / cast.Length) * cast.WallHeight), 0, screenHeight);
                    line = new Shape(cast.Z, cast.WallHeight, (50 + x, 50 + (screenHeight - lineHeight) / 2), (50 + x, 50 + (screenHeight + lineHeight) / 2));
                    int WallDistance = Clamp((int)(screenWidth / cast.Length), 1, 255);
                    brush = new SolidBrush(Color.FromArgb(255, 0, WallDistance, 0));
                    e.Graphics.DrawLine(new Pen(brush), (Point)line.Corners[0], (Point)line.Corners[1]);
                }
            }
        }
        /// <summary>
        /// clamps a value between min and max
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public int Clamp(int value, int min, int max)
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
        private void Game_Load(object sender, EventArgs e)
        {
            Shape tri = new Shape(1, 1, (1, 0), (1, 1), (0, 1));
            Shape rect = new Shape(1, 1, (1, 0), (0, 0), (0, 1), (1, 1));
            Shape rect2 = new Shape(1, 1, (1, 0), (0, 0), (0, 1), (1, 1));
            rect.Move((2, 2), 10);
            rect2.Scale((20, 20), 20);
            ToDraw.Add(rect2);
            ToDraw.Add(tri);
            ToDraw.Add(rect);

        }
        /// <summary>
        /// shoots a ray between 0-screenwidth
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        Queue<RayCast> ShootTowards(double x)
        {
            double RayHitDistance = 0;
            Queue<RayCast> casts = new Queue<RayCast>();
            List<RayCast> castsUnsorted = new List<RayCast>();
            //do the raycast for all the shapes in the map
            for (int i = 0; i < ToDraw.Count; i++)
            {
                Shape shape = ToDraw[i];
                //go through all corners in a shape
                foreach (Vector corner in shape.Corners)
                {
                    //connect this corner to the next corner for a line to intersect check with.
                    for (int a = 0; a < shape.Corners.Length; a++)
                    {
                        int b = a + 1;
                        if (a == shape.Corners.Length - 1)
                        {
                            b = 0;
                        }
                        Vector CornerA = shape.Corners[a];
                        Vector CornerB = shape.Corners[b];
                        Vector CameraRay = Vector.RadiansToVector((player.Rotation - fov * deg2rad) + (x / (screenWidth * 2)) * fov * deg2rad, 1);

                        double CurrentRayHitDistance = 0;

                        if (Vector.RayCast(player.Position, CameraRay, CornerA, CornerB, out Vector intersect))
                        {
                            CurrentRayHitDistance = (intersect - player.Position).Magnitude;
                            castsUnsorted.Add(new RayCast(CurrentRayHitDistance, ToDraw[i].WallHeight, ToDraw[i].Z));
                        }
                    }
                }
            }
            castsUnsorted.Sort();
            foreach ( RayCast cast in castsUnsorted)
            {
                casts.Enqueue(cast);
            }
            
            if (RayHitDistance != 0)
            {
                return casts;
            }
            else
            {
                casts.Enqueue(new RayCast(0, 0, 0));
                return casts;
            }
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            mouseDelta = (Vector)e.Location - prevMousePos;
            player.Rotation += (mouseDelta.X / 200);
            prevMousePos = (Vector)e.Location;
            this.Invalidate();
            this.Update();
        }
        private void Game_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 's')
            {
                player.Position += Vector.RotateVector(new Vector(0, 0.1), player.Rotation + Math.PI / 4);
            }
            if (e.KeyChar == 'w')
            {
                player.Position -= Vector.RotateVector(new Vector(0, 0.1), player.Rotation + Math.PI / 4);
            }
            if (e.KeyChar == 'd')
            {
                player.Position += Vector.RotateVector(new Vector(0.1, 0), player.Rotation + Math.PI / 4);
            }
            if (e.KeyChar == 'a')
            {
                player.Position -= Vector.RotateVector(new Vector(0.1, 0), player.Rotation + Math.PI / 4);
            }
            if (e.KeyChar == 'q')
            {
                player.Rotation -= 0.1;
            }
            if (e.KeyChar == 'e')
            {
                player.Rotation += 0.1;
            }
            this.Invalidate();
            this.Update();
        }
    }
}
