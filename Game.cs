using System;

namespace GameAiLib
{
    public static class Game
    {
        public static Player? Play(IGame game, IBrain player1, IBrain player2, bool player1Starts = true)
        {
            if (player1Starts)
            {
                player1.MakeMove(game, Player.Player1);
                if (game.Winner != null) { return Player.Player1; }
                if (game.IsTerminalState) { return null; }
            }
            while (true)
            {
                player2.MakeMove(game, Player.Player2);
                if (game.Winner != null) { return Player.Player2; }
                if (game.IsTerminalState) { return null; }
                player1.MakeMove(game, Player.Player1);
                if (game.Winner != null) { return Player.Player1; }
                if (game.IsTerminalState) { return null; }
            }
        }

        public static void Play(IGame game, IBrain brain, bool humanStarts = false)
        {
            bool skipPlayer1 = humanStarts;
            while (true)
            {
                if (!skipPlayer1)
                {
                    brain.MakeMove(game, Player.Player1);
                    Console.WriteLine(game);
                    if (game.Winner == Player.Player1)
                    {
                        Console.WriteLine("I won.");
                        break;
                    }
                    else if (game.IsTerminalState)
                    {
                        Console.WriteLine("It's a tie.");
                        break;
                    }
                }
                else
                {
                    skipPlayer1 = false;
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
                else if (game.IsTerminalState) 
                {
                    Console.WriteLine(game); 
                    Console.WriteLine("It's a tie."); 
                    break; 
                }
            } 
        }

        //public static double Minimax(this IGame node, int depth, Player player = Player.Player2)
        //{
        //    if (depth == 0 || node.IsTerminalState) { return node.Score; }
        //    double bestVal = player == Player.Player1 ? double.MinValue : double.MaxValue;
        //    foreach (int move in node.AvailableMoves)
        //    {
        //        node.MakeMove(move, player); 
        //        double val = Minimax(node, depth - 1, player.OtherPlayer());
        //        node.UndoMove(move, player);
        //        if (player == Player.Player1)
        //        {
        //            if (val.CompareTo(bestVal) > 0) { bestVal = val; }
        //        }
        //        else
        //        {
        //            if (val.CompareTo(bestVal) < 0) { bestVal = val; }
        //        }
        //    }
        //    return bestVal;
        //}

        //public static double AlphaBeta(this IGame node, int depth, double alpha, double beta, Player player = Player.Player2)
        //{
        //    if (depth == 0 || node.IsTerminalState) { return node.Score; }
        //    if (player == Player.Player1)
        //    {
        //        double v = double.MinValue;
        //        foreach (int move in node.AvailableMoves)
        //        {
        //            node.MakeMove(move, player); 
        //            v = Math.Max(v, AlphaBeta(node, depth - 1, alpha, beta, Player.Player2));
        //            node.UndoMove(move, player);
        //            alpha = Math.Max(alpha, v);
        //            if (beta <= alpha) { break; }
        //        }
        //        return v;
        //    }
        //    else
        //    {
        //        double v = double.MaxValue;
        //        foreach (int move in node.AvailableMoves)
        //        {
        //            node.MakeMove(move, player); 
        //            v = Math.Min(v, AlphaBeta(node, depth - 1, alpha, beta, Player.Player1));
        //            node.UndoMove(move, player);
        //            beta = Math.Min(beta, v);
        //            if (beta <= alpha) { break; }
        //        }
        //        return v;
        //    }
        //}
    }
}
