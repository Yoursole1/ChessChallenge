using ChessChallenge.API;
using System;

public class MyBot : IChessBot
{
    private static double[] scores = {1, 3, 3.5, 5, 8}; // pawn, knight, bishop, rook, queen

    public Move Think(Board board, Timer timer)
    {
        
        Move[] moves = board.GetLegalMoves();
        Console.WriteLine(staticEvaluation(board));
        return moves[0];
    }

    private Move search(Board board){


        return new Move();
    }

    private double staticEvaluation(Board board){
        double eval = 0;

        if (board.IsDraw()) {
            return 0;
        }

        if (board.IsWhiteToMove && board.IsInCheck() && board.IsInCheckmate()){
            return int.MaxValue;
        }
        if (!board.IsWhiteToMove && board.IsInCheck() && board.IsInCheckmate()){
            return int.MinValue;
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
    }
}