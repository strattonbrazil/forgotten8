using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace forgotten.Desktop
{
    public class TextAsset : Asset
    {
        public String Text;
        public int MaxLineLength = Int32.MaxValue;

        private string lastText;
        private List<TextBlurb> blurbs = new List<TextBlurb>();

        public TextAsset(String text = "")
        {
            this.Text = text;
        }

        public override void Draw(Vector2 targetSize)
        {
            DrawText(Color.Black, new Vector2(1, 1));
            DrawText(Color.White, Vector2.Zero);
        }

        private void DrawText(Color color, Vector2 offset)
        { 
            Vector2 pos = AbsolutePosition() + offset;

            foreach (TextBlurb blurb in blurbs)
            {
                GameSpriteBatch().DrawString(NormalFont(), blurb.Text, pos + blurb.Position, color);
            }
        }

        public override void Update(Vector2 targetSize, GameTime gameTime)
        {
            if (Text != lastText)
            {
                UpdatePositions();
                lastText = Text;
            }
        }

        private void UpdatePositions()
        {
            blurbs.Clear();

            int lineSpacing = NormalFont().LineSpacing;
            int lineOffset = 0;

            string[] sections = Text.Split('\n');
            for (int i = 0; i < sections.Length; i++)
            {
                string section = sections[i];
                if (section.Trim().Length == 0)
                {
                    lineOffset += lineSpacing;
                }
                else
                {
                    List<string> words = new List<string>(section.Split(' '));
                    string line = "";
                    while (words.Count > 0)
                    {
                        if (NormalFont().MeasureString(line + " " + words[0]).X > MaxLineLength)
                        {
                            //GameSpriteBatch().DrawString(NormalFont(), line, pos + new Vector2(0, lineOffset), color);
                            blurbs.Add(new TextBlurb()
                            {
                                Text = line,
                                Position = new Vector2(0, lineOffset)
                            });
                            lineOffset += lineSpacing;
                            line = words[0]; // handle this word on the following line
                        }
                        else
                        {
                            if (line.Length == 0)
                                line = words[0];
                            else
                                line += " " + words[0];
                        }
                        words.RemoveAt(0);
                    }
                    if (line.Length != 0)
                    {
                        blurbs.Add(new TextBlurb()
                        {
                            Text = line,
                            Position = new Vector2(0, lineOffset)
                        });
                        lineOffset += lineSpacing;
                    }
                }
            }
        }

        public class TextBlurb
        {
            public string Text;
            public Vector2 Position;
        }
    }
}
