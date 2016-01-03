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

        private static readonly uint[] mBits
            = new uint[] { 1, 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192, 16384, 32768, 65536, 131072 };

        private static readonly int[][] mRotIdxMap = new int[][] { 
            new[] { 2, 0 }, new[] { 5, 1 }, new[] { 8, 2 },
            new[] { 7, 5 }, new[] { 6, 8 },
            new[] { 3, 7 }
        };

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
        
        private uint BoardState()
        {
            uint state = 0;
            int bit = 0;
            foreach (byte[] row in mBoard)
            {
                foreach (byte cell in row)
                {
                    if (cell == 1) { state += mBits[bit]; }
                    else if (cell == 2) { state += mBits[bit + 9]; }
                    bit++;
                }
            }
            return state;
        }

        private void Rotate()
        {
            byte[] tmp = new byte[] { mBoard[0][0], mBoard[0][1], mBoard[0][2] };
            foreach (int[] idxPair in mRotIdxMap)
            {
                mBoard[idxPair[1] / 3][idxPair[1] % 3] = mBoard[idxPair[0] / 3][idxPair[0] % 3];
            }
            mBoard[2][0] = tmp[0];
            // TODO: roll-out the following
            int i = 0;
            foreach (int idx in new[] { 6, 3, 0 })
            {
                mBoard[idx / 3][idx % 3] = tmp[i++];
            }
        }

        private void Mirror()
        {
            byte[] tmp = mBoard[0];
            mBoard[0] = mBoard[2];
            mBoard[2] = tmp;
        }

        public object State
        {
            get
            {
                ulong minState = ulong.MaxValue;
                for (int j = 0; j < 2; j++)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        Rotate();
                        ulong state = BoardState();
                        if (state < minState) { minState = state; }
                    }
                    Mirror();
                }
                return minState;
            }
        }
    }
}
