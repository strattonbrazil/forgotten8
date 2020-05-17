using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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

        private Dictionary<string, string> outlinedParts = new Dictionary<string, string>();

        MouseTracker mouseTracker = new MouseTracker();

        public LayeredAsset(String contentDir)
        {
            this.contentDir = contentDir;
        }

        public void setOutlinedPart(string partName, string visibleName)
        {
            outlinedParts.Add(partName, visibleName);
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
                        int subwidth = texture.Width;
                        int subheight = texture.Height / numFrames;
                        LayeredPartInfo partInfo = new LayeredPartInfo(key, texture, subwidth, subheight);

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

                int subY = (partInfo.subheight * currFrame) % partTexture.Height;
                Rectangle srcRect = new Rectangle(0, subY, partInfo.subwidth, partInfo.subheight);
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

                if (outlinedParts.ContainsKey(partName))
                {
                    string partLabel = outlinedParts[partName];
                    int border = 4;
                    if (partInfo.mouseHovering)
                    {
                        drawColoredOutline(game, partPos, new Vector2(partInfo.subwidth, partInfo.subheight), new Color(Color.White, 20), border);

                    }
                    else
                    {
                        drawColoredOutline(game, partPos, new Vector2(partInfo.subwidth, partInfo.subheight), new Color(Color.Black, 20), border);
                    }
                    spriteBatch.DrawString(normalFont(game), partLabel, partPos - new Vector2(0, normalFont(game).LineSpacing), Color.White);
                }
            }
        }

        public override void Update(Vector2 targetSize, GameTime gameTime)
        {
            MouseState ms = Mouse.GetState();
            mouseTracker.Update(ms);

            Vector2 origin = AbsolutePosition();
            foreach (Tuple<string, LayeredPartInfo> part in partTextures)
            {
                LayeredPartInfo partInfo = part.Item2;
                Rectangle screenRect = new Rectangle((int)(origin.X + partInfo.dstX), (int)(origin.Y + partInfo.dstY), partInfo.subwidth, partInfo.subheight);
                partInfo.mouseHovering = screenRect.Contains(ms.Position);
            }

            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            lastElapsed += elapsed;
            while (lastElapsed > animDelay)
            {
                lastElapsed -= animDelay;
                currFrame += 1;
            }
        }

        class LayeredPartInfo
        {
            public readonly string name;
            public readonly int dstX;
            public readonly int dstY;
            public readonly int subwidth;
            public readonly int subheight;
            public readonly Texture2D texture;
            public bool mouseHovering = false;

            public LayeredPartInfo(string partKey, Texture2D texture, int subwidth, int subheight)
            {
                Match m = Regex.Match(partKey, @"^part_([a-zA-Z0-9_]+)_([0-9]+)_([0-9]+)$");
                name = m.Groups[1].ToString();
                dstX = Int32.Parse(m.Groups[2].ToString());
                dstY = Int32.Parse(m.Groups[3].ToString());
                this.texture = texture;
                this.subwidth = subwidth;
                this.subheight = subheight;
            }
        }
    }
}
