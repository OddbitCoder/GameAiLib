using System;
using System.Collections.Generic;

namespace GameAiLib
{
    public enum Player
    { 
        Player1, // maximizing player (computer)
        Player2
    }

    public interface IGameBoard
    {
        bool IsTerminal { get; }
        double Score { get; } // give score for Player 1 
        int[] AvailableMoves { get; }
        void MakeMove(int move, Player player);
        void UndoMove(int move, Player player);
        Player? Winner { get; }
    }

    public static class Game
    {
        public static Player OtherPlayer(this Player player)
        {
            return player == Player.Player1 ? Player.Player2 : Player.Player1;
        }

        public static void Play(IGameBoard board, bool playerStarts = false, int maxDepth = int.MaxValue)
        {
            Random rand = new Random();
            bool skipFirstMove = playerStarts;
            while (true)
            {
                if (!skipFirstMove)
                {
                    double bestScore = double.MinValue;
                    List<int> bestMoves = new List<int>();
                    foreach (int move in board.AvailableMoves)
                    {
                        board.MakeMove(move, Player.Player1);
#if MINIMAX
                        double score = board.Minimax(maxDepth);
#else
                        double score = board.AlphaBeta(maxDepth, double.MinValue, double.MaxValue);
#endif
                        if (score > bestScore) { bestScore = score; bestMoves.Clear(); }
                        if (score == bestScore) { bestMoves.Add(move); }
                        board.UndoMove(move, Player.Player1);
                    }
                    board.MakeMove(bestMoves[rand.Next(bestMoves.Count)], Player.Player1);
                    Console.WriteLine(board);
                    if (board.Winner == Player.Player1)
                    {
                        Console.WriteLine("I won.");
                        break;
                    }
                    else if (board.IsTerminal)
                    {
                        Console.WriteLine("It's a tie.");
                        break;
                    }
                }
                else
                {
                    skipFirstMove = false;
                    Console.WriteLine(board);
                }
                Console.Write("Your move? ");
                int playerMove = Convert.ToInt32(Console.ReadLine());
                board.MakeMove(playerMove, Player.Player2);
                if (board.Winner == Player.Player2) 
                {
                    Console.WriteLine(board); 
                    Console.WriteLine("You won."); 
                    break; 
                }
                else if (board.IsTerminal) 
                {
                    Console.WriteLine(board); 
                    Console.WriteLine("It's a tie."); 
                    break; 
                }
            } 
        }

        public static double Minimax(this IGameBoard node, int depth, Player player = Player.Player2)
        {
            if (depth == 0 || node.IsTerminal) { return node.Score; }
            double bestVal = player == Player.Player1 ? double.MinValue : double.MaxValue;
            foreach (int move in node.AvailableMoves)
            {
                node.MakeMove(move, player); 
                double val = Minimax(node, depth - 1, player.OtherPlayer());
                node.UndoMove(move, player);
                if (player == Player.Player1)
                {
                    if (val.CompareTo(bestVal) > 0) { bestVal = val; }
                }
                else
                {
                    if (val.CompareTo(bestVal) < 0) { bestVal = val; }
                }
            }
            return bestVal;
        }

        public static double AlphaBeta(this IGameBoard node, int depth, double alpha, double beta, Player player = Player.Player2)
        {
            if (depth == 0 || node.IsTerminal) { return node.Score; }
            if (player == Player.Player1)
            {
                double v = double.MinValue;
                foreach (int move in node.AvailableMoves)
                {
                    node.MakeMove(move, player); 
                    v = Math.Max(v, AlphaBeta(node, depth - 1, alpha, beta, Player.Player2));
                    node.UndoMove(move, player);
                    alpha = Math.Max(alpha, v);
                    if (beta <= alpha) { break; }
                }
                return v;
            }
            else
            {
                double v = double.MaxValue;
                foreach (int move in node.AvailableMoves)
                {
                    node.MakeMove(move, player); 
                    v = Math.Min(v, AlphaBeta(node, depth - 1, alpha, beta, Player.Player1));
                    node.UndoMove(move, player);
                    beta = Math.Min(beta, v);
                    if (beta <= alpha) { break; }
                }
                return v;
            }
        }
    }
}
