using System;
using System.Collections.Generic;

namespace GameAi
{
    public enum Player
    { 
        Player1, // maximizing player (computer)
        Player2
    }

    public interface IGameState
    {
        bool IsTerminal { get; }
        double Score { get; } // give score for Player 1 
        int[] AvailableMoves { get; }
        void MakeMove(int move, Player player);
        void UndoMove(int move, Player player);
        Player? Winner { get; }
    }

    public static class Optimizer
    {
        public static Player OtherPlayer(this Player player)
        {
            return player == Player.Player1 ? Player.Player2 : Player.Player1;
        }

        public static void Play(IGameState state, bool playerStarts = false, int maxDepth = int.MaxValue)
        {
            Random rand = new Random();
            bool skipFirstMove = playerStarts;
            while (true)
            {
                if (!skipFirstMove)
                {
                    double bestScore = double.MinValue;
                    List<int> bestMoves = new List<int>();
                    foreach (int move in state.AvailableMoves)
                    {
                        state.MakeMove(move, Player.Player1);
#if MINIMAX
                        double score = state.Minimax(maxDepth);
#else
                        double score = state.AlphaBeta(maxDepth, double.MinValue, double.MaxValue);
#endif
                        if (score > bestScore) { bestScore = score; bestMoves.Clear(); }
                        if (score == bestScore) { bestMoves.Add(move); }
                        state.UndoMove(move, Player.Player1);
                    }
                    state.MakeMove(bestMoves[rand.Next(bestMoves.Count)], Player.Player1);
                    Console.WriteLine(state);
                    if (state.Winner == Player.Player1)
                    {
                        Console.WriteLine("I won.");
                        break;
                    }
                    else if (state.IsTerminal)
                    {
                        Console.WriteLine("It's a tie.");
                        break;
                    }
                }
                else
                {
                    skipFirstMove = false;
                    Console.WriteLine(state);
                }
                Console.Write("Your move? ");
                int playerMove = Convert.ToInt32(Console.ReadLine());
                state.MakeMove(playerMove, Player.Player2);
                if (state.Winner == Player.Player2) 
                {
                    Console.WriteLine(state); 
                    Console.WriteLine("You won."); 
                    break; 
                }
                else if (state.IsTerminal) 
                {
                    Console.WriteLine(state); 
                    Console.WriteLine("It's a tie."); 
                    break; 
                }
            } 
        }

        public static double Minimax(this IGameState node, int depth, Player player = Player.Player2)
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

        public static double AlphaBeta(this IGameState node, int depth, double alpha, double beta, Player player = Player.Player2)
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
