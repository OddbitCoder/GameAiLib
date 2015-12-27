using GameAiLib;

namespace GameAiLibTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //Game.Play(new TicTacToe(), playerStarts: true, minDepth: 2, maxDepth: 9, deepeningProbability: 0.99);
            Game.Compare(() => new TicTacToe(), 1000, true, 2, 9, 1, SkillLevel.Highest, 2, 9, 0.99, SkillLevel.Highest);

        }
    }
}
