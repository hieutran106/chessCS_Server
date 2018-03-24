using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessCS.ChessPieces
{
    class Bishop
    {
        public static List<Move> generateMove(int x, int y, ChessBoard chessBoard)
        {
            List<Move> moves = new List<Move>();
            //Color of chess piece at [x,y]
            bool color = char.IsUpper(chessBoard.Board[x, y]);
            for (int i = -1; i <= 1; i=i+2)
                for (int j = -1; j <= 1; j=j+2)
                {
                    //only move in diagonal lines
                    int step = 1;                   
                    while (true)
                    {
                        int x_des = x + i * step;
                        int y_des = y + j * step;
                        //Console.WriteLine($"Bishop move candidate [{x_des},{y_des}]");
                        if (ChessBoard.IsValidCoordinate(x_des, y_des))
                        {
                            if (chessBoard.Board[x_des, y_des] == '.')
                            {
                                Move move = new Move(x, y, x_des, y_des, chessBoard);
                                moves.Add(move);
                                step++;
                            }
                            else if (chessBoard.CanCapture(x_des, y_des, color))
                            {
                                Move move = new Move(x, y, x_des, y_des, chessBoard);
                                moves.Add(move);
                                break;
                            }
                            else break;
                        }
                        else break;
                    }
                }
            return moves;
        }
    }
}
