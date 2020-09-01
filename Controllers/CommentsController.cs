using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mmt.Api.Models;

namespace Mmt.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "DIRECTOR, SUPERVISOR, USER")]
    public class CommentsController : ControllerBase
    {
        private readonly MmtContext _context;

        public CommentsController(MmtContext context)
        {
            _context = context;
        }


        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Comment>>> Search(string? userName, int? curiosityId)
        {
            try
            {
                IQueryable<Comment> query = _context.Comments;
                if (!string.IsNullOrEmpty(userName))
                {
                    query = query.Where(c => c.UserName.Contains(userName));
                }

                if (curiosityId != null)
                {
                    query = query.Where(c => c.CuriosityId == curiosityId);
                }

                var result = await query.ToListAsync();

                if (result.Any())
                {
                    return Ok(result);
                }

                return NotFound();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
        }


        // GET: api/MmtComments/curiosityId
        [HttpGet("curiosityComments/{curiosityId}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Comment>>> GetCuriosityComments(int curiosityId)
        {
            var comments = await _context.Comments.Where(c => c.CuriosityId == curiosityId).ToListAsync();
            if (comments == null)
            {
                return NotFound();
            }
            return comments;
        }


        // GET: api/Comments
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Comment>>> GetComments()
        {
            return await _context.Comments.ToListAsync();
        }

        // GET: api/Comments/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Comment>> GetComment(int id)
        {
            var comment = await _context.Comments.FindAsync(id);

            if (comment == null)
            {
                return NotFound();
            }

            return comment;
        }

        // PUT: api/Comments/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        [Authorize(Roles = "DIRECTOR, SUPERVISOR, USER")]
        public async Task<IActionResult> PutComment(int id, [FromForm] Comment comment)
        {
            if (id != comment.Id)
            {
                return BadRequest();
            }

            _context.Entry(comment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CommentExists(id))
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

        // POST: api/Comments
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
       [AllowAnonymous]
        public async Task<ActionResult<Comment>> PostComment([FromForm] Comment comment)
        {
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetComment", new { id = comment.Id }, comment);
        }

        // DELETE: api/Comments/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "DIRECTOR, SUPERVISOR, USER")]
        public async Task<ActionResult<Comment>> DeleteComment(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return comment;
        }

        private bool CommentExists(int id)
        {
            return _context.Comments.Any(e => e.Id == id);
        }
    }
}
