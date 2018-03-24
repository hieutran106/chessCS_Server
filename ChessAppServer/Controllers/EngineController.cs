using ChessAppServer.Models;
using ChessCS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace ChessAppServer.Controllers
{
    public class EngineController : ApiController
    {
        [HttpPost]
        public BestMove GetBestMove([FromBody] RequestModel requestModel)
        {
            Debug.WriteLine(requestModel.Fen);
            ChessBoard cb = new ChessBoard();
            cb.Load(requestModel.Fen);
            Move move = cb.GetAIMove();
            return new BestMove(move.Src, move.Dst);
        }
        

    }
}
