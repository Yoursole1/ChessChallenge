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
        for (int i = 1; timer.MillisecondsElapsedThisTurn < timePerMove; i++){
            Move result = Search(board, timer, Move.NullMove, i, int.MinValue, int.MaxValue, board.IsWhiteToMove ? 1 : -1).best;
            if (result == Move.NullMove){ // search failed time
                break;
            }
            best = result;
        }
        return best;
    }
    
    static Dictionary<ulong, KeyValuePair<double, int>> tt = new Dictionary<ulong, KeyValuePair<double, int>>();
    public SearchResult Search(Board board, Timer timer, Move lastMove, int depth, double a, double b, int color)
    {
        ulong positionHash = board.ZobristKey;

        if (tt.ContainsKey(positionHash))
        {
            var ttEntry = tt[positionHash];

            if (ttEntry.Value >= depth)
            {
                double storedEvaluation = ttEntry.Key;

                if (storedEvaluation >= b)
                    return new SearchResult(lastMove, b);
                if (storedEvaluation <= a)
                    return new SearchResult(lastMove, a);

                a = Math.Max(a, storedEvaluation);
            }
        }
        
        if (depth == 0 || board.IsInCheckmate() || board.IsDraw())
        {
            return new SearchResult(lastMove, color * StaticEvaluation(board));
        }

        if (timer.MillisecondsElapsedThisTurn >= timePerMove)
        {
            return new SearchResult(Move.NullMove, 0);
        }

        SearchResult bestResult = new SearchResult(Move.NullMove, int.MinValue);

        List<Move> legalMoves = new List<Move>(board.GetLegalMoves());

        legalMoves.Sort((move1, move2) =>
        {
            board.MakeMove(move1);
            ulong hash1 = board.ZobristKey;
            board.UndoMove(move1);

            board.MakeMove(move2);
            ulong hash2 = board.ZobristKey;
            board.UndoMove(move2);

            double eval1 = tt.ContainsKey(hash1) ? tt[hash1].Key : 0;
            double eval2 = tt.ContainsKey(hash2) ? tt[hash2].Key : 0;

            return eval2.CompareTo(eval1);
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

        if (timer.MillisecondsElapsedThisTurn >= timePerMove)
        {
            return new SearchResult(Move.NullMove, 0);
        }

        tt[positionHash] = new KeyValuePair<double, int>(bestResult.evaluation, depth);
        
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