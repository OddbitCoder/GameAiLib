using System;
using System.Collections.Generic;

namespace GameAiLib
{
    public static class SixMenMorrisExtensions
    {
        public static string AsString(this ushort val)
        {
            var str = Convert.ToString(val, 2).PadLeft(16, '0');
            return str.Substring(0, 8) + "\n" + str.Substring(8);
        }

        public static int MoveIdx(this string move, int idx)
        {
            return move[idx] - 'A';
        }

        public static string MoveStr(this int move)
        {
            return ((char)(move + 'A')).ToString();
        }

        public static int PrevMove(this int move)
        {
            // 8 -> 15; 0 -> 7
            move--;
            if (move == 7) { move = 15; }
            else if (move == -1) { move = 7; }
            return move;
        }

        public static int NextMove(this int move)
        {
            // 15 -> 8; 7 -> 0
            move++;
            if (move == 16) { move = 8; }
            else if (move == 8) { move = 0; }
            return move;
        }
    }

    public class SixMenMorris : IGame
    {
        public struct UndoToken
        {
            public ushort position;
            public ushort mask;
        }

        private ushort position; // position of the player that made the last move
        private ushort mask; // position ^ mask: position of the player that is about to make a move
        private int moves; // number of moves (game depth)

        private const ushort diagMask
            = 0b01010101_01010101;

        public bool IsTerminalState 
            => moves == 12;

        public bool IsWinningState { get; private set; }
            = false;

        public bool Color { get; private set; } // color of the player that is about to make a move
            = true; // true = white, player that started the game

        private ushort Rotr(ushort pos)
        {
            return (ushort)(((pos & 0b11111110_11111111) >> 1) | ((pos & 0b00000001_00000001) << 7));
        }

        private ushort Rotl(ushort pos)
        {
            return (ushort)(((pos & 0b11111111_01111111) << 1) | ((pos & 0b10000000_10000000) >> 7));
        }

        private ushort GetMillPos(ushort pos, ushort mask)
        {
            // center piece missing
            ushort p = (ushort)(Rotr((ushort)(pos & Rotl(Rotl(pos)))) & ~diagMask);
            // clockwise piece missing
            ushort r = (ushort)(Rotl((ushort)(pos & Rotl(pos))) & diagMask);
            p |= r;
            // counter-clockwise piece missing
            r = (ushort)(Rotr((ushort)(pos & Rotr(pos))) & diagMask);
            p |= r;
            return (ushort)(p & ~mask);
        }

        private ushort GetMillsMask(ushort pos)
        {
            ushort p = (ushort)(pos & Rotl(pos) & Rotr(pos) & ~diagMask);
            return (ushort)(p | Rotr(p) | Rotl(p));
        }

        public IEnumerable<string> GetValidMoves()
        {
            ushort pos = (ushort)(position ^ mask);
            ushort posOther = position;
            var validMoves = new List<string>(32);
            if (moves < 12) // PHASE 1
            {
                ushort millPos = GetMillPos(pos, mask);
                if (millPos != 0)
                {
                    ushort millsMask = GetMillsMask(posOther);
                    ushort rmvMask = (ushort)(posOther & ~millsMask);
                    if (rmvMask == 0) { rmvMask = posOther; }
                    // moves that form a mill and remove one opponent's piece
                    // TODO: moves that occupy corners first?
                    for (int i = 0; i < 16; i++)
                    {
                        if ((millPos & (1 << i)) != 0)
                        {
                            // TODO: rmv pieces that occupy corners first?
                            for (int j = 0; j < 16; j++)
                            {
                                if ((rmvMask & (1 << j)) != 0)
                                {
                                    validMoves.Add(i.MoveStr() + j.MoveStr());
                                }
                            }
                        }
                    }
                }
                // other moves
                // TODO: moves that occupy corners first?
                ushort otherMoves = (ushort)(~mask & ~millPos);
                for (int i = 0; i < 16; i++)
                {
                    if ((otherMoves & (1 << i)) != 0)
                    {
                        validMoves.Add(i.MoveStr());
                    }
                }
            }
            else // PHASE 2
            {
                ushort x = (ushort)(Rotr(pos) & Rotl(pos) & ~mask);
                ushort cwMillPos = (ushort)(x & Rotr(Rotr(pos)) & diagMask);
                ushort ccwMillPos = (ushort)(x & Rotl(Rotl(pos)) & diagMask);
                ushort inMillPos = (ushort)(x & (pos >> 8) & ~diagMask);
                ushort outMillPos = (ushort)(x & (pos << 8) & ~diagMask);
                ushort rmvMask = 0;
                if ((ccwMillPos | cwMillPos | inMillPos | outMillPos) != 0)
                {
                    ushort millsMask = GetMillsMask(posOther);
                    rmvMask = (ushort)(posOther & ~millsMask);
                    if (rmvMask == 0) { rmvMask = posOther; }
                }
                if (cwMillPos != 0)
                {
                    for (int i = 0; i < 16; i++)
                    {
                        if ((cwMillPos & (1 << i)) != 0)
                        {
                            for (int j = 0; j < 16; j++)
                            {
                                if ((rmvMask & (1 << j)) != 0)
                                {
                                    validMoves.Add(i.PrevMove().MoveStr() + i.MoveStr() + j.MoveStr());
                                }
                            }
                        }
                    }
                }
                if (ccwMillPos != 0)
                {
                    for (int i = 0; i < 16; i++)
                    {
                        if ((ccwMillPos & (1 << i)) != 0)
                        {
                            for (int j = 0; j < 16; j++)
                            {
                                if ((rmvMask & (1 << j)) != 0)
                                {
                                    validMoves.Add(i.NextMove().MoveStr() + i.MoveStr() + j.MoveStr());
                                }
                            }
                        }
                    }
                }
                if (inMillPos != 0)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        if ((inMillPos & (1 << i)) != 0)
                        {
                            for (int j = 0; j < 16; j++)
                            {
                                if ((rmvMask & (1 << j)) != 0)
                                {
                                    validMoves.Add((i + 8).MoveStr() + i.MoveStr() + j.MoveStr());
                                }
                            }
                        }
                    }
                }
                if (outMillPos != 0)
                {
                    for (int i = 8; i < 16; i++)
                    {
                        if ((outMillPos & (1 << i)) != 0)
                        {
                            for (int j = 0; j < 16; j++)
                            {
                                if ((rmvMask & (1 << j)) != 0)
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
                ushort cwSlidePos = (ushort)(Rotl(pos) & ~mask & ~cwMillPos);
                if (cwSlidePos != 0)
                {
                    for (int i = 0; i < 16; i++)
                    {
                        if ((cwSlidePos & (1 << i)) != 0)
                        {
                            validMoves.Add(i.PrevMove().MoveStr() + i.MoveStr());
                        }
                    }
                }
                // counter-clockwise
                // TODO: moves that occupy corner first?
                ushort ccwSlidePos = (ushort)(Rotr(pos) & ~mask & ~ccwMillPos);
                if (ccwSlidePos != 0)
                {
                    for (int i = 0; i < 16; i++)
                    {
                        if ((ccwSlidePos & (1 << i)) != 0)
                        {
                            validMoves.Add(i.NextMove().MoveStr() + i.MoveStr());
                        }
                    }
                }
                // inwards
                ushort inSlidePos = (ushort)((pos >> 8) & ~mask & ~inMillPos & ~diagMask);
                if (inSlidePos != 0)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        if ((inSlidePos & (1 << i)) != 0)
                        {
                            validMoves.Add((i + 8).MoveStr() + i.MoveStr());
                        }
                    }
                }
                // outwards
                ushort outSlidePos = (ushort)((pos << 8) & ~mask & ~outMillPos & ~diagMask);
                if (outSlidePos != 0)
                {
                    for (int i = 8; i < 16; i++)
                    {
                        if ((outSlidePos & (1 << i)) != 0)
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
                mask = mask,
                position = position
            };
            mask |= (ushort)(1 << (move.MoveIdx(0)));
            position ^= mask;
            if (move.Length > 1) 
            {
                mask &= (ushort)~(1 << (move.MoveIdx(1))); // remove opponent's piece
            }
            Color = !Color;
            moves++;
            // TODO: check if win
            return undoToken;
        }

        public void UndoMove(object _undoToken)
        {
            var undoToken = (UndoToken)_undoToken;
            mask = undoToken.mask;
            position = undoToken.position;
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
                if ((position & (1 << i)) != 0)
                {
                    board = board.Replace((char)(i + 'a'), piece);
                }
            }
            // player that is about to make a move (his color is 'Color', his position is 'position ^ mask')
            piece = Color ? 'w' : 'x';
            ushort posOther = (ushort)(position ^ mask);
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
                if ((mask & (1 << i)) == 0)
                {
                    board = board.Replace((char)(i + 'a'), ' ');
                }
            }
            return board.Replace('w', 'o');
        }
    }
}
