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
        readonly int fov = 90;
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
            Input.Initialize(this);
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
        void OnUpdateFrame()
        {
            Vector moveBuffer = (0,0);
            if (Input.KeyDown(Keys.W))
            {
                moveBuffer -= Vector.RotateVector(new Vector(0, 1), player.Rotation + fov * deg2rad / 2);
            }
            if (Input.KeyDown(Keys.A))
            {
                moveBuffer -= Vector.RotateVector(new Vector(1, 0), player.Rotation + fov * deg2rad / 2);
            }
            if (Input.KeyDown(Keys.S))
            {
                moveBuffer += Vector.RotateVector(new Vector(0, 1), player.Rotation + fov * deg2rad / 2);
            }
            if (Input.KeyDown(Keys.D))
            {
                moveBuffer += Vector.RotateVector(new Vector(1, 0), player.Rotation + fov * deg2rad / 2);
            }
            if (Input.KeyDown(Keys.Q))
            {
                player.Rotation -= 1 * LFT / 1000;
            }
            if (Input.KeyDown(Keys.E))
            {
                player.Rotation += 1 * LFT / 1000;
            }
            if (moveBuffer.MagnitudeSquared >= 0.1)
            {
                CollisionMove(player, moveBuffer.Normalize() * (LFT / 200f));
            }
            Input.Update();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            OnUpdateFrame();
            Brush brush = new SolidBrush(Color.FromArgb(255, 0, 125, 0));
            Pen pen = new Pen(brush);
            Shader s = new Shader(Color.Black);
            int lineHeight;
            Shape line = new Shape(0, 0, Color.Black, (0, 0), (0, 0));
            List<RayCast> casts = new List<RayCast>();
            for (int x = 0; x < screenWidth; x++)
            {
                foreach (RayCast cast in ShootTowards(x))
                {

                    double wallDistance = Math.Sqrt(cast.Length * cast.Length + cast.Z * cast.Z);
                    double depthMulti = screenWidth / wallDistance;
                    lineHeight = (int)(depthMulti * cast.WallHeight);

                    line.Corners[0] = (x, depthMulti * cast.Z + (screenHeight - lineHeight) / 2);
                    line.Corners[1] = (x, depthMulti * cast.Z + (screenHeight + lineHeight) / 2);
                    
                    s = cast.Color;
                    s /= 255 *(wallDistance/cast.WallHeight /255);
                    
                    pen.Color = s;
                    e.Graphics.DrawLine(pen, (Point)line.Corners[0], (Point)line.Corners[1]);
                    //*/
                }
            }
            brush.Dispose();
            pen.Dispose();
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
            ToDraw[1].SetPos((2,2), Math.Sin(t)/10);

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
            tri.Move((2, 2), 0);
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
                        
                        Vector CameraRay = Vector.RadiansToVector((player.Rotation - fov * deg2rad) + (x / (screenWidth *1)) * fov * deg2rad, 1);

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
        void CollisionMove(Player thing, Vector direction)
        {
            
            bool blocked = false;
            double shortestDist = double.MaxValue;
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


                        double CurrentRayHitDistance;

                        if (Vector.SegmentIntersect(thing.Position, thing.Position+ direction, shape.Corners[a], shape.Corners[b], out Vector intersect))
                        {
                            blocked = true;
                            CurrentRayHitDistance = (intersect - thing.Position).Magnitude;
                            if(CurrentRayHitDistance < shortestDist)
                            {
                                shortestDist = CurrentRayHitDistance;
                            }
                        }
                    }
                }
            }
            if (blocked)
            {
                //thing.Position += direction.Normalize()*(shortestDist-0.0001);
                thing.Position += direction;
            }
            else
            {
                thing.Position += direction;
            }
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            mouseDelta = (Vector)e.Location - prevMousePos;
            player.Rotation += (mouseDelta.X * LFT / 2000);
            prevMousePos = (Vector)e.Location;
        }
        private void ExitButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
