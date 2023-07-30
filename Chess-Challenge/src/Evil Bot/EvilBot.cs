using System;
using ChessChallenge.API;

namespace ChessChallenge.Example
{

    public class EvilBot : IChessBot
    {
        private static double[] scores = { 1, 3, 3.5, 5, 10 }; // pawn, knight, bishop, rook, queen

    public Move Think(Board board, Timer timer)
    {
        bool maximizer = board.IsWhiteToMove;
        Move[] moves = board.GetLegalMoves();

        Move bestMove = moves[0];
        double bestEval = maximizer ? double.MinValue : double.MaxValue;

        foreach (Move m in moves)
        {
            double e = eval(m, board, timer.MillisecondsRemaining / 1000);
            if (maximizer && e >= bestEval)
            {
                bestMove = m;
                bestEval = e;
            }
            if (!maximizer && e <= bestEval)
            {
                bestMove = m;
                bestEval = e; 
            }
        }


        return bestMove;
    }



    private double search(Board board, int depth, double a, double b, bool maximizer)
    {
        if (depth == 0 || board.IsInCheckmate() || board.IsDraw())
        {
            return staticEvaluation(board);
        }

        if (maximizer)
        {
            double max = double.MinValue;
            foreach (Move move in board.GetLegalMoves())
            {
                board.MakeMove(move);
                double eval = search(board, depth - 1, a, b, false);
                board.UndoMove(move);
                max = Math.Max(max, eval);

                a = Math.Max(a, eval);
                if (b <= a)
                {
                    break;
                }
            }
            return max;
        }
        else
        {
            double min = double.MaxValue;
            foreach (Move move in board.GetLegalMoves())
            {
                board.MakeMove(move);
                double eval = search(board, depth - 1, a, b, true);
                board.UndoMove(move);
                min = Math.Min(min, eval);

                b = Math.Min(b, eval);
                if (b <= a)
                {
                    break;
                }
            }
            return min;
        }
    }

    private double eval(Move m, Board b, int time)
    {
        b.MakeMove(m);
        int depth = (time < 20) ? (time < 2) ? 2 : 3  : 4;
        double evaluation = search(b, depth, double.MinValue, double.MaxValue, b.IsWhiteToMove);
        b.UndoMove(m);
        return evaluation;
    }

    private double staticEvaluation(Board board)
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
                eval -= 3;
            }
        }

        if (!board.IsWhiteToMove)
        {
            if(board.IsInCheckmate()){
                return double.MaxValue;
            }
            if(board.IsInCheck()){
                eval += 3;
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

        eval += kingEval(board) + developmentEval(board);

        return eval;
    }

    private double developmentEval(Board b){
        double eval = 0;

        // foreach (Piece p in b.GetPieceList(PieceType.Knight, true)){
        //     if(p.Square.Rank == 0){
        //         eval -= 1;
        //     }
        // }
        // foreach (Piece p in b.GetPieceList(PieceType.Knight, false)){
        //     if(p.Square.Rank == 7){
        //         eval += 1;
        //     }
        // }

        // foreach (Piece p in b.GetPieceList(PieceType.Bishop, true)){
        //     if(p.Square.Rank == 0){
        //         eval -= 0.5;
        //     }
        // }
        // foreach (Piece p in b.GetPieceList(PieceType.Bishop, false)){
        //     if(p.Square.Rank == 7){
        //         eval += 0.5;
        //     }
        // }

        foreach (Piece p in b.GetPieceList(PieceType.Pawn, true)){
            int min = 10;
            foreach (Piece c in b.GetPieceList(PieceType.Pawn, true)){
                min = Math.Min(min, Math.Abs(c.Square.File - p.Square.File));
            }

            if (min == 1){
                eval += 0.2;
            }
        }
        
        foreach (Piece p in b.GetPieceList(PieceType.Knight, false)){
            int min = 10;
            foreach (Piece c in b.GetPieceList(PieceType.Pawn, true)){
                min = Math.Min(min, Math.Abs(c.Square.File - p.Square.File));
            }

            if (min == 1){
                eval -= 0.2;
            }
        }

        return eval;
    }

    private double kingEval(Board b){
        if (isEndgame(b)){
            return 0;
        }

        double eval = 0;
        if (!isEndgame(b)){
            eval += 0.5 * Math.Abs(b.GetKingSquare(true).File - 3.5) - 0.5 * Math.Abs(b.GetKingSquare(false).File - 3.5); // move king to sides
        }
        
        
        if (b.GetKingSquare(true).Rank > 0) {
            eval -= 1;
        }
        if (b.GetKingSquare(false).Rank < 7) {
            eval += 1;
        }

        return eval;
    }


    private bool isEndgame(Board b){
        int v = 2 * (b.GetPieceList(PieceType.Pawn, true).Count + b.GetPieceList(PieceType.Pawn, false).Count) + 
                b.GetPieceList(PieceType.Knight, true).Count + b.GetPieceList(PieceType.Knight, false).Count + 
                b.GetPieceList(PieceType.Bishop, true).Count + b.GetPieceList(PieceType.Bishop, false).Count +
                b.GetPieceList(PieceType.Rook, true).Count + b.GetPieceList(PieceType.Rook, false).Count +
                b.GetPieceList(PieceType.Queen, true).Count + b.GetPieceList(PieceType.Queen, false).Count;

        return v < 30; // arbitrary value picked because it works
    }
    }
}