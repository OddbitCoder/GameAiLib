using System;
using System.Linq;
using System.Collections.Generic;

namespace GameAiLib
{
    public static class Game
    {
        public static void Compare(Func<IGame> createGame, int numGames, bool player1Starts,
            int minDepthPlayer1, int maxDepthPlayer1, double deepeningProbabilityPlayer1, SkillLevel skillLevelPlayer1,
            int minDepthPlayer2, int maxDepthPlayer2, double deepeningProbabilityPlayer2, SkillLevel skillLevelPlayer2)
        {
            IGame game;
            Random rng = new Random();
            bool skipFirstMove = !player1Starts;
            int player1Wins = 0;
            int player2Wins = 0;
            int ties = 0;
            int move;
            for (int gameNumber = 0; gameNumber < numGames; gameNumber++)
            {
                //Console.WriteLine("=== Game {0} ===", gameNumber + 1);
                game = createGame();
                while (true)
                {
                    double bestScore = double.MinValue;
                    double bestShallowScore = double.MinValue;
                    List<int> bestMoves = new List<int>();
                    if (!skipFirstMove)
                    {
                        // player 1
                        foreach (int mv in game.AvailableMoves)
                        {
                            double shallowScore = game.EvalComputerMoveShallow(mv, skillLevelPlayer1);
                            game.MakeMove(mv, Player.Player1);
                            int depth = minDepthPlayer1;
                            for (int i = minDepthPlayer1; i < maxDepthPlayer1; i++)
                            {
                                if (rng.NextDouble() < 1.0 - deepeningProbabilityPlayer1) { break; }
                                depth++;
                            }
                            double score = game.AlphaBeta(depth, double.MinValue, double.MaxValue, skillLevelPlayer1);
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
                        //Console.WriteLine("~~~ Player 1 ~~~");
                        //Console.WriteLine("All moves: {0}", game.AvailableMoves.Select(x => x.ToString()).Aggregate((x, y) => x + "," + y));
                        //Console.WriteLine("Best moves: {0}", bestMoves.Select(x => x.ToString()).Aggregate((x, y) => x + "," + y));
                        move = bestMoves[rng.Next(bestMoves.Count)];
                        //Console.WriteLine("Move: {0}", move);
                        game.MakeMove(move, Player.Player1);
                        if (game.Winner == Player.Player1)
                        {
                            player1Wins++;
                            //Console.WriteLine("Player 1 wins.");
                            break;
                        }
                        else if (game.IsTerminal)
                        {
                            ties++;
                            //Console.WriteLine("It's a tie.");
                            break;
                        }
                    }
                    else { skipFirstMove = false; }
                    // player 2
                    bestScore = double.MinValue;
                    bestShallowScore = double.MinValue;
                    bestMoves = new List<int>();
                    game.SwapPlayers();
                    foreach (int mv in game.AvailableMoves)
                    {
                        double shallowScore = game.EvalComputerMoveShallow(mv, skillLevelPlayer2);
                        game.MakeMove(mv, Player.Player1); // *** Player1 is OK here
                        int depth = minDepthPlayer2;
                        for (int i = minDepthPlayer2; i < maxDepthPlayer2; i++)
                        {
                            if (rng.NextDouble() < 1.0 - deepeningProbabilityPlayer2) { break; }
                            depth++;
                        }
                        double score = game.AlphaBeta(depth, double.MinValue, double.MaxValue, skillLevelPlayer2);
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
                        game.UndoMove(mv, Player.Player1); // *** Player1 is OK here
                    }
                    //Console.WriteLine("~~~ Player 2 ~~~");
                    //Console.WriteLine("All moves: {0}", game.AvailableMoves.Select(x => x.ToString()).Aggregate((x, y) => x + "," + y));
                    //Console.WriteLine("Best moves: {0}", bestMoves.Select(x => x.ToString()).Aggregate((x, y) => x + "," + y));
                    move = bestMoves[rng.Next(bestMoves.Count)];
                    //Console.WriteLine("Move: {0}", move);
                    game.MakeMove(move, Player.Player1); // *** Player1 is OK here
                    if (game.Winner == Player.Player1) // *** Player1 is OK here
                    {
                        player2Wins++;
                        //Console.WriteLine("Player 2 wins.");
                        break;
                    }
                    else if (game.IsTerminal)
                    {
                        ties++;
                        //Console.WriteLine("It's a tie.");
                        break;
                    }
                    game.SwapPlayers();
                    //Console.WriteLine(game);
                }                
            }
            Console.WriteLine("Player 1 wins: {0}", player1Wins);
            Console.WriteLine("Player 2 wins: {0}", player2Wins);
            Console.WriteLine("Ties: {0}", ties);
        }

        public static void Play(IGame game, bool playerStarts, int minDepth = 2, int maxDepth = int.MaxValue, 
            double deepeningProbability = 1, SkillLevel skillLevel = SkillLevel.Normal)
        {
            Random rng = new Random();
            bool skipFirstMove = playerStarts;
            while (true)
            {
                if (!skipFirstMove)
                {
                    double bestScore = double.MinValue;
                    double bestShallowScore = double.MinValue;
                    List<int> bestMoves = new List<int>();
                    foreach (int mv in game.AvailableMoves)
                    {
                        double shallowScore = game.EvalComputerMoveShallow(mv, skillLevel);
                        game.MakeMove(mv, Player.Player1);
                        int depth = minDepth;
                        for (int i = minDepth; i < maxDepth; i++)
                        {
                            if (rng.NextDouble() < 1.0 - deepeningProbability) { break; }
                            depth++;
                        }
                        double score = game.AlphaBeta(depth, double.MinValue, double.MaxValue, skillLevel);
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

        public static double AlphaBeta(this IGame node, int maxDepth, double alpha, double beta, SkillLevel skillLevel, Player player = Player.Player2)
        {
            if (maxDepth == 0 || node.IsTerminal) { return node.Score(skillLevel); }
            if (player == Player.Player1)
            {
                double v = double.MinValue;
                foreach (int move in node.AvailableMoves)
                {
                    node.MakeMove(move, player);
                    v = Math.Max(v, AlphaBeta(node, maxDepth - 1, alpha, beta, skillLevel, Player.Player2));
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
                    v = Math.Min(v, AlphaBeta(node, maxDepth - 1, alpha, beta, skillLevel, Player.Player1));
                    node.UndoMove(move, player);
                    beta = Math.Min(beta, v);
                    if (beta <= alpha) { break; }
                }
                return v;
            }
        }
    }
}
