using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace forgotten.Desktop
{
    public class SystemPane : Pane
    {
        private readonly System system;

        public SystemPane(System system)
        {
            this.system = system;
            addChild("dialog", new Dialog(this, system));
        }

        public override void Draw(ForgottenGame game, Vector2 targetSize)
        {
            //throw new NotImplementedException();
        }

        public override void Update(Vector2 targetSize, GameTime gameTime)
        {
            //throw new NotImplementedException();
        }

        private class Dialog : Asset
        {
            private System system;
            private SpriteBatch spriteBatch;
            private Texture2D dummyTexture;
            private Texture2D backgroundTexture;
            private const int DIALOG_WIDTH = 1200;
            private const int DIALOG_HEIGHT = 700;
            private TextAsset systemLabel;
            private TextAsset difficultyLabel;
            private TextAsset planetInfoLabel;

            public Dialog(Pane pane, System system)
            {
                this.system = system;

                systemLabel = (TextAsset)addChild("systemLabel", new TextAsset(system.Name));
                difficultyLabel = (TextAsset)addChild("difficultyLabel", new TextAsset(system.Difficulty.ToString()));
                planetInfoLabel = (TextAsset)addChild("difficultyLabel", new TextAsset());

                int planetIndex = 0;
                foreach (Planet planet in system.planets)
                {
                    Func<bool> onClick = delegate ()
                    {
                        if (pane.isTopPane())
                        {
                            PaneStack.Instance.push(new PlanetPane(planet));
                        }
                        return true;
                    };
                    Func<bool,bool> onHover = delegate (bool hovered)
                    {
                        if (hovered)
                        {
                            planetInfoLabel.Text = "hovered over planet: " + planet.Name;
                        }
                        else
                        {
                            planetInfoLabel.Text = "";
                        }
                        return true;
                    };

                    var planetButton = new Button(planet.Name, onClick, onHover);
                    planetButton.Position = new Vector2(280 + (planetIndex*160), 280);
                    addChild("planetButton", planetButton);
                    planetIndex++;
                }

                if (system.Name == "Terragon")
                {
                    Func<bool> onClick = delegate ()
                    {
                        if (pane.isTopPane())
                        {
                            PaneStack.Instance.push(new SpacePortPane());
                        }
                        return true;
                    };
                    Func<bool, bool> onHover = delegate (bool hovered)
                    {
                        return true;
                    };
                    var spacePortButton = new Button("Space Port", onClick, onHover);
                    spacePortButton.Position = new Vector2(280, 380);
                    addChild("spacePortButton", spacePortButton);
                }
            }

            public override void Draw(ForgottenGame game, Vector2 targetSize)
            {
                if (spriteBatch == null)
                {
                    spriteBatch = game.CreateSpriteBatch();
                    dummyTexture = game.CreateDummyTexture();
                    backgroundTexture = game.Content.Load<Texture2D>("system_pane_background");

                }

                Vector2 borderOffset = new Vector2(2, 2);
                spriteBatch.Begin();
                spriteBatch.Draw(dummyTexture,
                                 Position - borderOffset,
                                 null, // source rect
                                 Color.Black,
                                 0,
                                 Vector2.Zero,
                                 new Vector2(DIALOG_WIDTH, DIALOG_HEIGHT) + borderOffset*2,
                                 SpriteEffects.None,
                                 0);

                spriteBatch.Draw(backgroundTexture,
                                 Position,
                                 null, // source rect
                                 Color.LightGray,
                                 0,
                                 Vector2.Zero,
                                 Vector2.One,
                                 SpriteEffects.None,
                                 0);
                                 
                spriteBatch.End();
            }

            public override void Update(Vector2 targetSize, GameTime gameTime)
            {
                this.Position = 0.5f * (targetSize - new Vector2(DIALOG_WIDTH, DIALOG_HEIGHT));

                systemLabel.Position = Position;
                difficultyLabel.Position = Position + new Vector2(0, 30);
                planetInfoLabel.Position = new Vector2(480, 380);
            }
        }

        private class Button : Asset
        {
            private SpriteBatch spriteBatch;
            private Texture2D dummyTexture;
            private SpriteFont normalFont;
            private String text;
            private Vector2 size;
            private MouseTracker mouseTracker = new MouseTracker();
            private bool isHovered;
            private Func<bool> clickCallback;
            private Func<bool,bool> hoverCallback;

            public Button(String text, Func<bool> onClick, Func<bool,bool> onHover)
            {
                this.text = text;
                this.clickCallback = onClick;
                this.hoverCallback = onHover;
            }

            public override void Draw(ForgottenGame game, Vector2 targetSize)
            {
                if (spriteBatch == null)
                {
                    spriteBatch = game.CreateSpriteBatch();
                    dummyTexture = game.CreateDummyTexture();
                    normalFont = game.Content.Load<SpriteFont>("Galaxy_normal");
                }

                var cornerPos = AbsolutePosition();

                spriteBatch.Begin();

                var fillColor = Color.CornflowerBlue;
                if (isHovered)
                    fillColor = Color.Blue;

                spriteBatch.Draw(dummyTexture,
                                 cornerPos,
                                 null, // source rect
                                 fillColor,
                                 0,
                                 Vector2.Zero,
                                 size,
                                 SpriteEffects.None,
                                 0);

                spriteBatch.DrawString(normalFont, text, cornerPos + new Vector2(10, 0), Color.White);

                spriteBatch.End();
            }

            public override void Update(Vector2 targetSize, GameTime gameTime)
            {
                MouseState ms = Mouse.GetState();
                mouseTracker.Update(ms);

                size = new Vector2(80, 30);

                var pos = AbsolutePosition();
                Rectangle buttonRect = new Rectangle((int)pos.X, (int)pos.Y, (int)size.X, (int)size.Y);
                bool wasHovered = isHovered;
                isHovered = buttonRect.Contains(ms.X, ms.Y);
                if (isHovered)
                    hoverCallback(true);
                else if (wasHovered)
                    hoverCallback(false);


                if (isHovered && mouseTracker.WasPressed())
                {
                    clickCallback();
                }
            }
        }
    }
}
