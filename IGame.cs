using System.Collections.Generic;

namespace GameAiLib
{
    public interface ICacheable<T>
    {
        T BoardCode { get; }
    }

    public interface IGame
    {
        bool IsTerminalState { get; }
        bool IsWinningState { get; }
        IEnumerable<string> GetValidMoves();
        object MakeMove(string move);
        void UndoMove(object undoToken);
        bool Color { get; } // who will make the next move? (true = the player that started the game)
    }
}
