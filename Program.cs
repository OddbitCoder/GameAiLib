using System.Collections.Generic;
using GameAiLib;

namespace GameAiLibTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Dictionary<object, int> memory = new Dictionary<object, int>();
            bool playerStarts = true;
            while (true)
            {
                Game.Play(new TicTacToe(), playerStarts, minDepth: 2, maxDepth: 9, deepeningProbability: 0.5, memory: memory);
                //playerStarts = !playerStarts;
            }
            //Game.Compare(() => new TicTacToe(), 1000, true, 2, 9, 1, SkillLevel.Highest, 2, 9, 0.99, SkillLevel.Highest);
            //Game.Compare(() => new TicTacToe(), 1000, true, 2, 9, 1, SkillLevel.Highest, 0, 0, 1, SkillLevel.Highest);
        }
    }
}
