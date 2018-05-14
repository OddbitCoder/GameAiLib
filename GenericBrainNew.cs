using System;
using System.Linq;
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
            if (moveCache != null && moveCache.Lookup(game, out IMoveCacheItem item))
            {
                var moves = item.Moves.ToArray();
                if (((Connect4New)game).movesList.Count > 0)
                    Console.WriteLine(((Connect4New)game).movesList.Select(x => x.ToString()).Aggregate((a, b) => a + "," + b));
                Console.WriteLine($"Good moves: {moves.Select(x => x.ToString()).Aggregate((a, b) => a + "," + b)}");
                int move = moves[rndGen.Next(moves.Length)];
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
        protected abstract double EvalGame(IGameNew game); 
    }
}
