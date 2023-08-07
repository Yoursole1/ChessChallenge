using ChessChallenge.API;
using System;


public class MyBot : IChessBot
{

    // {first bit is the 0 number, first bit is the multiplier, 9 on first bit means the 0 value is really 0, so ignore first bit of a[0], first bit no meaning (last 8 bits no meaning)}
    private ulong[] KING_PST = {3333333333333333333, 2333333333993333339, 1933333333333333003, 1333390039300000000}; 
    private ulong[] QUEEN_PST = {5013333101344443134, 6777743347997433479, 1974334444443134444, 1310133331000000000};
    private ulong[] ROOK_PST = {1000000006669966600, 2000000000000000000, 9000000000000000000, 1000004440000000000};
    private ulong[] BISHOP_PST = {3033333300333333013, 6344331155995514458, 1854444455444144554, 1410111111000000000};
    private ulong[] KNIGHT_PST = {5022222202333333223, 4999932239999322399, 1993223799932233333, 1320522225000000000};
    private ulong[] PAWN_PST = {2222222228888988866, 9678666444674443345, 1633333345333333003, 1332222222200000000};
    // To be used when the enemy king is all alone (or with only pawns left maybe, to push the king to the sides of the board)
    // private ulong[] KING_EDGE_PST = {};
    private int[] PIECES = {100, 300, 350, 500, 900};


    public Move Think(Board board, Timer timer)
    {
        Static_Evaluation(board);
        return board.GetLegalMoves()[0];
    }


    public int Static_Evaluation(Board board){
        int eval = 0;

        foreach (PieceType type in Enum.GetValues(typeof(PieceType))){

            if (type == PieceType.None){ // find way to make this more compact
                continue;
            }
            ulong bitboardW = board.GetPieceBitboard(type, true);
            ulong bitboardB = board.GetPieceBitboard(type, false);

            eval += PIECES[(int)type + 1] * (board.GetPieceList(type, true).Count - board.GetPieceList(type, false).Count);

            

        }
        
        return eval;
    }

}