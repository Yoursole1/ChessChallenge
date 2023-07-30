using ChessChallenge.API;
using System.Collections.Generic;
using System;

public class MyBot : IChessBot
{

    // prep for 50 move game
    const int timePerMove = 900; //MS 1200
    static IDictionary <ulong, double> transposition = new Dictionary<ulong, double>(); // should be an array ;-;

    static double[] scores = {1, 3, 3.5, 5, 9};

    public Move Think(Board board, Timer timer)
    {
        Move best = Move.NullMove;
        for (int i = 1; timer.MillisecondsElapsedThisTurn < timePerMove; i++){
            Move result = Search(board, timer, Move.NullMove, i, int.MinValue, int.MaxValue, board.IsWhiteToMove ? 1 : -1).best;
            if (result == Move.NullMove){ // search failed time
                break;
            }
            best = result;
        }
        return best;
    }


    
    public SearchResult Search(Board board, Timer timer, Move lastMove, int depth, double a, double b, int color)
    {
        if (depth == 0 || board.IsInCheckmate() || board.IsDraw())
        {
            return new SearchResult(lastMove, color * StaticEvaluation(board));
        }

        if (timer.MillisecondsElapsedThisTurn >= timePerMove)
        {
            return new SearchResult(Move.NullMove, 0);
        }

        SearchResult bestResult = new SearchResult(Move.NullMove, int.MinValue);

        foreach (Move move in board.GetLegalMoves())
        {
            board.MakeMove(move);
            SearchResult result = Search(board, timer, move, depth - 1, -b, -a, -color);
            result.evaluation = -result.evaluation; 
            result.best = move;
            board.UndoMove(move);

            if (result.evaluation > bestResult.evaluation)
            {
                bestResult = result;
            }

            a = Math.Max(a, result.evaluation);
            if (b <= a)
            {
                break;
            }
        }

        if (timer.MillisecondsElapsedThisTurn >= timePerMove)
        {
            return new SearchResult(Move.NullMove, 0);
        }

        return bestResult;
    }


    private double StaticEvaluation(Board board)
    {
        double eval = 0;

        if (board.IsDraw())
        {
            return 0;
        }


        if (board.IsWhiteToMove)
        {
            if(board.IsInCheckmate()){
                return double.MinValue;
            }
            if(board.IsInCheck()){
                eval -= 0.5;
            }
        }

        if (!board.IsWhiteToMove)
        {
            if(board.IsInCheckmate()){
                return double.MaxValue;
            }
            if(board.IsInCheck()){
                eval += 0.5;
            }
        }



        eval += scores[0] * board.GetPieceList(PieceType.Pawn, true).Count +
                (scores[1] + (1/8) * board.GetPieceList(PieceType.Pawn, true).Count - 1) * board.GetPieceList(PieceType.Knight, true).Count +
                scores[2] * board.GetPieceList(PieceType.Bishop, true).Count +
                (scores[3] + (-1/8) * board.GetPieceList(PieceType.Pawn, true).Count + 1) * board.GetPieceList(PieceType.Rook, true).Count +
                scores[4] * board.GetPieceList(PieceType.Queen, true).Count;


        eval -= scores[0] * board.GetPieceList(PieceType.Pawn, false).Count +
                (scores[1] + (1/8) * board.GetPieceList(PieceType.Pawn, true).Count - 1) * board.GetPieceList(PieceType.Knight, false).Count +
                scores[2] * board.GetPieceList(PieceType.Bishop, false).Count +
                (scores[3] + (-1/8) * board.GetPieceList(PieceType.Pawn, true).Count + 1) * board.GetPieceList(PieceType.Rook, false).Count +
                scores[4] * board.GetPieceList(PieceType.Queen, false).Count;

        eval += pawnEval(board, true) + pawnEval(board, false) + developmentEval(board);

        return eval;
    }


    public double pawnEval(Board b, bool white){
        double eval = 0;
        foreach (Piece p in b.GetPieceList(PieceType.Pawn, white)){
            int min = 10;
            foreach (Piece c in b.GetPieceList(PieceType.Pawn, white)){
                min = Math.Min(min, Math.Abs(c.Square.File - p.Square.File));
            }

            if (min == 1){
                if (white){
                    eval += 0.2;
                }else{
                    eval -= 0.2;
                }
                
            }
        }
        return eval;
    }

    public double developmentEval(Board b){
        double eval = 0;

        if (b.GameMoveHistory.Length < 10){
            foreach (Piece p in b.GetPieceList(PieceType.Knight, true)){
            if(p.Square.Rank == 0){
                eval -= 1;
            }
            }
            foreach (Piece p in b.GetPieceList(PieceType.Knight, false)){
                if(p.Square.Rank == 7){
                    eval += 1;
                }
            }

            foreach (Piece p in b.GetPieceList(PieceType.Bishop, true)){
                if(p.Square.Rank == 0){
                    eval -= 0.5;
                }
            }
            foreach (Piece p in b.GetPieceList(PieceType.Bishop, false)){
                if(p.Square.Rank == 7){
                    eval += 0.5;
                }
            }
        }
        

        return eval;
    }
}

public class SearchResult {
    public Move best {get; set;}
    public double evaluation {get; set;}

    public SearchResult(Move best, double evaluation){
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