using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessCS
{
    public class HashEntry
    {
        public static int HASH_EXACT;
        public static int HASH_ALPHA;
        public static int HASH_BETA;

        public ulong Hash { get; set; }
        public int Value { get; private set; }
        public int Flag { get; private set; }
        public int Depth { get; private set; }
        public HashEntry(ulong hash, int depth)
        {
            Hash = hash;
            Depth = depth;
        }
        

    }
}
