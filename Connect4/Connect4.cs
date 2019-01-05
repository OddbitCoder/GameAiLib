using System;
using System.Collections.Generic;

namespace GameAiLib
{
    public static class ToStringExtensions
    {
        public static string AsString(this ulong val)
        {
            var str = Convert.ToString((long)val, 2).PadLeft(49, '0');
            var final = "";
            for (int i = 0; i < 49; i += 7)
            {
                final += str.Substring(i, 7) + Environment.NewLine;
            }
            return final;
        }
    }

    public class Connect4 : IGame, ICacheable<ulong>
    {
        public struct UndoToken
        {
            public ulong position;
            public ulong mask;
            public ulong movesCode;
        }

        public class NegamaxBrain : GenericNegamaxBrain
        {
            private const double MAX_SCORE
                = 4242; // (6 * 7) * 100 + (6 * 7)
            private int[] cols
                = new[] { 3, 2, 4, 1, 5, 0, 6 };

            public NegamaxBrain(int maxDepth = int.MaxValue, ICache cache = null, IMoveCache moveCache = null, bool iterative = true) 
                : base(maxDepth, cache, moveCache, iterative, maxScore: MAX_SCORE)
            {
            }

            protected override double NegamaxEval(IGame _game)
            {
                var game = (Connect4)_game;
                if (game.IsWinningState)
                {
                    bool player1Wins = !game.Color;
                    return (player1Wins ? 1 : -1) * MAX_SCORE; 
                }
                else
                {
                    ulong positionOther = game.position ^ game.mask;
                    int scoreCurrent = CountOnes(ComputeWinningPositions(game.position, game.mask));
                    int scoreOther = CountOnes(ComputeWinningPositions(positionOther, game.mask));
                    int scorePairsCurrent = CountOnes(ComputeWinningPositionsForPairs(game.position, game.mask));
                    int scorePairsOther = CountOnes(ComputeWinningPositionsForPairs(positionOther, game.mask));
                    int score = (scoreCurrent * 100 + scorePairsCurrent) - (scoreOther * 100 + scorePairsOther);
                    if (game.Color) { score = -score; }
                    return score;
                }
            }

            private string ComputeWinningMove(ulong position, ulong mask)
            {
                ulong winMask = ComputeWinningPositions(position, mask);
                if ((winMask & 1) != 0) { return "0"; }
                ulong winMoves = ((winMask >> 1) & (mask | topMask)) << 1;
                if (winMoves != 0)
                {
                    for (int i = 0; i < 7; i++)
                    {
                        if ((winMoves & (sideMask << (7 * i))) != 0)
                        {
                            return i.ToString();
                        }
                    }
                }
                return null;
            }

            protected override IEnumerable<string> GetValidMovesOptimized(IGame _game)
            {
                var game = (Connect4)_game;
                // check if win is possible
                var winMove = ComputeWinningMove(game.position ^ game.mask, game.mask);
                if (winMove != null) { return new[] { winMove }; }
                // check if block is needed
                var blockMove = ComputeWinningMove(game.position, game.mask);
                if (blockMove != null) { return new[] { blockMove }; }
                var list = new List<string>(7);
                for (int i = 0; i < 7; i++)
                {
                    int col = cols[i];
                    if (((1ul << 5 << (col * 7)) & game.mask) == 0)
                    {
                        list.Add(col.ToString());
                    }
                }
                return list;
            }

            private ulong ComputeWinningPositionsForPairs(ulong position, ulong mask)
            {
                ulong nmask = ~mask & boardMask;

                // vertical
                ulong r = position & (position << 1); // two in a row?
                r = (r << 1) & nmask; // free cell above?
                r = (r << 1) & nmask; // another free cell above?
                r |= r >> 1;

                // horizontal
                // left
                ulong p = position & (position >> 7); // two in a row?
                p = (p >> 7) & nmask; // free cell to the left?
                p = (p >> 7) & nmask; // another free cell to the left?
                r |= p | (p << 7);
                // right
                p = position & (position << 7); // two in a row?
                p = (p << 7) & nmask; // free cell to the right?
                p = (p << 7) & nmask; // another free cell to the right?
                r |= p | (p >> 7);
                // left, right
                p = position & (position << 7); // two in a row?
                p = (p << 7) & nmask; // free cell to the right?
                p = (p >> 21) & nmask; // free cell to the left?
                r |= p | (p << 21);

                // diagonal 
                // left-up
                p = position & (position >> 6); 
                p = (p >> 6) & nmask; 
                p = (p >> 6) & nmask; 
                r |= p | (p << 6);
                // left-down
                p = position & (position >> 8); 
                p = (p >> 8) & nmask; 
                p = (p >> 8) & nmask; 
                r |= p | (p << 8);
                // right-up
                p = position & (position << 8); 
                p = (p << 8) & nmask; 
                p = (p << 8) & nmask; 
                r |= p | (p >> 8);
                // right-down
                p = position & (position << 6); 
                p = (p << 6) & nmask; 
                p = (p << 6) & nmask; 
                r |= p | (p >> 6);
                // right-up, left-down
                p = position & (position << 8); 
                p = (p << 8) & nmask;
                p = (p >> 24) & nmask;
                r |= p | (p << 24);
                // left-up, right-down
                p = position & (position >> 6);
                p = (p >> 6) & nmask;
                p = (p << 18) & nmask;
                r |= p | (p >> 18);

                return r;
            }

            private ulong ComputeWinningPositions(ulong position, ulong mask)
            {
                // vertical
                ulong r = (position << 1) & (position << 2) & (position << 3);

                // horizontal
                ulong p = (position << 7) & (position << 2 * 7);
                r |= p & (position << 3 * 7);
                r |= p & (position >> 7);
                p = (position >> 7) & (position >> 2 * 7);
                r |= p & (position << 7);
                r |= p & (position >> 3 * 7);

                // diagonal 1
                p = (position << 6) & (position << 2 * 6);
                r |= p & (position << 3 * 6);
                r |= p & (position >> 6);
                p = (position >> 6) & (position >> 2 * 6);
                r |= p & (position << 6);
                r |= p & (position >> 3 * 6);

                // diagonal 2
                p = (position << 8) & (position << 2 * 8);
                r |= p & (position << 3 * 8);
                r |= p & (position >> 8);
                p = (position >> 8) & (position >> 2 * 8);
                r |= p & (position << 8);
                r |= p & (position >> 3 * 8);

                return r & (boardMask ^ mask);
            }

            private int CountOnes(ulong m)
            {
                int c = 0;
                for (c = 0; m != 0; c++) { m &= m - 1; }
                return c;
            }
        }

        private const ulong topMask
            = 0b1000000_1000000_1000000_1000000_1000000_1000000_1000000ul;
        private const ulong bottomMask
            = 0b0000001_0000001_0000001_0000001_0000001_0000001_0000001ul;
        private const ulong sideMask
            = 0b0000000_0000000_0000000_0000000_0000000_0000000_0111111ul;
        private const ulong boardMask
            = bottomMask * ((1ul << 6) - 1);

        private ulong position; // position of the player that made the last move
        private ulong mask;     // position ^ mask: position of the player that is about to make a move
        private ulong movesCode;
        private int moves;

        public Connect4()
        {
        }

        public bool IsWinningState { get; private set; } 
            = false;

        private bool IsFull
        {
            get { return moves == 6 * 7; }
        }

        public bool Color { get; private set; } 
            = true;

        public bool IsTerminalState
        {
            get { return IsWinningState || IsFull; }
        }

        public ulong BoardCode
        {
            get
            {
                return position + mask;
            }
        }

        public IEnumerable<string> GetValidMoves()
        {
            var list = new List<string>(7);
            for (int col = 0; col < 7; col++)
            {
                if (((1ul << 5 << (col * 7)) & mask) == 0)
                {
                    list.Add(col.ToString());
                }
            }
            return list;
        }

        public object MakeMove(string move)
        {
            var undoToken = new UndoToken {
                position = position,
                mask = mask,
                movesCode = movesCode
            };
            int moveAsInt = move.FirstCharAsInt();
            mask |= mask + (1ul << (moveAsInt * 7));
            position ^= mask;
            moves++;
            movesCode = (movesCode << 3) + (ulong)(moveAsInt + 1);
            Color = !Color;
            // check if this resulted in a win
            // horizontal
            ulong m = position & (position >> 7);
            if ((m & (m >> 14)) != 0) { IsWinningState = true; return undoToken; }
            // diag 1
            m = position & (position >> 6);
            if ((m & (m >> 12)) != 0) { IsWinningState = true; return undoToken; }
            // diag 2
            m = position & (position >> 8);
            if ((m & (m >> 16)) != 0) { IsWinningState = true; return undoToken; }
            // vertical
            m = position & (position >> 1);
            if ((m & (m >> 2)) != 0) { IsWinningState = true; return undoToken; }
            return undoToken;
        }

        public void UndoMove(object _undoToken)
        {
            var undoToken = (UndoToken)_undoToken;
            position = undoToken.position;
            mask = undoToken.mask;
            movesCode = undoToken.movesCode;
            IsWinningState = false;
            moves--;
            Color = !Color;
        }

        public ulong MovesCode()
        {
            return movesCode;
        }

        public override string ToString()
        {
            // WARNME: this does not work correctly (swaps o and x every round)
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
