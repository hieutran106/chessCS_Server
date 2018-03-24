using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChessAppServer.Models
{
    public class BestMove
    {
        public int Src { get; set; }
        public int Dst { get; set; }
        public BestMove(int src, int dst)
        {
            Src = src;
            Dst = dst;
        }
    }
}