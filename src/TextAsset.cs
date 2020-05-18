using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace forgotten.Desktop
{
    public class TextAsset : Asset
    {
        public String Text;

        public TextAsset(String text = "")
        {
            this.Text = text;
        }

        public override void Draw(Vector2 targetSize)
        {
            GameSpriteBatch().DrawString(NormalFont(), Text, AbsolutePosition(), Color.White);
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
