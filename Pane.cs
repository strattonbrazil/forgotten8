using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace forgotten.Desktop
{
    public abstract class Pane : Asset
    {
        override public void Draw(Vector2 targetSize)
        {
            foreach (KeyValuePair<String,Asset> kvp in children)
            {
                Asset asset = kvp.Value;
                asset.DrawTree(targetSize);
            }
        }
    }
}
