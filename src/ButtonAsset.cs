using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace forgotten.Desktop
{
    public class ButtonAsset : Asset
    {
        private String text;
        private Vector2 size;
        private bool isHovered;
        private Func<bool> clickCallback;
        private Func<bool, bool> hoverCallback;
        private int padding = 10;

        public ButtonAsset(String text, Func<bool> onClick, Func<bool, bool> onHover)
        {
            this.text = text;
            this.clickCallback = onClick;
            this.hoverCallback = onHover;
        }

        public override void Draw(Vector2 targetSize)
        {
            var cornerPos = AbsolutePosition();

            var fillColor = Color.LightBlue;
            if (isHovered)
                fillColor = Color.Blue;

            DrawColoredRect(cornerPos, size, fillColor);

            GameSpriteBatch().DrawString(NormalFont(), text, cornerPos + new Vector2(padding, 0), Color.White);
        }

        public override void Update(Vector2 targetSize, GameTime gameTime)
        {
            MouseState ms = Mouse.GetState();
            mouseTracker().Update(ms);

            size = new Vector2(NormalFont().MeasureString(text).X + padding*2, 30);

            var pos = AbsolutePosition();
            Rectangle buttonRect = new Rectangle((int)pos.X, (int)pos.Y, (int)size.X, (int)size.Y);
            bool wasHovered = isHovered;
            isHovered = buttonRect.Contains(ms.X, ms.Y);
            if (isHovered)
                hoverCallback(true);
            else if (wasHovered)
                hoverCallback(false);

            if (isHovered && mouseTracker().WasPressed())
            {
                clickCallback();
            }
        }

    }
}
