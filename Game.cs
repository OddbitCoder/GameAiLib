using System;
using System.Linq;
using System.Collections.Generic;

namespace GameAiLib
{
    public enum Player
    { 
        Player1, // maximizing player (computer)
        Player2
    }

    public interface IPlayer
    {
        int ChooseMove(IEnumerable<KeyValuePair<int, Score>> availableMoves, int currentDepth);
    }

    public class PerfectPlayer : IPlayer
    {
        private Random mRng
            = new Random();

        public int ChooseMove(IEnumerable<KeyValuePair<int, Score>> availableMoves, int currentDepth)
        {
            double bestScore = double.MinValue;
            List<int> bestMoves = new List<int>();
            foreach (KeyValuePair<int, Score> move in availableMoves)
            {
                Score score = move.Value;
                if (score.Val > bestScore) { bestScore = score.Val; bestMoves.Clear(); }
                if (score.Val == bestScore) { bestMoves.Add(move.Key); }
            }
            Console.WriteLine("Best moves: {0}", bestMoves.Select(x => x.ToString()).Aggregate((x, y) => x + ", " + y));
            return bestMoves[mRng.Next(bestMoves.Count)];
        }
    }

    public interface IGame
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
        public static void Play(IGame game, bool playerStarts, IPlayer playerAi, int maxDepth = int.MaxValue)
        {
            int depth = 0;
            bool skipFirstMove = playerStarts;
            while (true)
            {
                if (!skipFirstMove)
                {
                    List<KeyValuePair<int, Score>> moves = new List<KeyValuePair<int, Score>>();
                    foreach (int move in game.AvailableMoves)
                    {
                        game.MakeMove(move, Player.Player1);
                        Score score = game.AlphaBeta(depth + 1, maxDepth, double.MinValue, double.MaxValue);
                        moves.Add(new KeyValuePair<int, Score>(move, score));
                        game.UndoMove(move, Player.Player1);
                    }
                    Console.WriteLine("All moves: {0}", moves.Select(x => x.ToString()).Aggregate((x, y) => x + ", " + y));
                    game.MakeMove(playerAi.ChooseMove(moves, depth), Player.Player1);
                    depth++;
                    Console.WriteLine("Depth after move: {0}", depth);
                    Console.WriteLine(game);
                    if (game.Winner == Player.Player1)
                    {
                        Console.WriteLine("I won.");
                        break;
                    }
                    else if (game.IsTerminal)
                    {
                        Console.WriteLine("It's a tie.");
                        break;
                    }
                }
                else
                {
                    skipFirstMove = false;
                    Console.WriteLine(game);
                }
                Console.Write("Your move? ");
                int playerMove = Convert.ToInt32(Console.ReadLine());
                game.MakeMove(playerMove, Player.Player2);
                depth++;
                if (game.Winner == Player.Player2) 
                {
                    Console.WriteLine(game); 
                    Console.WriteLine("You won."); 
                    break; 
                }
                else if (game.IsTerminal) 
                {
                    Console.WriteLine(game); 
                    Console.WriteLine("It's a tie."); 
                    break; 
                }
            } 
        }

        public static Score AlphaBeta(this IGame node, int depth, int maxDepth, double alpha, double beta, Player player = Player.Player2)
        {
            if (maxDepth == 0 || node.IsTerminal) 
            { 
                return new Score { Val = node.Score, Depth = depth }; 
            }
            if (player == Player.Player1)
            {
                Score v = Score.MinValue;
                foreach (int move in node.AvailableMoves)
                {
                    node.MakeMove(move, player);
                    Score w = AlphaBeta(node, depth + 1, maxDepth - 1, alpha, beta, Player.Player2);
                    if (v.Val < w.Val) { v = w; }
                    else if (v.Val == w.Val && w.Depth < v.Depth) { v = w; }
                    node.UndoMove(move, player);
                    alpha = Math.Max(alpha, v.Val);
                    if (beta <= alpha) { break; }
                }
                return v;
            }
            else
            {
                Score v = Score.MaxValue;
                foreach (int move in node.AvailableMoves)
                {
                    node.MakeMove(move, player);
                    Score w = AlphaBeta(node, depth + 1, maxDepth - 1, alpha, beta, Player.Player1);
                    if (v.Val > w.Val) { v = w; }
                    else if (v.Val == w.Val && w.Depth < v.Depth) { v = w; }
                    node.UndoMove(move, player);
                    beta = Math.Min(beta, v.Val);
                    if (beta <= alpha) { break; }
                }
                return v;
            }
        }
    }
}
