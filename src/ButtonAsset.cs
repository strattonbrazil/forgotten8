using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace forgotten.Desktop
{
    public class ButtonAsset : Asset
    {
        public Vector2 Size; 
        private String text;
        private bool isHovered;
        private Func<bool> clickCallback;
        private Func<bool, bool> hoverCallback;
        private int padding = 10;

        public ButtonAsset(String text, Func<bool> onClick, Func<bool, bool> onHover)
        {
            this.text = text;
            this.clickCallback = onClick;
            this.hoverCallback = onHover;

            UpdateSize();
        }

        public override void Draw(Vector2 targetSize)
        {
            var cornerPos = AbsolutePosition();

            var fillColor = Color.LightBlue;
            if (isHovered)
                fillColor = Color.Blue;

            DrawColoredRect(cornerPos, Size, fillColor);

            Console.WriteLine("button text: " + text);
            GameSpriteBatch().DrawString(NormalFont(), text, cornerPos + new Vector2(padding, padding), Color.White);
        }

        public override void Update(Vector2 targetSize, GameTime gameTime)
        {
            MouseState ms = Mouse.GetState();
            mouseTracker().Update(ms);

            UpdateSize();

            var pos = AbsolutePosition();
            Rectangle buttonRect = new Rectangle((int)pos.X, (int)pos.Y, (int)Size.X, (int)Size.Y);
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

        private void UpdateSize()
        {
            var fm = NormalFont().MeasureString(text);
            Size = new Vector2(fm.X + padding * 2, fm.Y + padding * 2);
        }
    }
}
