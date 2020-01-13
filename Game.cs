using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Doob_eternal_2001
{
    public partial class Game : Form
    {
        readonly double deg2rad = 1f / 180f * Math.PI;
        static List<Shape> ToDraw = new List<Shape>();
        readonly Player player = new Player(new Vector(0, 0), 0, (float)Math.PI);
        readonly int fov = 180;
        readonly int screenWidth = 800;
        readonly int screenHeight = 400;
        double t = 0;
        Vector mouseDelta;
        Vector prevMousePos;
        long LFT = 0;
        System.Diagnostics.Stopwatch watch;
        public Game()
        {

            watch = System.Diagnostics.Stopwatch.StartNew();
            InitializeComponent();
            DoubleBuffered = true;
            Width = screenWidth;
            Height = screenHeight;

            Timer timer = new Timer();
            timer.Interval = (1000/120); // 120fps
            timer.Tick += new EventHandler(Tick);
            timer.Start();
        }

        private void Tick(object sender, EventArgs e)
        {

            Invalidate();
            Update();
        }
        internal static void UpdateToDraw(List<Shape> newShapeList)
        {
            ToDraw = newShapeList;
        }
        
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Brush brush = new SolidBrush(Color.FromArgb(255, 0, 125, 0));
            Pen pen = new Pen(brush);
            Shader s = new Shader(Color.Black);
            for (int x = 0; x < screenWidth; x++)
            {
                int lineHeight;
                Shape line;
                foreach (RayCast cast in ShootTowards(x))
                {
                    double wallDistance = Math.Sqrt(cast.Length * cast.Length + cast.Z * cast.Z);
                    double depthMulti = screenWidth / wallDistance;
                    lineHeight = (int)(depthMulti * cast.WallHeight);
                    line = new Shape(0, 0, cast.Color, ( x, depthMulti * cast.Z+(screenHeight - lineHeight) / 2), (x, depthMulti* cast.Z+(screenHeight + lineHeight) / 2));
                    s = cast.Color;
                    s /= 255 *( Math.Pow(wallDistance/cast.WallHeight /255,1));
                    
                    pen.Color = s;
                    e.Graphics.DrawLine(pen, (Point)line.Corners[0], (Point)line.Corners[1]);
                    brush.Dispose();
                }
            }
            using ( Brush brush2 = new SolidBrush(Color.FromArgb(255, 255, 0, 0)))
            {
                Font font = new Font("Arial", 16);
                e.Graphics.DrawString("ms since last frame: " + LFT, font, brush2, 50, 50);
            }

            /*
            foreach (Shape shape in ToDraw)
            {
                shape.Scale((100, 100), 1);
                e.Graphics.DrawPolygon(new Pen(Brushes.Red), shape.Corners.Select(x => new PointF((float)x.X+50, (float)x.Y+50)).ToArray());

                shape.Scale((0.01, 0.01), 1);
            }*/
            t += 0.1;
            ToDraw[1].SetPos((0,0), Math.Sin(t)/10);

            LFT = watch.ElapsedMilliseconds;

            watch = System.Diagnostics.Stopwatch.StartNew();
        }
        private void Game_Load(object sender, EventArgs e)
        {
            Shape tri   = new Shape(1, 1,Color.Blue, (1, 0), (1, 1), (0, 1));
            Shape rect  = new Shape(1, 1,Color.Blue, (1, 0), (0, 0), (0, 1), (1, 1));
            int cornerc = 8;
            Vector[] starCorners = new Vector[cornerc];

            double starRots = (360 / cornerc) * deg2rad;
            for (int i = 0; i < starCorners.Length; i++)
            {
                //if (i % 2 == 0)
                //{ 
                //    starCorners[i] = Vector.RotateVector((1 , 0),starRots*i);
                //}
                //else
                {
                    starCorners[i] = Vector.RotateVector((0.5 + (i%2), 0), starRots * i);
                }
            }
            Shape treePart = new Shape(0, 1, new Shader(125, 0, 0, 125), starCorners);
            tri.Scale((1,1),0.2);
            
            Shape rect2 = new Shape(-1, 1, new Shader(255,0,125,0),(1, 0), (0, 0), (0, 1), (1, 1));
            rect.Move((2, 2), 0);
            rect2.Scale((20, 20), 5);
            ToDraw.Add(rect2);
            ToDraw.Add(tri);

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
                        
                        Vector CameraRay = Vector.RadiansToVector((player.Rotation - fov * deg2rad) + (x / (screenWidth * 2)) * fov * deg2rad, 1);

                        double CurrentRayHitDistance = 0;

                        if (Vector.RayCast(player.Position, CameraRay, shape.Corners[a], shape.Corners[b], out Vector intersect))
                        {
                            CurrentRayHitDistance = (intersect - player.Position).Magnitude;
                            castsUnsorted.Add(new RayCast(CurrentRayHitDistance, ToDraw[i].WallHeight, ToDraw[i].Z-player.Height, ToDraw[i].Color));
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
                casts.Enqueue(new RayCast(0, 0, 0,Color.White));
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
                player.Position += Vector.RotateVector(new Vector(0, 0.1), player.Rotation );

            }
            if (e.KeyChar == 'w')
            {
                player.Position -= Vector.RotateVector(new Vector(0, 0.1), player.Rotation );
            }
            if (e.KeyChar == 'd')
            {
                player.Position += Vector.RotateVector(new Vector(0.1, 0), player.Rotation );
            }
            if (e.KeyChar == 'a')
            {
                player.Position -= Vector.RotateVector(new Vector(0.1, 0), player.Rotation );
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

        private void ExitButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
