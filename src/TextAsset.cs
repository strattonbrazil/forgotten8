using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace forgotten.Desktop
{
    public class TextAsset : Asset
    {
        public String Text;
        private SpriteFont normalFont;

        public TextAsset(String text = "")
        {
            this.Text = text;
        }

        public override void Draw(ForgottenGame game, Vector2 targetSize)
        {
            var spriteBatch = game.spriteBatch;
            if (normalFont == null)
            {
                normalFont = game.Content.Load<SpriteFont>("Galaxy_normal");
            }

            spriteBatch.DrawString(normalFont, Text, AbsolutePosition(), Color.White);
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
