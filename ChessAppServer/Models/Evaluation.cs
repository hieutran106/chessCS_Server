using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessCS.ChessPieces;


namespace ChessCS
{
    public class Evaluation
    {
        private int[,] pawnEvalWhite ={//attribute to http://chessprogramming.wikispaces.com/Simplified+evaluation+function
        { 0,  0,  0,  0,  0,  0,  0,  0},
        {50, 50, 50, 50, 50, 50, 50, 50},
        {10, 10, 20, 30, 30, 20, 10, 10},
        { 5,  5, 10, 25, 25, 10,  5,  5},
        { 0,  0,  0, 20, 20,  0,  0,  0},
        { 5, -5,-10,  0,  0,-10, -5,  5},
        { 5, 10, 10,-20,-20, 10, 10,  5},
        { 0,  0,  0,  0,  0,  0,  0,  0}};
        private int[,] pawnEvalBlack;


        private int[,] rookEvalWhite={
        { 0,  0,  0,  0,  0,  0,  0,  0},
        { 5, 10, 10, 10, 10, 10, 10,  5},
        {-5,  0,  0,  0,  0,  0,  0, -5},
        {-5,  0,  0,  0,  0,  0,  0, -5},
        {-5,  0,  0,  0,  0,  0,  0, -5},
        {-5,  0,  0,  0,  0,  0,  0, -5},
        {-5,  0,  0,  0,  0,  0,  0, -5},
        { 0,  0,  0,  5,  5,  0,  0,  0}};
        private int[,] rookEvalBlack;

        private int[,] knightEvalWhite={
        {-50,-40,-30,-30,-30,-30,-40,-50},
        {-40,-20,  0,  0,  0,  0,-20,-40},
        {-30,  0, 10, 15, 15, 10,  0,-30},
        {-30,  5, 15, 20, 20, 15,  5,-30},
        {-30,  0, 15, 20, 20, 15,  0,-30},
        {-30,  5, 10, 15, 15, 10,  5,-30},
        {-40,-20,  0,  5,  5,  0,-20,-40},
        {-50,-40,-30,-30,-30,-30,-40,-50}};
        private int[,] knightEvalBlack;

        private int[,] bishopEvalWhite={
        {-20,-10,-10,-10,-10,-10,-10,-20},
        {-10,  0,  0,  0,  0,  0,  0,-10},
        {-10,  0,  5, 10, 10,  5,  0,-10},
        {-10,  5,  5, 10, 10,  5,  5,-10},
        {-10,  0, 10, 10, 10, 10,  0,-10},
        {-10, 10, 10, 10, 10, 10, 10,-10},
        {-10,  5,  0,  0,  0,  0,  5,-10},
        {-20,-10,-10,-10,-10,-10,-10,-20}};
        static int[,] bishopEvalBlack;
        private int[,] queenEvalWhite={
        {-20,-10,-10, -5, -5,-10,-10,-20},
        {-10,  0,  0,  0,  0,  0,  0,-10},
        {-10,  0,  5,  5,  5,  5,  0,-10},
        { -5,  0,  5,  5,  5,  5,  0, -5},
        {  0,  0,  5,  5,  5,  5,  0, -5},
        {-10,  5,  5,  5,  5,  5,  0,-10},
        {-10,  0,  5,  0,  0,  0,  0,-10},
        {-20,-10,-10, -5, -5,-10,-10,-20}};
        private int[,] queenEvalBlack;

        private int[,] kingEvalWhite={
        {-30,-40,-40,-50,-50,-40,-40,-30},
        {-30,-40,-40,-50,-50,-40,-40,-30},
        {-30,-40,-40,-50,-50,-40,-40,-30},
        {-30,-40,-40,-50,-50,-40,-40,-30},
        {-20,-30,-30,-40,-40,-30,-30,-20},
        {-10,-20,-20,-20,-20,-20,-20,-10},
        { 20, 20,  0,  0,  0,  0, 20, 20},
        { 20, 30, 10,  0,  0, 10, 30, 20}};
        private int[,] kingEvalBlack;

        private Dictionary<char, int> pieceValue;

        #region config
        bool evalPosition = true;
        #endregion
        public Evaluation()
        {
            pawnEvalBlack = FlipMatrix(pawnEvalWhite);
            rookEvalBlack = FlipMatrix(rookEvalWhite);
            knightEvalBlack = FlipMatrix(knightEvalWhite);
            bishopEvalBlack = FlipMatrix(bishopEvalWhite);
            queenEvalBlack = FlipMatrix(queenEvalWhite);
            kingEvalBlack = FlipMatrix(kingEvalWhite);
            pieceValue = new Dictionary<char, int>()
            {
                { 'P',100},
                { 'R',500},
                { 'N',300},
                { 'B',300},
                { 'Q',900},
                { 'K',9000},
                { 'p',-100},
                { 'r',-500},
                { 'n',-300},
                { 'b',-300},
                { 'q',-900},
                { 'k',-9000},
            };

        }
        public int[,] FlipMatrix(int[,] matrix)
        {
            int[,] ret = new int[8, 8];
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    ret[i, j] = matrix[7 - i, 7 - j];
            return ret;
        }
        public int EvaluateBoard(ChessBoard chessBoard)
        {
            int totalEvaluation = 0;
            char[,] board = chessBoard.Board;
            for (int i=0;i<64;i++)
            {
                int row = i / 8;
                int col = i % 8;
                char piece = chessBoard.Board[row, col];
                if (piece!='.')
                {
                    totalEvaluation += EvaluatePiece(piece);
                    if (evalPosition)
                    {
                        totalEvaluation += EvaluatePosition(piece, row, col);
                    }
                }               
            }
            return totalEvaluation;
        }
        private int EvaluatePiece(char piece)
        {
            return pieceValue[piece];
        }
        private int EvaluatePosition(char piece, int i, int j)
        {
            bool isWhite = char.IsUpper(piece);
            switch (char.ToUpper(piece))
            {
                case 'P': return isWhite ? pawnEvalWhite[i, j] : pawnEvalBlack[i, j];
                case 'R': return isWhite?rookEvalWhite[i,j]:rookEvalBlack[i,j];
                case 'N': return isWhite?knightEvalWhite[i,j]:knightEvalBlack[i,j];
                case 'B': return isWhite?bishopEvalWhite[i,j]:bishopEvalBlack[i,j];
                case 'Q': return isWhite?queenEvalWhite[i,j]:queenEvalBlack[i,j];
                case 'K':return isWhite?kingEvalWhite[i,j]:kingEvalBlack[i,j];;
                default: return 0;
            }
        }
    
    }
}
