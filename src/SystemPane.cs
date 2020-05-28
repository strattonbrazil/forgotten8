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
            AddChild("dialog", new Dialog(this, system));
        }

        public override void Draw(Vector2 targetSize)
        {
        }

        public override void Update(Vector2 targetSize, GameTime gameTime)
        {
        }

        private class Dialog : Asset
        {
            private System system;
            private Texture2D backgroundTexture;
            private const int DIALOG_WIDTH = 1200;
            private const int DIALOG_HEIGHT = 700;
            private TextAsset systemLabel;
            private TextAsset difficultyLabel;
            private TextAsset planetInfoLabel;

            public Dialog(Pane pane, System system)
            {
                this.system = system;

                systemLabel = AddChild("systemLabel", new TextAsset(system.Name));
                difficultyLabel = AddChild("difficultyLabel", new TextAsset(system.Difficulty.ToString()));
                planetInfoLabel = AddChild("difficultyLabel", new TextAsset());

                int planetIndex = 0;
                foreach (Planet planet in system.Planets)
                {
                    Func<bool> onClick = delegate ()
                    {
                        if (pane.IsTopPane())
                        {
                            PaneStack.Instance.Push(new PlanetPane(planet));
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

                    var planetButton = new ButtonAsset(planet.Name, onClick, onHover);
                    planetButton.Position = new Vector2(280 + (planetIndex*160), 280);
                    AddChild("planetButton", planetButton);
                    planetIndex++;
                }

                if (system.Name == "Terragon")
                {
                    Func<bool> onClick = delegate ()
                    {
                        if (pane.IsTopPane())
                        {
                            PaneStack.Instance.Push(new SpacePortPane());
                        }
                        return true;
                    };
                    Func<bool, bool> onHover = delegate (bool hovered)
                    {
                        return true;
                    };
                    var spacePortButton = new ButtonAsset("Space Port", onClick, onHover);
                    spacePortButton.Position = new Vector2(280, 380);
                    AddChild("spacePortButton", spacePortButton);
                }
            }

            public override void Draw(Vector2 targetSize)
            {
                if (backgroundTexture == null)
                {
                    backgroundTexture = Game().Content.Load<Texture2D>("system_pane_background");
                }

                Vector2 borderOffset = new Vector2(2, 2);
                DrawColoredRect(Position - borderOffset,
                                new Vector2(DIALOG_WIDTH, DIALOG_HEIGHT) + borderOffset * 2,
                                Color.Black);

                GameSpriteBatch().Draw(backgroundTexture,
                                       Position,
                                       null, // source rect
                                       Color.LightGray,
                                       0,
                                       Vector2.Zero,
                                       Vector2.One,
                                       SpriteEffects.None,
                                       0);
                                 
            }

            public override void Update(Vector2 targetSize, GameTime gameTime)
            {
                this.Position = 0.5f * (targetSize - new Vector2(DIALOG_WIDTH, DIALOG_HEIGHT));

                systemLabel.Position = Position;
                difficultyLabel.Position = Position + new Vector2(0, 30);
                planetInfoLabel.Position = new Vector2(480, 380);
            }
        }
    }
}
