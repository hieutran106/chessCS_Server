using ChessAppServer.Infrastructure;
using ChessAppServer.Models;
using ChessCS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Security.Claims;

namespace ChessAppServer.Controllers
{
    public class EngineController : ApiController
    {
        [HttpPost]
        public object Login([FromBody] LoginRequestModel login)
        {
            AppUser user = UserManager.Find(login.Username, login.Password);
            if (user!=null)
            {
                return new {
                    success = true,
                    token = JwtManager.GenerateToken(login.Username),
                    username = login.Username,
                    email = user.Email
                };
            } else
            {
                return new { success = false, message="Invalid username or password" };
            }
        }
        [HttpPost]
        public object TestToken([FromBody] RequestModel requestModel)
        {
            if (ValidateToken(requestModel.Token))
            {
                return new { success=true, bestMove="abc" };
            } else return new { success=false, message= "Invalid token" };
        }
        private bool ValidateToken(string token)
        {
            ClaimsPrincipal simplePrinciple = JwtManager.GetPrincipal(token);
            ClaimsIdentity identity = simplePrinciple?.Identity as ClaimsIdentity;
            return (identity == null) ? false : true;

        }
        [HttpPost]
        public object GetBestMove([FromBody] RequestModel requestModel)
        {
            if (ValidateToken(requestModel.Token))
            {
                ChessBoard cb = new ChessBoard();
                cb.Load(requestModel.Fen);
                Debug.WriteLine("difficulty:" + requestModel.Difficulty);
                int difficulty = 2 * requestModel.Difficulty + 2;
                Move move = cb.GetAIMove(difficulty);
                return new BestMove(move.Src, move.Dst);
            } else
            {
                return new { success = false, message = "Invalid token" };
            }            
        }

        private AppUserManager UserManager
        {
            get
            {
                return HttpContext.Current.GetOwinContext().GetUserManager<AppUserManager>();
            }
        }


    }
}
