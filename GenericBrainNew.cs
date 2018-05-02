using System;
using System.Collections.Generic;

namespace GameAiLib
{
    public abstract class GenericBrainNew : IBrainNew
    {
        private static Random rng 
            = new Random();

        public int MakeMove(IGameNew game)
        {
            double bestScore = double.MinValue;
            var bestMoves = new List<int>();
            foreach (int move in game.AvailableMoves)
            {
                var undoToken = game.MakeMove(move);
                double score = EvalGame(game);
                Console.WriteLine($"{move}: {score}");
                if (score > bestScore) { bestScore = score; bestMoves.Clear(); }
                if (score == bestScore) { bestMoves.Add(move); }
                game.UndoMove(undoToken);
            }
            int bestMove = bestMoves[rng.Next(bestMoves.Count)];
            game.MakeMove(bestMove);
            return bestMove;
        }

        protected abstract double EvalGame(IGameNew game);
    }
}
