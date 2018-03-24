using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessCS.ChessPieces
{
    class Rook
    {
        static int[,] delta = { { 1, 0 }, { -1, 0 }, { 0, 1 }, { 0, -1 } }; //only move vertically and horizontally
        public static List<Move> generateMove(int x, int y, ChessBoard chessBoard)
        {
            List<Move> moves = new List<Move>();
            //Color of chess piece at [x,y]
            bool color = char.IsUpper(chessBoard.Board[x, y]);
            for (int i = 0; i < delta.GetLength(0); i++)
            {
                int step = 1;
                while (true)
                {
                    int x_des = x + delta[i,0] * step;
                    int y_des = y + delta[i,1] * step;
                    //Console.WriteLine($"Rook move candidate [{x_des},{y_des}]");
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
