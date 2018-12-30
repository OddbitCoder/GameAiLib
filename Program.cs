using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace GameAiLib
{
    class Program
    {
        private static double NegamaxEval(IGame game)
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

        private static double Negamax(TicTacToe game, int depth, bool color)
        {
            if (depth == 0 || game.IsTerminalState)
            {
                var score = (color ? 1 : -1) * NegamaxEval(game);
                return score;
            }
            double bestValue = double.MinValue;
            foreach (var move in game.GetValidMoves())
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

        private static double NegamaxAlphaBeta(TicTacToe game, int depth, double alpha, double beta, bool color)
        {
            if (depth == 0 || game.IsTerminalState)
            {
                return (color ? 1 : -1) * NegamaxEval(game);
            }
            double bestValue = double.MinValue;
            int c = 1;
            foreach (var move in game.GetValidMoves().Take(2))
            {
                object undoToken = game.MakeMove(move);
                var dirName = $"./{game.ToOneLineString()}";
                Directory.CreateDirectory(dirName);
                Directory.SetCurrentDirectory(dirName);
                double v = -NegamaxAlphaBeta(game, depth - 1, -beta, -alpha, !color);
                Directory.SetCurrentDirectory("..");
                bestValue = Math.Max(bestValue, v);
                alpha = Math.Max(alpha, v);
                var newDirName = $"./{c++} {game.ToOneLineString()} ({(game.Color ? 'x' : 'o')} {move}={v} alpha={alpha} beta={beta} cut={alpha>=beta})";
                Try(() => Directory.Move(dirName, newDirName));
                game.UndoMove(undoToken);
                if (alpha >= beta) { break; }
            }
            return bestValue;
        }

        static void Recurse(Connect4 game, int d, string query, List<string> queries)
        {
            if (d < 0) { return; }
            queries.Add(query);
            foreach (var move in game.GetValidMoves())
            {
                var token = game.MakeMove(move);
                if (!game.IsTerminalState)
                {
                    Recurse(game, d - 1, query + (move + 1), queries);
                }
                game.UndoMove(token);
            }
        }

        static void Main(string[] args)
        {
            //var game = new Connect4New();
            //var queries = new List<string>();
            //Recurse(game, 8, "", queries);
            //Console.WriteLine($"{queries.Count} queries. Starting Web acq.");
            //var fn = @"C:\Work\GameAiLib\data\starting8.ojpl";
            //var done = new HashSet<string>(File.ReadAllLines(fn).Select(line => {
            //    var m = Regex.Match(line, @"""pos"":""(.*?)""");
            //    //Console.WriteLine(m.Result("$1"));
            //    return m.Result("$1");
            //}));

            //ServicePointManager.DefaultConnectionLimit = 32;
            //using (var w = new StreamWriter(fn, append: true))
            //{
            //    Parallel.ForEach(queries.Where(q=>!done.Contains(q)), new ParallelOptions { MaxDegreeOfParallelism = 32 }, query =>
            //    {
            //        //if (!done.Contains(query))
            //        //{
            //            using (WebClient client = new WebClient())
            //            {
            //                try
            //                {

            //                    string htmlCode = client.DownloadString("http://connect4.gamesolver.org/solve?pos=" + query);
            //                    lock (w)
            //                    {
            //                        Console.WriteLine($"{query}");
            //                        w.WriteLine(htmlCode);
            //                    }


            //                }
            //                catch (Exception e) { Console.WriteLine($"{query}! {e}"); }
            //            }
            //        //}
            //        //else
            //        //{
            //        //    Console.WriteLine($"{query}*");
            //        //}
            //    });
            //}



            //return;



            var cache = new Connect4Cache();
            var moveCache = new Connect4MoveCache(@"C:\Work\GameAiLib\data\starting.ojpl");
            //bool player1Starts = true;
            //int p1w = 0, p2w = 0, t = 0;
            //var time = DateTime.Now;
            //for (var i = 0; i < 100; i++)
            //{
            //    Console.Write(".");
            //    var winner =
            //        //Game.Play(new Connect4(), new Connect4.MinimaxBrain(5), new Connect4.MinimaxBrain(0), player1Starts);
            //        //Game.Play(new Connect4New(), new Connect4(), new Connect4New.NegamaxBrain(15, moveCache: moveCache), new Connect4.MinimaxBrain(15), player1Starts);
            //        Game.Play(new Connect4New(), new Connect4New.NegamaxBrain(10, cache, moveCache), new Connect4New.NegamaxBrain(10, cache, moveCache), player1Starts);
            //    if (winner == Player.Player1) { p1w++; }
            //    else if (winner == Player.Player2) { p2w++; }
            //    else { t++; }
            //    player1Starts = !player1Starts;
            //}
            //Console.WriteLine($"Time: {(DateTime.Now - time).TotalSeconds}");
            //Console.WriteLine(p1w);
            //Console.WriteLine(p2w);
            //Console.WriteLine(t);
            //Console.WriteLine($"Lookups: {cache.numLookups}");
            //Console.WriteLine($"Cache hits: {cache.cacheHits}");
            //Console.WriteLine($"Exact hits: {cache.exactHits}");
            //Game.Play(new Connect4(), new Connect4.MinimaxBrain(maxDepth: 10));
            Game.PlayNew(new Connect4(), new Connect4.NegamaxBrain(15, new Connect4Cache(), null), skipPlayer1: true);





            //// initialize a game
            //var game = new TicTacToeNew();
            //// make several moves
            ////game.MakeMove(4); // o
            ////game.MakeMove(5); // x
            ////game.MakeMove(2); // o
            ////game.MakeMove(6); // x
            ////game.MakeMove(1); // o
            ////game.MakeMove(0); // x

            //Directory.SetCurrentDirectory(@"C:\Work\GameAiLib\tree"); // point to the tree dir
            //Console.WriteLine(game);
            //Console.WriteLine(game.ToOneLineString());
            //int c = 1;
            //foreach (int move in game.AvailableMoves.Take(2))
            //{
            //    var undoToken = game.MakeMove(move);
            //    var dirName = $"./{game.ToOneLineString()}";
            //    Directory.CreateDirectory(dirName);
            //    Directory.SetCurrentDirectory(dirName);
            //    //double score = -Negamax(game, 0, game.Color);
            //    double score = -NegamaxAlphaBeta(game, int.MaxValue, double.MinValue, double.MaxValue, game.Color);
            //    Directory.SetCurrentDirectory("..");
            //    var newDirName = $"./{c++} {game.ToOneLineString()} ({(game.Color ? 'x' : 'o')} {move}={score}={game.IsTerminalState})";
            //    Try(() => Directory.Move(dirName, newDirName));
            //    game.UndoMove(undoToken);
            //}


        }
    }
}