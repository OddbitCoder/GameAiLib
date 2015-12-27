namespace GameAiLib
{
    public interface IGame
    {
        bool IsTerminal { get; }
        double Score(SkillLevel skillLevel); // gives score for Player 1 (assessed with given skill level)
        int[] AvailableMoves { get; }
        void MakeMove(int move, Player player);
        void UndoMove(int move, Player player);
        Player? Winner { get; }
        double EvalComputerMoveShallow(int move, SkillLevel skillLevel);
        void SwapPlayers(); // only needed for computer vs computer
    }
}
