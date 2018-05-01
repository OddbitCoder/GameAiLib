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

//01  function negamax(node, depth, color)
//02     if depth = 0 or node is a terminal node
//03         return color * the heuristic value of node
//04     bestValue := −∞
//05     foreach child of node
//06         v := −negamax(child, depth − 1, −color)
//07         bestValue := max(bestValue, v)
//08     return bestValue

        private double Negamax(IGameNew game, int depth, bool color)
        {
            if (depth == 0 || game.IsTerminalState)
            {
                return (color ? 1 : -1) * NegamaxEval(game);
            }
            double bestValue = double.MinValue;
            foreach (int move in OrderedMoves(game))
            {
                object undoToken = game.MakeMove(move);
                double v = -Negamax(game, depth - 1, !color);
                game.UndoMove(undoToken);
                bestValue = Math.Max(bestValue, v);
            }
            return bestValue;
        }

//01 function negamax(node, depth, α, β, color)
//02     if depth = 0 or node is a terminal node
//03         return color * the heuristic value of node
//04     childNodes := GenerateMoves(node)
//05     childNodes := OrderMoves(childNodes)
//06     bestValue := −∞
//07     foreach child in childNodes
//08         v := −negamax(child, depth − 1, −β, −α, −color)
//09         bestValue := max(bestValue, v)
//10         α := max(α, v)
//11         if α ≥ β
//12             break
//13     return bestValue

        private double NegamaxAlphaBeta(IGameNew game, int depth, double alpha, double beta, bool color)
        {
            if (depth == 0 || game.IsTerminalState)
            {
                return (color ? 1 : -1) * NegamaxEval(game);
            }
            double bestValue = double.MinValue;
            foreach (int move in OrderedMoves(game))
            {
                object undoToken = game.MakeMove(move);
                double v = -NegamaxAlphaBeta(game, depth - 1, -beta, -alpha, !color);
                game.UndoMove(undoToken);
                if (alpha >= beta) { break; }
                bestValue = Math.Max(bestValue, v);
                alpha = Math.Max(alpha, v);
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

        //private double NegamaxAlphaBetaWithTable(IGameNew game, int depth, double alpha, double beta, bool color)
        //{
        //    if (depth == 0 || game.IsTerminalState)
        //    {
        //        return (color ? 1 : -1) * NegamaxEval(game);
        //    }
        //    double bestValue = double.MinValue;
        //    foreach (int move in OrderedMoves(game))
        //    {
        //        object undoToken = game.MakeMove(move);
        //        double v = -NegamaxAlphaBeta(game, depth - 1, -beta, -alpha, !color);
        //        game.UndoMove(undoToken);
        //        if (alpha >= beta) { break; }
        //        bestValue = Math.Max(bestValue, v);
        //        alpha = Math.Max(alpha, v);
        //    }
        //    return bestValue;
        //}

        protected override double EvalGame(IGameNew game)
        {
            //Initial call for Player A's root node
            //rootNegamaxValue := negamax(rootNode, depth, 1)
            //rootMinimaxValue := rootNegamaxValue
            //Initial call for Player B's root node
            //rootNegamaxValue := negamax(rootNode, depth, −1)
            //rootMinimaxValue := −rootNegamaxValue
#if SIMPLE_NEGAMAX
            double rootNegamaxValue = Negamax(game, maxDepth, color: game.CurrentPlayer == Player.Player1);
#else
            double rootNegamaxValue = NegamaxAlphaBeta(game, maxDepth, alpha: double.MinValue, beta: double.MaxValue, color: game.CurrentPlayer == Player.Player1);
#endif
            return game.CurrentPlayer == Player.Player1 ? rootNegamaxValue : -rootNegamaxValue;
        }

        protected abstract double NegamaxEval(IGameNew game);

        protected virtual IEnumerable<int> OrderedMoves(IGameNew game)
        {
            return game.AvailableMoves();
        }
    }
}
