namespace GameAiLib
{
    public enum Flag : byte
    {
        EXACT,
        UPPER,
        LOWER
    }

    public interface ICacheItem
    {
        int Depth { get; }
        Flag Flag { get; }
        double Val { get; }
    }

    public interface ICache
    {
        bool Lookup(IGameNew game, out ICacheItem item);
        void Put(IGameNew game, int depth, Flag flag, double val);
    }
}
