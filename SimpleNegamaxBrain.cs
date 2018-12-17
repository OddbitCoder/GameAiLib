namespace GameAiLib
{
    public class SimpleNegamaxBrain : GenericNegamaxBrain
    {
        protected override double NegamaxEval(IGame game)
        {
            if (game.IsWinningState)
            {
                bool player1Wins = !game.Color;
                return player1Wins ? 1 : -1;
            }
            return 0;
        }
    }
}
