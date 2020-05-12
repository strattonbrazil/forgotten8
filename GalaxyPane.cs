using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace forgotten.Desktop
{
    public class GalaxyPane : Pane
    {
        private SpriteBatch spriteBatch;
        private Texture2D systemTexture;
        private Texture2D playerTexture;
        private Texture2D dummyTexture;
        private SpriteFont normalFont;

        private System hoverSystem;
        private System dstSystem;
        private Vector2 playerPos = new Vector2(0,0);

        const float WORLD_WIDTH = 16;
        const float WORLD_HEIGHT = 9;
        const int SYSTEM_HIT_R = 15;

        MouseTracker mouseTracker = new MouseTracker();

        public GalaxyPane()
        {
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

            for (int x = 0; x < WORLD_WIDTH; x++)
            {
                for (int y = 0; y < WORLD_HEIGHT; y++)
                {
                    const float HALF_WORLD_WIDTH = WORLD_WIDTH * 0.5f;
                    const float HALF_WORLD_HEIGHT = WORLD_HEIGHT * 0.5f;
                    Vector2 initPos = new Vector2(x + 0.5f - HALF_WORLD_WIDTH, y + 0.5f - HALF_WORLD_HEIGHT);

                    // TODO: fix this
                    initPos += new Vector2(randomFloat(0.3f, true), randomFloat(0.3f, true));
                    if (randomFloat(1) > 0.3f)
                    {
                        System system = new System(initPos);
                        system.Difficulty = initPos.Length() / new Vector2(HALF_WORLD_WIDTH, HALF_WORLD_HEIGHT).Length();
                        System.systems.Add(system);
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
                for (int systemId = 0; systemId < System.systems.Count; systemId++)
                {
                    System s = System.systems[systemId];
                    float distance = s.Position.Length();
                    if (distance < closestD)
                    {
                        closestD = distance;
                        closestSystemId = systemId;
                    }
                }
                System.systems[closestSystemId].Name = "Terragon";

                int count = 0;
                foreach (System s in System.systems)
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
            Vector2 worldSize = new Vector2(WORLD_WIDTH, WORLD_HEIGHT);
            ScreenUtils su = new ScreenUtils(targetSize, worldSize, 40);

            foreach (System system in System.systems)
            {
                Vector2 screenPos = su.WorldToScreen(system.Position);
                Rectangle screenRect = new Rectangle((int)screenPos.X - SYSTEM_HIT_R, (int)screenPos.Y - SYSTEM_HIT_R,
                                                     SYSTEM_HIT_R * 2, SYSTEM_HIT_R * 2);
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

            if (isTopPane() && mouseTracker.WasPressed() && hoverSystem != null && dstSystem == null)
            {
                dstSystem = hoverSystem;
            }

            float worldVelocity = 0.2f;
            if (dstSystem != null)
            {
                Vector2 dstPos = dstSystem.Position;
                Vector2 dir = Vector2.Normalize(dstPos - playerPos);

                float distanceTraveled = worldVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
                playerPos = playerPos + (dir * distanceTraveled);

                if (Vector2.Distance(playerPos, dstPos) < distanceTraveled)
                { // arrived
                    Console.WriteLine("arrived!");
                    PaneStack.Instance.push(new SystemPane(dstSystem));
                    dstSystem = null;
                }
            }

            //throw new NotImplementedException();
        }

        public override void Draw(ForgottenGame game, Vector2 targetSize)
        {
            if (spriteBatch == null)
            {
                spriteBatch = game.CreateSpriteBatch();
                systemTexture = game.Content.Load<Texture2D>("system");

                const int PLAYER_WIDTH = 8;
                const int PLAYER_HEIGHT = 12;
                playerTexture = new Texture2D(game.GraphicsDevice, PLAYER_WIDTH, PLAYER_HEIGHT);
                Color[] data = new Color[PLAYER_WIDTH * PLAYER_HEIGHT];
                for (int i = 0; i < PLAYER_WIDTH * PLAYER_HEIGHT; i++)
                {
                    data[i] = Color.CornflowerBlue;
                }
                playerTexture.SetData(data);

                dummyTexture = game.CreateDummyTexture(); 

                normalFont = game.Content.Load<SpriteFont>("Galaxy_normal");
            }

            Vector2 worldSize = new Vector2(WORLD_WIDTH, WORLD_HEIGHT);
            ScreenUtils su = new ScreenUtils(targetSize, worldSize, 40);

            spriteBatch.Begin();

            // draw hover system
            if (hoverSystem != null)
            {
                Vector2 hoverPos = su.WorldToScreen(hoverSystem.Position) - new Vector2(SYSTEM_HIT_R, SYSTEM_HIT_R);

                Vector2 texToScreen = new Vector2(SYSTEM_HIT_R*2, SYSTEM_HIT_R*2);
                spriteBatch.Draw(dummyTexture,
                                 hoverPos,
                                 null, // source rect
                                 Color.YellowGreen,
                                 0,
                                 Vector2.Zero,
                                 texToScreen,
                                 SpriteEffects.None,
                                 0);
            }

            foreach (System system in System.systems)
            {
                // draw system
                //
                float spriteRadius = 3 + (int)Math.Ceiling(Math.Sqrt(system.planets.Count));
                Vector2 systemScreenPos = su.WorldToScreen(system.Position) - new Vector2(spriteRadius, spriteRadius);

                float texToScreenRatio = spriteRadius * 2.0f / systemTexture.Width;
                Vector2 texToScreen = new Vector2(texToScreenRatio, texToScreenRatio);
                spriteBatch.Draw(systemTexture, 
                                 systemScreenPos, 
                                 null, // source rect
                                 system.Color,
                                 0,
                                 Vector2.Zero,
                                 texToScreen,
                                 SpriteEffects.None,
                                 0);

                float nameWidth = normalFont.MeasureString(system.Name).X;
                Vector2 namePos = su.WorldToScreen(system.Position) + new Vector2(-nameWidth * 0.5f, 10);
                namePos = new Vector2((int)namePos.X, (int)namePos.Y);
                spriteBatch.DrawString(normalFont, system.Name, namePos, Color.White);
            }

            // draw player
            //
            Vector2 screenPos = su.WorldToScreen(playerPos);
            spriteBatch.Draw(playerTexture, screenPos);

            spriteBatch.End();

            /*
            for (int systemId = 0; systemId < systems.size(); systemId++)
            {
                SystemPtr s = systems[systemId];
                // draw background
                if (systemId == _systemHoverId)
                {
                    sf::Vector2f screenPos = su.worldToScreen(s->pos());
                    //sf::Rect<float> bounds(screenPos.x - 5, screenPos.y - 5, 10, 10);
                    sf::RectangleShape hitBox;
                    hitBox.setPosition(screenPos.x - SYSTEM_HIT_R, screenPos.y - SYSTEM_HIT_R);
                    hitBox.setSize(sf::Vector2f(SYSTEM_HIT_R * 2, SYSTEM_HIT_R * 2));
                    hitBox.setFillColor(sf::Color(255, 255, 0));
                    target.draw(hitBox);
                }

                int spriteRadius = s->size();
                systemSprite.setPosition(su.worldToScreen(s->pos()) - sf::Vector2f(spriteRadius, spriteRadius));
                resizeSprite(systemSprite, spriteRadius * 2, spriteRadius * 2);
                systemSprite.setColor(s->color());
                target.draw(systemSprite);

                sf::Text text(s->name(), font, 14);
            float nameWidth = text.getGlobalBounds().width;
            sf::Vector2f textPosition = round(su.worldToScreen(s->pos()) + sf::Vector2f(-nameWidth * 0.5, 10));
            text.setPosition(textPosition);
            target.draw(text);
        }

        sf::CircleShape player(playerSizeP);
        player.setFillColor(sf::Color(150, 50, 250));
    player.setPosition(su.worldToScreen(_playerPos) - sf::Vector2f(playerSizeP, playerSizeP));
    target.draw(player);

*/

        }
    }
}
