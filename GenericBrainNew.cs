using System;
using System.Collections.Generic;

namespace GameAiLib
{
    public abstract class GenericBrainNew : IBrainNew
    {
        private static Random rndGen 
            = new Random();

        private IMoveCache moveCache;

        public GenericBrainNew(IMoveCache moveCache = null)
        {
            this.moveCache = moveCache;
        }

        public int MakeMove(IGameNew game)
        {
            double bestScore = double.MinValue;
            var bestMoves = new List<int>();
            foreach (int move in game.AvailableMoves)
            {
                var undoToken = game.MakeMove(move);
                double score = EvalGame(game);
                if (score > bestScore) { bestScore = score; bestMoves.Clear(); }
                if (score == bestScore) { bestMoves.Add(move); }
                game.UndoMove(undoToken);
            }
            int bestMove = bestMoves[rndGen.Next(bestMoves.Count)];
            game.MakeMove(bestMove);
            return bestMove;
        }

        // evaluates the player that made the last move
        protected abstract double EvalGame(IGameNew game); 
    }
}
