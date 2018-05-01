using System;

namespace GameAiLib
{
    public interface ICache
    {
        bool GetBoundMin(IGame game, out double val);
        bool GetBoundMax(IGame game, out double val);
        void PutBoundMin(IGame game, double boundMin);
        void PutBoundMax(IGame game, double boundMax);
    }
}
