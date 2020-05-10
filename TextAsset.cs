using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace forgotten.Desktop
{
    public class TextAsset : Asset
    {
        public String Text;
        private SpriteFont normalFont;
        private SpriteBatch spriteBatch;

        public TextAsset(String text = "")
        {
            this.Text = text;
        }

        public override void Draw(ForgottenGame game, Vector2 targetSize)
        {
            if (spriteBatch == null)
            {
                spriteBatch = game.CreateSpriteBatch();
                normalFont = game.Content.Load<SpriteFont>("Galaxy_normal");
            }

            spriteBatch.Begin();
            spriteBatch.DrawString(normalFont, Text, AbsolutePosition(), Color.White);
            spriteBatch.End();
        }

        public override void Update(Vector2 targetSize, GameTime gameTime)
        {
        }

        private Vector2 AbsoluteFlooredPosition()
        {
            Vector2 abs = AbsolutePosition();
            return new Vector2((int)abs.X, (int)abs.Y);
        }
    }
}
