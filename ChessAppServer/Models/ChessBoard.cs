using ChessCS.ChessPieces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessCS
{
    public enum Player
    {
        White,
        Black
    }
    public class ChessBoard
    {
        /*
         * chess piece = WHITE / black
         * pawn = P/p
         * Rook = R/r
         * Knight = N/n
         * Bishop = B/b
         * Queen = Q/q
         * King = K/k
         *  
         */
        public static bool BLACK = false;
        public static bool WHITE = true;

        public bool ActiveColor { get; set; }
        public int FullMove { get; set; }
        public char[,] Board { get; set; }

        #region hash value
        private ZobristHash zobrist;
        public ulong Hash { get; set; }
        #endregion

        public ChessBoard()
        {
            Board = new char[8, 8];
            zobrist = new ZobristHash();
           
        }
        
        //Reset the board
        public void Reset()
        {
            //FEN for starting position
            string startingPosition = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w";
            Load(startingPosition);
            ActiveColor = WHITE;
            FullMove = 1;
            UpdateHash();
        }
        public void UpdateHash()
        {
            Hash = zobrist.Hash(Board);
        }
        //Returns the FEN string for the current board
        public string GetFEN()
        {
            StringBuilder fen_str = new StringBuilder(50);
            for (int i=0;i<8;i++)
            {
                int number = 0;
                for (int j=0;j<8;j++)
                {
                    if (Board[i,j]!='.')
                    {
                        fen_str.Append(Board[i, j]);
                       
                    } else
                    {
                        number++;
                        int next = j + 1;
                        if (next == 8 || (Board[i, next] != '.'))
                        {
                            fen_str.Append(number);
                            number = 0;
                        }
                        

                    }
                }
                if (i<7)
                    fen_str.Append('/');                                 
            }
            return fen_str.ToString();
        }
        public void Load(string fen)
        {
            string[] block = fen.Split(' ');

            //process piece placement
            string piecePlacement = block[0];
            Console.WriteLine(piecePlacement);
            string[] tokens = piecePlacement.Split('/');
            if (tokens.Length==8)
            {
                for (int i=0;i<8;i++)
                {
                    int index = 0;
                    for (int j=0;j<tokens[i].Length;j++)
                    {
                        char charAt = tokens[i][j];
                        int number;
                        if (!int.TryParse(charAt.ToString(), out number))
                        {
                            //Character is a chess piece
                            Board[i, index] = charAt;
                            index++;
                        } else
                        {
                            //insert empty square into board
                            for (int k=0;k<number;k++)
                            {
                                Board[i, index + k] = '.';
                            }
                            index += number;
                        }
                    }
                    if (index != 8)
                        Console.WriteLine("Invalid FEN notation");
                }
            } else
            {
                Console.WriteLine("Invalid FEN notation, reset the board");
            }
            //active color and full move
            string activeColor_str = block[1];
            ActiveColor = (activeColor_str[0] == 'w') ? WHITE : BLACK;
            //full move
            int fullMove;
            int.TryParse(block[block.Length-1], out fullMove);
            FullMove = fullMove;
            //hash
            UpdateHash();
        }
        //Put a piece 
        public void Put(char p,string position)
        {
            if (97<=position[0] && position[0]<=104)
            {
                if (49<=position[1] && position[1]<=56)
                {
                    //convert char 'a'->'h' to number
                    int i = position[0] - 97;
                    //convert char '1' -> '8' to number
                    int j = position[1] - 49;
                    Board[i, j] = p;
                }
            }
        }
        public static bool IsValidCoordinate(int x, int y)
        {
            if ((0 <= x && x <= 7) && (0 <= y && y <= 7))
                return true;
            else return false;
        }
        public Move GetMove(int src_x, int src_y, int des_x, int des_y)
        {
            Move move = new ChessCS.Move(src_x, src_y, des_x, des_y, this);
            return move;
        }
        public void MakeMove(Move move)
        {
            //Update board data
            Board[move.X_Src, move.Y_Src] = '.';
            Board[move.X_Des, move.Y_Des] = move.Piece;
            //Update hash value
            Hash = zobrist.UpdateHash(Hash, move);
            //
            if (move.PawnPromotion)
            {
                if (ActiveColor == WHITE)
                    Board[move.X_Des, move.Y_Des] = 'Q';
                else Board[move.X_Des, move.Y_Des] = 'q';
            }
            //update active color and full move
            if (ActiveColor==WHITE)
            {
                ActiveColor = BLACK;
            } else
            {
                FullMove++;
                ActiveColor = WHITE;
            }
        }
        public void UndoMove(Move move)
        {
            //Update board data
            Board[move.X_Src, move.Y_Src] = move.Piece;
            Board[move.X_Des, move.Y_Des] = move.Capture;
            //Update hash value
            Hash = zobrist.UpdateHash(Hash, move);
            //update active color and full move
            if (ActiveColor==BLACK)
            {
                ActiveColor = WHITE;
            } else
            {
                ActiveColor = BLACK;
                FullMove--;
            }
        }
        public bool CanCapture(int x,int y, bool color)
        {
            if ((color == WHITE && char.IsLower(Board[x, y])) //IsLower --> black
                || (color == BLACK && char.IsUpper(Board[x, y]))) //IsUpper --> white
                return true;
            else return false;
        }
       
        //All possible moves
        public List<Move> PossibleMoves(bool color)
        {
            List<Move> possibleMoves = new List<Move>();
            
            for (int i=0;i<8;i++)
                for (int j=0;j<8;j++)
                {
                    List<Move> pieceMoves = PieceMoves(Board[i, j], color, i, j);
                    if (pieceMoves != null)
                    {
                        //pieceMove is not generated if piece is '.'
                        possibleMoves.AddRange(pieceMoves);
                    }
                }
            
            return possibleMoves;
        }
        public List<Move> LegalMovesForPlayer(int player)
        {
            bool color = (player == 1) ? BLACK : WHITE;
            return PossibleMoves(color);
        }
        private List<Move> PieceMoves(char piece, bool color,int i, int j)
        {
            List<Move> pieceMoves = null;
            if ((color==WHITE&&char.IsUpper(piece)) ||
                (color==BLACK && char.IsLower(piece))) {
                switch (piece)
                {
                    case 'P':
                    case 'p':
                        pieceMoves = Pawn.generateMove(i, j, this);                       
                        break;
                    case 'R':
                    case 'r':
                        pieceMoves = Rook.generateMove(i, j, this);
                        break;
                    case 'N':
                    case 'n':
                        pieceMoves = Knight.generateMove(i, j, this);
                        break;
                    case 'B':
                    case 'b':
                        pieceMoves = Bishop.generateMove(i, j, this);
                        break;
                    case 'Q':
                    case 'q':
                        pieceMoves = Queen.generateMove(i, j, this);
                        break;
                    case 'K':
                    case 'k':
                        pieceMoves = King.generateMove(i, j, this);
                        break;
                    default:
                        break;
                }
            }
            return pieceMoves;
            
        }
        public void PrintBoard()
        {
            Console.WriteLine("   +------------------------+");
            for (int i = 0; i < 8; i++)
            {
                Console.Write($"{8-i} |");
                for (int j=0;j<8;j++)
                {
                    Console.Write($"  {(char)Board[i, j]}");
                }
                Console.WriteLine(" |");
            }
            Console.WriteLine("   +------------------------+");
            Console.WriteLine("     a  b  c  d  e  f  g  h");
        }

        
        public Move GetAIMove(int difficulty)
        {
            if (FullMove == 1 && ActiveColor == ChessBoard.BLACK)
            {
                Move move = GetMove(1, 4, 3, 4);
                return move;
            }
            else if (FullMove == 2 && ActiveColor == ChessBoard.BLACK)
            {
                Move move = GetMove(0, 1, 2, 2);
                return move;
            }
            else
            {
                Move bestMove = Search.SearchMove(this, false, difficulty, debug:false);
                Console.WriteLine("Best move:" + bestMove);
                return bestMove;
            }
        }

        public ChessBoard FastCopy()
        {
            ChessBoard clonedBoard = new ChessBoard();
            clonedBoard.Board = this.Board.Clone() as char[,];
            clonedBoard.ActiveColor = this.ActiveColor;
            clonedBoard.FullMove = this.FullMove;
            return clonedBoard;
        }
        public int CheckEndGame()
        {
            int whiteKingPos = -1;
            int blackKingPos = -1;
            for (int i=0;i<64;i++)
            {
                int row = i / 8;
                int col = i % 8;
                if (Board[row,col]=='K')
                {
                    whiteKingPos = i;
                } else if (Board[row,col]=='k')
                {
                    blackKingPos = i;
                }
                
            }
            if (whiteKingPos == -1 && blackKingPos != -1)
            {
                return -1; //BLACK win
            }
            else if (blackKingPos == -1 && whiteKingPos != -1)
            {
                return 1;//WHITE win
            }
            else return 0;//Draw
        }
    }
}
