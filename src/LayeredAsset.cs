using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace forgotten.Desktop
{
    public class LayeredAsset : Asset
    {
        private String contentDir;
        private List<Tuple<string,LayeredPartInfo>> partTextures = new List<Tuple<string,LayeredPartInfo>>();
        private Texture2D baseTexture;
        private int numFrames;
        private int currFrame;
        private float animDelay;
        private float lastElapsed; // time passed since last frame update
        private bool initialized = false;

        private Dictionary<string, bool> outlinedParts = new Dictionary<string, bool>();

        public LayeredAsset(String contentDir)
        {
            this.contentDir = contentDir;
        }

        public void setOutlinedPart(string partName, bool outlined = true)
        {
            outlinedParts.Add(partName, outlined);
        }

        public override void Draw(ForgottenGame game, Vector2 targetSize)
        {
            var spriteBatch = game.spriteBatch;
            if (!initialized)
            {
                string baseDir = Directory.GetCurrentDirectory() + "/" + game.Content.RootDirectory + "/" + contentDir;
                DirectoryInfo dir = new DirectoryInfo(baseDir);
                if (!dir.Exists)
                    throw new DirectoryNotFoundException();

                FileInfo[] files = dir.GetFiles("*.*");
                foreach (FileInfo file in files)
                {
                    string key = Path.GetFileNameWithoutExtension(file.Name);
                    Texture2D texture = game.Content.Load<Texture2D>(baseDir + "/" + key);
                    if (key.StartsWith("base_"))
                    {
                        baseTexture = texture;
                        Match m = Regex.Match(key, @"_count([0-9]+)_delay([0-9]+)");
                        if (m.Success)
                        {
                            numFrames = Int32.Parse(m.Groups[1].ToString());
                            animDelay = Int32.Parse(m.Groups[2].ToString()) / 1000.0f;
                        }
                        else
                        {
                            Console.WriteLine("unable to find count and delay: " + key);
                        }
                    }
                    else
                    {
                        LayeredPartInfo partInfo = new LayeredPartInfo(key, texture);
                        partTextures.Add(new Tuple<string, LayeredPartInfo>(partInfo.name, partInfo));
                    }
                    Texture2D tex = game.Content.Load<Texture2D>(baseDir + "/" + key);
                }

                initialized = true;
            }

            Vector2 origin = AbsolutePosition();

            spriteBatch.Draw(baseTexture,
                         origin,
                         null, // source rect
                         Color.White,
                         0,
                         Vector2.Zero,
                         Vector2.One,
                         SpriteEffects.None,
                         0);

            foreach (Tuple<string, LayeredPartInfo> part in partTextures)
            {
                string partName = part.Item1;
                LayeredPartInfo partInfo = part.Item2;
                Texture2D partTexture = partInfo.texture;

                int partSubheight = partTexture.Height / numFrames;
                int subY = (partSubheight * currFrame) % partTexture.Height;
                Rectangle srcRect = new Rectangle(0, subY, partTexture.Width, partSubheight);
                Vector2 partPos = origin + new Vector2(partInfo.dstX, partInfo.dstY);
                spriteBatch.Draw(partTexture,
                                 partPos,
                                 srcRect, // source rect
                                 Color.White,
                                 0,
                                 Vector2.Zero,
                                 Vector2.One,
                                 SpriteEffects.None,
                                 0);
            }

            foreach (Tuple<string, LayeredPartInfo> part in partTextures)
            {
                string partName = part.Item1;
                LayeredPartInfo partInfo = part.Item2;
                Texture2D partTexture = partInfo.texture;
                Vector2 partPos = origin + new Vector2(partInfo.dstX, partInfo.dstY);
                int partSubheight = partTexture.Height / numFrames;

                if (outlinedParts.ContainsKey(partName) && outlinedParts[partName])
                {
                    drawColoredOutline(game, partPos, new Vector2(partTexture.Width, partSubheight), new Color(Color.Black, 20), 32);
                }
            }
        }

        public override void Update(Vector2 targetSize, GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            lastElapsed += elapsed;
            while (lastElapsed > animDelay)
            {
                lastElapsed -= animDelay;
                currFrame += 1;
            }

            //throw new NotImplementedException();
        }

        class LayeredPartInfo
        {
            public readonly string name;
            public readonly int dstX;
            public readonly int dstY;
            public readonly Texture2D texture;

            public LayeredPartInfo(string partKey, Texture2D texture)
            {
                Match m = Regex.Match(partKey, @"^part_([a-zA-Z0-9_]+)_([0-9]+)_([0-9]+)$");
                name = m.Groups[1].ToString();
                dstX = Int32.Parse(m.Groups[2].ToString());
                dstY = Int32.Parse(m.Groups[3].ToString());
                this.texture = texture;
            }
        }
    }
}
