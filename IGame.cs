namespace GameAiLib
{
    public interface IGame
    {
        bool IsTerminalState { get; }
        int[] AvailableMoves { get; }
        void MakeMove(int move, Player player);
        void UndoMove(int move, Player player);
        Player? Winner { get; }
    }
}
