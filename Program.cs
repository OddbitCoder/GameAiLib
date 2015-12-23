using GameAiLib;

namespace GameAiLibTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Game.Play(new TicTacToe(), playerStarts: true, maxDepth: 9, difficultyLevel: 0);
        }
    }
}
