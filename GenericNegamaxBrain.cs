using System;
using System.Collections.Generic;

namespace GameAiLib
{
    public abstract class GenericNegamaxBrain : GenericBrainNew
    {
        private int maxDepth;

        public GenericNegamaxBrain(int maxDepth = int.MaxValue)
        {
            this.maxDepth = maxDepth;
        }

        private double Negamax(IGameNew game, int depth, bool color)
        {
            if (depth == 0 || game.IsTerminalState)
            {
                var score = (color ? 1 : -1) * NegamaxEval(game);
                return score;
            }
            double bestValue = double.MinValue;
            foreach (int move in OrderedMoves(game))
            {
                var undoToken = game.MakeMove(move);
                double v = -Negamax(game, depth - 1, !color);
                game.UndoMove(undoToken);
                bestValue = Math.Max(bestValue, v);
            }
            return bestValue;
        }

        private double NegamaxAlphaBeta(IGameNew game, int depth, double alpha, double beta, bool color)
        {
            if (depth == 0 || game.IsTerminalState)
            {
                return (color ? 1 : -1) * NegamaxEval(game);
            }
            double bestValue = double.MinValue;
            foreach (int move in OrderedMoves(game))
            {
                var undoToken = game.MakeMove(move);
                double v = -NegamaxAlphaBeta(game, depth - 1, -beta, -alpha, !color);
                game.UndoMove(undoToken);
                bestValue = Math.Max(bestValue, v);
                alpha = Math.Max(alpha, v);
                if (alpha >= beta) { break; }
            }
            return bestValue;
        }

        //function negamax(node, depth, α, β, color)
        //alphaOrig := α

        //// Transposition Table Lookup; node is the lookup key for ttEntry
        //ttEntry := TranspositionTableLookup(node)
        //if ttEntry is valid and ttEntry.depth ≥ depth
        //    if ttEntry.Flag = EXACT
        //        return ttEntry.Value
        //    else if ttEntry.Flag = LOWERBOUND
        //        α := max(α, ttEntry.Value)
        //    else if ttEntry.Flag = UPPERBOUND
        //        β := min(β, ttEntry.Value)
        //    endif
        //    if α ≥ β
        //        return ttEntry.Value
        //endif

        //if depth = 0 or node is a terminal node
        //    return color * the heuristic value of node

        //bestValue := -∞
        //childNodes := GenerateMoves(node)
        //childNodes := OrderMoves(childNodes)
        //foreach child in childNodes
        //    v := -negamax(child, depth - 1, -β, -α, -color)
        //    bestValue := max(bestValue, v)
        //    α := max(α, v)
        //    if α ≥ β
        //        break

        //// Transposition Table Store; node is the lookup key for ttEntry
        //ttEntry.Value := bestValue
        //if bestValue ≤ alphaOrig
        //    ttEntry.Flag := UPPERBOUND
        //else if bestValue ≥ β
        //    ttEntry.Flag := LOWERBOUND
        //else
        //    ttEntry.Flag := EXACT
        //endif
        //ttEntry.depth := depth
        //TranspositionTableStore(node, ttEntry)

        //return bestValue

        private double NegamaxAlphaBetaWithTable(IGameNew game, int depth, double alpha, double beta, bool color, ICacheNew cache) // TODO: check if correct
        {
            double alphaOrig = alpha;
            if (cache != null && cache.Lookup(game, out ICacheItem item) && item.Depth >= depth)
            {
                if (item.Flag == Flag.EXACT) { return item.Val; }
                else if (item.Flag == Flag.LOWER) { alpha = Math.Max(alpha, item.Val); }
                else if (item.Flag == Flag.UPPER) { beta = Math.Min(beta, item.Val); }
                if (alpha >= beta) { return item.Val; }
            }
            if (depth == 0 || game.IsTerminalState)
            {
                return (color ? 1 : -1) * NegamaxEval(game);
            }
            double bestValue = double.MinValue;
            foreach (int move in OrderedMoves(game))
            {
                var undoToken = game.MakeMove(move);
                double v = -NegamaxAlphaBetaWithTable(game, depth - 1, -beta, -alpha, !color, cache);
                game.UndoMove(undoToken);
                bestValue = Math.Max(bestValue, v);
                alpha = Math.Max(alpha, v);
                if (alpha >= beta) { break; }
            }
            if (cache != null)
            {
                Flag flag = Flag.EXACT;
                if (bestValue <= alphaOrig) { flag = Flag.UPPER; }
                else if (bestValue >= beta) { flag = Flag.LOWER; }
                cache.Put(game, depth, flag, bestValue);
            }
            return bestValue;
        }

        protected override double EvalGame(IGameNew game)
        {
#if SIMPLE_NEGAMAX
            return -Negamax(game, maxDepth, game.Color);
#else
            return -NegamaxAlphaBeta(game, maxDepth, double.MinValue, double.MaxValue, game.Color);
#endif
        }

        // evaluates the player that started the game
        protected abstract double NegamaxEval(IGameNew game); 

        protected virtual IEnumerable<int> OrderedMoves(IGameNew game)
        {
            return game.AvailableMoves;
        }
    }
}
