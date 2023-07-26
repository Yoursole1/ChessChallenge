using ChessChallenge.API;

public class MyBot : IChessBot
{

    // prep for 50 move game
    const int timePerMove = 1200; //MS

    public Move Think(Board board, Timer timer)
    {
       return Move.NullMove;
    }

    
    public SearchResult Search(Board board, Timer timer){
        
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