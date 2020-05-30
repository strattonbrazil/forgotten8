using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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
        private float animOffset = 0;

        public TextAsset(String text = "")
        {
            this.Text = text;
        }

        public override void Draw(Vector2 targetSize)
        {
            DrawText(Color.Black, new Vector2(-1, -1));
            DrawText(Color.Black, new Vector2(1, 1));
            DrawText(Color.White, Vector2.Zero);
        }

        private void DrawText(Color color, Vector2 offset)
        { 
            Vector2 pos = AbsolutePosition() + offset;

            foreach (TextBlurb blurb in blurbs)
            {
                var moved = pos + blurb.Position;
                if (blurb.Animated)
                    moved.X += animOffset;
                GameSpriteBatch().DrawString(NormalFont(), blurb.Text, moved, color);
            }
        }

        public override void Update(Vector2 targetSize, GameTime gameTime)
        {
            if (Text != lastText)
            {
                UpdatePositions();
                lastText = Text;
            }

            var gameTimeFlat = (int)gameTime.TotalGameTime.TotalSeconds;
            animOffset = 2 * (gameTimeFlat % 2) - 1;
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
                            AddLine(line, lineOffset);
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
                        AddLine(line, lineOffset);
                        lineOffset += lineSpacing;
                    }
                }
            }
        }

        private void AddLine(string line, int lineOffset)
        {
            Console.WriteLine("adding line: " + line);
            float lineIndent = 0;
            string[] parts = Regex.Split(line, @"(\*[a-z]+\*)+");
            foreach (string part in parts)
            {
                blurbs.Add(new TextBlurb()
                {
                    Text = part,
                    Position = new Vector2(lineIndent, lineOffset),
                    Animated = part.StartsWith("*") && part.EndsWith("*")
                });
                lineIndent += NormalFont().MeasureString(part).X;
            }
        }

        public class TextBlurb
        {
            public string Text;
            public Vector2 Position;
            public bool Animated;
        }
    }
}
