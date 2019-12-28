using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doob_eternal_2001
{
    class Player
    {
        public Vector Position { get; set; }
        public double Height { get; set; }
        public double Rotation { get; set; }
        public Player(Vector pos, double z, float rot)
        {
            Position = pos;
            Height = z;
            Rotation = rot;
        }
    }
}
