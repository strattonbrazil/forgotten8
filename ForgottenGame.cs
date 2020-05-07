using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace forgotten.Desktop
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class ForgottenGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D system;

        const float WORLD_WIDTH = 16;
        const float WORLD_HEIGHT = 9;

        public ForgottenGame()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1254;
            graphics.PreferredBackBufferHeight = 716;
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();

            Random rnd = new Random();

            float randomFloat(float range, bool centered = false)
            {
                float val = (float)rnd.NextDouble();
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

            PaneStack.Instance.push(new GalaxyPane());
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            system = this.Content.Load<Texture2D>("system");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            foreach (Pane pane in PaneStack.Instance.panes)
            {
                Viewport viewport = graphics.GraphicsDevice.Viewport;
                Vector2 targetSize = new Vector2(viewport.Width, viewport.Height);
                pane.UpdateTree(targetSize, gameTime);
                pane.Draw(targetSize);
            }
            spriteBatch.Begin();
            spriteBatch.Draw(system, new Vector2(0, 0));
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
