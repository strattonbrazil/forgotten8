using System;
using System.Collections.Generic;
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
        Texture2D dummyTexture;


        public ForgottenGame()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1254;
            graphics.PreferredBackBufferHeight = 716;
            Content.RootDirectory = "Content";
        }

        public SpriteBatch CreateSpriteBatch()
        {
            return new SpriteBatch(GraphicsDevice);
        }

        public Texture2D CreateDummyTexture()
        {
            var dummyTexture = new Texture2D(GraphicsDevice, 1, 1);
            Color[] dummyData = new Color[] { Color.White };
            dummyTexture.SetData(dummyData);
            return dummyTexture;
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
            Console.WriteLine("initializing");
            base.Initialize();

            this.IsMouseVisible = true;
            this.Window.AllowUserResizing = true;

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

            dummyTexture = CreateDummyTexture();
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

            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                if (PaneStack.Instance.panes.Count == 1)
                    Exit();
            }


            Viewport viewport = graphics.GraphicsDevice.Viewport;
            Vector2 targetSize = new Vector2(viewport.Width, viewport.Height);

            List<Pane> currentPanes = new List<Pane>(PaneStack.Instance.panes);
            foreach (Pane pane in currentPanes)
            {
                pane.UpdateTree(targetSize, gameTime);
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            Viewport viewport = graphics.GraphicsDevice.Viewport;
            Vector2 targetSize = new Vector2(viewport.Width, viewport.Height);

            foreach (Pane pane in PaneStack.Instance.panes)
            {
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                spriteBatch.Draw(dummyTexture,
                                 Vector2.Zero,
                                 null, // source rect
                                 Color.Black * 0.8f,
                                 0,
                                 Vector2.Zero,
                                 targetSize,
                                 SpriteEffects.None,
                                 0);
                spriteBatch.End();

                pane.DrawTree(this, targetSize);
            }
            base.Draw(gameTime);
        }
    }
}
