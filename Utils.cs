namespace GameAiLib
{
    public static class Utils
    {
        public static int CountOnes(this ushort val)
        {
            int c = 0;
            for (c = 0; val != 0; c++) { val &= (ushort)(val - 1); }
            return c; 
        }

        public static int CountOnes(this ulong val)
        {
            int c = 0;
            for (c = 0; val != 0; c++) { val &= val - 1; }
            return c;
        }

        public static int MoveIdx(this string move, int idx)
        {
            return move[idx] - 'A';
        }

        public static string MoveStr(this int move)
        {
            return ((char)(move + 'A')).ToString();
        }
    }
}
