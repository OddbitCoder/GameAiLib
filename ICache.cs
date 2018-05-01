using System;

namespace GameAiLib
{
    public interface ICache
    {
        bool GetBoundMin(IGame game, out double val, Player player);
        bool GetBoundMax(IGame game, out double val, Player player);
        void PutBoundMin(IGame game, double boundMin, Player player);
        void PutBoundMax(IGame game, double boundMax, Player player);
    }
}
