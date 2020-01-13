using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doob_eternal_2001
{
    class Input
    {
        private static List<Keys> keysDown;
        private static List<Keys> keysDownLast;
        private static List<MouseButtons> buttonsDown;
        private static List<MouseButtons> buttonsDownLast;

        public static void Initialize(Game game)
        {
            
            keysDown        = new List<Keys>();
            keysDownLast    = new List<Keys>();
            buttonsDown     = new List<MouseButtons>();
            buttonsDownLast = new List<MouseButtons>();

            game.MouseDown  += Game_MouseDown;
            game.MouseUp    += Game_MouseUp;
            game.KeyDown    += Game_KeyDown;
            game.KeyUp      += Game_KeyUp;
        }

        private static void Game_KeyUp(object sender, KeyEventArgs e)
        {
            while (keysDown.Contains(e.KeyCode))
                keysDown.Remove(e.KeyCode);
        }

        private static void Game_KeyDown(object sender, KeyEventArgs e)
        {
            if (!keysDown.Contains(e.KeyCode))
                keysDown.Add(e.KeyCode);
        }

        private static void Game_MouseUp(object sender, MouseEventArgs e)
        {
            while (buttonsDown.Contains(e.Button))
                buttonsDown.Remove(e.Button);
        }

        private static void Game_MouseDown(object sender, MouseEventArgs e)
        {
            if (!buttonsDown.Contains(e.Button))
                buttonsDown.Add(e.Button);
        }

        public static void Update()
        {
            keysDownLast = new List<Keys>(keysDown);
            buttonsDownLast = new List<MouseButtons>(buttonsDown);
        }

        public static bool KeyPress(Keys key)
        {
            return (keysDown.Contains(key) && !keysDownLast.Contains(key));
        }
        public static bool KeyRelease(Keys key)
        {
            return (!keysDown.Contains(key) && keysDownLast.Contains(key));
        }
        public static bool KeyDown(Keys key)
        {
            return (keysDown.Contains(key));
        }

        public static bool MousePress(MouseButtons button)
        {
            return (buttonsDown.Contains(button) && !buttonsDownLast.Contains(button));
        }
        public static bool MouseRelease(MouseButtons button)
        {
            return (!buttonsDown.Contains(button) && buttonsDownLast.Contains(button));
        }
        public static bool MouseDown(MouseButtons button)
        {
            return (buttonsDown.Contains(button));
        }
    }
}
