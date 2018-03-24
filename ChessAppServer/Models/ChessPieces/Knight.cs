using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessCS.ChessPieces
{
    class Knight
    {
        static int[,] delta = { { -1, -2 }, { -2, -1 }, { -2, +1 }, { -1, +2 }, { 1, +2 }, { 2, 1 }, { +2, -1 }, { 1, -2 } };
        public static List<Move> generateMove(int x, int y, ChessBoard chessBoard)
        {
            List<Move> moves = new List<Move>();
            //Color of chess piece at [x,y]
            bool color = char.IsUpper(chessBoard.Board[x, y]);
            for (int i=0;i<delta.GetLength(0);i++)
            {
                int x_des = x + delta[i,0];
                int y_des = y + delta[i,1];
                //Console.WriteLine($"Knight move candidate [{x_des},{y_des}]");
                if (ChessBoard.IsValidCoordinate(x_des,y_des) && 
                    (chessBoard.Board[x_des,y_des]=='.' || chessBoard.CanCapture(x_des,y_des,color))) {
                    Move move = chessBoard.GetMove(x, y, x_des, y_des);
                    moves.Add(move);
                }
            }
            return moves;
        }
    }
}
