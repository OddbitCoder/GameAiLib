using System;
using System.Linq;

namespace GameAiLib
{
    class TicTacToe : IGame
    {
        private byte[][] mBoard
            = new byte[3][];
        private Player? mWinner
            = null;
        private int mDepth
            = 0;

        public TicTacToe()
        {
            for (int row = 0; row < 3; row++)
            {
                mBoard[row] = new byte[3];
            }
        }

        public Player? Winner
        {
            get { return mWinner; }
        }

        private bool IsFull
        {
            get { return mDepth == 9; }
        }

        public bool IsTerminal
        {
            get { return mWinner != null || IsFull; }
        }

        public double Score(SkillLevel skillLevel)
        {
            return mWinner == null ? 0 : (mWinner == Player.Player1 ? 1 : -1);
        }

        public int[] AvailableMoves
        {
            get
            {
                int[] moves = new int[9 - mDepth];
                int offset = 0;
                int i = 0;
                foreach (byte[] row in mBoard)
                {
                    for (int col = 0; col < 3; col++)
                    {
                        if (row[col] == 0) { moves[i++] = offset + col; }
                    }
                    offset += 3;
                }
                return moves;
            }
        }

        public void MakeMove(int move, Player player)
        {
            int row = move / 3;
            int col = move % 3;
            mBoard[row][col] = (byte)(player == Player.Player1 ? 1 : 2);
            mDepth++;
            // check if this resulted in a win
            if ((mBoard[row][0] == mBoard[row][1] && mBoard[row][1] == mBoard[row][2]) ||
                (mBoard[0][col] == mBoard[1][col] && mBoard[1][col] == mBoard[2][col]) ||
                (row == col && mBoard[0][0] == mBoard[1][1] && mBoard[1][1] == mBoard[2][2]) ||
                (row + col == 2 && mBoard[0][2] == mBoard[1][1] && mBoard[1][1] == mBoard[2][0]))
            {
                mWinner = player;
            }
        }

        public double EvalComputerMoveShallow(int move, SkillLevel skillLevel)
        {
            int row = move / 3;
            int col = move % 3;
            try
            {
                // win if you can
                mBoard[row][col] = 1;
                if ((mBoard[row][0] == mBoard[row][1] && mBoard[row][1] == mBoard[row][2]) ||
                    (mBoard[0][col] == mBoard[1][col] && mBoard[1][col] == mBoard[2][col]) ||
                    (row == col && mBoard[0][0] == mBoard[1][1] && mBoard[1][1] == mBoard[2][2]) ||
                    (row + col == 2 && mBoard[0][2] == mBoard[1][1] && mBoard[1][1] == mBoard[2][0]))
                {
                    return 4;
                }
                // prevent opponent from winning
                mBoard[row][col] = 2;
                if ((mBoard[row][0] == mBoard[row][1] && mBoard[row][1] == mBoard[row][2]) ||
                    (mBoard[0][col] == mBoard[1][col] && mBoard[1][col] == mBoard[2][col]) ||
                    (row == col && mBoard[0][0] == mBoard[1][1] && mBoard[1][1] == mBoard[2][2]) ||
                    (row + col == 2 && mBoard[0][2] == mBoard[1][1] && mBoard[1][1] == mBoard[2][0]))
                {
                    return 3;
                }
                // two in a row?
                mBoard[row][col] = 1;
                if ((mBoard[row][0] + mBoard[row][1] + mBoard[row][2] == 2) ||
                    (mBoard[0][col] + mBoard[1][col] + mBoard[2][col] == 2) ||
                    (row == col && mBoard[0][0] + mBoard[1][1] + mBoard[2][2] == 2) ||
                    (row + col == 2 && mBoard[0][2] + mBoard[1][1] + mBoard[2][0] == 2))
                {
                    return 2;
                }
                // one in a row?
                if ((mBoard[row][0] + mBoard[row][1] + mBoard[row][2] == 1) ||
                    (mBoard[0][col] + mBoard[1][col] + mBoard[2][col] == 1) ||
                    (row == col && mBoard[0][0] + mBoard[1][1] + mBoard[2][2] == 1) ||
                    (row + col == 2 && mBoard[0][2] + mBoard[1][1] + mBoard[2][0] == 1))
                {
                    return 1;
                }
                return 0;
            }
            finally
            {
                mBoard[row][col] = 0;
            }
        }

        public void UndoMove(int move, Player player)
        {
            int row = move / 3;
            int col = move % 3;
            mBoard[row][col] = 0;
            mWinner = null;
            mDepth--;
        }

        public void SwapPlayers()
        {
            foreach (byte[] row in mBoard)
            {
                for (int col = 0; col < 3; col++)
                {
                    row[col] = row[col] == 1 ? (byte)2 : (row[col] == 2 ? (byte)1 : (byte)0);
                }
            }
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
}
