using System.Collections.Generic;

namespace GameAiLib
{
    public interface IMoveCacheItem
    {
        IList<int> Moves { get; }
    }

    public interface IMoveCache
    {
        bool Lookup(IGameNew game, out IMoveCacheItem item);
    }
}
