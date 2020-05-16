﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace forgotten.Desktop
{
    public class SpacePortPane : Pane
    {
        private SpriteBatch spriteBatch;
        private Texture2D dummyTexture;
        //private Texture2D spacePortTexture;

        public SpacePortPane()
        {
            var label = new TextAsset("Space Port");
            addChild("spacePortLabel", label);
        }

        public override void Draw(ForgottenGame game, Vector2 targetSize)
        {
            if (spriteBatch == null)
            {
                spriteBatch = game.CreateSpriteBatch();
                dummyTexture = game.CreateDummyTexture();
                //spacePortTexture = game.Content.Load<Texture2D>("space_station");
            }

            spriteBatch.Begin();
            spriteBatch.Draw(dummyTexture,
                             Vector2.Zero,
                             null, // source rect
                             Color.CornflowerBlue,
                             0,
                             Vector2.Zero,
                             targetSize,
                             SpriteEffects.None,
                             0);

            /*

            Vector2 position = 0.5f * (targetSize - new Vector2(spacePortTexture.Width, spacePortTexture.Height));

            spriteBatch.Draw(spacePortTexture,
                             position,
                             null,
                             Color.White,
                             0,
                             Vector2.Zero,
                             Vector2.One,
                             SpriteEffects.None,
                             0);
            */

            spriteBatch.End();
        }

        public override void Update(Vector2 targetSize, GameTime gameTime)
        {
            //throw new NotImplementedException();
        }
    }
}