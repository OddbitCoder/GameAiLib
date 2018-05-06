using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace GameAiLib
{
    class Program
    {
        private static double NegamaxEval(IGameNew game)
        {
            if (game.IsWinningState)
            {
                bool player1Wins = !game.Color;
                return player1Wins ? 1 : -1;
            }
            return 0;
        }

        private static void Try(Action a)
        {
            while (true) try { a(); break; } catch { }
        }

        private static double Negamax(TicTacToeNew game, int depth, bool color)
        {
            if (depth == 0 || game.IsTerminalState)
            {
                var score = (color ? 1 : -1) * NegamaxEval(game);
                return score;
            }
            double bestValue = double.MinValue;
            foreach (int move in game.AvailableMoves)
            {
                var undoToken = game.MakeMove(move);
                var dirName = $"./{game.ToOneLineString()}";
                Directory.CreateDirectory(dirName);
                Directory.SetCurrentDirectory(dirName);
                double v = -Negamax(game, depth - 1, !color);
                Directory.SetCurrentDirectory("..");
                var newDirName = $"./{game.ToOneLineString()} ({(game.Color ? 'x' : 'o')} {move}={v}={game.IsTerminalState})";
                Try(() => Directory.Move(dirName, newDirName));
                game.UndoMove(undoToken);
                bestValue = Math.Max(bestValue, v);
            }
            return bestValue;
        }

        private static double NegamaxAlphaBeta(IGameNew game, int depth, double alpha, double beta, bool color)
        {
            if (depth == 0 || game.IsTerminalState)
            {
                return (color ? 1 : -1) * NegamaxEval(game);
            }
            double bestValue = double.MinValue;
            foreach (int move in game.AvailableMoves)
            {
                object undoToken = game.MakeMove(move);
                double v = -NegamaxAlphaBeta(game, depth - 1, -beta, -alpha, !color);
                game.UndoMove(undoToken);
                bestValue = Math.Max(bestValue, v);
                alpha = Math.Max(alpha, v);
                if (alpha >= beta) { break; }
            }
            return bestValue;
        }

        static void Main(string[] args)
        {
            var cache = new Connect4Cache();
            bool player1Starts = true;
            int p1w = 0, p2w = 0, t = 0;
            var time = DateTime.Now;
            for (var i = 0; i < 1000; i++)
            {
                Console.Write(".");
                var winner =
                    //Game.Play(new Connect4(), new Connect4.MinimaxBrain(5), new Connect4.MinimaxBrain(0), player1Starts);
                    //Game.Play(new Connect4New(), new Connect4(), new Connect4New.NegamaxBrain(3, cache), new Connect4.MinimaxBrain(3), player1Starts);
                    Game.Play(new Connect4New(), new Connect4New.NegamaxBrain(5, cache), new Connect4New.NegamaxBrain(5, cache), player1Starts);
                if (winner == Player.Player1) { p1w++; }
                else if (winner == Player.Player2) { p2w++; }
                else { t++; }
                player1Starts = !player1Starts;
            }
            Console.WriteLine($"Time: {(DateTime.Now - time).TotalSeconds}");
            Console.WriteLine(p1w);
            Console.WriteLine(p2w);
            Console.WriteLine(t);
            Console.WriteLine($"Lookups: {cache.numLookups}");
            Console.WriteLine($"Cache hits: {cache.cacheHits}");
            Console.WriteLine($"Exact hits: {cache.exactHits}");
            //Game.Play(new Connect4(), new Connect4.MinimaxBrain(maxDepth: 10));
            //Game.PlayNew(new Connect4New(), new Connect4New.NegamaxBrain(15, cache), skipPlayer1: true);





            //// initialize a game
            //var game = new TicTacToeNew();
            //// make several moves
            //game.MakeMove(4); // o
            //game.MakeMove(5); // x
            ////game.MakeMove(2); // o
            ////game.MakeMove(6); // x
            ////game.MakeMove(1); // o
            ////game.MakeMove(0); // x

            //Directory.SetCurrentDirectory(@"C:\Work\GameAiLib\tree"); // point to the tree dir
            //Console.WriteLine(game);
            //Console.WriteLine(game.ToCode());
            //foreach (var move in game.AvailableMoves)
            //{
            //    var undoToken = game.MakeMove(move); 
            //    var dirName = $"./{game.ToCode()}";
            //    Directory.CreateDirectory(dirName);
            //    Directory.SetCurrentDirectory(dirName);
            //    //double score = -Negamax(game, 0, game.Color);
            //    double score = -NegamaxAlphaBeta(game, int.MaxValue, double.MinValue, double.MaxValue, game.Color);
            //    Directory.SetCurrentDirectory("..");
            //    var newDirName = $"./{game.ToCode()} ({(game.Color ? 'x' : 'o')} {move}={score}={game.IsTerminalState})";
            //    Try(() => Directory.Move(dirName, newDirName));
            //    game.UndoMove(undoToken);
            //}


        }
    }
}