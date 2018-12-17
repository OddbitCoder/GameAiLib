namespace GameAiLib
{
    public static class Extensions
    {
        public static int FirstCharAsInt(this string val)
        {
            return val[0] - '0';
        }
    }
}
