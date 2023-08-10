using ChessChallenge.API;
using System.Collections.Generic;
using System;

public class MyBot : IChessBot
{

    // prep for 50 move game
    const int timePerMove = 900; //MS 1200

    private readonly ulong[,] PST = {
        {2222222228888988866, 9678666444674443345, 1633333345333333003, 1332222222200000000},
        {5022222202333333223, 4999932239999322399, 1993223799932233333, 1320522225000000000},
        {3033333300333333013, 6344331155995514458, 1854444455444144554, 1410111111000000000},
        {1000000006669966600, 2000000000000000000, 9000000000000000000, 1000004440000000000},
        {5013333101344443134, 6777743347997433479, 1974334444443134444, 1310133331000000000},
        {3333333333333333333, 2333333333993333339, 1933333333333333003, 1333390039300000000}
    };

    private int[] PIECES = {100, 300, 350, 500, 900, 0};

    public Move Think(Board board, Timer timer)
    {
        Move best = Move.NullMove;
        if (timer.MillisecondsRemaining > 2000){
            for (int i = 1; i < 10; i++){
                Move result = Search(board, timer, Move.NullMove, i, double.MinValue, double.MaxValue, board.IsWhiteToMove ? 1 : -1).best;
                if (result == Move.NullMove){ // search failed time
                    break;
                }
                best = result;
            }
        }else{
            best =  Search(board, timer, Move.NullMove, 3, double.MinValue, double.MaxValue, board.IsWhiteToMove ? 1 : -1).best;
        }
        

        if(best.IsNull){
            best = board.GetLegalMoves()[0]; // silly hack to make not error lmao
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
            return new SearchResult(lastMove, color * Static_Evaluation(board));
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


    public int Static_Evaluation(Board board){
        int eval = 0;

        if (board.IsDraw())
        {
            return 0;
        }


        if (board.IsWhiteToMove)
        {
            if(board.IsInCheckmate()){
                return int.MinValue + 1;
            }
            if(board.IsInCheck()){
                eval -= 300;
            }
        }

        if (!board.IsWhiteToMove)
        {
            if(board.IsInCheckmate()){
                return int.MaxValue - 1;
            }
            if(board.IsInCheck()){
                eval += 300;
            }
        }

        foreach (PieceType type in Enum.GetValues(typeof(PieceType))){

            if (type == PieceType.None){ 
                continue;
            }
            ulong bitboardW = board.GetPieceBitboard(type, true);
            ulong b = board.GetPieceBitboard(type, false);

            ulong bitboardB = 0;

            for (int i = 0; i < 64; i++){
                if ((b & (1UL << i)) != 0){
                    int reversedIndex = 63 - i;
                    bitboardB |= 1UL << reversedIndex;
                }
            }


            eval += PIECES[(int)type - 1] * (board.GetPieceList(type, true).Count - board.GetPieceList(type, false).Count);



            for (int i = 63; i >= 0; i--){ // first bit read is bottom left
                int bitW = (int)((bitboardW >> i) & 1);
                int bitB = (int)((bitboardB >> i) & 1);

                int id = (int) Math.Ceiling((i + 0.9D) % 8 - 8 * Math.Floor((double) (i + 0.9) / 8) + 55);
                int subArray = (int) Math.Floor(4 * id / 72D);
                id -= subArray * 18 - 1;

                ulong pst = PST[(int)type - 1, subArray];
                int multiplier = (int) (pst / Math.Pow(10, 18 - id) % 10);

                multiplier -= PST[(int)type - 1, 2] / Math.Pow(10, 18) == 9 ? 0 : (int)(PST[(int)type - 1, 0] / (ulong) Math.Pow(10, 18));
                multiplier *= (int)(PST[(int)type - 1, 1] / (ulong) Math.Pow(10, 18));
                multiplier *= (board.GameMoveHistory.Length > 20) ? 0 : 1;

                eval += bitW * multiplier;
                eval -= bitB * multiplier;
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