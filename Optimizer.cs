using System;

namespace GameAi
{
    public enum Player
    { 
        Player1, // maximizing player (computer)
        Player2
    }

    public interface IGameState
    {
        bool IsTerminal { get; }
        double Score { get; } // give score for Player 1 
        int[] AvailableMoves { get; }
        void MakeMove(int move, Player player);
        void UndoMove(int move, Player player);
        Player? GetWinner();
    }

    public static class Optimizer
    {
        public static Player OtherPlayer(this Player player)
        {
            return player == Player.Player1 ? Player.Player2 : Player.Player1;
        }

        public static byte PlayerVal(this Player player)
        {
            return (byte)(player == Player.Player1 ? 1 : 2);
        }

        public static Player PlayerFromVal(byte val)
        {
            return val == 1 ? Player.Player1 : Player.Player2;
        }

        public static void Play(IGameState state, int maxDepth = 5)
        {
            do
            {
                double bestScore = double.MinValue;
                int bestMove = -1;
                foreach (int move in state.AvailableMoves)
                {
                    state.MakeMove(move, Player.Player1);
                    double score = state.Minimax(maxDepth);
                    if (score > bestScore) { bestScore = score; bestMove = move; }
                    state.UndoMove(move, Player.Player1);
                }
                state.MakeMove(bestMove, Player.Player1);
                Console.WriteLine(state);
                if (state.GetWinner() == Player.Player1) { Console.WriteLine("I won."); break; }
                else if (state.IsTerminal) { Console.WriteLine("It's a tie."); break; }
                Console.Write("Your move? ");
                int playerMove = Convert.ToInt32(Console.ReadLine());
                state.MakeMove(playerMove, Player.Player2);
                if (state.GetWinner() == Player.Player2) { Console.WriteLine("You won."); break; }
                else if (state.IsTerminal) { Console.WriteLine("It's a tie."); break; }
            }
            while (true);
        }

        public static double Minimax(this IGameState node, int depth, Player player = Player.Player2)
        {
            if (depth == 0 || node.IsTerminal) { return node.Score; }
            double bestVal = player == Player.Player1 ? double.MinValue : double.MaxValue;
            foreach (int move in node.AvailableMoves)
            {
                node.MakeMove(move, player);
                double val = Minimax(node, depth - 1, player.OtherPlayer());
                node.UndoMove(move, player);
                if (player == Player.Player1)
                {
                    if (val.CompareTo(bestVal) > 0) { bestVal = val; }
                }
                else
                {
                    if (val.CompareTo(bestVal) < 0) { bestVal = val; }
                }
            }
            return bestVal;
        }    
    }
}
