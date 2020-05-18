﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace forgotten.Desktop
{
    public class SpacePortPane : Pane
    {
        private LayeredAsset spacePort;

        public SpacePortPane()
        {
            var label = new TextAsset("Space Port");
            addChild("spacePortLabel", label);

            spacePort = new LayeredAsset("autogenerated/space_port");
            spacePort.Position = new Vector2(200, 100);
            spacePort.setOutlinedPart("cinema", "Space Tavern");
            addChild("spacePortLayered", spacePort);
        }

        public override void Draw(ForgottenGame game, Vector2 targetSize)
        {
            drawColoredRect(game,
                            Vector2.Zero,
                            targetSize,
                            Color.CornflowerBlue);
        }

        public override void Update(Vector2 targetSize, GameTime gameTime)
        {

            //throw new NotImplementedException();
        }
    }
}
