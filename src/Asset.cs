using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace forgotten.Desktop
{
    public abstract class Asset
    {
        protected List<KeyValuePair<String, Asset>> children = new List<KeyValuePair<string, Asset>>();

        public Asset Parent;

        public Vector2 Position { get; set; } = new Vector2();
        private Texture2D dummyTexture;
        private SpriteFont _normalFont; 

        public Vector2 AbsolutePosition()
        {
            var pos = Position;
            var parent = this.Parent;
            while (parent != null)
            {
                pos += parent.Position;
                parent = parent.Parent;
            }
            return pos;
        }

        public Asset AddChild(String name, Asset asset)
        {
            asset.Parent = this;
            children.Add(new KeyValuePair<string, Asset>(name, asset));
            return asset;
        }

        public abstract void Update(Vector2 targetSize, GameTime gameTime);

        public void UpdateTree(Vector2 targetSize, GameTime gameTime)
        {
            Update(targetSize, gameTime);
            foreach (KeyValuePair<String,Asset> kvp in children)
            {
                kvp.Value.UpdateTree(targetSize, gameTime);
            }
        }

        public abstract void Draw(Vector2 targetSize);

        public void DrawTree(Vector2 targetSize)
        {
            Draw(targetSize);
            foreach (KeyValuePair<String,Asset> kvp in children) {
                kvp.Value.DrawTree(targetSize);
            }
        }

        protected ForgottenGame Game()
        {
            return ForgottenGame.GlobalRef;
        }

        protected SpriteBatch GameSpriteBatch()
        {
            return Game().spriteBatch;
        }

        protected SpriteFont NormalFont()
        {
            if (_normalFont == null)
                _normalFont = Game().Content.Load<SpriteFont>("Galaxy_normal");
            return _normalFont;
        }

        protected void DrawColoredRect(Vector2 pos, Vector2 size, Color color)
        {
            if (dummyTexture == null)
            {
                CreateDummyTexture();
            }

            GameSpriteBatch().Draw(dummyTexture,
                                   pos,
                                   null, // source rect
                                   color,
                                   0,
                                   Vector2.Zero,
                                   size,
                                   SpriteEffects.None,
                                   0);

        }

        protected void DrawColoredOutline(Vector2 pos, Vector2 size, Color color, int border)
        {
            // left side
            DrawColoredRect(pos, new Vector2(border, size.Y), color);

            // bottom side
            DrawColoredRect(pos + new Vector2(border, size.Y - border), new Vector2(size.X - border*2, border), color);

            // right side
            DrawColoredRect(pos + new Vector2(size.X - border, 0), new Vector2(border, size.Y), color);

            // top side
            DrawColoredRect(pos + new Vector2(border, 0), new Vector2(size.X - border*2, border), color);
        }

        private void CreateDummyTexture()
        {
            dummyTexture = new Texture2D(Game().GraphicsDevice, 1, 1);
            Color[] dummyData = new Color[] { Color.White };
            dummyTexture.SetData(dummyData);
        }
    }
}
