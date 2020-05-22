using System;
using Microsoft.Xna.Framework.Input;

namespace forgotten.Desktop
{
    public class MouseTracker
    {
        private MouseState prevMs;
        private MouseState prevPrevMs;
        private Pane pane;

        public MouseTracker(Pane pane)
        {
            this.pane = pane;
        }

        public void Update(MouseState ms)
        {
            prevPrevMs = prevMs;
            prevMs = ms;
        }

        public bool WasPressed()
        {
            if (prevPrevMs != null && prevMs != null && pane.IsTopPane())
            {
                return prevPrevMs.LeftButton == ButtonState.Released && prevMs.LeftButton == ButtonState.Pressed;
            }
            return false;
        }
    }
}
