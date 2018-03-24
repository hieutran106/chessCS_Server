using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessCS
{
    public class TranspositionTable
    {
        private static ulong TABLE_SIZE = 1000000;
        public static int VAL_UNKNOWN = -10001;
        private HashEntry[] table;
        public TranspositionTable()
        {
            table = new HashEntry[TABLE_SIZE];
        }
        public void RecordHash(ulong hash, int depth)
        {
            HashEntry entry = new HashEntry(hash, depth);
            int index = (int)(hash % TABLE_SIZE);
            table[index] = entry;
        }
        public bool ProbeHash(ulong hash, int depth)
        {
            int index = (int)(hash % TABLE_SIZE);
            HashEntry entry = table[index];
            if (entry!=null )
            {
                if (entry.Hash==hash)
                {
                    if (entry.Depth <depth)
                    {
                        Console.WriteLine("Cut-off at deeper branch");
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
