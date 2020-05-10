using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace forgotten.Desktop
{
    public abstract class Asset
    {
        protected List<KeyValuePair<String, Asset>> children = new List<KeyValuePair<string, Asset>>();

        public Asset Parent;

        public Vector2 Position { get; set; } = new Vector2();

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
    }
}
