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
        private List<Tuple<string,LayeredPartInfo>> partTextures = new List<Tuple<string,LayeredPartInfo>>();
        private Texture2D baseTexture;
        private int numFrames;
        private int currFrame;
        private float animDelay;
        private float lastElapsed; // time passed since last frame update
        public Vector2 Size = new Vector2(0,0); 

        private Dictionary<string, string> outlinedParts = new Dictionary<string, string>();

        public LayeredAsset(String contentDir)
        {
            string baseDir = Directory.GetCurrentDirectory() + "/" + Game().Content.RootDirectory + "/" + contentDir;
            DirectoryInfo dir = new DirectoryInfo(baseDir);
            if (!dir.Exists)
                throw new DirectoryNotFoundException();

            FileInfo[] files = dir.GetFiles("*.*");
            // make sure "base_" is first since it has metadata
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].Name.StartsWith("base_"))
                {
                    FileInfo tmp = files[0];
                    files[0] = files[i];
                    files[i] = tmp;
                }
            }

            foreach (FileInfo file in files)
            {
                string key = Path.GetFileNameWithoutExtension(file.Name);
                Texture2D texture = Game().Content.Load<Texture2D>(baseDir + "/" + key);
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
                Texture2D tex = Game().Content.Load<Texture2D>(baseDir + "/" + key);
            }
        }

        public void SetOutlinedPart(string partName, string visibleName)
        {
            outlinedParts.Add(partName, visibleName);
        }

        public bool IsPartHovered(string partName)
        {
            foreach (Tuple<string, LayeredPartInfo> part in partTextures)
            {
                if (partName == part.Item1)
                {
                    LayeredPartInfo partInfo = part.Item2;
                    return partInfo.MouseHovering;
                }
            }
            Console.WriteLine("unknown part name: " + partName);
            return false;
        }

        public override void Draw(Vector2 targetSize)
        {
            Vector2 origin = AbsolutePosition();

            GameSpriteBatch().Draw(baseTexture,
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
                GameSpriteBatch().Draw(partTexture,
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
                    if (partInfo.MouseHovering)
                    {
                        DrawColoredOutline(partPos, new Vector2(partInfo.subwidth, partInfo.subheight), new Color(Color.White, 20), border);

                    }
                    else
                    {
                        DrawColoredOutline(partPos, new Vector2(partInfo.subwidth, partInfo.subheight), new Color(Color.Black, 20), border);
                    }
                    GameSpriteBatch().DrawString(NormalFont(), partLabel, partPos - new Vector2(0, NormalFont().LineSpacing), Color.White);
                }
            }
        }

        public override void Update(Vector2 targetSize, GameTime gameTime)
        {
            MouseState ms = Mouse.GetState();
            mouseTracker().Update(ms);

            Vector2 origin = AbsolutePosition();
            foreach (Tuple<string, LayeredPartInfo> part in partTextures)
            {
                LayeredPartInfo partInfo = part.Item2;
                Rectangle screenRect = new Rectangle((int)(origin.X + partInfo.dstX), (int)(origin.Y + partInfo.dstY), partInfo.subwidth, partInfo.subheight);
                partInfo.MouseHovering = screenRect.Contains(ms.Position);
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
            public bool MouseHovering = false;

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
