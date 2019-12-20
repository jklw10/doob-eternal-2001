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
        double rotation;
        public double Rotation;
        public Player(Vector pos, float rot)
        {
            Position = pos;
            Rotation = rot;
        }
    }
}
