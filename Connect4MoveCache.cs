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
        public IList<int> Moves
        {
            get
            {
                var listOfMoves = new List<int>(7);
                for (int move = 1; move <= 7; move++)
                {
                    if ((moves & (1 << (move - 1))) != 0)
                    {
                        listOfMoves.Add(move - 1);
                    }
                }
                return listOfMoves;
            }
        }
    }

    public class Connect4MoveCache : IMoveCache
    {
        private Connect4MoveCacheItem[] items;

        public Connect4MoveCache(string fileName, bool winOrTie = false, int maxDepth = 5)
        {
            const string regex = @"""pos"":""(.*?)"",""score"":\[(.*?)\]";
            int keySize = 3 * maxDepth; // in bits
            items = new Connect4MoveCacheItem[1 << keySize];
            var data = File.ReadAllLines(fileName);
            foreach (var line in data)
            {
                var match = Regex.Match(line, regex);
                var key = match.Groups[1].Value;
                if (key.Length > maxDepth) { continue; }
                ulong code = 0;
                foreach (int move in key.Select(x => Convert.ToInt32(x.ToString())))
                {
                    code = (code << 3) + (ulong)move;
                }
                //Console.WriteLine($"{key} {code}");
                byte goodMoves = 0;
                var scores = match.Groups[2].Value.Split(',')
                    .Select(x => Convert.ToInt32(x))
                    .Select((score, pos) => new { score, pos })
                    .ToArray();
                int maxScore = scores.Max(item => item.score);
                if (maxScore > 0)
                {
                    foreach (var item in scores.Where(item => (!winOrTie && item.score > 0) || (winOrTie && item.score >= 0))) 
                    {
                        goodMoves += (byte)(1 << item.pos);
                    }
                }
                else
                {
                    foreach (var item in scores.Where(item => item.score == maxScore))
                    {
                        goodMoves += (byte)(1 << item.pos);
                    }
                }
                //Console.WriteLine(goodMoves);
                //Console.WriteLine();
                items[code] = new Connect4MoveCacheItem {
                    moves = goodMoves
                };
            };
        }

        public bool Lookup(IGameNew _game, out IMoveCacheItem _item)
        {
            var game = (Connect4New)_game;
            ulong code = game.MovesCode();
            if (code < (ulong)items.Length)
            {
                var item = items[code];
                _item = item;
                return item.moves != 0;
            }
            _item = null;
            return false;
        }
    }
}
