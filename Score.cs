namespace GameAiLib
{
    public class Score 
    {
        public double Val;
        public int Depth;

        public static readonly Score MinValue
            = new Score { Val = double.MinValue };
        public static readonly Score MaxValue
            = new Score { Val = double.MaxValue };

        public override string ToString()
        {
            return Val + "@" + Depth;
        }
    }
}