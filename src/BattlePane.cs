using System;
using Microsoft.Xna.Framework;

namespace forgotten.Desktop
{
    public class BattlePane : Pane
    {
        public BattlePane()
        {
        }

        public override void Draw(Vector2 targetSize)
        {
            DrawColoredRect(new Vector2(0, 0),
                            targetSize,
                            Color.DarkRed);
        }

        public override void Update(Vector2 targetSize, GameTime gameTime)
        {
        }
    }
}
