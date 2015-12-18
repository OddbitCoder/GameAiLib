using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameAi;

namespace TicTacToe
{
    class Program
    {
        class GameState : IGameState
        {
            private byte[][] mBoard
                = new byte[3][];
            private Player? mWinner
                = null;

            public GameState()
            {
                for (int row = 0; row < 3; row++)
                {
                    mBoard[row] = new byte[3];
                }
            }

            public Player? GetWinner()
            {
                return mWinner;
            }

            private bool IsFull
            {
                get { return !mBoard.Any(row => row.Any(x => x == 0)); }
            }

            public bool IsTerminal
            {
                get { return GetWinner() != null || IsFull; }
            }

            public double Score
            {
                get 
                {
                    Player? winner = GetWinner();
                    return (winner == null ? 0 : (winner == Player.Player1 ? 1 : -1));
                }
            }

            public int[] AvailableMoves
            {
                get
                {
                    List<int> moves = new List<int>(9);
                    int offset = 0;
                    foreach (byte[] row in mBoard)
                    {
                        for (int col = 0; col < 3; col++)
                        {
                            if (row[col] == 0) { moves.Add(offset + col); }
                        }
                        offset += 3;
                    }
                    return moves.ToArray();
                }
            }

            public void MakeMove(int move, Player player)
            {
                int row = move / 3;
                int col = move % 3;
                mBoard[row][col] = player.PlayerVal();
                // check if this resulted in a win
                if ((mBoard[row][0] == mBoard[row][1] && mBoard[row][1] == mBoard[row][2]) || 
                    (mBoard[0][col] == mBoard[1][col] && mBoard[1][col] == mBoard[2][col]) ||
                    (row == col && mBoard[0][0] == mBoard[1][1] && mBoard[1][1] == mBoard[2][2]) ||
                    (row + col == 2 && mBoard[0][2] == mBoard[1][1] && mBoard[1][1] == mBoard[2][0]))
                {
                    mWinner = player;
                }
            }

            public void UndoMove(int move, Player player)
            {
                int row = move / 3;
                int col = move % 3;
                mBoard[row][col] = 0;
                mWinner = null;
            }

            public override string ToString()
            {
                string str = "";
                int i = 0;
                string[] moves = new string[] { "012", "345", "678" };
                foreach (byte[] row in mBoard)
                {
                    str += row.Select(x => x == 0 ? "·" : (x == 1 ? "o" : "x")).Aggregate((x, y) => x + y) + " " + moves[i++] + Environment.NewLine;
                }
                return str.TrimEnd();
            }
        }

        static void Main(string[] args)
        {
            Optimizer.Play(new GameState(), maxDepth: int.MaxValue);
        }
    }
}
