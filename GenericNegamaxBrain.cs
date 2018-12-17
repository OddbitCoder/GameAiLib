using System;
using System.Linq;
using System.Collections.Generic;

namespace GameAiLib
{
    public abstract class GenericNegamaxBrain : IBrain
    {
        private int maxDepth;
        private bool iterative;

        private ICache cache;
        private IMoveCache moveCache;

        private static Random rng
            = new Random();

        public GenericNegamaxBrain(int maxDepth = int.MaxValue, ICache cache = null, IMoveCache moveCache = null,
            bool iterative = false) 
        {
            this.maxDepth = maxDepth;
            this.cache = cache;
            this.iterative = iterative;
            this.moveCache = moveCache;
        }

        private double Negamax(IGame game, int depth, bool color)
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

        private double NegamaxAlphaBeta(IGame game, int depth, double alpha, double beta, bool color)
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

        private double NegamaxAlphaBetaWithTable(IGame game, int depth, double alpha, double beta, bool color, ICache cache) 
        {
            //alphaOrig := α
            double alphaOrig = alpha;
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
            if (cache != null && cache.Lookup(game, out ICacheItem item) && item.Depth >= depth)
            {
                if (item.Flag == Flag.EXACT) { return item.Val; }
                else if (item.Flag == Flag.LOWER) { alpha = Math.Max(alpha, item.Val); }
                else if (item.Flag == Flag.UPPER) { beta = Math.Min(beta, item.Val); }
                if (alpha >= beta) { return item.Val; }
            }
            //if depth = 0 or node is a terminal node
            //    return color * the heuristic value of node
            if (depth == 0 || game.IsTerminalState)
            {
                return (color ? 1 : -1) * NegamaxEval(game);
            }
            //bestValue := -∞
            //childNodes := GenerateMoves(node)
            //childNodes := OrderMoves(childNodes)
            //foreach child in childNodes
            //    v := -negamax(child, depth - 1, -β, -α, -color)
            //    bestValue := max(bestValue, v)
            //    α := max(α, v)
            //    if α ≥ β
            //        break
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
            if (cache != null)
            {
                Flag flag = Flag.EXACT;
                if (bestValue <= alphaOrig) { flag = Flag.UPPER; }
                else if (bestValue >= beta) { flag = Flag.LOWER; }
                cache.Put(game, depth, flag, bestValue);
            }
            //return bestValue
            return bestValue;
        }

        private double EvalGame(IGame game, int maxDepth)
        {
#if NEGAMAX_SIMPLE
            return -Negamax(game, maxDepth, game.Color);
#elif NEGAMAX_ALPHABETA_NO_CACHE
            return -NegamaxAlphaBeta(game, maxDepth, double.MinValue, double.MaxValue, game.Color);
#else
            return -NegamaxAlphaBetaWithTable(game, maxDepth, double.MinValue, double.MaxValue, game.Color, cache);
#endif
        }

        // evaluates the player that started the game
        protected abstract double NegamaxEval(IGame game); 

        protected virtual IEnumerable<int> OrderedMoves(IGame game)
        {
            return game.AvailableMoves;
        }

        public int MakeMove(IGame game)
        {
            if (moveCache != null && moveCache.Lookup(game, out IMoveCacheItem item))
            {
                var moves = item.Moves;
                Console.WriteLine($"Good moves: {moves.Select(x => x.ToString()).Aggregate((a, b) => a + "," + b)}");
                int move = moves[rng.Next(moves.Count)];
                game.MakeMove(move);
                return move;
            }
            else
            {
                double bestScore = double.MinValue;
                var bestMoves = new List<int>();
                for (int i = 0; i <= maxDepth; i++)
                {
                    foreach (int move in game.AvailableMoves)
                    {
                        var undoToken = game.MakeMove(move);
                        double score = EvalGame(game, i);
                        Console.WriteLine($"{move}: {score}");
                        if (score > bestScore) { bestScore = score; bestMoves.Clear(); }
                        if (score == bestScore) { bestMoves.Add(move); }
                        game.UndoMove(undoToken);
                    }
                }
                int bestMove = bestMoves[rng.Next(bestMoves.Count)];
                game.MakeMove(bestMove);
                return bestMove;
            }
        }
    }
}
