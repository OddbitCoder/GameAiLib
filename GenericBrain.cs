using System;
using System.Collections.Generic;

namespace GameAiLib
{
    public abstract class GenericBrain : IBrain
    {
        private static Random rng 
            = new Random();

        public void MakeMove(IGame game, Player player)
        {
            double bestScore = double.MinValue;
            var bestMoves = new List<int>();
            foreach (int move in game.AvailableMoves)
            {
                game.MakeMove(move, player);
                double score = EvalGame(game);
                if (score > bestScore) { bestScore = score; bestMoves.Clear(); }
                if (score == bestScore) { bestMoves.Add(move); }
                game.UndoMove(move, player);
            }
            game.MakeMove(bestMoves[rng.Next(bestMoves.Count)], player);
        }

        public abstract double EvalGame(IGame game);
    }
}
