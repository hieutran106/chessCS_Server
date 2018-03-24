using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessCS
{
    public class MNResult
    {
        public Move Move { get; set; }
        public int Value { get; set; }
        public MNResult(Move move, int value)
        {
            Move = move;
            Value = value;
        }
        public override string ToString()
        {
            return (Move.ToString() + " value: " + Value);
        }
    }
}
