using System;
using System.Collections.Generic;

namespace GameAiLib
{
    public abstract class GenericBrainOld : IBrainOld
    {
        private static Random rng 
            = new Random();

        public int MakeMove(IGameOld game, Player player)
        {
            double bestScore = double.MinValue;
            var bestMoves = new List<int>();
            foreach (int move in game.AvailableMoves(player))
            {
                var undoToken = game.MakeMove(move, player);
                double score = EvalGame(game, player);
                //Console.WriteLine($"{move}: {score}");
                if (score > bestScore) { bestScore = score; bestMoves.Clear(); }
                if (score == bestScore) { bestMoves.Add(move); }
                game.UndoMove(move, player, undoToken);
            }
            int bestMove = bestMoves[rng.Next(bestMoves.Count)];
            game.MakeMove(bestMove, player);
            return bestMove;
        }

        protected abstract double EvalGame(IGameOld game, Player player);
    }
}
