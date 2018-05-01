using System;
using System.Collections.Generic;

namespace GameAiLib
{
    public abstract class GenericMinimaxBrain : GenericBrain
    {
        private int maxDepth;
        private ICache cache;

        public GenericMinimaxBrain(int maxDepth = int.MaxValue, ICache cache = null)
        {
            this.maxDepth = maxDepth;
            this.cache = cache;
        }

        private double Minimax(IGame game, int depth, Player player, bool maximize)
        {
            c++;
            if (depth == 0 || game.IsTerminalState)
            {
                // 'maximize ? player : player.OtherPlayer()' is the same as 'player' that is passed to EvalGame
                return MinimaxEval(game, maximize ? player : player.OtherPlayer());
            }
            if (maximize)
            {
                double bestVal = double.MinValue;
                foreach (int move in OrderedMoves(game, player))
                {
                    var undoToken = game.MakeMove(move, player);
                    double v = Minimax(game, depth - 1, player.OtherPlayer(), maximize: false);
                    bestVal = Math.Max(bestVal, v);
                    game.UndoMove(move, player, undoToken);
                }
                return bestVal;
            }
            else
            {
                double bestVal = double.MaxValue;
                foreach (int move in OrderedMoves(game, player))
                {
                    var undoToken = game.MakeMove(move, player);
                    double v = Minimax(game, depth - 1, player.OtherPlayer(), maximize: true);
                    bestVal = Math.Min(bestVal, v);
                    game.UndoMove(move, player, undoToken);
                }
                return bestVal;
            }
        }

        long c = 0;

        private double AlphaBeta(IGame game, int depth, double alpha, double beta, Player player, bool maximize)
        {
            if (cache != null)
            {
                if (maximize && cache.GetBoundMax(game, out double boundMax)) { return boundMax; }
                else if (!maximize && cache.GetBoundMin(game, out double boundMin)) { return boundMin; }
            }
            c++;
            if (depth == 0 || game.IsTerminalState)
            {
                // 'maximize ? player : player.OtherPlayer()' is the same as 'player' that is passed to EvalGame
                return MinimaxEval(game, maximize ? player : player.OtherPlayer());
            }
            if (maximize)
            {
                double v = double.MinValue;
                foreach (int move in OrderedMoves(game, player))
                {
                    var undoToken = game.MakeMove(move, player);
                    v = Math.Max(v, AlphaBeta(game, depth - 1, alpha, beta, player.OtherPlayer(), maximize: false));
                    game.UndoMove(move, player, undoToken);
                    alpha = Math.Max(alpha, v);
                    if (beta <= alpha) { break; }
                }
                if (cache != null) { cache.PutBoundMax(game, v); }
                return v;
            }
            else
            {
                double v = double.MaxValue;
                foreach (int move in OrderedMoves(game, player))
                {
                    var undoToken = game.MakeMove(move, player);
                    v = Math.Min(v, AlphaBeta(game, depth - 1, alpha, beta, player.OtherPlayer(), maximize: true));
                    game.UndoMove(move, player, undoToken);
                    beta = Math.Min(beta, v);
                    if (beta <= alpha) { break; }
                }
                if (cache != null) { cache.PutBoundMin(game, v); }
                return v;
            }
        }

        protected override double EvalGame(IGame game, Player player)
        {
            c = 0;
#if SIMPLE_MINIMAX
            double val = Minimax(game, maxDepth, player.OtherPlayer(), maximize: false);
#else
            double val = AlphaBeta(game, maxDepth, int.MinValue, int.MaxValue, player.OtherPlayer(), maximize: false);
#endif
            Console.WriteLine($"Num states={c}");
            return val;
        }

        protected abstract double MinimaxEval(IGame game, Player player);

        protected virtual IEnumerable<int> OrderedMoves(IGame game, Player player)
        {
            return game.AvailableMoves(player);
        }
    }
}
