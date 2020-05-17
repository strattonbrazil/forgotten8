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

        public Asset addChild(String name, Asset asset)
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

        public abstract void Draw(ForgottenGame game, Vector2 targetSize);

        public void DrawTree(ForgottenGame game, Vector2 targetSize)
        {
            Draw(game, targetSize);
            foreach (KeyValuePair<String,Asset> kvp in children) {
                kvp.Value.DrawTree(game, targetSize);
            }
        }

        protected SpriteFont normalFont(ForgottenGame game)
        {
            if (_normalFont == null)
                _normalFont = game.Content.Load<SpriteFont>("Galaxy_normal");
            return _normalFont;
        }

        protected void drawColoredRect(ForgottenGame game, Vector2 pos, Vector2 size, Color color)
        {
            if (dummyTexture == null)
            {
                createDummyTexture(game);
            }

            game.spriteBatch.Draw(dummyTexture,
                                  pos,
                                  null, // source rect
                                  color,
                                  0,
                                  Vector2.Zero,
                                  size,
                                  SpriteEffects.None,
                                  0);

        }

        protected void drawColoredOutline(ForgottenGame game, Vector2 pos, Vector2 size, Color color, int border)
        {
            // left side
            drawColoredRect(game, pos, new Vector2(border, size.Y), color);

            // bottom side
            drawColoredRect(game, pos + new Vector2(border, size.Y - border), new Vector2(size.X - border*2, border), color);

            // right side
            drawColoredRect(game, pos + new Vector2(size.X - border, 0), new Vector2(border, size.Y), color);

            // top side
            drawColoredRect(game, pos + new Vector2(border, 0), new Vector2(size.X - border*2, border), color);
        }

        private void createDummyTexture(Game game)
        {
            dummyTexture = new Texture2D(game.GraphicsDevice, 1, 1);
            Color[] dummyData = new Color[] { Color.White };
            dummyTexture.SetData(dummyData);
        }
    }
}
