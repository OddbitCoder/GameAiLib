using GameAiLib;

namespace GameAiLibTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Game.Play(new TicTacToe(), playerStarts: true, minDepth: 2, maxDepth: 9, difficultyLevel: 0.99);
        }
    }
}
