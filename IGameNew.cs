using System.Collections.Generic;

namespace GameAiLib
{
    public interface IGameNew
    {
        bool IsTerminalState { get; }
        bool IsWinningState { get; }
        IEnumerable<int> AvailableMoves { get; }
        object MakeMove(int move);
        void UndoMove(object undoToken);
        bool Color { get; }
    }
}
