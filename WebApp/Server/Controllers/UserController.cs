using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazorApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(DataContext context, IConfiguration configuration) : ControllerBase
    {
        //READ
		#region api/User Return All the Users From Database
		[HttpGet]
        [Authorize(Roles ="Admin")]
        public async Task<ActionResult<List<User>>> GetUsers()
        {
            var users =await context.Users.ToListAsync();
            return Ok(users);
        }
        #endregion
        //READ
        #region api/User/username Return the informatios of this user 
        [Authorize(Roles = "Admin")]
        [HttpGet("{username}")]
        public async Task<ActionResult<User>> GetSingleUser(string username)
        {
            var user = await context.Users.FirstOrDefaultAsync(x => x.Username == username);
            if (user == null)
            {
                return BadRequest("User not found");
            }
            return Ok(user);
        }
        #endregion

        //CREATE
		#region Register a User in the Database
		[HttpPost]
		public async Task<ActionResult<User>>CreateUser(User createuser)
		{
            context.Users.Add(createuser);
            await context.SaveChangesAsync();
			return Ok(createuser);
		}
        #endregion

        //GET WebGL link
        [HttpGet("link")]
        public string GetLink()
        {
            string Unity_Link = configuration.GetSection("AppSettings:Link").Value;
            return Unity_Link;
        }

	}
}