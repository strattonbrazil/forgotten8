using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace forgotten.Desktop
{
    public class BoardingPane : Pane
    {
        private Texture2D boarderTexture;

        private const string ASCII_LAYOUT = @"""
OOOOOOOOOOOOO
OBBBOBBBBBOBO
OBOOOBOOOOOOO
OOOBOOOBBOBBO
OBOBBBBBBOBBO
OBOBBBOOOOOOO
OBOOOBOBBOBBO
OBBBOOOBBOOOO
OOOBOBOBBBBBO
OBOOOBOOOBOOO
OBOBOBBBOBOBO
OOOBOOOOOOOBO
OBBBOBBBBOBBO
OOOOOOOOOOOOO
""";
        Cell[,] cells;
        Boarder[] boarders;
        readonly int numRows;
        readonly int numColumns;
        readonly float turningTime = 1.0f;

        public BoardingPane()
        {
            List<string> lines = new List<string>(ASCII_LAYOUT.Split('\n'));
            lines = lines.FindAll(line => line.Contains("O") || line.Contains("B"));
            numRows = lines.Count;
            numColumns = lines[0].Trim().Length;
            cells = new Cell[lines.Count, numColumns];
            for (int row = 0; row < numRows; row++)
            {
                String line = lines[row].Trim();
                for (int column = 0; column < numColumns; column++)
                {
                    cells[row, column] = new Cell(row, column, line[column]);
                }
            }
            Console.WriteLine("num rows: " + numRows);
            Console.WriteLine("num columns: " + numColumns);

            boarders = new Boarder[3];
            for (int i = 0; i < boarders.Length; i++)
            {
                boarders[i] = new Boarder();
                boarders[i].path.Add(cells[0, 3 + i]);
                boarders[i].progress = 0.0f;
                boarders[i].turning = turningTime;
            }

            boarderTexture = Game().Content.Load<Texture2D>("boarder");
        }

        public override void Draw(Vector2 targetSize)
        {
            Vector2 origin = new Vector2(30, 30);
            Vector2 cellSize = new Vector2(64, 64);
            Vector2 halfCellSize = cellSize * 0.5f;
            //Vector2 boarderSize = new Vector2(32, 32);
            //Vector2 halfBoarderSize = boarderSize * 0.5f;
            for (int row = 0; row < numRows; row++)
            {
                for (int column = 0; column < numColumns; column++)
                {
                    Color color = cells[row, column].blocked ? Color.DarkMagenta : Color.CornflowerBlue;
                    DrawColoredRect(origin + cellSize * new Vector2(column, row),
                        cellSize, color);
                }
            }

            foreach (Boarder boarder in boarders)
            {
                int boarderSize = 32;
                Vector2 halfBoarderSize = new Vector2(0.5f * boarderSize, 0.5f * boarderSize);

                Cell srcCell = boarder.path[0];
                Vector2 pos = origin + cellSize * new Vector2(srcCell.column, srcCell.row) + halfCellSize;// - halfBoarderSize;
                if (boarder.path.Count > 1)
                {
                    Cell dstCell = boarder.path[1];
                    Vector2 dstPos = origin + cellSize * new Vector2(dstCell.column, dstCell.row) + halfCellSize;// - halfBoarderSize;
                    pos = pos * (1 - boarder.progress) + dstPos * boarder.progress;
                }

                Vector2 texToScreen = new Vector2((float)boarderSize / boarderTexture.Width, (float)boarderSize / boarderTexture.Height);
                GameSpriteBatch().Draw(boarderTexture,
                                       pos,
                                       null, // source rect
                                       Color.White,
                                       boarder.rotation,
                                       0.5f * new Vector2(boarderTexture.Width, boarderTexture.Height),//halfBoarderSize,// Vector2.Zero,
                                       texToScreen,
                                       SpriteEffects.None,
                                       0);
            }
        }

        public override void Update(Vector2 targetSize, GameTime gameTime)
        {
            HashSet<Boarder> toUpdate = new HashSet<Boarder>(boarders);
            while (toUpdate.Count > 0)
            {
                // silly pop
                Boarder boarder = null;
                foreach (Boarder b in toUpdate)
                {
                    boarder = b;
                    toUpdate.Remove(b);
                    break;
                }

                if (boarder.path.Count < 20)
                    BuildCellPath(boarder.path, 20);

                if (boarder.progress > 1)
                {
                    Cell prevCell = boarder.path[0];
                    boarder.path.RemoveAt(0);
                    boarder.progress = 0;

                    Cell currentCell = boarder.path[0];
                    Cell nextCell = boarder.path[1];

                    if (nextCell.row < currentCell.row && prevCell.row == currentCell.row) // going up now
                    {
                        boarder.prevRotation = boarder.rotation;
                        boarder.targetRotation = (float)Math.PI;
                        boarder.turning = turningTime;
                    }
                    else if (nextCell.column > currentCell.column && prevCell.row != currentCell.row) // going right now
                    {
                        boarder.prevRotation = boarder.rotation;
                        boarder.targetRotation = (float)(Math.PI * 1.5f);
                        boarder.turning = turningTime;
                    }
                    else if (nextCell.row > currentCell.row && prevCell.column != currentCell.column) // going down now
                    {
                        boarder.prevRotation = boarder.rotation;
                        boarder.targetRotation = 0;
                        boarder.turning = turningTime;
                    }
                    else if (nextCell.column < currentCell.column && prevCell.row != currentCell.row) // going left now
                    {
                        boarder.prevRotation = boarder.rotation;
                        boarder.targetRotation = (float)(Math.PI * 0.5f);
                        boarder.turning = turningTime;
                    }
                }
                else if (boarder.turning > 0)
                {
                    boarder.turning -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    // TODO: clamp this

                    // c = 1 -> starting turn, c = 0 -> ending turn
                    float c = boarder.turning / turningTime;
                    boarder.rotation = boarder.targetRotation * (1-c) + boarder.prevRotation * c;
                }
                else
                {
                    // if someone else going to my next spot, don't
                    Cell srcCell = boarder.path[0];
                    Cell dstCell = boarder.path[1];
                    bool wait = false;
                    foreach (Boarder other in boarders)
                    {
                        if (boarder == other)
                            continue;
                        if (other.path.Count > 1 && other.path[1] == boarder.path[1] && other.progress > boarder.progress)
                            wait = true;
                    }
                    if (!wait)
                    {
                        boarder.progress += 1 * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    }
                }
            }
        }

        private void BuildCellPath(List<Cell> path, int depth=3)
        {
            if (depth == 0)
                return;

            List<Cell> possibleBranches = new List<Cell>();

            Cell cell = path[path.Count - 1];
            Cell parent = null;
            if (path.Count > 1)
                parent = path[path.Count - 2];

            Cell leftCell = cell.column > 0 ? cells[cell.row, cell.column - 1] : null;
            if (leftCell != null && !leftCell.blocked && (parent == null || leftCell != parent))
            {
                possibleBranches.Add(leftCell);
            }

            Cell upCell = cell.row > 0 ? cells[cell.row - 1, cell.column] : null;
            if (upCell != null && !upCell.blocked && (parent == null || upCell != parent))
            {
                possibleBranches.Add(upCell);
            }

            Cell rightCell = cell.column < numColumns - 1 ? cells[cell.row, cell.column + 1] : null;
            if (rightCell != null && !rightCell.blocked && (parent == null || rightCell != parent))
            {
                possibleBranches.Add(rightCell);
            }

            Cell downCell = cell.row < numRows - 1 ? cells[cell.row + 1, cell.column] : null;
            if (downCell != null && !downCell.blocked && (parent == null || downCell != parent))
            {
                possibleBranches.Add(downCell);
            }

            if (possibleBranches.Count > 0)
            {
                path.Add(GameRandom.Instance.Choose(possibleBranches));
                BuildCellPath(path, depth - 1);
            }
        }

        public class Cell
        {
            public readonly bool blocked;
            public readonly int row;
            public readonly int column;

            public Cell(int row, int column, char c)
            {
                this.row = row;
                this.column = column;
                this.blocked = (c == 'B');
            }
        }

        public class Boarder
        {
            public List<Cell> path = new List<Cell>();
            //public Cell src, dst;
            public float turning = 0; // how much time left turning
            public float progress = 0;

            public float prevRotation = 0;
            public float targetRotation = 0;
            public float rotation = 0;
        }
    }
}
