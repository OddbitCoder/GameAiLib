namespace GameAiLib
{
    public class SimpleNegamaxBrain : GenericNegamaxBrain
    {
        protected override double NegamaxEval(IGameNew game)
        {
            if (game.IsWinningState)
            {
                return game.Color ? 1 : -1;
            }
            return 0;
        }
    }
}
