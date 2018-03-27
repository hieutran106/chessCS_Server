using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
namespace ChessCS
{
    public class Search
    {
        public static bool BLACK = false;
        public static bool WHITE = true;

        public static int BETA = 10000;
        public static int ALPHA = -10000;

        private int visitedNode = 0;

        
        private Evaluation evaluation;
        private ChessBoard examinedBoard;
        private Move[,] searchKiller;
        private bool nullPruning;
        //transposition table
        private TranspositionTable table;
        //history heuristic dic
        private Dictionary<string, int> historyDic;
        //history evaluation
        private Dictionary<string, int> hisEval;
        public SearchResult Result { get; private set; }
        private int maxDepth;
        public int MaxDepth {
            get { return maxDepth; }
            set
            {
                maxDepth = value;
                searchKiller = new Move[maxDepth, 2];
            }
        }
        public Search()
        {
            evaluation = new Evaluation();
            //searchKiller = new Move[maxDepth, 2];
            this.nullPruning = true;
            table = new TranspositionTable();
            historyDic = new Dictionary<string, int>();
            hisEval = new Dictionary<string, int>();
        }

        public static Move SearchMove(ChessBoard examinedBoard, bool player,int difficuty, bool debug)
        {
            Search search = new Search();
            search.examinedBoard = examinedBoard.FastCopy();
            int color = (player == BLACK) ? 1 : -1;
            //Iterative Deepening Search
            for (int maxDepth=1; maxDepth <= difficuty; maxDepth++)
            {
                search.MaxDepth = maxDepth;
                search.Negamax(maxDepth, ALPHA, BETA, color, false, true);
                SearchResult result = search.Result;
                Console.WriteLine($"Depth={maxDepth}: Visited {result.VisitedNode} nodes, took {result.Elapse.TotalSeconds} seconds, best move: {result.BestMove}");
            }
            return search.Result.BestMove;
        }
        
        private int Negamax(int depth, int alpha, int beta, int color, bool debug, bool allowNull)
        {
            Stopwatch sw = null;
            if (depth==MaxDepth)
            {
                sw = new Stopwatch();
                sw.Start();
            }
            if (depth <= 0)
            {
                int score = Evaluate(color, debug, depth=0);
                return score;
            }
            //Null move
            if (allowNull)
            {
                int nullTurn = -color;
                int nullValue = -Negamax(depth - 1 - 2, -beta, -beta + 1, nullTurn, debug, false);
                if (nullValue >= beta)
                {
                    return nullValue; //Cut off
                }
                    
            }

            List<Move> possibleMoves = null;
            possibleMoves = (debug) ? DebugLegalMoves() : examinedBoard.LegalMovesForPlayer(color);
            if (possibleMoves.Count == 0)
            {
                int score = Evaluate(color, debug, depth);
                return score;
            }
            int bestValue = Int32.MinValue;
            Move bestMove = null;
            OrderMove(possibleMoves, depth);
            foreach (Move eleMove in possibleMoves)
            {
                visitedNode++;
                examinedBoard.MakeMove(eleMove);
                //Print information
                //Console.Write("\n" + new string('\t', MaxDepth - depth) + eleMove.ToString());
                int value = -Negamax(depth - 1, -beta, -alpha, -color, debug,nullPruning);
                examinedBoard.UndoMove(eleMove);
                //save hisEval
                if (depth==MaxDepth)
                {
                    hisEval[eleMove.ToString()] = value;
                }
                
                // We found a new max, also keep track of move
                if (value > bestValue)
                {
                    bestValue = value;
                    bestMove = eleMove;
                }
                // If our max is greater than our lower bound, update our lower bound
                if (value>alpha)
                {
                    alpha = value;
                }
                
                // Alpha-beta pruning
                if (alpha >= beta)
                {
                    //Console.Write(new string('\t', MaxDepth - depth) + "Cut off");
                    //Save killer move
                    if (eleMove.Capture == '.')
                    {
                        searchKiller[MaxDepth - depth, 1] = searchKiller[MaxDepth - depth, 0];
                        searchKiller[MaxDepth - depth, 0] = eleMove;
                    }
                    //save history 
                    SaveHistoryHeuristic(eleMove,depth);
                    
                    break;
                }

            }
            if (depth== MaxDepth)
            {
                sw.Stop();
                Result = new SearchResult(visitedNode, bestMove, sw.Elapsed);
            }
            return bestValue;
        }
        private List<Move> DebugLegalMoves()
        {
            Console.Write("How many moves are there:");
            int count = Convert.ToInt32(Console.ReadLine());
            List<Move> possibleMoves = new List<Move>();
            for (int i = 0; i < count; i++)
            {
                Move debugMove = new Move(1, 4, 3, 4, this.examinedBoard);
                possibleMoves.Add(debugMove);
            }
            return possibleMoves;
        }
        private int Evaluate(int color, bool debug, int depthleft)
        {
            int score;
            if (!debug)
            {
                score = evaluation.EvaluateBoard(this.examinedBoard) + (MaxDepth - depthleft) * 10;
                score = -color * score;

                //Console.WriteLine(" Eva:" + score);
            }
            else
            {
                Console.Write("What is the score:");
                score = Convert.ToInt32(Console.ReadLine());
            }

            return score;
        }
        private void OrderMove(List<Move> possibleMoves, int depth)
        {
            if (MaxDepth > 1 && depth== MaxDepth)
            {
                //We user history evaluation
                Console.WriteLine($"Number of possible move: {possibleMoves.Count()}, number keys in history dictionary: {hisEval.Keys.Count()}");
                foreach (string key in hisEval.Keys)
                {
                    foreach (Move eleMove in possibleMoves)
                    {
                        if (eleMove.ToString()==key)
                        {
                            eleMove.Value = hisEval[key];
                        }
                    }
                }
                //Reset record
                hisEval.Clear();
            } else
            {
                //
                bool killerHeuristic = true;
                bool historyHeuristic = true;
                foreach (Move eleMove in possibleMoves)
                {
                    eleMove.Evaluate();
                    if (depth == MaxDepth || !killerHeuristic || !historyHeuristic)
                        continue;

                    KillerHeuristic(eleMove, depth);
                    HistoryHeuristic(eleMove, depth);
                }
            }
            
            //sort move
            //Console.Write("\n Before sort:");
            //possibleMoves.ForEach(x => Console.Write(x.Value + " "));
            
            //order move by descending value
            possibleMoves.Sort( (x,y) => y.Value.CompareTo(x.Value));
            //Console.Write("\n After sort:");
            //possibleMoves.ForEach(x => Console.Write(x.Value + " "));

        }
        private void SaveHistoryHeuristic(Move eleMove, int depth)
        {
            string key = eleMove.ToString();
            if (historyDic.ContainsKey(key))
            {
                int history_depth = historyDic[key];
                if (history_depth > depth)
                {
                    //Console.WriteLine("Record history");
                    historyDic[key] = depth;
                }
            }
            else
            {
                historyDic[key] = depth;
            }
        }
        private void HistoryHeuristic(Move move, int depth)
        {
            string key = move.ToString();
            if (historyDic.ContainsKey(key)) {
                int history_depth = historyDic[key];
                if (history_depth<depth)
                {
                    //Console.WriteLine("Found a cut off at deeper branch");
                    move.Value += history_depth * history_depth;
                }
            }
        }

        private void KillerHeuristic(Move eleMove, int depth)
        {
            //killer heuristic
            Move primaryKiller = searchKiller[MaxDepth - depth, 0];
            Move secondaryKiller = searchKiller[MaxDepth - depth, 1];
            if (primaryKiller != null)
            {
                if (eleMove.Equals(primaryKiller))
                {
                    eleMove.Value += 90;
                }
            }
            else if (secondaryKiller != null)
            {
                if (eleMove.Equals(secondaryKiller))
                {
                    eleMove.Value += 80;
                }
            }
        }



        private MNResult AlphaBeta(int depth, int beta, int alpha, Move move, bool player, bool debug = false)
        {
            //BLACK is max player
            bool isMaxPlayer = (player == BLACK) ? true : false;

            if (depth == 0)
            {
                return EvaluateNode(move, player, debug);
            }
            List<Move> possibleMoves = null;
            if (!debug)
            {
                possibleMoves = examinedBoard.PossibleMoves(player);
            }
            else
            {
                Console.Write("How many moves are there:");
                int count = Convert.ToInt32(Console.ReadLine());
                possibleMoves = new List<Move>();


                for (int i = 0; i < count; i++)
                {
                    Move debugMove = new Move(1, 4, 3, 4, this.examinedBoard);
                    possibleMoves.Add(debugMove);
                }

            }


            if (possibleMoves.Count == 0)
            {
                return EvaluateNode(move, player, debug);
            }
            //sort later           
            foreach (Move eleMove in possibleMoves)
            {

                examinedBoard.MakeMove(eleMove);
                bool nextPlayer = !player;
                MNResult result = AlphaBeta(depth - 1, beta, alpha, eleMove, nextPlayer, debug);
                int value = result.Value;

                examinedBoard.UndoMove(eleMove);
                //BLACK is Max Player
                if (isMaxPlayer)
                {
                    if (value > alpha) //Max Nodes can only make restriction on the lower bound
                    {
                        alpha = value;
                        if (depth == MaxDepth)
                        {
                            move = result.Move;
                        }
                    }

                }
                else
                {
                    if (value < beta)
                    {
                        beta = value;
                        if (depth == MaxDepth)
                        {
                            move = result.Move;
                        }
                    }
                }
                if (alpha >= beta) //pruning
                {
                    if (isMaxPlayer)
                    {
                        return new MNResult(move, alpha);
                    }
                    else
                    {
                        return new MNResult(move, beta);
                    }
                }

            }

            // Travel all child node, no prunning
            if (isMaxPlayer)
            {
                //value of node is alpha value
                return new MNResult(move, alpha);
            }
            else
            {
                //value of min node is beta value
                return new MNResult(move, beta);
            }
        }
        private MNResult EvaluateNode(Move move, bool player, bool debug)
        {
            int score;
            if (!debug)
            {
                score = evaluation.EvaluateBoard(this.examinedBoard);
                //Negate the value
                if (player == BLACK)
                    score = -score;

            }
            else
            {
                Console.Write("What is the score:");
                score = Convert.ToInt32(Console.ReadLine());
            }
            MNResult result = new MNResult(move, score);
            return result;

        }
        //Depricate
        private MNResult RootNegaMax(int depth, int alpha, int beta, int color, bool debug)
        {
            List<Move> possibleMoves = null;
            possibleMoves = (debug) ? DebugLegalMoves() : examinedBoard.LegalMovesForPlayer(color);

            int bestValue = Int32.MinValue;
            Move bestMove = null;
            //sort move
            OrderMove(possibleMoves, depth);
            //
            foreach (Move eleMove in possibleMoves)
            {
                visitedNode++;
                examinedBoard.MakeMove(eleMove);
                //Print information
                Console.WriteLine(new string('\t', MaxDepth - depth) + eleMove.ToString());
                int value = -Negamax(depth - 1, -beta, -alpha, -color, debug, nullPruning);
                examinedBoard.UndoMove(eleMove);

                if (value > bestValue)
                {
                    bestValue = value;
                    bestMove = eleMove;
                }

                alpha = Math.Max(alpha, value);
                if (alpha >= beta)
                {
                    Console.WriteLine(new string('\t', MaxDepth - depth) + "Cut off");
                    break;
                }

            }
            return new MNResult(bestMove, bestValue);
        }
    }
    public class SearchResult
    {
        public int VisitedNode { get; private set; }
        public Move BestMove { get; private set; }
        public TimeSpan Elapse { get; private set; }
        public SearchResult(int node, Move move, TimeSpan time)
        {
            VisitedNode = node;
            BestMove = move;
            Elapse = time;
        }
    }
}
