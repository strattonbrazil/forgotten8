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
            return new Vector2((screenW - padding * 2) * ((invAspectRatio * w.X + worldH * 0.5f) / worldH) + padding,
                               (screenH - padding * 2) * ((w.Y + worldH * 0.5f) / worldH) + padding);
        }

        public Vector2 ScreenToWorld(Point screenP)
        {
            // TODO: probably can refactor this to be cleaner, but seems to work
            float worldX = ((((screenP.X - padding) * worldH) / (screenW - padding * 2)) - worldH * 0.5f) * aspectRatio;
            float worldY = (screenP.Y / (screenH - padding * 2) - padding) * worldH - worldH * 0.5f;
            return new Vector2(worldX, worldY);
        }

        public float ScaleToScreen(float w)
        {
            return (screenH - 2 * padding) * (w / worldH);
        }
    }
}
