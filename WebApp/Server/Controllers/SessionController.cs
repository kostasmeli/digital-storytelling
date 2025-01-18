using BlazorApp.Shared.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazorApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionController(DataContext context) : ControllerBase
    {
        //READ
        [Authorize(Roles = "Admin")]
        #region Return all the Sessions From Database
        [HttpGet]
        public async Task<ActionResult<List<DialogueSession>>> GetSessions()
        { 
            var sessions = await context.DialogueSessions.ToListAsync();
            return Ok(sessions);
        }
        #endregion
        //READ
        [Authorize(Roles = "Admin")]
        #region Return the Sessions of specific User
        [HttpGet("{username}")]
        public async Task<ActionResult<List<DialogueSession>>> GetUserSessions(string username)
        {
            var userSessions= await context.DialogueSessions.Where(s=>s.Username==username).ToListAsync();
            return Ok(userSessions);
        }
        #endregion
        //CREATE
        [Authorize(Roles = "User,Admin")]
        #region Register a Session in the Database
        [HttpPost]
        public async Task<ActionResult<string>> CreateSession(SessionDTO session)
        {
            var user = await context.Users.FirstOrDefaultAsync(x => x.Username == session.Username);
            if (user == null)
            {
                return BadRequest();
            }
            else
            {
                var _dSession = new DialogueSession();
                _dSession.Username = session.Username;
                _dSession.date = session.date;
                _dSession.id = session.id;
                _dSession.Score = session.Score;
                _dSession.User = user;
                _dSession.DialogueTitle = session.DialogueTitle;
                _dSession.MaxScore = session.MaxScore;
                context.DialogueSessions.Add(_dSession);
                await context.SaveChangesAsync();
                return Ok("Session Registered");
            }

        }
        #endregion

    }
}
