using System;

namespace GameAiLib
{
    public static class Game
    {
        public static Player? Play(IGame game, IBrain player1, IBrain player2, bool player1Starts = true)
        {
            while (true)
            {
                if (player1Starts)
                {
                    player1.MakeMove(game, Player.Player1);
#if CHECK_INTEGRITY
                    if (!game.CheckIntegrity()) { Console.WriteLine("Integrity broken!!!"); }
#endif
                    if (game.Winner != null) { return Player.Player1; }
                    if (game.IsTerminalState) { return null; }
                }
                player1Starts = true;
                player2.MakeMove(game, Player.Player2);
#if CHECK_INTEGRITY
                if (!game.CheckIntegrity()) { Console.WriteLine("Integrity broken!!!"); }
#endif
                if (game.Winner != null) { return Player.Player2; }
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
                    Console.WriteLine(((Connect4.MinimaxBrain)brain).MoveScore(Player.Player1, (Connect4)game));
                    Console.WriteLine(((Connect4.MinimaxBrain)brain).MoveScore(Player.Player2, (Connect4)game));
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
                    Console.WriteLine(((Connect4.MinimaxBrain)brain).MoveScore(Player.Player1, (Connect4)game));
                    Console.WriteLine(((Connect4.MinimaxBrain)brain).MoveScore(Player.Player2, (Connect4)game));
                }
                Console.Write("Your move? ");
                int playerMove = Convert.ToInt32(Console.ReadLine()); // TODO: repeat this if the move is not valid
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
    }
}
