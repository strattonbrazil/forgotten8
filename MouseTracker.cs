using System;
using Microsoft.Xna.Framework.Input;

namespace forgotten.Desktop
{
    public class MouseTracker
    {
        private MouseState prevMs;
        private MouseState prevPrevMs;

        public MouseTracker()
        {
        }

        public void Update(MouseState ms)
        {
            prevPrevMs = prevMs;
            prevMs = ms;
        }

        public bool WasPressed()
        {
            if (prevPrevMs != null && prevMs != null)
            {
                return prevPrevMs.LeftButton == ButtonState.Released && prevMs.LeftButton == ButtonState.Pressed;
            }
            return false;
        }
    }
}
