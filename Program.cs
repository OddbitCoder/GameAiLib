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
                var dirName = $"./{game.ToCode()}";
                Directory.CreateDirectory(dirName);
                Directory.SetCurrentDirectory(dirName);
                double v = -Negamax(game, depth - 1, !color);
                Directory.SetCurrentDirectory("..");
                var newDirName = $"./{game.ToCode()} ({(game.Color ? 'x' : 'o')} {move}={v}={game.IsTerminalState})";
                Try(() => Directory.Move(dirName, newDirName));
                game.UndoMove(undoToken);
                bestValue = Math.Max(bestValue, v);
            }
            return bestValue;
        }

        static void Main(string[] args)
        {
            //bool player1Starts = true;
            //int p1w = 0, p2w = 0, t = 0;
            //for (var i = 0; i < 100; i++)
            //{
            //    Console.Write(".");
            //    var winner =
            //        //Game.Play(new TicTacToe(), new SimpleMinimaxBrain(), new SimpleMinimaxBrain(), player1Starts);
            //        Game.Play(new TicTacToeNew(), new TicTacToe(), new SimpleNegamaxBrain(), new SimpleMinimaxBrain(), player1Starts);
            //        //Game.Play(new Connect4New(), new Connect4New.NegamaxBrain(1), new Connect4New.NegamaxBrain(1), player1Starts);
            //    if (winner == Player.Player1) { p1w++; }
            //    else if (winner == Player.Player2) { p2w++; }
            //    else { t++; }
            //    player1Starts = !player1Starts;
            //}
            //Console.WriteLine(p1w);
            //Console.WriteLine(p2w);
            //Console.WriteLine(t);
            //Game.Play(new Connect4(), new Connect4.MinimaxBrain(maxDepth: 10));
            //Game.PlayNew(new TicTacToeNew(), new SimpleNegamaxBrain(), skipPlayer1: true);

            // initialize a game
            var game = new TicTacToeNew();
            // make several moves
            game.MakeMove(4); // o
            game.MakeMove(5); // x
            game.MakeMove(2); // o
            game.MakeMove(6); // x
            game.MakeMove(1); // o
            game.MakeMove(0); // x

            Directory.SetCurrentDirectory(@"C:\Work\GameAiLib\tree"); // point to the tree dir
            Console.WriteLine(game);
            Console.WriteLine(game.ToCode());
            foreach (var move in game.AvailableMoves)
            {
                var undoToken = game.MakeMove(move); 
                var dirName = $"./{game.ToCode()}";
                Directory.CreateDirectory(dirName);
                Directory.SetCurrentDirectory(dirName);
                double score = -Negamax(game, 0, game.Color);
                Directory.SetCurrentDirectory("..");
                var newDirName = $"./{game.ToCode()} ({(game.Color ? 'x' : 'o')} {move}={score}={game.IsTerminalState})";
                Try(() => Directory.Move(dirName, newDirName));
                game.UndoMove(undoToken);
            }
           
           
        }
    }
}