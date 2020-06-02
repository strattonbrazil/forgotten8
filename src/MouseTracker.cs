using System;
using Microsoft.Xna.Framework.Input;

namespace forgotten.Desktop
{
    public class MouseTracker
    {
        public MouseState? prevMs = null;
        public MouseState? prevPrevMs = null;
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
                return prevPrevMs.Value.LeftButton == ButtonState.Released && prevMs.Value.LeftButton == ButtonState.Pressed;
            }
            return false;
        }
    }
}
