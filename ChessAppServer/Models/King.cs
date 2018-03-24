using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessCS.ChessPieces
{
    class King
    {
        static int[,] delta = { 
            { -1, -1 }, { 0, -1 }, { +1, -1 },
            { -1, 0 }, { +1, 0 }, 
            { -1, +1 }, { 0, +1 }, { +1, +1 } };
        public static List<Move> generateMove(int x, int y, ChessBoard chessBoard)
        {
            List<Move> moves = new List<Move>();
            //Color of chess piece at [x,y]
            bool color = char.IsUpper(chessBoard.Board[x, y]);
            //GetLeng(0) - get length of dimension 0
            for (int i = 0; i < delta.GetLength(0); i++)
            {
                int x_des = x + delta[i, 0];
                int y_des = y + delta[i, 1];
                if (ChessBoard.IsValidCoordinate(x_des, y_des) &&
                    (chessBoard.Board[x_des,y_des]=='.'||chessBoard.CanCapture(x_des,y_des,color)))
                {
                    Move move = chessBoard.GetMove(x, y, x_des, y_des);
                    moves.Add(move);
                }
            }
            return moves;
        }
        public static bool IsKingSafe(ChessBoard chessBoard)
        {
            return true;
        }
    }
}
