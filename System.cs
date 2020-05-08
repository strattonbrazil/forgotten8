using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace forgotten.Desktop
{
    public class System
    {
        public Vector2 Position { get; set; }
        public String Name { get; set; }
        public float Size { get; set; } // screen radius
        public Color Color { get; set; }

        public static List<System> systems = new List<System>();

        public System(Vector2 position)
        {
            this.Position = position;

            Random rnd = new Random();
            Size = (float)rnd.NextDouble() * 3 + 3;

            float colorSpace = (float)rnd.NextDouble();
            if (colorSpace < .3)
            {
                Color = new Color(150, 250, 250);
            }
            else if (colorSpace < .7)
            {
                Color = new Color(250, 150, 250);
            }
            else
            {
                Color = new Color(250, 250, 150);
            }
        }

    }
}
