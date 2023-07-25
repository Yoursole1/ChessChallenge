using System;
using System.Collections.Generic;
using System.Linq;
using ChessChallenge.API;

namespace ChessChallenge.Example
{
<<<<<<< HEAD

public class EvilBot : IChessBot
{
    private readonly Dictionary<int, List<Move>> _killerMoves = new();

// Initialize the transposition table
    private readonly Dictionary<ulong, TranspositionTableEntry> _transpositionTable = new();

    public Move Think(Board board, Timer timer)
    {
        // Set a depth limit for the search
        var maxDepth = 5;

        // Clear the Killer Heuristic table for each new search
        _killerMoves.Clear();

        var bestMove = Move.NullMove;

        for (var depth = 1; depth <= maxDepth; depth++)
        {
            var result = MiniMaxAlphaBeta(board, depth, int.MinValue, int.MaxValue, board.IsWhiteToMove);
            bestMove = result.Move;
        }

        return bestMove;
    }

    private EvaluationResult MiniMaxAlphaBeta(Board board, int depth, int alpha, int beta, bool isMaximizing)
    {
        if (depth == 0 || board.IsDraw() || board.IsInCheckmate())
            return new EvaluationResult(Evaluate(board), Move.NullMove);
        
        if (_transpositionTable.TryGetValue(board.ZobristKey, out var entry) && entry.Depth >= depth)
        {
            switch (entry.NodeType)
            {
                case TranspositionNodeType.Exact:
                    return new EvaluationResult(entry.Evaluation, entry.BestMove);
                case TranspositionNodeType.LowerBound when entry.Evaluation > alpha:
                    alpha = entry.Evaluation;
                    break;
                case TranspositionNodeType.UpperBound when entry.Evaluation < beta:
                    beta = entry.Evaluation;
                    break;
            }

            if (alpha >= beta)
                return new EvaluationResult(entry.Evaluation, entry.BestMove);
        }

        var legalMoves = board.GetLegalMoves().ToList();
        var bestEval = isMaximizing ? int.MinValue : int.MaxValue;
        var bestMove = Move.NullMove;
        var killerMoveList = _killerMoves.TryGetValue(depth, out var killerMove) ? killerMove : new List<Move>();
        foreach (var move in killerMoveList.Where(move => legalMoves.Contains(move)))
        {
            legalMoves.Remove(move);
            legalMoves.Insert(0, move);
        }
        
        
        foreach (var move in legalMoves.Where(move => move != bestMove))
        {
            board.MakeMove(move);
            var eval = MiniMaxAlphaBeta(board, depth - 1, alpha, beta, !isMaximizing).Evaluation;
            board.UndoMove(move);

            if (isMaximizing)
            {
                if (eval > bestEval)
                {
                    bestEval = eval;
                    bestMove = move;
=======
    // A simple bot that can spot mate in one, and always captures the most valuable piece it can.
    // Plays randomly otherwise.
    public class EvilBot : IChessBot
{
    private static double[] scores = {1, 3, 3.5, 5, 8}; // pawn, knight, bishop, rook, queen

    public Move Think(Board board, Timer timer)
    {
        bool maximizer = board.IsWhiteToMove;
        Move[] moves = board.GetLegalMoves();
        
        Move bestMove = moves[0];
        double bestEval = maximizer ? double.MinValue : double.MaxValue;

        foreach (Move m in moves){
            double e = eval(m, board, timer.MillisecondsRemaining / 1000);
            if (maximizer && e >= bestEval){
                bestMove = m;
                bestEval = e;
            }
            if(!maximizer && e <= bestEval){
                bestMove = m;
                bestEval = e;
            }
        }


        return bestMove;
    }

    

    private double search(Board board, int depth, double a, double b, bool maximizer){
        if (depth == 0 || board.IsInCheckmate() || board.IsDraw()){
            return staticEvaluation(board);
        }

        if (maximizer){
            double max = double.MinValue;
            foreach (Move move in board.GetLegalMoves()){
                board.MakeMove(move);
                double eval = search(board, depth - 1, a, b, false);
                board.UndoMove(move);
                max = Math.Max(max, eval);

                a = Math.Max(a, eval);
                if (b <= a){
                    break;
                }
            }
            return max;
        } else {
            double min = double.MaxValue;
            foreach (Move move in board.GetLegalMoves()){
                board.MakeMove(move);
                double eval = search(board, depth - 1, a, b, true);
                board.UndoMove(move);
                min = Math.Min(min, eval);

                b = Math.Min(b, eval);
                if (b <= a){
                    break;
>>>>>>> 983bdf539847467c8d677c24dbdf54ed1b769e74
                }

                alpha = Math.Max(alpha, eval);
            }
<<<<<<< HEAD
            else
            {
                if (eval < bestEval)
                {
                    bestEval = eval;
                    bestMove = move;
                }

                beta = Math.Min(beta, eval);
            }

            if (beta <= alpha)
                break;
        }

        // Store the evaluated position in the transposition table
        var nodeType = bestEval <= alpha ? TranspositionNodeType.UpperBound :
            bestEval >= beta ? TranspositionNodeType.LowerBound :
            TranspositionNodeType.Exact;

        _transpositionTable[board.ZobristKey] = new TranspositionTableEntry(depth, bestEval, bestMove, nodeType);

        // Update the Killer Heuristic table with the new best move for this depth
        if (bestMove == Move.NullMove) return new EvaluationResult(bestEval, bestMove);
        if (!_killerMoves.ContainsKey(depth))
            _killerMoves[depth] = new List<Move>();

        if (_killerMoves[depth].Contains(bestMove)) return new EvaluationResult(bestEval, bestMove);
        // Limit the number of killer moves to store to, say, two
        if (_killerMoves[depth].Count >= 2)
            _killerMoves[depth].RemoveAt(0);

        _killerMoves[depth].Add(bestMove);

        return new EvaluationResult(bestEval, bestMove);
    }

    private int Evaluate(Board board)
    {
        if (board.IsWhiteToMove && board.IsInCheckmate()) return int.MinValue;
        if (!board.IsWhiteToMove && board.IsInCheckmate()) return int.MaxValue;

        var score = 0;

        if (board.IsRepeatedPosition()) return 0;

        // Evaluate material count
        score += CountMaterial(board, true) - CountMaterial(board, false);

        // Evaluate pawn structure
        score += EvaluatePawnStructure(board);

        // Evaluate king safety
        score += EvaluateKingSafety(board);

        return score;
    }

    private int CountMaterial(Board board, bool isWhite)
    {
        var pieceLists = board.GetAllPieceLists();

        return (from pieceList in pieceLists
            where pieceList.IsWhitePieceList == isWhite
            let pieceCount = pieceList.Count
            select pieceCount * PieceValue(pieceList.TypeOfPieceInList)).Sum();
    }

    private int PieceValue(PieceType pieceType)
    {
        // You can assign values to each piece type as per their relative strength
        // Example values:
        switch (pieceType)
        {
            case PieceType.Pawn: return 1;
            case PieceType.Knight: return 3;
            case PieceType.Bishop: return 3;
            case PieceType.Rook: return 5;
            case PieceType.Queen: return 9;
            case PieceType.King: return 100; // A high value to prioritize keeping the king safe
            default: return 0;
=======
            return min;
        }
    }

    private double eval(Move m, Board b, int time){
        b.MakeMove(m);
        int depth = (time < 30) ? 3 : 4;
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
>>>>>>> 983bdf539847467c8d677c24dbdf54ed1b769e74
        }
    }

    private int EvaluatePawnStructure(Board board)
    {
        // Implement your pawn structure evaluation here
        // You can consider factors like doubled pawns, isolated pawns, passed pawns, etc.
        var pawnStructureScore = 0;

        // Example: Penalize doubled pawns for both sides
        pawnStructureScore -= 10 * CountDoubledPawns(board, true);
        pawnStructureScore -= 10 * CountDoubledPawns(board, false);

<<<<<<< HEAD
        return pawnStructureScore;
    }

    private int CountDoubledPawns(Board board, bool isWhite)
    {
        var doubledPawnsCount = 0;
        var pawnFiles = new HashSet<int>(); // To store files with pawns for the current color

        foreach (var piece in board.GetPieceList(PieceType.Pawn, isWhite))
        {
            var file = piece.Square.File;
            if (pawnFiles.Contains(file))
                doubledPawnsCount++;
            else
                pawnFiles.Add(file);
        }

        return doubledPawnsCount;
    }


// Evaluate king safety
    private int EvaluateKingSafety(Board board)
    {
        // Implement your king safety evaluation here
        // Consider factors like pawn shield, open lines around the king, etc.
        var kingSafetyScore = 0;

        // Example: Penalize if the king is exposed with no pawn shield
        if (IsKingExposed(board, true))
            kingSafetyScore -= 20;

        if (IsKingExposed(board, false))
            kingSafetyScore += 20;

        return kingSafetyScore;
    }

// Check if the king of a given fileor is exposed
    private bool IsKingExposed(Board board, bool isWhite)
    {
        // Get the position of the king for the given color
        var kingSquare = board.GetKingSquare(isWhite);

        // Check for open lines around the king
        // (Example: In this simple implementation, we check if the squares adjacent to the king are attacked by enemy pieces)

        // Check if the squares adjacent to the king are attacked by enemy pieces
        var file = kingSquare.File;
        var rank = kingSquare.Rank;

        var isExposed = false;

        // Check the squares around the king for attacks from enemy pieces
        for (var f = file - 1; f <= file + 1; f++)
        {
            for (var r = rank - 1; r <= rank + 1; r++)
            {
                if (f is < 0 or >= 8 || r is < 0 or >= 8 || (f == file && r == rank)) continue;
                var targetSquare = new Square(f, r);
                var pieceOnSquare = board.GetPiece(targetSquare);

                if (pieceOnSquare.IsWhite == isWhite || !board.SquareIsAttackedByOpponent(targetSquare)) continue;
                isExposed = true;
                break;
                // The king is exposed
            }

            if (isExposed)
                break;
        }

        return isExposed;
    }


    private sealed record EvaluationResult(int Evaluation, Move Move);

    // Transposition table entry to store the evaluated position
    private sealed record TranspositionTableEntry(int Depth, int Evaluation, Move BestMove,
        TranspositionNodeType NodeType);

    // Enum to define the node type in the transposition table
    private enum TranspositionNodeType
    {
        Exact,
        LowerBound,
        UpperBound
=======
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
}
}