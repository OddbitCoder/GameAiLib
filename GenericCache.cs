using System;

namespace GameAiLib
{
    public class GenericCache<T> : ICache where T : struct
    {
        public struct Item : ICacheItem 
        {
            public T boardCode;
            public ushort depth;
            public Flag flag;
            public short val;
            // interface
            public int Depth => depth;
            public Flag Flag => flag;
            public double Val => val;
        }

        public long CacheHits { get; private set; }
        public long ExactHits { get; private set; }
        public long NumLookups { get; private set; }

        private Item[] items;

        public GenericCache(ulong cacheSize = 8388593ul)
        {
            items = new Item[cacheSize];
        }

        public bool Lookup(IGame game, out ICacheItem item)
        {
            NumLookups++;
            T boardCode = ((ICacheable<T>)game).BoardCode;
            ulong key = Convert.ToUInt64(boardCode) % (ulong)items.Length;
            item = items[key];
            bool hit = items[key].boardCode.Equals(boardCode);
            if (hit)
            {
                CacheHits++;
                if (item.Flag == Flag.EXACT)
                {
                    ExactHits++;
                }
            }
            return hit;
        }

        public void Put(IGame game, int depth, Flag flag, double val)
        {
            T boardCode = ((ICacheable<T>)game).BoardCode;
            ulong key = Convert.ToUInt64(boardCode) % (ulong)items.Length;
            items[key] = new Item {
                boardCode = boardCode,
                depth = (ushort)depth,
                flag = flag,
                val = (short)val
            };
        }
    }
}
