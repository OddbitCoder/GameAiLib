using System;
using System.Linq;

namespace GameAiLib
{
    class Program
    {
        static void Main(string[] args)
        {
            //bool player1Starts = true;
            //int p1w = 0, p2w = 0, t = 0;
            //for (var i = 0; i < 100000; i++)
            //{
            //    var winner = Game.Play(new Connect4(), new SimpleMinimaxBrain(), new SimpleMinimaxBrain(), player1Starts);
            //    if (winner == Player.Player1) { p1w++; }
            //    else if (winner == Player.Player2) { p2w++; }
            //    else { t++; }
            //    player1Starts = !player1Starts;
            //}
            //Console.WriteLine(p1w);
            //Console.WriteLine(p2w);
            //Console.WriteLine(t);
            Game.Play(new Connect4(), new Connect4.MinimaxBrain(10), humanStarts: false);
        }
    }
}