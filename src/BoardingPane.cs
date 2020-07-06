using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace forgotten.Desktop
{
    public class BoardingPane : Pane
    {
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

        public BoardingPane()
        {
            List<string> lines = new List<string>(ASCII_LAYOUT.Split('\n'));
            lines = lines.FindAll(line => line.Contains("O"));
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
            }
        }

        public override void Draw(Vector2 targetSize)
        {
            Vector2 origin = new Vector2(30, 30);
            Vector2 cellSize = new Vector2(64, 64);
            Vector2 halfCellSize = cellSize * 0.5f;
            Vector2 boarderSize = new Vector2(32, 32);
            Vector2 halfBoarderSize = boarderSize * 0.5f;
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
                Cell srcCell = boarder.path[0];
                Vector2 pos = origin + cellSize * new Vector2(srcCell.column, srcCell.row) + halfCellSize - halfBoarderSize;
                if (boarder.path.Count > 1)
                {
                    Cell dstCell = boarder.path[1];
                    Vector2 dstPos = origin + cellSize * new Vector2(dstCell.column, dstCell.row) + halfCellSize - halfBoarderSize;
                    pos = pos * (1 - boarder.progress) + dstPos * boarder.progress;
                }
                DrawColoredRect(pos, boarderSize, Color.Crimson);
            }
        }

        public override void Update(Vector2 targetSize, GameTime gameTime)
        {
            foreach (Boarder boarder in boarders)
            {
                if (boarder.path.Count > 1)
                {
                    if (boarder.progress > 1)
                    {
                        boarder.path.RemoveAt(0);
                        boarder.progress = 0;
                    }
                    else
                    {
                        boarder.progress += 10 * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    }
                }
                else // build path
                {
                    Cell srcCell = boarder.path[0];
                    Tree tree = new Tree(srcCell);
                    BuildCellTree(tree, null, 10);


                    while (tree != null && tree.branches.Count > 0)
                    {
                        tree = GameRandom.Instance.Choose(tree.branches);
                        boarder.path.Add(tree.cell);
                    }
                }
            }
        }

        private void BuildCellTree(Tree tree, Tree parent, int depth=3)
        {
            if (depth == 0)
                return;

            Cell c = tree.cell;

            Cell leftCell = c.column > 0 ? cells[c.row, c.column - 1] : null;
            if (leftCell != null && !leftCell.blocked && (parent == null || leftCell != parent.cell))
            {
                tree.branches.Add(new Tree(leftCell));
            }

            Cell upCell = c.row > 0 ? cells[c.row - 1, c.column] : null;
            if (upCell != null && !upCell.blocked && (parent == null || upCell != parent.cell))
            {
                tree.branches.Add(new Tree(upCell));
            }

            Cell rightCell = c.column < numColumns - 1 ? cells[c.row, c.column + 1] : null;
            if (rightCell != null && !rightCell.blocked && (parent == null || rightCell != parent.cell))
            {
                tree.branches.Add(new Tree(rightCell));
            }

            Cell downCell = c.row < numRows - 1 ? cells[c.row + 1, c.column] : null;
            if (downCell != null && !downCell.blocked && (parent == null || downCell != parent.cell))
            {
                tree.branches.Add(new Tree(downCell));
            }

            foreach (Tree b in tree.branches)
            {
                BuildCellTree(b, tree, depth - 1);
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
            public float progress = 0;
        }

        public class Tree
        {
            public readonly Cell cell;
            public List<Tree> branches = new List<Tree>();
            public Tree(Cell cell)
            {
                this.cell = cell;
            }
        }
    }
}
