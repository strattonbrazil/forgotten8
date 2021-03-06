﻿using System;
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
        public GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;
        private bool escapeWasDown = false;
        private Texture2D dummyTexture;

        public static ForgottenGame GlobalRef;

        private RenderTarget2D renderTarget;

        public ForgottenGame()
        {
            // store for quick access
            GlobalRef = this;

            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1254;
            graphics.PreferredBackBufferHeight = 716;
            graphics.PreferMultiSampling = true;
            Content.RootDirectory = "Content";
        }

        public SpriteBatch CreateSpriteBatch()
        {
            return new SpriteBatch(GraphicsDevice);
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

            //PaneStack.Instance.Push(new GalaxyPane());
            //PaneStack.Instance.Push(new BattlePane());
            PaneStack.Instance.Push(new BoardingPane());
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            dummyTexture = new Texture2D(GraphicsDevice, 1, 1);
            Color[] dummyData = new Color[] { Color.White };
            dummyTexture.SetData(dummyData);
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
            // NOTE: its weird to do this in Update() as opposed to Draw(), but putting it here
            // so the targetsize between Update() and Draw() are the same
            if (renderTarget == null || renderTarget.Width != GraphicsDevice.PresentationParameters.BackBufferWidth * 2)
            {
                renderTarget = new RenderTarget2D(GraphicsDevice, GraphicsDevice.PresentationParameters.BackBufferWidth * 2, GraphicsDevice.PresentationParameters.BackBufferHeight * 2);
            }


            if (!escapeWasDown && Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                if (PaneStack.Instance.panes.Count == 1)
                    Exit();
                else
                    PaneStack.Instance.panes.RemoveAt(PaneStack.Instance.panes.Count - 1);
            }
            escapeWasDown = Keyboard.GetState().IsKeyDown(Keys.Escape);

            Vector2 targetSize = new Vector2(renderTarget.Width, renderTarget.Height);

            List<Pane> currentPanes = new List<Pane>(PaneStack.Instance.panes);
            foreach (Pane pane in currentPanes)
            {
                pane.UpdateTree(targetSize, gameTime);
            }

            base.Update(gameTime);
        }

        public Vector2 MouseScale()
        {
            return new Vector2(renderTarget.Width / (float)GraphicsDevice.PresentationParameters.BackBufferWidth,
                               renderTarget.Height / (float)GraphicsDevice.PresentationParameters.BackBufferHeight);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {

            GraphicsDevice.SetRenderTarget(renderTarget);
            DrawScene();
            GraphicsDevice.SetRenderTarget(null);

            Vector2 invScale = new Vector2((float)GraphicsDevice.PresentationParameters.BackBufferWidth / renderTarget.Width,
                                           (float)GraphicsDevice.PresentationParameters.BackBufferHeight / renderTarget.Height);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.LinearClamp);
            spriteBatch.Draw((Texture2D)renderTarget, 
                             Vector2.Zero, 
                             null, // source rect
                             Color.White,
                             0,
                             Vector2.Zero,
                             invScale,
                             SpriteEffects.None,
                             0);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawScene()
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

                pane.DrawTree(targetSize);
                spriteBatch.End();
            }
        }
    }
}
