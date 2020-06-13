using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace forgotten.Desktop
{
    public class TextureAnimationAsset : Asset
    {
        Texture2D texture;
        int rectWidth;
        int rectHeight;
        int rows;
        int columns;
        Vector2 cellSize;
        Vector2 halfSpriteSize;
        int spriteIndex = 0;
        float animStart = -1;
        public Vector2 Acceleration;
        public Vector2 Velocity;

        public TextureAnimationAsset(Texture2D texture, int rows, int columns)
        {
            this.texture = texture;
            this.rows = rows;
            this.columns = columns;
            this.rectWidth = texture.Width / columns;
            this.rectHeight = texture.Height / rows;
            this.cellSize = new Vector2(this.rectWidth, this.rectHeight);
            this.halfSpriteSize = 0.5f * this.cellSize; 
        }

        public override void Draw(Vector2 targetSize)
        {
            int spriteRow = spriteIndex / columns;
            int spriteColumn = spriteIndex % columns;
            Rectangle srcRect = new Rectangle((int)(cellSize.X * spriteColumn), 
                                              (int)(cellSize.Y * spriteRow),
                                              rectWidth, rectHeight);

            GameSpriteBatch().Draw(texture,
                                   AbsolutePosition() - halfSpriteSize,
                                   srcRect, // source rect
                                   Color.White,
                                   0,
                                   Vector2.Zero,
                                   Vector2.One,
                                   SpriteEffects.None,
                                   0);
        }

        public override void Update(Vector2 targetSize, GameTime gameTime)
        {
            float totalSeconds = (float)gameTime.TotalGameTime.TotalSeconds;
            if (animStart < 0)
                animStart = totalSeconds;

            spriteIndex = (int)(20 * (totalSeconds - animStart));
        }

        public bool IsFinished()
        {
            return spriteIndex >= rows * columns;
        }
    }
}
