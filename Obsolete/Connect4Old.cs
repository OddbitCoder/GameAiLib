using System;
using System.Linq;
using System.Collections.Generic;

namespace GameAiLib
{
    public class Connect4Old : IGameOld
    {
        private const ulong bottomMask
            = 0b0000001_0000001_0000001_0000001_0000001_0000001_0000001ul;
        private const ulong boardMask
            = bottomMask * ((1ul << 6) - 1);

        public struct UndoToken
        {
            public ulong position;
            public ulong mask;
        }

        //public class Cache : ICache
        //{
        //    private const int nBitsPages
        //        = 5;
        //    private const int nBitsItems
        //        = 26;
        //    private ulong nItems;
        //    private int nPages;

        //    private struct CacheItem
        //    {
        //        public ulong key;
        //        public sbyte val;
        //    }

        //    private CacheItem[][] items;

        //    public Cache()
        //    {
        //        nPages = 1 << nBitsPages;
        //        nItems = 1ul << nBitsItems;
        //        items = new CacheItem[nPages][];
        //        for (int i = 0; i < nPages; i++)
        //        {
        //            items[i] = new CacheItem[nItems];
        //            //for (ulong j = 0; j < nItems; j++)
        //            //{
        //            //    items[i][j] = new CacheItem();
        //            //}
        //        }
        //    }

        //    public bool GetBoundMax(IGame game, out double val, Player player)
        //    {
        //        ulong boardKey = ((Connect4)game).BoardKey(player);
        //        ulong itemIdx = boardKey << (64 - nBitsItems) >> (64 - nBitsItems);
        //        ulong pageIdx = boardKey << (64 - nBitsItems - nBitsPages) >> (64 - nBitsPages);
        //        val = items[pageIdx][itemIdx].val;
        //        return items[pageIdx][itemIdx].key == boardKey;
        //    }

        //    public bool GetBoundMin(IGame game, out double val, Player player)
        //    {
        //        val = 0;
        //        return false;
        //    }

        //    public void PutBoundMax(IGame game, double val, Player player)
        //    {
        //        ulong boardKey = ((Connect4)game).BoardKey(player);
        //        ulong itemIdx = boardKey << (64 - nBitsItems) >> (64 - nBitsItems);
        //        ulong pageIdx = boardKey << (64 - nBitsItems - nBitsPages) >> (64 - nBitsPages);
        //        items[pageIdx][itemIdx] = new CacheItem {
        //            val = (sbyte)val,
        //            key = boardKey
        //        };
        //    }

        //    public void PutBoundMin(IGame game, double val, Player player)
        //    {
        //        //ulong boardKey = ((Connect4)game).BoardKey();
        //        //ulong itemIdx = boardKey << (64 - nBitsItems) >> (64 - nBitsItems);
        //        //ulong pageIdx = boardKey << (64 - nBitsItems - nBitsPages) >> (64 - nBitsPages);
        //        //var item = items[pageIdx][itemIdx];
        //        //items[pageIdx][itemIdx] = new CacheItem {
        //        //    val = val,
        //        //    key = boardKey,
        //        //    max = false
        //        //};
        //    }
        //}

        public class MinimaxBrain : GenericMinimaxBrain
        {
            public MinimaxBrain(int maxDepth = int.MaxValue) : base(maxDepth)
            {
            }

            protected override double MinimaxEval(IGameOld _game, Player player)
            {
                var game = (Connect4Old)_game;
                if (game.Winner == null)
                {
                    int scoreP1 = PopCount(ComputeWinningPositions(game.position, game.mask));
                    int scoreP2 = PopCount(ComputeWinningPositions(game.position ^ game.mask, game.mask));
                    return player == Player.Player1 ? (scoreP1 - scoreP2) : (scoreP2 - scoreP1);
                }
                if (game.Winner == player)
                {
                    return 6 * 7;
                }
                else
                {
                    return -6 * 7;
                }
            }

            private int[] order
                = new[] { 5, 3, 1, 0, 2, 4, 6 };

            protected override IEnumerable<int> OrderedMoves(IGameOld game, Player player)
            {
                return game.AvailableMoves(player).OrderBy(m => order[m]);
            }

            private ulong ComputeWinningPositions(ulong position, ulong mask)
            {
                const int HEIGHT = 6;

                // vertical
                ulong r = (position << 1) & (position << 2) & (position << 3);

                // horizontal
                ulong p = (position << (HEIGHT + 1)) & (position << 2 * (HEIGHT + 1));
                r |= p & (position << 3 * (HEIGHT + 1));
                r |= p & (position >> (HEIGHT + 1));
                p = (position >> (HEIGHT + 1)) & (position >> 2 * (HEIGHT + 1));
                r |= p & (position << (HEIGHT + 1));
                r |= p & (position >> 3 * (HEIGHT + 1));

                // diagonal 1
                p = (position << HEIGHT) & (position << 2 * HEIGHT);
                r |= p & (position << 3 * HEIGHT);
                r |= p & (position >> HEIGHT);
                p = (position >> HEIGHT) & (position >> 2 * HEIGHT);
                r |= p & (position << HEIGHT);
                r |= p & (position >> 3 * HEIGHT);

                // diagonal 2
                p = (position << (HEIGHT + 2)) & (position << 2 * (HEIGHT + 2));
                r |= p & (position << 3 * (HEIGHT + 2));
                r |= p & (position >> (HEIGHT + 2));
                p = (position >> (HEIGHT + 2)) & (position >> 2 * (HEIGHT + 2));
                r |= p & (position << (HEIGHT + 2));
                r |= p & (position >> 3 * (HEIGHT + 2));

                return r & (boardMask ^ mask);
            }

            private int PopCount(ulong m)
            {
                int c = 0;
                for (c = 0; m != 0; c++) { m &= m - 1; }
                return c;
            }
        }

        public ulong position;
        public ulong mask;
        private int moves;
        private Player? winner;

        public Player? Winner
        {
            get { return winner; }
        }

        private bool IsFull
        {
            get { return moves == 6 * 7; }
        }

        public bool IsTerminalState
        {
            get { return winner != null || IsFull; }
        }

        public IEnumerable<int> AvailableMoves(Player player)
        {
            var list = new List<int>(7);
            for (int col = 0; col < 7; col++)
            {
                if (((1ul << 5 << (col * 7)) & mask) == 0)
                {
                    list.Add(col);
                }
            }
            return list;
        }

        public object MakeMove(int move, Player player)
        {
            var undoToken = new UndoToken {
                position = position,
                mask = mask
            };
            ulong pos;
            if (player == Player.Player1)
            {
                position ^= mask;
                mask |= mask + (1ul << (move * 7));
                position ^= mask;
                pos = position;
            }
            else
            {
                mask |= mask + (1ul << (move * 7));
                pos = position ^ mask;
            }
            moves++;
            // check if this resulted in a win
            // horizontal
            ulong m = pos & (pos >> 7);
            if ((m & (m >> 14)) != 0) { winner = player; }
            // diag 1
            m = pos & (pos >> 6);
            if ((m & (m >> 12)) != 0) { winner = player; }
            // diag 2
            m = pos & (pos >> 8);
            if ((m & (m >> 16)) != 0) { winner = player; }
            // vertical
            m = pos & (pos >> 1);
            if ((m & (m >> 2)) != 0) { winner = player; }
            return undoToken;
        }

        public void UndoMove(int move, Player player, object _undoToken)
        {
            var undoToken = (UndoToken)_undoToken;
            position = undoToken.position;
            mask = undoToken.mask;
            winner = null;
            moves--;
        }

        public bool CheckIntegrity()
        {
            var boardStr = ToString();
            int countPlayer1 = 0, countPlayer2 = 0;
            foreach (var ch in boardStr)
            {
                switch (ch)
                {
                    case 'o': countPlayer1++;
                        break;
                    case 'x': countPlayer2++;
                        break;
                }
            }
            return Math.Abs(countPlayer1 - countPlayer2) <= 1 && moves == countPlayer1 + countPlayer2;
        }

        public override string ToString()
        {
            var posPlayer1 = Convert.ToString((long)position, 2).PadLeft(49, '0');
            var posPlayer2 = Convert.ToString((long)(position ^ mask), 2).PadLeft(49, '0');
            var board = new char[49, 49];
            for (int i = 0, j = 48; i < 49; i++, j--)
            {
                int row = i % 7;
                int col = i / 7;
                board[row, col] = posPlayer1[j] == '1' ? 'o' : (posPlayer2[j] == '1' ? 'x' : '·');
            }
            var boardStr = "";
            for (int row = 5; row >= 0; row--)
            {
                for (int col = 0; col < 7; col++)
                {
                    boardStr += board[row, col] + " ";
                }
                boardStr += Environment.NewLine;
            }
            return boardStr + "0 1 2 3 4 5 6";
        }
    }
}
