using System;

namespace GameAiLib
{
    class Program
    {
        static void Main(string[] args)
        {
            //Game.Play(new TicTacToe(), new RandomBrain());
            bool player1Starts = true;
            int p1w = 0, p2w = 0, t = 0;
            for (var i = 0; i < 100000; i++)
            {
                var winner = Game.Play(new TicTacToe(), new RandomBrain(), new RandomBrain(), player1Starts);
                if (winner == Player.Player1) { p1w++; }
                else if (winner == Player.Player2) { p2w++; }
                else { t++; }
                player1Starts = !player1Starts;
            }
            Console.WriteLine(p1w);
            Console.WriteLine(p2w);
            Console.WriteLine(t);
        }
    }
}