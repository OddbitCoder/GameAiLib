using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.IO;

namespace GameAiLib
{
    public struct Connect4MoveCacheItem : IMoveCacheItem
    {
        public byte moves;
        // interface
        public IEnumerable<int> Moves
        {
            get
            {
                var moves = new List<int>(7);
                //for (int i = 0; i < 7; i++)
                //{

                //}
                return moves;
            }
        }
    }

    public class Connect4MoveCache : IMoveCache
    {
        private static int MAX_DEPTH
            = 5;
        private static int KEY_SIZE_BITS
            = 3 * MAX_DEPTH;

        private Connect4MoveCacheItem[] items;

        public Connect4MoveCache(string fileName)
        {
            items = new Connect4MoveCacheItem[1 << KEY_SIZE_BITS];
            var data = File.ReadAllLines(fileName);
            const string regex = @"""pos"":""(.*?)"",""score"":\[(.*?)\]";
            foreach (var line in data)
            {
                var match = Regex.Match(line, regex);
                var key = match.Groups[1].Value;
                if (key.Length > MAX_DEPTH) { continue; }
                ulong code = 0;
                foreach (int move in key.Select(x => Convert.ToInt32(x.ToString())))
                {
                    code = (code << 3) + (ulong)move;
                }
                //Console.WriteLine($"{key} {code}");
                byte goodMoves = 0;
                int pos = 0;
                foreach (int score in match.Groups[2].Value.Split(',').Select(x => Convert.ToInt32(x)))
                {
                    //Console.WriteLine($"{pos} {score}");
                    if (score >= 0) // WARNME: win or tie (TODO: win-only option)
                    {
                        goodMoves += (byte)(1 << pos);
                    }
                    pos++;
                }
                //Console.WriteLine(goodMoves);
                //Console.WriteLine();
                items[code] = new Connect4MoveCacheItem {
                    moves = goodMoves
                };
            };
        }

        public bool Lookup(IGameNew game, out IMoveCacheItem item)
        {
            item = null;
            return false;
        }
    }
}
