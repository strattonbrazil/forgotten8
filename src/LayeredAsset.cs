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
        private SpriteBatch spriteBatch;
        private String contentDir;
        private List<Tuple<string,Texture2D>> partTextures = new List<Tuple<string, Texture2D>>();
        private Texture2D baseTexture;
        private int numFrames;
        private int currFrame;
        private float animDelay;
        private float lastElapsed; // time passed since last frame update

        public LayeredAsset(String contentDir)
        {
            this.contentDir = contentDir;
        }

        public override void Draw(ForgottenGame game, Vector2 targetSize)
        {
            if (spriteBatch == null)
            {
                spriteBatch = game.CreateSpriteBatch();
                //Load directory info, abort if none
                string baseDir = Directory.GetCurrentDirectory() + "/" + game.Content.RootDirectory + "/" + contentDir;
                //string baseDir = game.Content.RootDirectory + "\\" + contentDir;
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
                        partTextures.Add(new Tuple<string, Texture2D>(key, texture));
                    Texture2D tex = game.Content.Load<Texture2D>(baseDir + "/" + key);
                }
            }

            Vector2 origin = AbsolutePosition();

            spriteBatch.Begin();
            spriteBatch.Draw(baseTexture,
                         origin,
                         null, // source rect
                         Color.White,
                         0,
                         Vector2.Zero,
                         Vector2.One,
                         SpriteEffects.None,
                         0);

            foreach (Tuple<string,Texture2D> part in partTextures)
            {
                string partName = part.Item1;
                Match m = Regex.Match(partName, @"_([0-9]+)_([0-9]+)$");
                int dstX = Int32.Parse(m.Groups[1].ToString());
                int dstY = Int32.Parse(m.Groups[2].ToString());

                Texture2D partTexture = part.Item2;
                int partSubheight = partTexture.Height / numFrames;
                int subY = (partSubheight * currFrame) % partTexture.Height;
                Rectangle srcRect = new Rectangle(0, subY, partTexture.Width, partSubheight);
                spriteBatch.Draw(partTexture,
                                 origin + new Vector2(dstX, dstY),
                                 srcRect, // source rect
                                 Color.White,
                                 0,
                                 Vector2.Zero,
                                 Vector2.One,
                                 SpriteEffects.None,
                                 0);
            }

            spriteBatch.End();
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
    }
}
