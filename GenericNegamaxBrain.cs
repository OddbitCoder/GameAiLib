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
//07         bestValue := max(bestValue, v )
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
//09         bestValue := max(bestValue, v )
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
                if (alpha >= beta) { break; }
                bestValue = Math.Max(bestValue, v);
                alpha = Math.Max(alpha, v);
                game.UndoMove(undoToken);
            }
            return bestValue;
        }

        protected override double EvalGame(IGameNew game)
        {
            //Initial call for Player A's root node
            //rootNegamaxValue := negamax(rootNode, depth, 1)
            //rootMinimaxValue := rootNegamaxValue
            //Initial call for Player B's root node
            //rootNegamaxValue := negamax(rootNode, depth, −1)
            //rootMinimaxValue := −rootNegamaxValue
//#if SIMPLE_NEGAMAX
            double rootNegamaxValue = Negamax(game, maxDepth, color: game.CurrentPlayer == Player.Player1);
            return game.CurrentPlayer == Player.Player1 ? rootNegamaxValue : -rootNegamaxValue;
//#else
//            double rootNegamaxValue = NegamaxAlphaBeta(game, maxDepth, alpha: double.MinValue, beta: double.MaxValue, color: game.CurrentPlayer == Player.Player1);
//            return game.CurrentPlayer == Player.Player1 ? rootNegamaxValue : -rootNegamaxValue;
//#endif
        }

        protected abstract double NegamaxEval(IGameNew game);

        protected virtual IEnumerable<int> OrderedMoves(IGameNew game)
        {
            return game.AvailableMoves();
        }
    }
}
