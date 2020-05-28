using System;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace forgotten.Desktop
{
    public class AnimatedAsset : Asset
    {
        private Texture2D[] textures;
        private int numFrames;
        private int currFrame;
        private float animDelay;
        private float lastElapsed; // time passed since last frame update

        public int Height()
        {
            return textures[0].Height;
        }

        public int Width()
        {
            return textures[0].Width;
        }

        public AnimatedAsset(String contentDir)
        {
            string baseDir = Directory.GetCurrentDirectory() + "/" + Game().Content.RootDirectory + "/" + contentDir;

            DirectoryInfo dir = new DirectoryInfo(baseDir);
            if (!dir.Exists)
                throw new DirectoryNotFoundException();

            FileInfo[] files = dir.GetFiles("*.*");
            textures = new Texture2D[files.Length];
            numFrames = files.Length;
            for (int i = 0; i < files.Length; i++)
            {
                FileInfo file = files[i];
                int fileIndex = 0;
                string key = Path.GetFileNameWithoutExtension(file.Name);
                Match m = Regex.Match(key, @"_([0-9]+)_delay([0-9]+)");
                if (m.Success)
                {
                    fileIndex = Int32.Parse(m.Groups[1].ToString());
                    animDelay = Int32.Parse(m.Groups[2].ToString()) / 1000.0f;
                }
                else
                {
                    Console.WriteLine("unable to find count and delay: " + key);
                }

                textures[fileIndex-1] = Game().Content.Load<Texture2D>(baseDir + "/" + key);
            }
        }

        public override void Draw(Vector2 targetSize)
        {
            GameSpriteBatch().Draw(textures[currFrame],
                                   AbsolutePosition(),
                                   null, // source rect
                                   Color.White,
                                   0,
                                   Vector2.Zero,
                                   Vector2.One,
                                   SpriteEffects.None,
                                   0);
        }

        public override void Update(Vector2 targetSize, GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            lastElapsed += elapsed;
            while (lastElapsed > animDelay)
            {
                lastElapsed -= animDelay;
                currFrame = (currFrame + 1) % numFrames;
            }
        }
    }
}
