using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ChessAppServer.Models
{
    public class RequestModel
    {        
            [JsonProperty("fen")]
            public string Fen { get; set; } 
            public string Token { get; set; }
            public int Difficulty { get; set; }
    }
    public class LoginRequestModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}