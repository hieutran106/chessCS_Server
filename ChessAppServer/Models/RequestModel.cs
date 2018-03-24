using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChessAppServer.Models
{
    public class RequestModel
    {
        
            [JsonProperty("fen")]
            public string Fen { get; set; }

        
    }
}