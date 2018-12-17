using System.Collections.Generic;

namespace GameAiLib
{
    public interface IGame
    {
        bool IsTerminalState { get; }
        bool IsWinningState { get; }
        IEnumerable<int> AvailableMoves { get; }
        object MakeMove(int move);
        void UndoMove(object undoToken);
        bool Color { get; } // who will make the next move? (true = the player that started the game)
    }
}
