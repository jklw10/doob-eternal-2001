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
        double deg2rad = 1f/ 180f * Math.PI;
        Rectangle[] lines;
        private Brush brush = new SolidBrush(Color.FromArgb(255, 0, 125, 0));
        //Brush debugbrush = new SolidBrush(Color.FromArgb(255, 255,0, 0));
        List<Shape> map = new List<Shape>();
        Player player = new Player(new Vector(0, 0), (float)Math.PI);
        //Bitmap prevFrame;
        //Queue<Bitmap> frameBuffer = new Queue<Bitmap>();
        int fov = 90;
        int screenWidth = 200;
        //int scale = 10;
        //double d = 0;
        //Vector examplePos = (400, 400);
        Vector mouseDelta;
        Vector prevMousePos;
        //Thread updateThread;
        public Game()
        {
            InitializeComponent();
            DoubleBuffered = true;
            
        }
        
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            mouseDelta = (Vector)e.Location - prevMousePos;
            player.Rotation += (mouseDelta.X /200) ;
            prevMousePos = (Vector)e.Location;
            Refresh();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            ///*
            //e.Graphics.DrawLine(new Pen(debugbrush), (Point)player.Position, (Point)(Vector.RadiansToVector(player.Rotation, 20)));
            for (int x = 0; x < lines.Length; x++)
            {
                int lineHeight = Clamp(ShootTowards(x), 0, 255);
                lines[x] = new Rectangle(new Point(50 + x * 2, 50 + (255 - lineHeight) / 2), new Size(2, lineHeight));
                brush = new SolidBrush(Color.FromArgb(255, 0, lines[x].Height, 0));
                e.Graphics.FillRectangle(brush, lines[x]);
            }
        }
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
        private void Game_Paint(object sender, PaintEventArgs e)
        {
        }
        private void Game_Load(object sender, EventArgs e)
        {
            Shape tri   = new Shape((1, 0), (1, 1), (0, 1));
            Shape rect = new Shape((3, 3), (3, 4), (2, 4), (2, 3));
            Shape rect2 = new Shape((3, 3), (3, 4), (2, 4), (2, 3));
            rect2.Move((2, 2));
            rect2.Scale((20, 20));
            map.Add(rect2);
            map.Add(tri);
            map.Add(rect);
            lines = new Rectangle[screenWidth];

        }
        public int ShootTowards(double x)
        {
            double RayHitDistance = 0;
            double DepthBuffer = double.MaxValue; 
            //do the raycast for all the shapes in the map
            foreach (Shape shape in map)
            {
                //go through all corners in a shape
                foreach(Vector corner in shape.Corners)
                {
                    //connect this corner to the next corner for a line to intersect check with.
                    for(int a = 0; a < shape.Corners.Length; a++)
                    {
                        int b = a+1;
                        if(a == shape.Corners.Length - 1)
                        {
                            b = 0;
                        }
                        Vector CornerA = shape.Corners[a];
                        Vector CornerB = shape.Corners[b];
                        Vector CameraRay = Vector.RadiansToVector((player.Rotation - fov*deg2rad)+ (x / (screenWidth*2)) * fov * deg2rad, 1);
                        
                        double CurrentRayHitDistance = 0;
                        
                        if (Vector.RayCast(player.Position, CameraRay, CornerA, CornerB, out Vector intersect))
                        {
                            CurrentRayHitDistance = (intersect-player.Position).Magnitude;
                            if (CurrentRayHitDistance < DepthBuffer)
                            {
                                RayHitDistance = CurrentRayHitDistance;
                                DepthBuffer = CurrentRayHitDistance;
                                //e.Graphics.FillRectangle(debugbrush, new Rectangle((Point)(examplePos + (intersect) * scale), new Size(3, 3)));
                            }
                            

                        }
                        if(Vector.IsBetween(CameraRay + player.Position, CornerA, CornerB))
                        {
                            //e.Graphics.FillRectangle(debugbrush, new Rectangle((Point)(examplePos + (CameraRay + player.Position) * scale), new Size(3, 3)));
                        }
                    }
                }
            }
            if(RayHitDistance != 0 )
            {
                return (int)(screenWidth / RayHitDistance);
            }
            else
            {
                return 0;
            }
        }
        
        private void Game_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == 's')
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
            
            this.Refresh();
            
        }
        
    }
}
