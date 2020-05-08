using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace forgotten.Desktop
{
    public class GalaxyPane : Pane
    {
        private SpriteBatch spriteBatch;
        private Texture2D systemTexture;

        const float WORLD_WIDTH = 16;
        const float WORLD_HEIGHT = 9;

        public GalaxyPane()
        {
            Random rnd = new Random();

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
                    //Console.WriteLine(randomFloat(0.3f, true));

                    if (randomFloat(1) > 0.3f)
                    {
                        System.systems.Add(new System(initPos));
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
                    if (s.Name == "")
                    {
                        s.Name = PLANET_NAMES[count % PLANET_NAMES.Length];
                        count++;
                    }
                }
            }
        }

        public override void Update(Vector2 targetSize, GameTime gameTime)
        {
            //throw new NotImplementedException();
        }

        public override void Draw(ForgottenGame game, Vector2 targetSize)
        {
            if (spriteBatch == null)
            {
                spriteBatch = game.createSpriteBatch();
                systemTexture = game.Content.Load<Texture2D>("system");
            }

            Vector2 worldSize = new Vector2(WORLD_WIDTH, WORLD_HEIGHT);
            ScreenUtils su = new ScreenUtils(targetSize, worldSize, 40);

            spriteBatch.Begin();

            foreach (System system in System.systems)
            {
                // draw hover text

                float spriteRadius = system.Size;
                Vector2 screenPos = su.WorldToScreen(system.Position) - new Vector2(spriteRadius, spriteRadius);
                //spriteBatch.Draw(systemTexture, screenPos);

                //Rectangle dstRect = new Rectangle(screenPos, new Vector2(spriteRadius * 2, spriteRadius * 2));
                //dstRect.
                float texToScreenRatio = spriteRadius * 2.0f / systemTexture.Width;
                Vector2 texToScreen = new Vector2(texToScreenRatio, texToScreenRatio);
                spriteBatch.Draw(systemTexture, 
                                 screenPos, 
                                 null, // source rect
                                 system.Color,
                                 0,
                                 Vector2.Zero,
                                 texToScreen,
                                 SpriteEffects.None,
                                 0);
            }

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
