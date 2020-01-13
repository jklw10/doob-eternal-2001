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
                Vector CameraRay = Vector.RadiansToVector((player.Rotation - fov * deg2rad) + (x / (screenWidth * 1f)) * fov * deg2rad, 1);
                foreach (RayCast cast in ShootTowards(player.Position,CameraRay))
                {
                    
                    double wallDistance = Math.Sqrt(cast.Length * cast.Length + cast.Z * cast.Z);
                    double depthMulti = screenWidth / wallDistance;
                    lineHeight = (int)(depthMulti * cast.WallHeight*(1f+Math.Cos(CameraRay.Angle-player.Rotation+((fov/2f)*deg2rad))));

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
            tri.Scale((1,1),0.2);
            Shape rect2 = new Shape(-1, 1, new Shader(255,0,125,0),(1, 0), (0, 0), (0, 1), (1, 1));
            
            rect2.Scale((20, 20), 5);
            ToDraw.Add(rect2);
            ToDraw.Add(tri);

        }
        /// <summary>
        /// shoots a ray between 0-screenwidth
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        Queue<RayCast> ShootTowards(Vector Position, Vector RayDirection)
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
                        

                        double CurrentRayHitDistance = 0;

                        if (Vector.RayCast(player.Position, RayDirection, shape.Corners[a], shape.Corners[b], out Vector intersect))
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
            Queue<RayCast> casts = ShootTowards(thing.Position, direction);
            double l = casts.Last().Length;
            if (l >= direction.Magnitude)
            {
                thing.Position += Vector.RadiansToVector(direction.Angle, l - 0.01);
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
