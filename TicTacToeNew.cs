using System;
using System.Collections.Generic;
using System.Linq;

namespace GameAiLib
{
    public class TicTacToeNew : IGameNew
    {
        private byte[][] board
            = new byte[3][];
        private Player? winner
            = null;
        private int depth
            = 0;
        private Player currentPlayer;

        public TicTacToeNew(Player startingPlayer = Player.Player1)
        {
            for (int row = 0; row < 3; row++)
            {
                board[row] = new byte[3];
            }
            currentPlayer = startingPlayer;
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

        public IEnumerable<int> AvailableMoves()
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

        public object MakeMove(int move)
        {
            int row = move / 3;
            int col = move % 3;
            board[row][col] = (byte)(currentPlayer == Player.Player1 ? 1 : 2);
            depth++;
            // check if this resulted in a win
            if ((board[row][0] == board[row][1] && board[row][1] == board[row][2]) ||
                (board[0][col] == board[1][col] && board[1][col] == board[2][col]) ||
                (row == col && board[0][0] == board[1][1] && board[1][1] == board[2][2]) ||
                (row + col == 2 && board[0][2] == board[1][1] && board[1][1] == board[2][0]))
            {
                winner = currentPlayer;
            }
            currentPlayer = currentPlayer.OtherPlayer();
            return move;
        }

        public Player CurrentPlayer
        {
            get { return currentPlayer; }
        }

        public void UndoMove(object undoToken)
        {
            int move = (int)undoToken;
            int row = move / 3;
            int col = move % 3;
            board[row][col] = 0;
            winner = null;
            depth--;
            currentPlayer = currentPlayer.OtherPlayer();
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
