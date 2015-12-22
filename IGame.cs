namespace GameAiLib
{
    public interface IGame
    {
        bool IsTerminal { get; }
        double Score { get; } // gives score for Player 1 
        int[] AvailableMoves { get; }
        void MakeMove(int move, Player player);
        void UndoMove(int move, Player player);
        Player? Winner { get; }
    }
}
