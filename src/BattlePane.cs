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
        enum BoardingState
        {
            PREPARED, BOARDING, DESTROYED
        }

        AnimatedAsset alien;
        Texture2D battleship;
        Texture2D enemy;
        Texture2D boardingShip;
        SampledRegion enemyRegion;
        SampledRegion boardingRegion;
        Vector2 shipPosW;
        Vector2 turretPosW;
        float turretRotation = (float)Math.PI;
        const int NUM_STREAKS = 150;
        Streak[] streaks;

        List<Laser> friendlyLasers = new List<Laser>();

        int maxLasers = 4;
        float laserBuffer = 3.2f;
        int boardingHealth = 4;

        BoardingState boardingState = BoardingState.PREPARED;
        //bool boardingAttack = PREPARED;

        Texture2D explosionTexture;
        Texture2D turret;

        public BattlePane()
        {
            battleship = Game().Content.Load<Texture2D>("spaceship_battle");
            enemy = Game().Content.Load<Texture2D>("enemyship_battle");
            turret = Game().Content.Load<Texture2D>("turret");
            boardingShip = Game().Content.Load<Texture2D>("boarding_ship");

            streaks = new Streak[NUM_STREAKS];

            float enemyAspect = (float)enemy.Width / enemy.Height;
            enemyRegion = new SampledRegion(enemy, enemyAspect * 0.5f, 0.5f);

            float boardingAspect = (float)boardingShip.Width / boardingShip.Height;
            boardingRegion = new SampledRegion(boardingShip, boardingAspect * 0.25f, 0.25f);

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
                float desiredHeight = su.ScaleToScreen(boardingRegion.Height);
                Vector2 textureSize = new Vector2(boardingShip.Width, boardingShip.Height);
                Vector2 invScale = new Vector2(su.ScaleToScreen(boardingRegion.Width), su.ScaleToScreen(boardingRegion.Height)) / textureSize;
                Vector2 shipScreenSize = invScale * textureSize;
                Vector2 shipPos = su.WorldToScreen(boardingRegion.Position);

                GameSpriteBatch().Draw(boardingShip,
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


                float desiredSize2 = su.ScaleToScreen(shipWorldSize * 0.5f);
                float invScale2 = desiredSize2 / (float)turret.Height;
                Vector2 turretScreenSize = new Vector2(invScale2 * turret.Width, invScale2 * turret.Height);
                Vector2 turretPos = su.WorldToScreen(turretPosW);

                GameSpriteBatch().Draw(turret,
                                       turretPos,
                                       null, // source rect
                                       Color.White,
                                       turretRotation,
                                       0.5f * new Vector2(turret.Width, turret.Height),
                                       invScale2,
                                       SpriteEffects.None,
                                       0);
            }

            foreach (Laser laser in friendlyLasers)
            {
                Vector2 laserPos = su.WorldToScreen(laser.Pos);
                Vector2 laserTailPos = su.WorldToScreen(laser.Pos - laser.Dir * laser.Length);

                DrawColoredLine(laserPos, laserTailPos, Color.Red, 4);
            }


            int fullChargeWidth = 80;
            int chargePadding = 4;
            Vector2 bufferOrigin = new Vector2(targetSize.X * 0.5f, targetSize.Y - 100);
            //int maxLasers = 4;
            //float laserBuffer = 0;
            for (int i = 0; i < maxLasers; i++)
            {
                float chargeWidth = (float)Math.Min(1.0f, laserBuffer - i) * fullChargeWidth;
                DrawColoredRect(bufferOrigin + new Vector2((fullChargeWidth + chargePadding) * i, 0), new Vector2(fullChargeWidth, 20), Color.Orange);
                DrawColoredRect(bufferOrigin + new Vector2((fullChargeWidth + chargePadding) * i, 0), new Vector2(chargeWidth, 20), Color.Red);
            }
        }

        public override void Update(Vector2 targetSize, GameTime gameTime)
        {
            float aspect = targetSize.X / (float)targetSize.Y;
            ScreenUtils su = new ScreenUtils(targetSize, new Vector2(aspect * 2, 2), 0);

            Vector2 turretD = su.ScreenToWorld(MouseTracker().Position) - turretPosW;
            float dstTurretRotation = (float)(-Math.Atan2(-turretD.Y, turretD.X)) + (float)Math.PI * 1.5f;

            float totalTime = (float)gameTime.TotalGameTime.TotalSeconds;
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //laserBuffer = laserBuffer + gameTime.

            // NOTE: this code sucks, but I'm tired
            if (turretRotation < dstTurretRotation)
                turretRotation += 0.5f * elapsedTime;
            else
                turretRotation -= 0.5f * elapsedTime;

            laserBuffer = Math.Min(laserBuffer + elapsedTime * 0.5f, maxLasers);

            MouseTracker().Update();
            if (MouseTracker().WasPressed() && laserBuffer >= 1)
            {
                //Vector2 target = su.ScreenToWorld(MouseTracker().Position);
                Laser laser = new Laser();
                laser.Pos = turretPosW;
                laser.Dir = new Vector2(-(float)Math.Sin(turretRotation), (float)Math.Cos(turretRotation));   //Vector2.Normalize(target - laser.Pos);
                laser.Length = 0.1f;
                laser.Speed = 0.05f;
                friendlyLasers.Add(laser);

                laserBuffer -= 1;

                if (boardingState == BoardingState.PREPARED)
                    boardingState = BoardingState.BOARDING;
            }

            enemyRegion.Position = new Vector2(0.4f * (float)Math.Sin(totalTime * 1.5f), -0.6f + 0.02f * (float)Math.Cos(totalTime * 0.5f));

            if (boardingState == BoardingState.PREPARED)
                boardingRegion.Position = enemyRegion.Position;
            else if (boardingState == BoardingState.BOARDING)
            {
                Vector2 d = shipPosW - boardingRegion.Position;
                d.Normalize();
                boardingRegion.Position += d * 0.1f * elapsedTime;
            } else if (boardingState == BoardingState.DESTROYED)
            {
                // TODO: animate this, use acceleration
                boardingRegion.Acceleration = new Vector2(1, 0);
                boardingRegion.Velocity = boardingRegion.Acceleration * elapsedTime;
                boardingRegion.Position += boardingRegion.Velocity;

                //asset.Velocity = asset.Acceleration * elapsedTime;
                //asset.Position += su.ScaleToScreen(asset.Velocity);
            }

            shipPosW = new Vector2(0.2f * (float)Math.Sin(totalTime), 0.6f + 0.02f * (float)Math.Cos(totalTime * 0.5f));
            turretPosW = shipPosW + new Vector2(0.26f, 0.0f);

            for (int i = 0; i < NUM_STREAKS; i++)
            {
                float speed = 2 + (i % 4);
                float x = -aspect + (float)Math.Cos(i) + speed*totalTime; //20 + 50 * (float)Math.Cos(i);
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
                Vector2? enemyImpactPos = enemyRegion.Trace(prevPos, newPos);
                Vector2? boardingImpactPos = boardingRegion.Trace(prevPos, newPos);
                float boardingDistance = shipPosW.Y - boardingRegion.Position.Y;

                if (boardingState == BoardingState.BOARDING && boardingImpactPos != null && boardingDistance > 0.15f)
                {
                    var explosion = new TextureAnimationAsset(explosionTexture, 3, 5);
                    explosion.Position = su.WorldToScreen(boardingImpactPos.Value);
                    explosion.Acceleration = new Vector2(1, 0);
                    AddChild("explosion_effect", explosion);
                    boardingHealth--;
                    if (boardingHealth == 0)
                    {
                        boardingState = BoardingState.DESTROYED;
                    }
                }
                else if (enemyImpactPos != null)
                {
                    var explosion = new TextureAnimationAsset(explosionTexture, 3, 5);
                    explosion.Position = su.WorldToScreen(enemyImpactPos.Value);
                    explosion.Acceleration = new Vector2(1, 0);
                    AddChild("explosion_effect", explosion);
                } else if (laser.Pos.Length() < 10)
                {
                    unimpactedLasers.Add(laser);
                }
                laser.Pos = laser.Pos + laser.Dir * laser.Speed;
            }
            friendlyLasers = unimpactedLasers;

            foreach (KeyValuePair<string,Asset> pair in children)
            {
                if (typeof(TextureAnimationAsset).IsInstanceOfType(pair.Value))
                {
                    TextureAnimationAsset asset = (TextureAnimationAsset)pair.Value;
                    asset.Velocity = asset.Acceleration * elapsedTime;
                    asset.Position += su.ScaleToScreen(asset.Velocity);
                }
            }

            bool isNotFinishedExplosion(Asset asset)
            {
                if (typeof(TextureAnimationAsset).IsInstanceOfType(asset))
                {
                    TextureAnimationAsset anim = (TextureAnimationAsset)asset;
                    return !anim.IsFinished();
                }

                return true;
            }

            children = children.FindAll(pair => isNotFinishedExplosion(pair.Value));
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
        public Vector2 Acceleration;
        public Vector2 Velocity;
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
