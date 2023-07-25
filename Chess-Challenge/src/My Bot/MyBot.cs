using ChessChallenge.API;
using System;

public class MyBot : IChessBot
{
<<<<<<< HEAD
    private static double[] scores = { 1, 3, 3.5, 5, 8 }; // pawn, knight, bishop, rook, queen
=======
    private static double[] scores = {1, 3, 3.5, 5, 8}; // pawn, knight, bishop, rook, queen
>>>>>>> 983bdf539847467c8d677c24dbdf54ed1b769e74

    public Move Think(Board board, Timer timer)
    {
        bool maximizer = board.IsWhiteToMove;
        Move[] moves = board.GetLegalMoves();
<<<<<<< HEAD

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
=======
        
        Move bestMove = moves[0];
        double bestEval = maximizer ? double.MinValue : double.MaxValue;

        foreach (Move m in moves){
            double e = eval(m, board, timer.MillisecondsRemaining / 1000);
            if (maximizer && e >= bestEval){
                bestMove = m;
                bestEval = e;
            }
            if(!maximizer && e <= bestEval){
>>>>>>> 983bdf539847467c8d677c24dbdf54ed1b769e74
                bestMove = m;
                bestEval = e;
            }
        }


        return bestMove;
    }

<<<<<<< HEAD


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
=======
    

    private double search(Board board, int depth, double a, double b, bool maximizer){
        if (depth == 0 || board.IsInCheckmate() || board.IsDraw()){
            return staticEvaluation(board);
        }

        if (maximizer){
            double max = double.MinValue;
            foreach (Move move in board.GetLegalMoves()){
>>>>>>> 983bdf539847467c8d677c24dbdf54ed1b769e74
                board.MakeMove(move);
                double eval = search(board, depth - 1, a, b, false);
                board.UndoMove(move);
                max = Math.Max(max, eval);

                a = Math.Max(a, eval);
<<<<<<< HEAD
                if (b <= a)
                {
=======
                if (b <= a){
>>>>>>> 983bdf539847467c8d677c24dbdf54ed1b769e74
                    break;
                }
            }
            return max;
<<<<<<< HEAD
        }
        else
        {
            double min = double.MaxValue;
            foreach (Move move in board.GetLegalMoves())
            {
=======
        } else {
            double min = double.MaxValue;
            foreach (Move move in board.GetLegalMoves()){
>>>>>>> 983bdf539847467c8d677c24dbdf54ed1b769e74
                board.MakeMove(move);
                double eval = search(board, depth - 1, a, b, true);
                board.UndoMove(move);
                min = Math.Min(min, eval);

                b = Math.Min(b, eval);
<<<<<<< HEAD
                if (b <= a)
                {
=======
                if (b <= a){
>>>>>>> 983bdf539847467c8d677c24dbdf54ed1b769e74
                    break;
                }
            }
            return min;
        }
<<<<<<< HEAD
=======
    }

    private double eval(Move m, Board b, int time){
        b.MakeMove(m);
        int depth = (time < 20) ? 3 : 4;
        double evaluation = search(b, depth, double.MinValue, double.MaxValue, b.IsWhiteToMove);
        b.UndoMove(m);
        return evaluation;
    }

    private double staticEvaluation(Board board){
        double eval = 0;

        if (board.IsDraw()) {
            return 0;
        }


        if (board.IsWhiteToMove && board.IsInCheckmate()){
            return double.MinValue;
        }

        if (!board.IsWhiteToMove && board.IsInCheckmate()){
            return double.MaxValue;
        }

        eval += scores[0] * board.GetPieceList(PieceType.Pawn, true).Count + 
                scores[1] * board.GetPieceList(PieceType.Knight, true).Count+ 
                scores[2] * board.GetPieceList(PieceType.Bishop, true).Count+ 
                scores[3] * board.GetPieceList(PieceType.Rook, true).Count+ 
                scores[4] * board.GetPieceList(PieceType.Queen, true).Count;
        

        eval -= scores[0] * board.GetPieceList(PieceType.Pawn, false).Count + 
                scores[1] * board.GetPieceList(PieceType.Knight, false).Count+ 
                scores[2] * board.GetPieceList(PieceType.Bishop, false).Count+ 
                scores[3] * board.GetPieceList(PieceType.Rook, false).Count+ 
                scores[4] * board.GetPieceList(PieceType.Queen, false).Count;

        return eval;
>>>>>>> 983bdf539847467c8d677c24dbdf54ed1b769e74
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


        if (board.IsWhiteToMove && board.IsInCheckmate())
        {
            return double.MinValue;
        }

        if (!board.IsWhiteToMove && board.IsInCheckmate())
        {
            return double.MaxValue;
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

        // eval += pawnEval(board) + knightEval(board) + bishopEval(board) + rookEval(board) + queenEval(board) + kingEval(board);

        return eval;
    }

    // private double pawnEval(Board b){

    // }
    // private double knightEval(Board b){

    // }
    // private double bishopEval(Board b){

    // }
    // private double rookEval(Board b){

    // }
    // private double queenEval(Board b){

    // }
    // private double kingEval(Board b){

    // }
}