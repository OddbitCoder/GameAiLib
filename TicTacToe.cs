using System;
using System.Collections.Generic;
using System.Linq;

namespace GameAiLib
{
    public class TicTacToe : IGame
    {
        private byte[][] board
            = new byte[3][];
        private Player? winner
            = null;
        private int depth
            = 0;

        public TicTacToe()
        {
            for (int row = 0; row < 3; row++)
            {
                board[row] = new byte[3];
            }
        }

        public Player? Winner
        {
            get { return winner; }
        }

        private bool IsFull
        {
            get { return depth == 9; }
        }

        public bool IsTerminalState
        {
            get { return winner != null || IsFull; }
        }

        public IEnumerable<int> AvailableMoves(Player player)
        {
            var moves = new int[9 - depth];
            int offset = 0;
            int i = 0;
            foreach (var row in board)
            {
                for (int col = 0; col < 3; col++)
                {
                    if (row[col] == 0) { moves[i++] = offset + col; }
                }
                offset += 3;
            }
            return moves;
        }

        public object MakeMove(int move, Player player)
        {
            int row = move / 3;
            int col = move % 3;
            board[row][col] = (byte)(player == Player.Player1 ? 1 : 2);
            depth++;
            // check if this resulted in a win
            if ((board[row][0] == board[row][1] && board[row][1] == board[row][2]) ||
                (board[0][col] == board[1][col] && board[1][col] == board[2][col]) ||
                (row == col && board[0][0] == board[1][1] && board[1][1] == board[2][2]) ||
                (row + col == 2 && board[0][2] == board[1][1] && board[1][1] == board[2][0]))
            {
                winner = player;
            }
            return null;
        }

        public void UndoMove(int move, Player player, object undoToken)
        {
            int row = move / 3;
            int col = move % 3;
            board[row][col] = 0;
            winner = null;
            depth--;
        }

        public bool CheckIntegrity()
        {
            return true;
        }

        public override string ToString()
        {
            var str = "";
            int i = 0;
            var moves = new string[] { "012", "345", "678" };
            foreach (var row in board)
            {
                str += row.Select(x => x == 0 ? "·" : (x == 1 ? "o" : "x")).Aggregate((x, y) => x + y) + " " + moves[i++] + Environment.NewLine;
            }
            return str.TrimEnd();
        }
    }
}
