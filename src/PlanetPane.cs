using System;
using Microsoft.Xna.Framework;

namespace forgotten.Desktop
{
    public class PlanetPane : Pane
    {
        public PlanetPane(Planet planet)
        {
            Console.WriteLine(planet.Name);
        }

        public override void Draw(Vector2 targetSize)
        {
            //throw new NotImplementedException();
        }

        public override void Update(Vector2 targetSize, GameTime gameTime)
        {
            //throw new NotImplementedException();
        }
    }
}
