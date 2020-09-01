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
    public class CuriosityLikesController : ControllerBase
    {
        private readonly MmtContext _context;

        public CuriosityLikesController(MmtContext context)
        {
            _context = context;
        }

        
        // GET: api/CuriosityLikes/5
        [HttpGet("{curiosityId}")]
        public async Task<ActionResult<CuriosityLike>> GetCuriosityLike(int curiosityId)
        {
            var curiosityLike = await _context.CuriosityLikes.Where(c => c.CuriosityId == curiosityId).FirstOrDefaultAsync();

            if (curiosityLike == null)
            {
                return NotFound();
            }

            return curiosityLike;
        }


        // POST: api/CuriosityLikes
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<CuriosityLike>> PostCuriosityLike(int curiosityId)
        {
            var curiosityLike = await _context.CuriosityLikes.Where(l => l.CuriosityId == curiosityId).FirstOrDefaultAsync();

            if (curiosityLike == null)
            {
                var newCuriosityLike = new CuriosityLike()
                {
                    CuriosityId = curiosityId,
                    Likes = 1
                };

                _context.CuriosityLikes.Add(newCuriosityLike);
                await _context.SaveChangesAsync();
                return CreatedAtAction("GetCuriosityLike", new { curiosityId = newCuriosityLike.CuriosityId }, newCuriosityLike);
            }
            else
            {
                curiosityLike.Likes += 1;
                _context.CuriosityLikes.Update(curiosityLike);
                await _context.SaveChangesAsync();
                return Ok(curiosityLike);

            }
        }



        //PUT: api/CuriosityLikes/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
         //more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{curiosityId}")]
        //[Authorize(Roles ="ADMIN, DIRECTOR, SUPERVISOR")]
        public async Task<IActionResult> PutCuriosityLike(int curiosityId, int numberOfLikes)
        {

            var curiosityLike = await _context.CuriosityLikes.FirstOrDefaultAsync(l => l.CuriosityId == curiosityId);

            if (curiosityLike == null)
            {
                return NotFound();
            }

            curiosityLike.Likes = numberOfLikes;

            _context.Entry(curiosityLike).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CuriosityLikeExists(curiosityLike.Id))
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

        [HttpPost("reset")]
        public async Task<IActionResult> ResetCuriosityLikes()
        {
            
            await _context.CuriosityLikes.ForEachAsync(
                l => {
                    l.Likes = 0;
                    _context.Entry(l).State = EntityState.Modified;
                     });

                await _context.SaveChangesAsync();

            return NoContent();
        }


        // GET: api/CuriosityLikes
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<CuriosityLike>>> GetCuriosityLikes()
        //{
        //    return await _context.CuriosityLikes.ToListAsync();
        //}


        // PUT: api/CuriosityLikes/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutCuriosityLike(int id, CuriosityLike curiosityLike)
        //{
        //    if (id != curiosityLike.Id)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(curiosityLike).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!CuriosityLikeExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}


        // DELETE: api/CuriosityLikes/5
        //[HttpDelete("{id}")]
        //public async Task<ActionResult<CuriosityLike>> DeleteCuriosityLike(int id)
        //{
        //    var curiosityLike = await _context.CuriosityLikes.FindAsync(id);
        //    if (curiosityLike == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.CuriosityLikes.Remove(curiosityLike);
        //    await _context.SaveChangesAsync();

        //    return curiosityLike;
        //}

        private bool CuriosityLikeExists(int id)
        {
            return _context.CuriosityLikes.Any(e => e.Id == id);
        }
    }
}
