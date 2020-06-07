using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace forgotten.Desktop
{
    public class ButtonAsset : Asset
    {
        public Vector2 Size; 
        //private String text;
        private bool isHovered;
        private Func<bool> clickCallback;
        private Func<bool, bool> hoverCallback;
        private int padding = 10;
        private TextAsset textAsset;

        public ButtonAsset(String text, Func<bool> onClick, Func<bool, bool> onHover)
        {
            this.clickCallback = onClick;
            this.hoverCallback = onHover;

            textAsset = AddChild("text", new TextAsset(text));
            textAsset.Position = new Vector2(padding, padding);

            UpdateSize();
        }

        public override void Draw(Vector2 targetSize)
        {
            var cornerPos = AbsolutePosition();

            var fillColor = Color.LightBlue;
            if (isHovered)
                fillColor = Color.Blue;

            DrawColoredRect(cornerPos, Size, fillColor);
        }

        public override void Update(Vector2 targetSize, GameTime gameTime)
        {
            MouseState ms = Mouse.GetState();
            MouseTracker().Update(ms);

            UpdateSize();

            var pos = AbsolutePosition();
            Rectangle buttonRect = new Rectangle((int)pos.X, (int)pos.Y, (int)Size.X, (int)Size.Y);
            bool wasHovered = isHovered;
            isHovered = buttonRect.Contains(ms.X, ms.Y);
            if (isHovered)
                hoverCallback(true);
            else if (wasHovered)
                hoverCallback(false);

            if (isHovered && MouseTracker().WasPressed())
            {
                clickCallback();
            }
        }

        private void UpdateSize()
        {
            Size = textAsset.Size + new Vector2(padding * 2, padding * 2);
        }
    }
}
