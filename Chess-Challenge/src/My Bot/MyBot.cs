using ChessChallenge.API;
using System.Collections.Generic;
using System;

public class MyBot : IChessBot
{

    // prep for 50 move game
    const int timePerMove = 900; //MS 1200

    static double[] scores = {1, 3, 3.5, 5, 9};

    public Move Think(Board board, Timer timer)
    {
        Move best = Move.NullMove;
        for (int i = 1; i < 10; i++){
            Move result = Search(board, timer, Move.NullMove, i, double.MinValue, double.MaxValue, board.IsWhiteToMove ? 1 : -1).best;
            if (result == Move.NullMove){ // search failed time
                break;
            }
            best = result;
        }

        if(best.IsNull){
            best = board.GetLegalMoves()[0];
        }
        return best;
    }
    
    static Dictionary<ulong, KeyValuePair<double, int>> tt = new Dictionary<ulong, KeyValuePair<double, int>>();
    public SearchResult Search(Board board, Timer timer, Move lastMove, int depth, double a, double b, int color)
    {
        
        ulong positionHash = board.ZobristKey;

        if (tt.TryGetValue(positionHash, out var ttEntry) && !lastMove.IsNull)
        {
            if (ttEntry.Value >= depth)
            {
                return new SearchResult(lastMove, ttEntry.Key);
            }
        }
        
        if (depth <= 0 || board.IsInCheckmate() || board.IsDraw())
        {
            return new SearchResult(lastMove, color * StaticEvaluation(board));
        }

        if (timer.MillisecondsElapsedThisTurn >= timePerMove)
        {
            return new SearchResult(Move.NullMove, 0);
        }

        SearchResult bestResult = new SearchResult(Move.NullMove, int.MinValue);

        List<Move> legalMoves = new List<Move>(board.GetLegalMoves());
        legalMoves.Sort((m1, m2) =>
        {
            if (m1.IsCapture && !m2.IsCapture)
                return -1;
            if (!m1.IsCapture && m2.IsCapture)
                return 1;
            return 0;
        });

        foreach (Move move in legalMoves)
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

        tt[positionHash] = new KeyValuePair<double, int>(bestResult.evaluation, depth);


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

        eval += developmentEval(board);

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