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
        bool Lookup(IGame game, out ICacheItem item);
        void Put(IGame game, int depth, Flag flag, double val);
    }
}
