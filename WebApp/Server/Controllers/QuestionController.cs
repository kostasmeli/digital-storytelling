using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace BlazorApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionController(DataContext context) : ControllerBase
    {
        //READ
        [Authorize(Roles ="User,Admin")]
        [HttpGet]
        public async Task<ActionResult<List<Question>>> GetQuestions()
        {
            var questions = await context.Questions.Include(q => q.Answers).ToListAsync();
            if (questions == null)
            {
                return BadRequest("No Question Found");
            }
            return Ok(questions);
        }
        //READ
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Question>> GetQuestionById(int id)
        {
            var questions = await context.Questions.Include(q => q.Answers).FirstOrDefaultAsync(x=>x.Id == id);
            if(questions== null)
            {
                return NotFound("No Question Found");
            }
            return Ok(questions);
        }
        //CREATE
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Question>> CreateQuestion(Question _question)
        {
            context.Questions.Add(_question);
            await context.SaveChangesAsync();
            return Ok(_question);
        }

        //UPDATE
        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<ActionResult<Question>> UpdateQuestion(Question _question)
        {
            context.Questions.Update(_question);
            await context.SaveChangesAsync();
            return Ok(_question);
        }
        //DELETE
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<List<Question>>> DeleteQuestion (int id)
        {
            var dbquestion = await context.Questions.Include(q=>q.Answers).FirstOrDefaultAsync(q=>q.Id == id);
            context.Questions.Remove(dbquestion);
            await context.SaveChangesAsync();
            var result = await context.Questions.Include(q => q.Answers).ToListAsync();
            return Ok(result);
        }
    }
}
