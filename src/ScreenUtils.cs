using System;
using Microsoft.Xna.Framework;

namespace forgotten.Desktop
{
    public class ScreenUtils
    {
        private readonly float screenW;
        private readonly float screenH;
        private readonly float worldW;
        private readonly float worldH;
        private readonly float aspectRatio;
        private readonly float invAspectRatio;
        private readonly float padding;

        public ScreenUtils(Vector2 targetSize, Vector2 worldSize, float padding)
        {
            screenW = targetSize.X;
            screenH = targetSize.Y;
            worldW = worldSize.X;
            worldH = worldSize.Y;
            aspectRatio = screenW / (float)screenH;
            invAspectRatio = 1 / aspectRatio;
            this.padding = padding;
        }

        public Vector2 WorldToScreen(Vector2 w)
        {
            return new Vector2((screenW - padding * 2) * ((invAspectRatio * w.X + (worldH * 0.5f)) / worldH) + padding,
                               (screenH - padding * 2) * ((w.Y + (worldH * 0.5f)) / worldH) + padding);
        }
    }
}
