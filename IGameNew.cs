using System.Collections.Generic;

namespace GameAiLib
{
    public interface IGameNew
    {
        bool IsTerminalState { get; }
        IEnumerable<int> AvailableMoves();
        object MakeMove(int move);
        void UndoMove(object undoToken);
        Player? Winner { get; }
        Player CurrentPlayer { get; }
    }
}
