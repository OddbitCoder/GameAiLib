using System;

namespace GameAiLib
{
    public abstract class GenericMinimaxBrain : GenericBrain
    {
        private double Minimax(IGame game, int depth, Player player, bool maximize)
        {
            if (depth == 0 || game.IsTerminalState)
            {
                // 'maximize ? player : player.OtherPlayer()' is the same as 'player' that is passed to EvalGame
                return MinimaxEval(game, maximize ? player : player.OtherPlayer());
            }
            if (maximize)
            {
                double bestVal = double.MinValue;
                foreach (int move in game.AvailableMoves(player))
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
                foreach (int move in game.AvailableMoves(player))
                {
                    var undoToken = game.MakeMove(move, player);
                    double v = Minimax(game, depth - 1, player.OtherPlayer(), maximize: true);
                    bestVal = Math.Min(bestVal, v);
                    game.UndoMove(move, player, undoToken);
                }
                return bestVal;
            }
        }

        private double AlphaBeta(IGame game, int depth, double alpha, double beta, Player player, bool maximize)
        {
            if (depth == 0 || game.IsTerminalState)
            {
                // 'maximize ? player : player.OtherPlayer()' is the same as 'player' that is passed to EvalGame
                return MinimaxEval(game, maximize ? player : player.OtherPlayer());
            }
            if (maximize)
            {
                double v = double.MinValue;
                foreach (int move in game.AvailableMoves(player))
                {
                    var undoToken = game.MakeMove(move, player);
                    v = Math.Max(v, AlphaBeta(game, depth - 1, alpha, beta, player.OtherPlayer(), maximize: false));
                    game.UndoMove(move, player, undoToken);
                    alpha = Math.Max(alpha, v);
                    if (beta <= alpha) { break; }
                }
                return v;
            }
            else
            {
                double v = double.MaxValue;
                foreach (int move in game.AvailableMoves(player))
                {
                    var undoToken = game.MakeMove(move, player);
                    v = Math.Min(v, AlphaBeta(game, depth - 1, alpha, beta, player.OtherPlayer(), maximize: true));
                    game.UndoMove(move, player, undoToken);
                    beta = Math.Min(beta, v);
                    if (beta <= alpha) { break; }
                }
                return v;
            }
        }

        protected override double EvalGame(IGame game, Player player)
        {
#if SIMPLE_MINIMAX
            return Minimax(game, /*maxDepth=*/int.MaxValue, player.OtherPlayer(), maximize: false);
#else
            return AlphaBeta(game, /*maxDepth=*/int.MaxValue, int.MinValue, int.MaxValue, player.OtherPlayer(), maximize: false);
#endif
        }

        protected abstract double MinimaxEval(IGame game, Player player);
    }
}
