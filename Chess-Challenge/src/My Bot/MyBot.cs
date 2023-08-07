using ChessChallenge.API;
using System;


public class MyBot : IChessBot
{

    // {first bit is the 0 number, first bit is the multiplier, 9 on first bit means the 0 value is really 0, so ignore first bit of a[0], first bit no meaning (last 8 bits no meaning)}

    private readonly ulong[,] PST = {
        {2222222228888988866, 9678666444674443345, 1633333345333333003, 1332222222200000000},
        {5022222202333333223, 4999932239999322399, 1993223799932233333, 1320522225000000000},
        {3033333300333333013, 6344331155995514458, 1854444455444144554, 1410111111000000000},
        {1000000006669966600, 2000000000000000000, 9000000000000000000, 1000004440000000000},
        {5013333101344443134, 6777743347997433479, 1974334444443134444, 1310133331000000000},
        {3333333333333333333, 2333333333993333339, 1933333333333333003, 1333390039300000000}
    };
    // To be used when the enemy king is all alone (or with only pawns left maybe, to push the king to the sides of the board)
    // private ulong[] KING_EDGE_PST = {};
    private int[] PIECES = {100, 300, 350, 500, 900, 0};


    public Move Think(Board board, Timer timer)
    {
        Console.WriteLine(Static_Evaluation(board));
        return board.GetLegalMoves()[0];
    }


    public int Static_Evaluation(Board board){
        int eval = 0;

        foreach (PieceType type in Enum.GetValues(typeof(PieceType))){

            if (type == PieceType.None){ 
                continue;
            }
            ulong bitboardW = board.GetPieceBitboard(type, true);
            ulong b = board.GetPieceBitboard(type, false);
            ulong bitboardB = 0;

            while (b != 0)
            {
                bitboardB <<= 1;          
                bitboardB |= b & 1;      
                b >>= 1;             
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
                eval += bitW * multiplier;
                eval -= bitB * multiplier;
            }

        }
        
        return eval;
    }

}