using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace forgotten.Desktop
{
    public class GalaxyPane : Pane
    {
        private Texture2D systemTexture;
        private Texture2D playerTexture;

        private System hoverSystem;
        private System dstSystem;
        private Vector2 playerPos = new Vector2(0,0);

        const float WorldWidth = 16;
        const float WorldHeight = 9;
        const int SystemHitR = 15;

        MouseTracker mouseTracker;

        public GalaxyPane()
        {
            mouseTracker = new MouseTracker(this);

            GameRandom rnd = GameRandom.Instance;

            float randomFloat(float range, bool centered = false)
            {
                float val = (float)rnd.NextDouble() * range;
                if (centered)
                {
                    val = 2 * val - range;
                }
                return val;
            }

            for (int x = 0; x < WorldWidth; x++)
            {
                for (int y = 0; y < WorldHeight; y++)
                {
                    const float HalfWorldWidth = WorldWidth * 0.5f;
                    const float HalfWorldHeight = WorldHeight * 0.5f;
                    Vector2 initPos = new Vector2(x + 0.5f - HalfWorldWidth, y + 0.5f - HalfWorldHeight);

                    // TODO: fix this
                    initPos += new Vector2(randomFloat(0.3f, true), randomFloat(0.3f, true));
                    if (randomFloat(1) > 0.3f)
                    {
                        System system = new System(initPos);
                        system.Difficulty = initPos.Length() / new Vector2(HalfWorldWidth, HalfWorldHeight).Length();
                        System.Systems.Add(system);
                    }
                }
            }

            // generate planet names
            {
                String[] PLANET_NAMES = {
                    "Fremulon",
                    "Erakis",
                    "Hyporayon",
                    "Kreeptan",
                    "Ohlderahn",
                    "Tarkalis",
                    "Ooban"
                };
                int closestSystemId = -1;
                float closestD = 1000000;
                for (int systemId = 0; systemId < System.Systems.Count; systemId++)
                {
                    System s = System.Systems[systemId];
                    float distance = s.Position.Length();
                    if (distance < closestD)
                    {
                        closestD = distance;
                        closestSystemId = systemId;
                    }
                }

                System.Systems[closestSystemId].SetHomeSystem();

                int count = 0;
                foreach (System s in System.Systems)
                {
                    if (s.Name == null)
                    {
                        s.Name = PLANET_NAMES[count % PLANET_NAMES.Length];
                        count++;
                    }
                }
            }
        }

        // given point on the screen identify the cursor
        public System GetSystem(Vector2 targetSize, Point p)
        {
            Vector2 worldSize = new Vector2(WorldWidth, WorldHeight);
            ScreenUtils su = new ScreenUtils(targetSize, worldSize, 40);

            foreach (System system in System.Systems)
            {
                Vector2 screenPos = su.WorldToScreen(system.Position);
                Rectangle screenRect = new Rectangle((int)screenPos.X - SystemHitR, (int)screenPos.Y - SystemHitR,
                                                     SystemHitR * 2, SystemHitR * 2);
                if (screenRect.Contains(p))
                {
                    return system;
                }
            }
            return null;
        }

        public override void Update(Vector2 targetSize, GameTime gameTime)
        {
            MouseState ms = Mouse.GetState();
            mouseTracker.Update(ms);
            hoverSystem = GetSystem(targetSize, ms.Position);

            if (IsTopPane() && mouseTracker.WasPressed() && hoverSystem != null && dstSystem == null)
            {
                dstSystem = hoverSystem;
            }

            float worldVelocity = 2;
            if (dstSystem != null)
            {
                Vector2 dstPos = dstSystem.Position;
                Vector2 dir = Vector2.Normalize(dstPos - playerPos);

                float distanceTraveled = worldVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
                playerPos = playerPos + (dir * distanceTraveled);

                if (Vector2.Distance(playerPos, dstPos) < distanceTraveled)
                { // arrived
                    PaneStack.Instance.Push(new SystemPane(dstSystem));
                    dstSystem = null;
                }
            }
        }

        public override void Draw(Vector2 targetSize)
        {
            if (systemTexture == null)
            {
                systemTexture = Game().Content.Load<Texture2D>("system");

                const int PLAYER_WIDTH = 8;
                const int PLAYER_HEIGHT = 12;
                playerTexture = new Texture2D(Game().GraphicsDevice, PLAYER_WIDTH, PLAYER_HEIGHT);
                Color[] data = new Color[PLAYER_WIDTH * PLAYER_HEIGHT];
                for (int i = 0; i < PLAYER_WIDTH * PLAYER_HEIGHT; i++)
                {
                    data[i] = Color.CornflowerBlue;
                }
                playerTexture.SetData(data);
            }

            Vector2 worldSize = new Vector2(WorldWidth, WorldHeight);
            ScreenUtils su = new ScreenUtils(targetSize, worldSize, 40);
            
            // draw hover system
            if (hoverSystem != null)
            {
                Vector2 hoverPos = su.WorldToScreen(hoverSystem.Position) - new Vector2(SystemHitR, SystemHitR);

                Vector2 texToScreen = new Vector2(SystemHitR*2, SystemHitR*2);

                DrawColoredRect(hoverPos, texToScreen, Color.YellowGreen);
            }

            foreach (System system in System.Systems)
            {
                // draw system
                //
                float spriteRadius = 3 + (int)Math.Ceiling(Math.Sqrt(system.Planets.Count));
                Vector2 systemScreenPos = su.WorldToScreen(system.Position) - new Vector2(spriteRadius, spriteRadius);

                float texToScreenRatio = spriteRadius * 2.0f / systemTexture.Width;
                Vector2 texToScreen = new Vector2(texToScreenRatio, texToScreenRatio);
                GameSpriteBatch().Draw(systemTexture, 
                                       systemScreenPos, 
                                       null, // source rect
                                       system.Color,
                                       0,
                                       Vector2.Zero,
                                       texToScreen,
                                       SpriteEffects.None,
                                       0);

                float nameWidth = NormalFont().MeasureString(system.Name).X;
                Vector2 namePos = su.WorldToScreen(system.Position) + new Vector2(-nameWidth * 0.5f, 10);
                namePos = new Vector2((int)namePos.X, (int)namePos.Y);
                GameSpriteBatch().DrawString(NormalFont(), system.Name, namePos, Color.White);
            }

            // draw player
            //
            Vector2 screenPos = su.WorldToScreen(playerPos);
            GameSpriteBatch().Draw(playerTexture, screenPos);
        }
    }
}
