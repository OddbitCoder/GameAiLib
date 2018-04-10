namespace GameAiLib
{
    public class RandomBrain : GenericBrain
    {
        protected override double EvalGame(IGame game, Player player)
        {
            return 1;
        }
    }
}
