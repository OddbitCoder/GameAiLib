namespace GameAiLib
{
    public struct Connect4CacheItem : ICacheItem
    {
        public ulong nodeCode;
        public byte depth;
        public Flag flag;
        public short val;
        // interface
        public int Depth => depth;
        public Flag Flag => flag;
        public double Val => val;
    }

    public class Connect4Cache : ICache
    {
        public long cacheHits;
        public long exactHits;
        public long numLookups;

        private Connect4CacheItem[] items;

        public Connect4Cache(ulong cacheSize = 8388593ul)
        {
            items = new Connect4CacheItem[cacheSize];
        }

        public bool Lookup(IGameNew game, out ICacheItem item)
        {
            numLookups++;
            ulong nodeCode = ((Connect4New)game).NodeCode();
            ulong key = nodeCode % (ulong)items.Length;
            item = items[key];
            bool hit = items[key].nodeCode == nodeCode;
            if (hit) { cacheHits++; if (item.Flag == Flag.EXACT) { exactHits++; } }
            return hit;
        }

        public void Put(IGameNew game, int depth, Flag flag, double val)
        {
            ulong nodeCode = ((Connect4New)game).NodeCode();
            ulong key = nodeCode % (ulong)items.Length;
            items[key] = new Connect4CacheItem {
                nodeCode = nodeCode,
                depth = (byte)depth,
                flag = flag,
                val = (short)val
            };
        }
    }
}
