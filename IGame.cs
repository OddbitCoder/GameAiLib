using System.Collections.Generic;

namespace GameAiLib
{
    public interface IGame
    {
        bool IsTerminalState { get; }
        IEnumerable<int> AvailableMoves(Player player);
        object MakeMove(int move, Player player);
        void UndoMove(int move, Player player, object undoToken);
        Player? Winner { get; }
        bool CheckIntegrity();
    }
}
