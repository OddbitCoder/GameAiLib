using System;
using System.Linq;
using System.Collections.Generic;

namespace GameAiLib
{
    public abstract class GenericBrain : IBrain
    {
        private static Random rndGen 
            = new Random();

        private IMoveCache moveCache;

        public GenericBrain(IMoveCache moveCache = null)
        {
            this.moveCache = moveCache;
        }

        public int MakeMove(IGame game)
        {
            if (moveCache != null && moveCache.Lookup(game, out IMoveCacheItem item))
            {
                var moves = item.Moves;
                Console.WriteLine($"Good moves: {moves.Select(x => x.ToString()).Aggregate((a, b) => a + "," + b)}");
                int move = moves[rndGen.Next(moves.Count)];
                game.MakeMove(move);
                return move;
            }
            else
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
                int bestMove = bestMoves[rndGen.Next(bestMoves.Count)];
                game.MakeMove(bestMove);
                return bestMove;
            }
        }

        // evaluates the player that made the last move
        protected abstract double EvalGame(IGame game); 
    }
}
