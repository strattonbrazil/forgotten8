using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace forgotten.Desktop
{
    public class System
    {
        public Vector2 Position { get; set; }
        public String Name { get; set; }

        public static List<System> systems = new List<System>();

        public System(Vector2 position)
        {
            this.Position = position;
        }

    }
}
