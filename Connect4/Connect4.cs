using System;
using System.Linq;
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

    public class Connect4 : IGame
    {
        public struct UndoToken
        {
            public ulong position;
            public ulong mask;
            public ulong movesCode;
        }

        public class NegamaxBrain : GenericNegamaxBrain
        {
            private int[] order
                = new[] { 5, 3, 1, 0, 2, 4, 6 };
            private const double MAX_SCORE
                = 4242; // (6 * 7) * 100 + (6 * 7)

            public NegamaxBrain(int maxDepth = int.MaxValue, ICache cache = null, IMoveCache moveCache = null, double maxScore = MAX_SCORE) 
                : base(maxDepth, cache, moveCache)
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
                    ulong otherPosition = game.position ^ game.mask;
                    int scoreP1 = CountOnes(ComputeWinningPositions(game.position, game.mask));
                    int scoreP2 = CountOnes(ComputeWinningPositions(otherPosition, game.mask));
                    int scorePairsP1 = CountOnes(ComputeWinningPositionsForPairs(game.position, game.mask));
                    int scorePairsP2 = CountOnes(ComputeWinningPositionsForPairs(otherPosition, game.mask));
                    int score = (scoreP1 * 100 + scorePairsP1) - (scoreP2 * 100 + scorePairsP2);
                    if (game.Color) { score = -score; }
                    return score;
                }
            }

            protected override IEnumerable<int> OrderedMoves(IGame game)
            {
                return game.AvailableMoves.OrderBy(m => order[m]);
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

        private const ulong bottomMask
            = 0b0000001_0000001_0000001_0000001_0000001_0000001_0000001ul;
        private const ulong boardMask
            = bottomMask * ((1ul << 6) - 1);

        private ulong position;
        private ulong mask;
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

        public IEnumerable<int> AvailableMoves
        {
            get
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
        }

        public object MakeMove(int move)
        {
            var undoToken = new UndoToken {
                position = position,
                mask = mask,
                movesCode = movesCode
            };
            mask |= mask + (1ul << (move * 7));
            position ^= mask;
            moves++;
            movesCode = (movesCode << 3) + (ulong)(move + 1);
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

        public ulong NodeCode()
        {
            return position + mask; 
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
