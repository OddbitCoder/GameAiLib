namespace GameAiLib
{
    public class SimpleNegamaxBrain : GenericNegamaxBrain
    {
        protected override double NegamaxEval(IGameNew game)
        {
            if (game.Winner == null) { return 0; }
            else if (game.Winner == Player.Player1) { return 1; }
            else { return -1; }
        }
    }
}
