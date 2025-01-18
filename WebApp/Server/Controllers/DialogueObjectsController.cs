using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;


namespace BlazorApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DialogueObjectsController(DataContext context) : ControllerBase
    {
        [Authorize(Roles = "Admin")]
        // GET: api/DialogueObjects
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DialogueObject>>> GetDialogueObjects()
        {
            return await context.DialogueObjects
                                 .Include(d => d.QuestionSet)
                                 .ThenInclude(q => q.Answers)
                                 .ToListAsync();
        }


        // GET: api/DialogueObjects/valid
        [Authorize(Roles = "User,Admin")]
        [HttpGet("valid")]
        public async Task<ActionResult<IEnumerable<DialogueObject>>> GetValidDialogueObjects()
        {
            var dialogueObjects = await context.DialogueObjects
                                 .Include(d => d.QuestionSet.Where(q => q.isActive))
                                 .ThenInclude(q => q.Answers)
                                 .ToListAsync();

            dialogueObjects = dialogueObjects
            .Where(d => d.QuestionSet.Any()) // Exclude DialogueObjects with empty QuestionSet
            .ToList();

            return dialogueObjects;
        }

        // GET: api/DialogueObjects/5
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<DialogueObject>> GetDialogueObject(string id)
        {
            var dialogueObject = await context.DialogueObjects.FindAsync(id);

            if (dialogueObject == null)
            {
                return NotFound();
            }

            return dialogueObject;
        }

        // PUT: api/DialogueObjects/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDialogueObject(string id, DialogueObject dialogueObject)
        {
            /*
            if (id != dialogueObject.Title)
            {
                return BadRequest();
            }
            */
            context.Update(dialogueObject);

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DialogueObjectExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/DialogueObjects
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<DialogueObject>> PostDialogueObject(DialogueObject dialogueObject)
        {
            context.DialogueObjects.Add(dialogueObject);
            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (DialogueObjectExists(dialogueObject.Title))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetDialogueObject", new { id = dialogueObject.Title }, dialogueObject);
        }

        // DELETE: api/DialogueObjects/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDialogueObject(string id)
        {
            var dialogueObject = await context.DialogueObjects.FindAsync(id);
            if (dialogueObject == null)
            {
                return NotFound();
            }

            context.DialogueObjects.Remove(dialogueObject);
            await context.SaveChangesAsync();

            return NoContent();
        }

        private bool DialogueObjectExists(string id)
        {
            return (context.DialogueObjects?.Any(e => e.Title == id)).GetValueOrDefault();
        }
    }
}
