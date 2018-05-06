using System;

namespace GameAiLib
{
    public struct Connect4CacheItem : ICacheItem
    {
        public ulong nodeCode;
        public byte depth;
        public Flag flag;
        public byte val;
        // interface
        public int Depth => depth;
        public Flag Flag => flag;
        public double Val => val;
    }

    public class Connect4Cache : ICache
    {
        private Connect4CacheItem[] items;
        private int nBitsKey;

        public Connect4Cache(int nBitsKey = 24)
        {
            items = new Connect4CacheItem[1ul << nBitsKey];
            this.nBitsKey = nBitsKey;
        }

        public bool Lookup(IGameNew game, out ICacheItem item)
        {
            ulong nodeCode = ((Connect4New)game).NodeCode();
            ulong key = (nodeCode << (64 - nBitsKey)) >> (64 - nBitsKey);
            item = items[key];
            return item != null && items[key].nodeCode == nodeCode;
        }

        public void Put(IGameNew game, int depth, Flag flag, double val)
        {
            ulong nodeCode = ((Connect4New)game).NodeCode();
            ulong key = (nodeCode << (64 - nBitsKey)) >> (64 - nBitsKey);
            items[key] = new Connect4CacheItem {
                nodeCode = nodeCode,
                depth = (byte)depth,
                flag = flag,
                val = (byte)val
            };
        }
    }
}
