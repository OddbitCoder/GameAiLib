using System;
using System.Collections.Generic;

namespace GameAiLib
{
    public static class SixMenMorrisExtensions // for debugging only
    {
        public static string AsString(this ushort val)
        {
            var str = Convert.ToString(val, 2).PadLeft(16, '0');
            return str.Substring(0, 8) + "\n" + str.Substring(8);
        }
    }

    public class SixMenMorris : IGame, ICacheable<ulong>
    {
        public struct UndoToken
        {
            public ushort playerMask;
            public ushort boardMask;
        }

        public class NegamaxBrain : GenericNegamaxBrain
        {
            private const double MAX_SCORE
                = 10000;

            public NegamaxBrain(int maxDepth = int.MaxValue, ICache cache = null, IMoveCache moveCache = null, bool iterative = true)
                : base(maxDepth, cache, moveCache, iterative, maxScore: MAX_SCORE)
            {
            }

            protected override double NegamaxEval(IGame _game)
            {
                var game = (SixMenMorris)_game;
                if (game.IsWinningState)
                {
                    return (game.Color ? -1 : 1) * MAX_SCORE;
                }
                else
                {
                    // TEMPLATE:
                    //ushort otherPlayerMask = (ushort)(game.playerMask ^ game.boardMask);
                    //int scorePlayer = f(game.playerMask);
                    //int scoreOpponent = f(otherPlayerMask);
                    //int score = scorePlayer - scoreOpponent;
                    //return game.Color ? -score : score;
                    ushort otherPlayerMask = (ushort)(game.playerMask ^ game.boardMask);
                    int scorePlayer = WeightedScore(
                        0, 
                        NumberOfMorrises(game.playerMask), 
                        NumberOfBlockedPieces(game.playerMask, otherPlayerMask), 
                        NumberOfPieces(game.playerMask),
                        NumberOfTwoPieceConfigs(game.playerMask, otherPlayerMask)
                    ); 
                    int scoreOpponent = WeightedScore(
                        0,
                        NumberOfMorrises(otherPlayerMask),
                        NumberOfBlockedPieces(otherPlayerMask, game.playerMask),
                        NumberOfPieces(otherPlayerMask),
                        NumberOfTwoPieceConfigs(otherPlayerMask, game.playerMask)
                    );
                    int score = scorePlayer - scoreOpponent;
                    return game.Color ? -score : score;
                }
            }

            private int NumberOfMorrises(ushort playerMask)
            {
                return Utils.CountOnes((ushort)(playerMask & Rotl(playerMask) & Rotr(playerMask) & ~diagMask));
            }

            private int NumberOfBlockedPieces(ushort playerMask, ushort otherPlayerMask)
            {
                return 0;
            }

            private int NumberOfPieces(ushort playerMask)
            {
                return Utils.CountOnes(playerMask);
            }

            private int NumberOfTwoPieceConfigs(ushort playerMask, ushort otherPlayerMask)
            {
                return Utils.CountOnes(GetMillMoveMask(playerMask, (ushort)(playerMask | otherPlayerMask)));
            }

            private int WeightedScore(
                int closedMorris, 
                int numberOfMorrises, 
                int numberOfBlockedPieces, 
                int numberOfPieces, 
                int numberOfTwoPieceConfigs
            )
            {
                // https://kartikkukreja.wordpress.com/2014/03/17/heuristicevaluation-function-for-nine-mens-morris/
                // Evaluation function for Phase 1 = 18 * (1) + 26 * (2) + 1 * (3) + 9 * (4) + 10 * (5) + 7 * (6)
                // Evaluation function for Phase 2 = 14 * (1) + 43 * (2) + 10 * (3) + 11 * (4) + 8 * (7) + 1086 * (8)
                // Evaluation function for Phase 3 = 16 * (1) + 10 * (5) + 1 * (6) + 1190 * (8)
                return 
                    18 * closedMorris + 
                    26 * numberOfMorrises + 
                    1 * numberOfBlockedPieces + 
                    9 * numberOfPieces + 
                    10 * numberOfTwoPieceConfigs;
            }
        }

        private ushort playerMask; // mask for the player that made the last move
        private ushort boardMask; // playerMask ^ boardMask: mask for the player that is about to make a move
        private int moves; // number of moves (depth)

        private const ushort diagMask
            = 0b01010101_01010101;

        public bool IsTerminalState
            => IsWinningState;

        public bool IsWinningState { get; private set; }
            = false;

        public bool Color { get; private set; } // color of the player that is about to make a move
            = true; // true = white = player that started the game

        public ulong BoardCode
        {
            get
            {
                return playerMask | ((ulong)boardMask << 16) | ((ulong)moves << 32);
            }
        }

        private static ushort Rotr(ushort val)
        {
            return (ushort)(((val & 0b11111110_11111111) >> 1) | ((val & 0b00000001_00000001) << 7));
        }

        private static ushort Rotl(ushort val)
        {
            return (ushort)(((val & 0b11111111_01111111) << 1) | ((val & 0b10000000_10000000) >> 7));
        }

        private static ushort Rotr2(ushort val)
        {
            return (ushort)(((val & 0b11111100_11111111) >> 2) | ((val & 0b00000011_00000011) << 6));
        }

        private static ushort Rotl2(ushort val)
        {
            return (ushort)(((val & 0b11111111_00111111) << 2) | ((val & 0b11000000_11000000) >> 6));
        }

        private static int PrevMove(int move)
        {
            move--;
            switch (move) // 8 -> 15; 0 -> 7
            {
                case 7:
                    return 15;
                case -1:
                    return 7;
                default:
                    return move;
            }
        }

        private static int NextMove(int move)
        {
            move++;
            switch (move) // 15 -> 8; 7 -> 0
            {
                case 16:
                    return 8;
                case 8:
                    return 0;
                default:
                    return move;
            }
        }

        private static ushort GetMillMoveMask(ushort playerMask, ushort boardMask)
        {
            // center piece missing
            ushort p = (ushort)(Rotl(playerMask) & Rotr(playerMask) & ~diagMask);
            // clockwise piece missing
            ushort r = (ushort)(Rotl(playerMask) & Rotl2(playerMask) & diagMask);
            p |= r;
            // counter-clockwise piece missing
            r = (ushort)(Rotr(playerMask) & Rotr2(playerMask) & diagMask); 
            p |= r;
            return (ushort)(p & ~boardMask);
        }

        private static ushort GetMillMask(ushort playerMask)
        {
            ushort p = (ushort)(playerMask & Rotl(playerMask) & Rotr(playerMask) & ~diagMask);
            return (ushort)(p | Rotr(p) | Rotl(p));
        }

        public IEnumerable<string> GetValidMoves()
        {
            ushort playerMask = (ushort)(this.playerMask ^ boardMask);
            ushort otherPlayerMask = this.playerMask;
            var validMoves = new List<string>(32);
            if (moves < 12) // PHASE 1
            {
                ushort millMoveMask = GetMillMoveMask(playerMask, boardMask);
                if (millMoveMask != 0)
                {
                    ushort millMask = GetMillMask(otherPlayerMask);
                    ushort rmvPieceMask = (ushort)(otherPlayerMask & ~millMask);
                    if (rmvPieceMask == 0) { rmvPieceMask = otherPlayerMask; }
                    // moves that form a mill and remove one opponent's piece
                    // TODO: moves that occupy corners first?
                    for (int i = 0; i < 16; i++)
                    {
                        if ((millMoveMask & (1 << i)) != 0)
                        {
                            // TODO: rmv pieces that occupy corners first?
                            for (int j = 0; j < 16; j++)
                            {
                                if ((rmvPieceMask & (1 << j)) != 0)
                                {
                                    validMoves.Add(i.MoveStr() + j.MoveStr());
                                }
                            }
                        }
                    }
                }
                // other moves
                // TODO: moves that occupy corners first?
                ushort otherMoveMask = (ushort)(~boardMask & ~millMoveMask);
                for (int i = 0; i < 16; i++)
                {
                    if ((otherMoveMask & (1 << i)) != 0)
                    {
                        validMoves.Add(i.MoveStr());
                    }
                }
            }
            else // PHASE 2
            {
                ushort millMask = GetMillMask(otherPlayerMask);
                ushort rmvPieceMask = (ushort)(otherPlayerMask & ~millMask);
                if (rmvPieceMask == 0) { rmvPieceMask = otherPlayerMask; }
                ushort x = (ushort)(Rotr(playerMask) & Rotl(playerMask) & ~boardMask);
                ushort cwMillMoveMask = (ushort)(x & Rotr2(playerMask) & diagMask);
                // moves that form a mill
                // clockwise
                if (cwMillMoveMask != 0)
                {
                    for (int i = 0; i < 16; i++)
                    {
                        if ((cwMillMoveMask & (1 << i)) != 0)
                        {
                            for (int j = 0; j < 16; j++)
                            {
                                if ((rmvPieceMask & (1 << j)) != 0)
                                {
                                    validMoves.Add(PrevMove(i).MoveStr() + i.MoveStr() + j.MoveStr());
                                }
                            }
                        }
                    }
                }
                // counter-clockwise
                ushort ccwMillMoveMask = (ushort)(x & Rotl2(playerMask) & diagMask);
                if (ccwMillMoveMask != 0)
                {
                    for (int i = 0; i < 16; i++)
                    {
                        if ((ccwMillMoveMask & (1 << i)) != 0)
                        {
                            for (int j = 0; j < 16; j++)
                            {
                                if ((rmvPieceMask & (1 << j)) != 0)
                                {
                                    validMoves.Add(NextMove(i).MoveStr() + i.MoveStr() + j.MoveStr());
                                }
                            }
                        }
                    }
                }
                // inwards
                ushort inMillMoveMask = (ushort)(x & (playerMask >> 8) & ~diagMask);
                if (inMillMoveMask != 0)
                {
                    for (int i = 1; i < 8; i += 2)
                    {
                        if ((inMillMoveMask & (1 << i)) != 0)
                        {
                            for (int j = 0; j < 16; j++)
                            {
                                if ((rmvPieceMask & (1 << j)) != 0)
                                {
                                    validMoves.Add((i + 8).MoveStr() + i.MoveStr() + j.MoveStr());
                                }
                            }
                        }
                    }
                }
                // outwards
                ushort outMillMoveMask = (ushort)(x & (playerMask << 8) & ~diagMask);
                if (outMillMoveMask != 0)
                {
                    for (int i = 9; i < 16; i += 2)
                    {
                        if ((outMillMoveMask & (1 << i)) != 0)
                        {
                            for (int j = 0; j < 16; j++)
                            {
                                if ((rmvPieceMask & (1 << j)) != 0)
                                {
                                    validMoves.Add((i - 8).MoveStr() + i.MoveStr() + j.MoveStr());
                                }
                            }
                        }
                    }
                }
                // other moves
                // clockwise
                // TODO: moves that occupy corner first?
                ushort cwSlideMoveMask = (ushort)(Rotl(playerMask) & ~boardMask & ~cwMillMoveMask);
                if (cwSlideMoveMask != 0)
                {
                    for (int i = 0; i < 16; i++)
                    {
                        if ((cwSlideMoveMask & (1 << i)) != 0)
                        {
                            validMoves.Add(PrevMove(i).MoveStr() + i.MoveStr());
                        }
                    }
                }
                // counter-clockwise
                // TODO: moves that occupy corner first?
                ushort ccwSlideMoveMask = (ushort)(Rotr(playerMask) & ~boardMask & ~ccwMillMoveMask);
                if (ccwSlideMoveMask != 0)
                {
                    for (int i = 0; i < 16; i++)
                    {
                        if ((ccwSlideMoveMask & (1 << i)) != 0)
                        {
                            validMoves.Add(NextMove(i).MoveStr() + i.MoveStr());
                        }
                    }
                }
                // inwards
                ushort inSlideMoveMask = (ushort)((playerMask >> 8) & ~boardMask & ~inMillMoveMask & ~diagMask);
                if (inSlideMoveMask != 0)
                {
                    for (int i = 1; i < 8; i += 2)
                    {
                        if ((inSlideMoveMask & (1 << i)) != 0)
                        {
                            validMoves.Add((i + 8).MoveStr() + i.MoveStr());
                        }
                    }
                }
                // outwards
                ushort outSlideMoveMask = (ushort)((playerMask << 8) & ~boardMask & ~outMillMoveMask & ~diagMask);
                if (outSlideMoveMask != 0)
                {
                    for (int i = 9; i < 16; i += 2)
                    {
                        if ((outSlideMoveMask & (1 << i)) != 0)
                        {
                            validMoves.Add((i - 8).MoveStr() + i.MoveStr());
                        }
                    }
                }
            }
            return validMoves;
        }

        public object MakeMove(string move)
        {
            var undoToken = new UndoToken {
                boardMask = boardMask,
                playerMask = playerMask
            };
            if (moves < 12) // PHASE 1
            {
                boardMask |= (ushort)(1 << (move.MoveIdx(0)));
                playerMask ^= boardMask;
                if (move.Length > 1)
                {
                    // remove opponent's piece
                    boardMask &= (ushort)~(1 << (move.MoveIdx(1))); 
                }
            }
            else // PHASE 2
            {
                boardMask &= (ushort)~(1 << (move.MoveIdx(0)));
                boardMask |= (ushort)(1 << (move.MoveIdx(1)));
                playerMask ^= boardMask;
                if (move.Length > 2)
                {
                    // remove opponent's piece
                    boardMask &= (ushort)~(1 << (move.MoveIdx(2)));
                    // check if down to 2 pieces
                    IsWinningState = Utils.CountOnes((ushort)(playerMask ^ boardMask)) <= 2;
                }
            }
            Color = !Color;
            moves++;
            return undoToken;
        }

        public void UndoMove(object _undoToken)
        {
            var undoToken = (UndoToken)_undoToken;
            boardMask = undoToken.boardMask;
            playerMask = undoToken.playerMask;
            Color = !Color;
            moves--;
            IsWinningState = false;
        }

        public override string ToString()
        {
            var board  = "i-------j-------k   I-------J-------K\n";
                board += "|       |       |   |       |       |\n";
                board += "|   a---b---c   |   |   A---B---C   |\n";
                board += "|   |       |   |   |   |       |   |\n";
                board += "p---h       d---l   P---H       D---L\n";
                board += "|   |       |   |   |   |       |   |\n";
                board += "|   g---f---e   |   |   G---F---E   |\n";
                board += "|       |       |   |       |       |\n";
                board += "o-------n-------m   O-------N-------M";
            // player that made the last move (his color is '!Color', his position is 'position')
            char piece = !Color ? 'w' : 'x';
            for (int i = 0; i < 16; i++)
            {
                if ((playerMask & (1 << i)) != 0)
                {
                    board = board.Replace((char)(i + 'a'), piece);
                }
            }
            // player that is about to make a move (his color is 'Color', his position is 'position ^ mask')
            piece = Color ? 'w' : 'x';
            ushort posOther = (ushort)(playerMask ^ boardMask);
            for (int i = 0; i < 16; i++)
            {
                if ((posOther & (1 << i)) != 0)
                {
                    board = board.Replace((char)(i + 'a'), piece);
                }
            }
            // empty places
            for (int i = 0; i < 16; i++)
            {
                if ((boardMask & (1 << i)) == 0)
                {
                    board = board.Replace((char)(i + 'a'), ' ');
                }
            }
            return board.Replace('w', 'o');
        }
    }
}
