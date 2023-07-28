using ChessChallenge.API;
using System.Collections.Generic;

public class MyBot : IChessBot
{

    // prep for 50 move game
    const int timePerMove = 1200; //MS
    static IDictionary <ulong, double> transposition = new Dictionary<ulong, double>(); // should be an array ;-;

    public Move Think(Board board, Timer timer)
    {
        Move best = Move.NullMove;
        for (int i = 1; timer.MillisecondsElapsedThisTurn < timePerMove; i++){
            best = Search(board, timer, i, double.MinValue, double.MaxValue, board.IsWhiteToMove).best;
        }
        return best;
    }


    
    public SearchResult Search(Board board, Timer timer, int depth, double a, double b, bool maximizer){

    }
}

class SearchResult {
    public Move best {get; }
    public int evaluation {get; }

    public SearchResult(Move best, int evaluation){
        this.best = best;
        this.evaluation = evaluation;
    }
}