using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace forgotten.Desktop
{
    public class BattlePane : Pane
    {
        AnimatedAsset alien;
        Texture2D battleship;
        Texture2D enemy;
        SampledRegion enemyRegion;
        Vector2 shipPosW;
        //Vector2 enemyPosW;
        const int NUM_STREAKS = 150;
        Streak[] streaks;

        List<Laser> friendlyLasers = new List<Laser>();

        Texture2D explosionTexture;

        public BattlePane()
        {
            battleship = Game().Content.Load<Texture2D>("spaceship_battle");
            enemy = Game().Content.Load<Texture2D>("enemyship_battle");
            streaks = new Streak[NUM_STREAKS];

            float enemyAspect = (float)enemy.Width / enemy.Height;
            enemyRegion = new SampledRegion(enemy, enemyAspect * 0.5f, 0.5f);

            explosionTexture = Game().Content.Load<Texture2D>("explosion_spritesheet");
        }

        public override void Draw(Vector2 targetSize)
        {
            float aspect = targetSize.X / (float)targetSize.Y;
            ScreenUtils su = new ScreenUtils(targetSize, new Vector2(aspect * 2, 2), 0);
            
            DrawColoredRect(new Vector2(0, 0),
                            targetSize,
                            Color.Black);

            foreach (Streak streak in streaks)
            {
                DrawColoredRect(su.WorldToScreen(streak.Position),
                                new Vector2(streak.Distance, streak.Thickness),
                                Color.White);
            }

            {
                float desiredHeight = su.ScaleToScreen(enemyRegion.Height);
                Vector2 textureSize = new Vector2(enemy.Width, enemy.Height);
                Vector2 invScale = new Vector2(su.ScaleToScreen(enemyRegion.Width), su.ScaleToScreen(enemyRegion.Height)) / textureSize;
                Vector2 shipScreenSize = invScale * textureSize;
                Vector2 shipPos = su.WorldToScreen(enemyRegion.Position);

                GameSpriteBatch().Draw(enemy,
                                       shipPos - 0.5f * shipScreenSize,
                                       null, // source rect
                                       Color.White,
                                       0,
                                       Vector2.Zero,
                                       invScale,
                                       SpriteEffects.None,
                                       0);
            }
            {
                float shipWorldSize = 0.5f;
                float desiredSize = su.ScaleToScreen(shipWorldSize);
                float invScale = desiredSize / (float)battleship.Height;

                Vector2 shipPos = su.WorldToScreen(shipPosW);
                Vector2 shipScreenSize = new Vector2(invScale * battleship.Width, invScale * battleship.Height);

                GameSpriteBatch().Draw(battleship,
                                       shipPos - 0.5f * shipScreenSize,
                                       null, // source rect
                                       Color.White,
                                       0,
                                       Vector2.Zero,
                                       invScale,
                                       SpriteEffects.None,
                                       0);
            }

            foreach (Laser laser in friendlyLasers)
            {
                Vector2 laserPos = su.WorldToScreen(laser.Pos);
                Vector2 laserTailPos = su.WorldToScreen(laser.Pos - laser.Dir * laser.Length);

                DrawColoredLine(laserPos, laserTailPos, Color.Red, 4);
            }
        }

        public override void Update(Vector2 targetSize, GameTime gameTime)
        {
            float aspect = targetSize.X / (float)targetSize.Y;
            ScreenUtils su = new ScreenUtils(targetSize, new Vector2(aspect * 2, 2), 0);

//            MouseState ms = Mouse.GetState();
//            Point p = ms.Position;
            MouseTracker().Update();
            if (MouseTracker().WasPressed())
            {
                Vector2 target = su.ScreenToWorld(MouseTracker().Position);
                Laser laser = new Laser();
                laser.Pos = new Vector2(0, 0);
                laser.Dir = Vector2.Normalize(target - laser.Pos);
                laser.Length = 0.1f;
                laser.Speed = 0.05f;
                friendlyLasers.Add(laser);
            }

            float elapsed = (float)gameTime.TotalGameTime.TotalSeconds;

            enemyRegion.Position = new Vector2(0.1f * (float)Math.Sin(elapsed * 1.5f), -0.6f + 0.02f * (float)Math.Cos(elapsed * 0.5f));
            //enemyPosW = new Vector2(0.1f * (float)Math.Sin(elapsed * 1.5f), -0.6f + 0.02f * (float)Math.Cos(elapsed * 0.5f));
            shipPosW = new Vector2(0.1f * (float)Math.Sin(elapsed), 0.6f + 0.02f * (float)Math.Cos(elapsed * 0.5f));

            for (int i = 0; i < NUM_STREAKS; i++)
            {
                float speed = 2 + (i % 4);
                float x = -aspect + (float)Math.Cos(i) + speed*elapsed; //20 + 50 * (float)Math.Cos(i);
                int screenWidths = (int)(x / (2 * aspect));
                x = x - screenWidths * (2 * aspect) - aspect;

                float y = 2 * ((i + 1.0f) / (NUM_STREAKS + 1)) - 1; //+ 30.0f * (float)Math.Sin(i);
                streaks[i].Position = new Vector2(x, y);
                streaks[i].Distance = targetSize.X * 0.002f * speed;
                streaks[i].Thickness = 2;
            }

            // remove distant lasers
            List<Laser> unimpactedLasers = new List<Laser>();
            foreach (Laser laser in friendlyLasers)
            {
                Vector2 prevPos = laser.Pos;
                Vector2 newPos = laser.Pos + laser.Dir * laser.Speed;
                Vector2? impactPos = enemyRegion.Trace(prevPos, newPos);

                if (impactPos != null)
                {
                    Console.WriteLine(impactPos);
                    var explosion = new TextureAnimationAsset(explosionTexture, 3, 5);
                    explosion.Position = su.WorldToScreen(impactPos.Value);
                    AddChild("explosion_effect", explosion);
                } else if (laser.Pos.Length() < 10)
                {
                    unimpactedLasers.Add(laser);
                }
                laser.Pos = laser.Pos + laser.Dir * laser.Speed;
            }
            friendlyLasers = unimpactedLasers;
        }

        class Laser
        {
            public Vector2 Dir;
            public Vector2 Pos;
            public float Length;
            public float Speed;
        }

        struct Streak
        {
            public Vector2 Position;
            public float Distance;
            public float Thickness;
        }


    }

    public class SampledRegion
    {
        Color[,] sampledGrid;
        public float Width;
        public float Height;
        public Vector2 Position;
        private readonly int verticalSamples = 30;
        private readonly int horizontalSamples;

        public SampledRegion(Texture2D texture, float width, float height)
        {
            float aspect = width / height;
            horizontalSamples = (int)(aspect * verticalSamples);
            sampledGrid = GetSampledGrid(texture, horizontalSamples, verticalSamples);

            this.Width = width;
            this.Height = height;
        }

        public Vector2? Trace(Vector2 p1, Vector2 p2)
        {
            float left = Position.X - 0.5f * Width;
            float right = Position.X + 0.5f * Width;
            float top = Position.Y - 0.5f * Height;
            float bottom = Position.Y + 0.5f * Height;

            float samplesPerWorld = verticalSamples / Height;
            float stepSize = Height / verticalSamples;
            int numSamples = (int)((p2 - p1).Length() * samplesPerWorld);

            Vector2 step = Vector2.Normalize(p2 - p1) * stepSize;

            bool ContainsSolid(Vector2 p)
            {
                int row = (int)(verticalSamples * (p.Y - top) / (bottom - top));
                int column = (int)(horizontalSamples * (p.X - left) / (right - left));
                return sampledGrid[row, column].A > 0.5f;
            }

            // TODO: sampling at p1 seems odd, should be sampling between p1 and p2
            for (int i = 0; i <= numSamples; i++)
            {
                Vector2 samplePos = p1 + step * i;
                if (left < samplePos.X && samplePos.X < right && top < samplePos.Y && samplePos.Y < bottom)
                {
                    if (ContainsSolid(samplePos))
                    {
                        return samplePos;
                    }
                }
            }

            return null;
        }

        private Color[,] GetSampledGrid(Texture2D tex, int width, int height)
        {
            Color[] rawData = new Color[tex.Width * tex.Height];
            tex.GetData<Color>(rawData);
            Color[,] rawDataAsGrid = new Color[height, width];
            for (int sampledRow = 0; sampledRow < height; sampledRow++)
            {
                for (int sampledColumn = 0; sampledColumn < width; sampledColumn++)
                {
                    // Assumes sampledRow major ordering of the array.
                    int srcRow = (int)(tex.Height * sampledRow / (float)height);
                    int srcColumn = (int)(tex.Width * sampledColumn / (float)width);
                    rawDataAsGrid[sampledRow, sampledColumn] = rawData[srcRow * tex.Width + srcColumn];
                }
            }
            return rawDataAsGrid;
        }
    }
}
