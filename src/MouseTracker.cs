using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace forgotten.Desktop
{
    public class MouseTracker
    {
        private MouseState? prevMs = null;
        private MouseState? prevPrevMs = null;
        public Point Position;
        private Pane pane;

        public MouseTracker(Pane pane)
        {
            this.pane = pane;
        }

        public void Update()
        {
            MouseState ms = Mouse.GetState();
            Point unscaled = ms.Position;

            Vector2 mouseP = new Vector2(ms.Position.X, ms.Position.Y) * ForgottenGame.GlobalRef.MouseScale();

            Position = new Point((int)mouseP.X, (int)mouseP.Y);

            prevPrevMs = prevMs;
            prevMs = ms;
        }

        public bool WasPressed()
        {
            if (prevPrevMs != null && prevMs != null && pane.IsTopPane())
            {
                return prevPrevMs.Value.LeftButton == ButtonState.Released && prevMs.Value.LeftButton == ButtonState.Pressed;
            }
            return false;
        }
    }
}
