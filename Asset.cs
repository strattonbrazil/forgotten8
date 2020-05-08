using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace forgotten.Desktop
{
    public abstract class Asset
    {
        protected List<KeyValuePair<String, Asset>> children = new List<KeyValuePair<string, Asset>>();

        public Vector2 Position { get; set; } = new Vector2();

        public void addChild(String name, Asset asset)
        {
            children.Add(new KeyValuePair<string, Asset>(name, asset));
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
