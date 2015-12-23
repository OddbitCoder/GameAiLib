using System;
using System.Linq;
using System.Collections.Generic;

namespace GameAiLib
{
    public static class Game
    {
        public static void Play(IGame game, bool playerStarts, int maxDepth, int minDepth = 2, double difficultyLevel = 1)
        {
            Random rng = new Random();
            bool skipFirstMove = playerStarts;
            while (true)
            {
                if (!skipFirstMove)
                {
                    List<KeyValuePair<int, double>> moves = new List<KeyValuePair<int, double>>();
                    double bestScore = double.MinValue;
                    double bestShallowScore = double.MinValue;
                    List<int> bestMoves = new List<int>();
                    foreach (int mv in game.AvailableMoves)
                    {
                        double shallowScore = game.EvalComputerMoveShallow(mv);
                        game.MakeMove(mv, Player.Player1);
                        int depth = minDepth;
                        for (int i = minDepth; i < maxDepth; i++)
                        {
                            if (rng.NextDouble() < 1.0 - difficultyLevel) { break; }
                            depth++;
                        }
                        double score = game.AlphaBeta(depth, double.MinValue, double.MaxValue);
                        Console.WriteLine("Move: {0} | Depth: {1} | Score: {2} | Shallow score: {3}", mv, depth, score, shallowScore);
                        if (score > bestScore || (score == bestScore && shallowScore > bestShallowScore)) 
                        { 
                            bestScore = score; 
                            bestShallowScore = shallowScore; 
                            bestMoves.Clear(); 
                        }
                        if (score == bestScore && shallowScore == bestShallowScore) 
                        { 
                            bestMoves.Add(mv); 
                        }
                        game.UndoMove(mv, Player.Player1);
                    }
                    Console.WriteLine("Best moves: {0}", bestMoves.Select(x => x.ToString()).Aggregate((x, y) => x + ", " + y));
                    int move = bestMoves[rng.Next(bestMoves.Count)];
                    Console.WriteLine("My move: {0}", move);
                    game.MakeMove(move, Player.Player1); 
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

        public static double AlphaBeta(this IGame node, int maxDepth, double alpha, double beta, Player player = Player.Player2)
        {
            if (maxDepth == 0 || node.IsTerminal) { return node.Score; }
            if (player == Player.Player1)
            {
                double v = double.MinValue;
                foreach (int move in node.AvailableMoves)
                {
                    node.MakeMove(move, player);
                    v = Math.Max(v, AlphaBeta(node, maxDepth - 1, alpha, beta, Player.Player2));
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
                    v = Math.Min(v, AlphaBeta(node, maxDepth - 1, alpha, beta, Player.Player1));
                    node.UndoMove(move, player);
                    beta = Math.Min(beta, v);
                    if (beta <= alpha) { break; }
                }
                return v;
            }
        }
    }
}
