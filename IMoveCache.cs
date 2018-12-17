using System.Collections.Generic;

namespace GameAiLib
{
    public interface IMoveCacheItem
    {
        IList<int> Moves { get; }
    }

    public interface IMoveCache
    {
        bool Lookup(IGame game, out IMoveCacheItem item);
    }
}
