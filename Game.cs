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
                    if (game.Winner != null) { return Player.Player1; }
                    if (game.IsTerminalState) { return null; }
                }
                player1Starts = true;
                player2.MakeMove(game, Player.Player2);
                if (game.Winner != null) { return Player.Player2; }
                if (game.IsTerminalState) { return null; }
            }
        }

        public static Player? Play(IGameNew game, IBrainNew player1, IBrainNew player2, bool player1Starts = true)
        {
            while (true)
            {
                if (player1Starts)
                {
                    player1.MakeMove(game);
                    if (game.IsWinningState) { return Player.Player1; }
                    if (game.IsTerminalState) { return null; }
                }
                player1Starts = true;
                player2.MakeMove(game);
                if (game.IsWinningState) { return Player.Player2; }
                if (game.IsTerminalState) { return null; }
            }
        }

        public static Player? Play(IGameNew game, IGame gameOld, IBrainNew player1, IBrain player2, bool player1Starts = true)
        {
            while (true)
            {
                if (player1Starts)
                {
                    //game.MakeMove(player2.MakeMove(gameOld, Player.Player1));
                    gameOld.MakeMove(player1.MakeMove(game), Player.Player1);
                    //Console.WriteLine(game);
                    //Debug.Assert(((Connect4)gameOld).position == ((Connect4New)game).position);
                    //Debug.Assert(((Connect4)gameOld).mask == ((Connect4New)game).mask);
                    if (game.IsWinningState) { return Player.Player1; }
                    if (game.IsTerminalState) { return null; }
                }
                player1Starts = true;
                game.MakeMove(player2.MakeMove(gameOld, Player.Player2));
                //gameOld.MakeMove(player1.MakeMove(game), Player.Player2);
                //Console.WriteLine(gameOld);
                //Console.ReadLine();
                //Debug.Assert(((Connect4)gameOld).position == (((Connect4New)game).position ^ ((Connect4New)game).mask));
                //Debug.Assert(((Connect4)gameOld).mask == ((Connect4New)game).mask);
                if (game.IsWinningState) { return Player.Player2; }
                if (game.IsTerminalState) { return null; }
            }
        }

        public static void PlayNew(IGameNew game, IBrainNew brain, bool skipPlayer1)
        {
            while (true)
            {
                if (!skipPlayer1)
                {
                    brain.MakeMove(game);
                    Console.WriteLine(game);
                    if (game.IsWinningState)
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
                int playerMove = Convert.ToInt32(Console.ReadLine()); // TODO: repeat this if the move is not valid
                game.MakeMove(playerMove);
                if (game.IsWinningState)
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
