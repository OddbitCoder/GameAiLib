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
                foreach (int move in game.AvailableMoves)
                {
                    game.MakeMove(move, player);
                    double v = Minimax(game, depth - 1, player.OtherPlayer(), maximize: false);
                    bestVal = Math.Max(bestVal, v);
                    game.UndoMove(move, player);
                }
                return bestVal;
            }
            else
            {
                double bestVal = double.MaxValue;
                foreach (int move in game.AvailableMoves)
                {
                    game.MakeMove(move, player);
                    double v = Minimax(game, depth - 1, player.OtherPlayer(), maximize: true);
                    bestVal = Math.Min(bestVal, v);
                    game.UndoMove(move, player);
                }
                return bestVal;
            }
        }

        protected override double EvalGame(IGame game, Player player)
        {
            return Minimax(game, /*maxDepth=*/int.MaxValue, player.OtherPlayer(), maximize: false);
        }

        protected abstract double MinimaxEval(IGame game, Player player);
    }

    //public static double AlphaBeta(this IGame node, int depth, double alpha, double beta, Player player = Player.Player2)
    //{
    //    if (depth == 0 || node.IsTerminalState) { return node.Score; }
    //    if (player == Player.Player1)
    //    {
    //        double v = double.MinValue;
    //        foreach (int move in node.AvailableMoves)
    //        {
    //            node.MakeMove(move, player); 
    //            v = Math.Max(v, AlphaBeta(node, depth - 1, alpha, beta, Player.Player2));
    //            node.UndoMove(move, player);
    //            alpha = Math.Max(alpha, v);
    //            if (beta <= alpha) { break; }
    //        }
    //        return v;
    //    }
    //    else
    //    {
    //        double v = double.MaxValue;
    //        foreach (int move in node.AvailableMoves)
    //        {
    //            node.MakeMove(move, player); 
    //            v = Math.Min(v, AlphaBeta(node, depth - 1, alpha, beta, Player.Player1));
    //            node.UndoMove(move, player);
    //            beta = Math.Min(beta, v);
    //            if (beta <= alpha) { break; }
    //        }
    //        return v;
    //    }
    //}
}
