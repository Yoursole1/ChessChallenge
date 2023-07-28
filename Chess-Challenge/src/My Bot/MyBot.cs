using ChessChallenge.API;
using System.Collections.Generic;

public class MyBot : IChessBot
{

    // prep for 50 move game
    const int timePerMove = 1200; //MS
    static IDictionary <Board, double> transposition = new Dictionary<Board, double>();

    public Move Think(Board board, Timer timer)
    {
        Move best = Move.NullMove;
        for (int i = 1; timer.MillisecondsElapsedThisTurn < timePerMove; i++){
            best = Search(board, timer, i, )
        }
        return best;
    }


    
    public SearchResult Search(Board board, Timer timer, int depth, double a, double b, bool maximizer){
        
    }
}

class SearchResult {
    Move best {get; }
    int evaluation {get; }

    public SearchResult(Move best, int evaluation){
        this.best = best;
        this.evaluation = evaluation;
    }
}