using System;
using Microsoft.Xna.Framework;

namespace forgotten.Desktop
{
    public class RectAsset : Asset
    {
        public Color FillColor { get; set; } = new Color();
        public Vector2 Size { get; set; } = new Vector2();

        public override void Update(Vector2 targetSize, GameTime gameTime)
        {

        }

        public override void Draw(Vector2 targetSize)
        {

        }
    }
}
