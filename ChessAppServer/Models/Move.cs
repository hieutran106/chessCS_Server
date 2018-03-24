using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessCS
{
    public class Move 
    {
        public int X_Des { get; set; }
        public int Y_Des { get; set; }
        public int X_Src { get; set; }
        public int Y_Src { get; set; }
        public int Src { get; set; }
        public int Dst { get; set; }

        public char Piece { get; private set; }
        public char Capture { get; private set; }
        public bool PawnPromotion { get; set; }
        //use for move ordering
        public int Value { get; set; }
        private static Dictionary<char, int> mvvLva = new Dictionary<char, int>()
            {
                { 'P',100},
                { 'N',200},
                { 'B',200},
                { 'R',300},
                { 'Q',500},
                { 'K',600},
            }; // Most value victim, least value attacker
        public Move(int x_src, int y_src, int x_dst, int y_dst, ChessBoard chessBoard)
        {
            X_Src = x_src;
            Y_Src = y_src;
            X_Des = x_dst;
            Y_Des = y_dst;
            //
            Src = x_src * 8 + y_src;
            Dst = x_dst * 8 + y_dst;
            //
            Piece = chessBoard.Board[x_src, y_src];
            Capture = chessBoard.Board[x_dst, y_dst];
            if (chessBoard.Board[x_src, y_src] == 'P')
            {
                if (x_dst == 0)
                {
                    PawnPromotion = true;
                }
            }
            else if (chessBoard.Board[x_src, y_src] == 'p')
            {
                if (x_dst == 7)
                {
                    PawnPromotion = true;
                }
            }
            
        }
        public void Evaluate()
        {
            this.Value = EvaluateMvvLva();
        }
        private int EvaluateMvvLva()
        {
            int score = 0;
            if (Capture != '.')
            {
                score = Move.mvvLva[char.ToUpper(Capture)] + 6 - Move.mvvLva[char.ToUpper(Piece)] / 100;
            }
            return score;
        }
        public static string PositionFromCoordinate(int x, int y)
        {

            StringBuilder position = new StringBuilder(2);
            position.Append((char)(x + 97));
            position.Append((char)(56 - y));
            return position.ToString();
        }
        public override string ToString()
        {
            StringBuilder move = new StringBuilder(40);
            move.Append($"{Piece}:[{X_Src},{Y_Src}]-[{X_Des},{Y_Des}]");
            if (Capture == '.')
            {
                move.Append(" --");
            }
            else
            {
                move.Append($" x{Capture}");
            }
            if (PawnPromotion)
            {
                move.Append("=Q");
            }
            else move.Append("  ");
            move.Append(" Val:" + Value);
            return move.ToString();
        }
        public bool Equals(Move other)
        {
            if (X_Src == other.X_Src &&
                Y_Src == other.Y_Src &&
                X_Des == other.X_Des &&
                Y_Des == other.Y_Des &&
                Piece == other.Piece &&
                Capture == other.Capture &&
                PawnPromotion == other.PawnPromotion)
                return true;
            else return false;
        }

        
    }
}
