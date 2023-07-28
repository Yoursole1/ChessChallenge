using ChessChallenge.API;
using System.Collections.Generic;
using System;

public class MyBot : IChessBot
{

    // prep for 50 move game
    const int timePerMove = 1200; //MS
    static IDictionary <ulong, double> transposition = new Dictionary<ulong, double>(); // should be an array ;-;

    public Move Think(Board board, Timer timer)
    {
        Move best = Move.NullMove;
        for (int i = 1; timer.MillisecondsElapsedThisTurn < timePerMove; i++){
            best = Search(board, timer, board.GameMoveHistory[board.GameMoveHistory.GetLength(0) - 1], i, int.MinValue, int.MaxValue, board.IsWhiteToMove).best;
        }
        return best;
    }


    
    public SearchResult Search(Board board, Timer timer, Move lastMove, int depth, int a, int b, bool maximizer){
        if (depth == 0 || board.IsInCheckmate() || board.IsDraw())
        {
            return new SearchResult(lastMove, StaticEvaluation(board));
        }

        if (maximizer)
        {
            SearchResult max = new SearchResult(Move.NullMove, int.MinValue);

            foreach (Move move in board.GetLegalMoves())
            {
                board.MakeMove(move);
                SearchResult result = Search(board, timer, move, depth - 1, a, b, false);
                board.UndoMove(move);

                max = result.Max(max);

                a = Math.Max(a, result.evaluation);
                if (b <= a)
                {
                    break;
                }
            }
            return max;
        }
        else
        {
            SearchResult min = new SearchResult(Move.NullMove, int.MaxValue);

            foreach (Move move in board.GetLegalMoves())
            {
                board.MakeMove(move);
                SearchResult result = Search(board, timer, move, depth - 1, a, b, true);
                board.UndoMove(move);

                min = result.Min(min);

                b = Math.Min(b, result.evaluation);
                if (b <= a)
                {
                    break;
                }
            }
            return min;
        }
    }

    public int StaticEvaluation(Board b){
        return 0;
    }
}

public class SearchResult {
    public Move best {get; }
    public int evaluation {get; }

    public SearchResult(Move best, int evaluation){
        this.best = best;
        this.evaluation = evaluation;
    }

    public SearchResult Max(SearchResult other){
        if (this.evaluation > other.evaluation){
            return this;
        }
        return other;
    }

    public SearchResult Min(SearchResult other){
        if (this.evaluation < other.evaluation){
            return this;
        }
        return other;
    }
}