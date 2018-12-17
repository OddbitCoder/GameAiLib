using System.Collections.Generic;

namespace GameAiLib
{
    public interface IMoveCacheItem
    {
        IList<string> Moves { get; }
    }

    public interface IMoveCache
    {
        bool Lookup(IGame game, out IMoveCacheItem item);
    }
}
