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
            addChild("dialog", new Dialog(system));
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
            private SpriteFont normalFont;
            private const int DIALOG_WIDTH = 1200;
            private const int DIALOG_HEIGHT = 700;

            public Dialog(System system)
            {
                this.system = system;

                var sortButton = new Button("Sort");
                sortButton.Position = new Vector2(80, 80);
                addChild("sortButton", sortButton);
            }

            public override void Draw(ForgottenGame game, Vector2 targetSize)
            {
                if (spriteBatch == null)
                {
                    spriteBatch = game.CreateSpriteBatch();
                    dummyTexture = game.CreateDummyTexture();
                    normalFont = game.Content.Load<SpriteFont>("Galaxy_normal");
                }

                spriteBatch.Begin();
                spriteBatch.Draw(dummyTexture,
                                 Position,
                                 null, // source rect
                                 Color.LightGray,
                                 0,
                                 Vector2.Zero,
                                 new Vector2(DIALOG_WIDTH, DIALOG_HEIGHT),
                                 SpriteEffects.None,
                                 0);

                spriteBatch.DrawString(normalFont, system.Name, Position, Color.White);

                spriteBatch.End();
            }

            public override void Update(Vector2 targetSize, GameTime gameTime)
            {
                this.Position = 0.5f * (targetSize - new Vector2(DIALOG_WIDTH, DIALOG_HEIGHT));
                //throw new NotImplementedException();
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

            public Button(String text)
            {
                this.text = text;
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
                isHovered = buttonRect.Contains(ms.X, ms.Y);

                if (isHovered && mouseTracker.WasPressed())
                {
                    Console.WriteLine("play sort game");
                }
            }
        }
    }
}
