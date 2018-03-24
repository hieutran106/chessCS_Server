using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessCS.ChessPieces
{
    class Pawn :ChessPiece
    {
        public static List<Move> generateMove(int x, int y, ChessBoard chessBoard)
        {
            List<Move> moves = new List<Move>();
            //Color of chess piece at [x,y]
            bool color = char.IsUpper(chessBoard.Board[x, y]);
            int dx = (color == ChessPiece.BLACK) ? 1 : -1;

             
            //Move ahead, no capture
            if (ChessBoard.IsValidCoordinate(x+dx, y) && chessBoard.Board[x+dx, y] == '.')
            {
                Move move = chessBoard.GetMove(x, y, x+dx, y);
                //promotion
                if ((color==BLACK && x==6)|| (color==WHITE && x==1)) {
                    move.PawnPromotion = true;
                }
                moves.Add(move);
            }
            
            
            //capture, diagonally forward one square to the left or right
            //dy = -1 : to the left; dy = 1: to the right
            for (int dy=-1;dy<=1;dy=dy+2)
            {
                int x_des = x + dx;
                int y_des = y + dy;
                if (ChessBoard.IsValidCoordinate(x_des,y_des) && chessBoard.CanCapture(x_des,y_des,color))
                {
                    Move move = chessBoard.GetMove(x, y, x_des, y_des);
                    //promotion
                    if ((color==BLACK && x == 6) || (color==WHITE && x == 1))
                    {
                        move.PawnPromotion = true;
                    }
                    moves.Add(move);
                }
            }
            //Move two squares from starting point
            if (color==WHITE)
            {
                if (x==6 && chessBoard.Board[5,y]=='.' && chessBoard.Board[4,y]=='.')
                {
                    Move move = chessBoard.GetMove(x, y, 4, y );
                    moves.Add(move);
                }
            } else //color = BLACK
            {
                if (x == 1 && chessBoard.Board[2, y] == '.' && chessBoard.Board[3, y] == '.')
                {
                    Move move = chessBoard.GetMove(x, y, 3, y);
                    moves.Add(move);
                }
            }
            return moves;
            
        }
    }
}
