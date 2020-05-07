using System;
using Microsoft.Xna.Framework;

namespace forgotten.Desktop
{
    public class GalaxyPane : Pane
    {
        public GalaxyPane()
        {
            RectAsset testBox = new RectAsset();
            testBox.
            addChild("testBox", testBox);
        }

        public override void Update(Vector2 targetSize, GameTime gameTime)
        {
            //throw new NotImplementedException();
        }
    }
}
