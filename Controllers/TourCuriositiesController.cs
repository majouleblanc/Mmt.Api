using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Mmt.Api.Models;

namespace Mmt.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "SUPERVISOR, DIRECTOR")]
    public class ToursAndCuriositiesManagerController : ControllerBase
    {
        private readonly MmtContext _context;
        private readonly IConfiguration _Configuration;

        public ToursAndCuriositiesManagerController(MmtContext context, IConfiguration configuration)
        {
            _context = context;
            _Configuration = configuration;
        }

        [HttpGet("GetTourCuriosities/{tourId:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTourCuriosities(int tourId)
        {
            var tour = await _context.Tours.FirstOrDefaultAsync(t => t.Id == tourId);

            if (tour == null)
            {
                ModelState.AddModelError("", $"Tour with id : {tourId} not found");
                return BadRequest(ModelState);
            }

            var curiosities = await _context.tourCuriosities.Where(tc => tc.TourId == tourId).Select(c=>c.Curiosity).ToListAsync();

            curiosities.ForEach(c => c.Image = _Configuration["CuriositiesImagePath"] + c.Image);
            return Ok(curiosities);

            //var TourCuriosities = await _context.tourCuriosities.Where(tc => tc.TourId == tourId).Include(tc => tc.Curiosity).ToListAsync();

            //return Ok(TourCuriosities);
        }


        [HttpGet("GetCuriosityTours/{curiosityId:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCuriosityTours(int curiosityId)
        {
            var curiosity = await _context.Curiosities.FirstOrDefaultAsync(c => c.Id == curiosityId);

            if (curiosity == null)
            {
                ModelState.AddModelError("", $"curiosity with id = {curiosityId} not found");
                return BadRequest(ModelState);
            }

            var tours = await _context.tourCuriosities.Where(tc => tc.CuriosityId == curiosityId).Select(tc => tc.Tour).ToListAsync();

            //var tours = await _context.tourCuriosities.Where(tc => tc.CuriosityId == cusiosityId).Include(t => t.Tour).ToListAsync();
            return Ok(tours);
        }


        [HttpPost("ManageCuriositiesForTourWith/{tourId:int}")]
        public async Task<IActionResult> ManageCuriositiesForTourWith(int tourId, [FromForm] int[] CuriositiesIds = null)
        {
            var tour = await _context.Tours.FirstOrDefaultAsync(t => t.Id == tourId);
            if (tour == null)
            {
                ModelState.AddModelError("", $"tour with id : {tourId} not found");
                return NotFound(ModelState);
            }

            if (CuriositiesIds == null)
            {
                ModelState.AddModelError("", $"curiosities Ids cannot be null");
                return BadRequest(ModelState);
            }

            List<TourCuriosity> tourCuriosities = new List<TourCuriosity>();

            
            foreach (var curiosityId in CuriositiesIds)
            {
                if (!CuriosityExists(curiosityId))
                {
                    ModelState.AddModelError("", $"the curiosity with id ${curiosityId} not found");
                }
                tourCuriosities.Add(new TourCuriosity { Tour = tour, Curiosity = (await _context.Curiosities.FirstOrDefaultAsync(c => c.Id == curiosityId)) });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var oldTourCuriosities = await _context.tourCuriosities.Where(tc => tc.TourId == tourId).ToListAsync();

            if (oldTourCuriosities != null)
            {
                _context.tourCuriosities.RemoveRange(oldTourCuriosities);
            }

            await _context.tourCuriosities.AddRangeAsync(tourCuriosities);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }

            return Ok(new { tour, CuriositiesIds });
        }



        [HttpPost("ManageToursForCuriosityWith/{curiosityId:int}")]
        public async Task<IActionResult> ManageToursForCuriosityWith(int curiosityId, [FromForm] int[] toursIds = null)
        {
            var curiosity = await _context.Curiosities.FirstOrDefaultAsync(c => c.Id == curiosityId);
            if (curiosity == null)
            {
                ModelState.AddModelError("", $"Curiosity with id : {curiosityId} not found");
                return NotFound(ModelState);
            }

            if (toursIds == null)
            {
                ModelState.AddModelError("", $"Tour Ids cannot be null");
                return BadRequest(ModelState);
            }

            List<TourCuriosity> tourCuriosities = new List<TourCuriosity>();

            foreach (var tourId in toursIds)
            {
                if (!TourExists(tourId))
                {
                    ModelState.AddModelError("", $"the tour with id ${tourId} not found");
                }
                tourCuriosities.Add(new TourCuriosity { Curiosity = curiosity, Tour = (await _context.Tours.FirstOrDefaultAsync(t => t.Id == tourId)) });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var oldTourCuriosities = await _context.tourCuriosities.Where(tc => tc.CuriosityId == curiosityId).ToListAsync();

            if (oldTourCuriosities != null)
            {
                _context.tourCuriosities.RemoveRange(oldTourCuriosities);
            }

            await _context.tourCuriosities.AddRangeAsync(tourCuriosities);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }

            return Ok(new { curiosity, toursIds });
        }



        // GET: api/TourCuriosities
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<TourCuriosity>>> GettourCuriosities()
        //{
        //    return await _context.tourCuriosities.ToListAsync();
        //}

        // GET: api/TourCuriosities/5
        //[HttpGet("{id}")]
        //public async Task<ActionResult<TourCuriosity>> GetTourCuriosity(int id)
        //{
        //    var tourCuriosity = await _context.tourCuriosities.FindAsync(id);

        //    if (tourCuriosity == null)
        //    {
        //        return NotFound();
        //    }

        //    return tourCuriosity;
        //}

        // PUT: api/TourCuriosities/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutTourCuriosity(int id, TourCuriosity tourCuriosity)
        //{
        //    if (id != tourCuriosity.TourId)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(tourCuriosity).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!TourCuriosityExists(id))
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

        // POST: api/TourCuriosities
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        //[HttpPost]
        //public async Task<ActionResult<TourCuriosity>> PostTourCuriosity(TourCuriosity tourCuriosity)
        //{
        //    _context.tourCuriosities.Add(tourCuriosity);
        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateException)
        //    {
        //        if (TourCuriosityExists(tourCuriosity.TourId))
        //        {
        //            return Conflict();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return CreatedAtAction("GetTourCuriosity", new { id = tourCuriosity.TourId }, tourCuriosity);
        //}

        // DELETE: api/TourCuriosities/5
        //[HttpDelete("{id}")]
        //public async Task<ActionResult<TourCuriosity>> DeleteTourCuriosity(int id)
        //{
        //    var tourCuriosity = await _context.tourCuriosities.FindAsync(id);
        //    if (tourCuriosity == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.tourCuriosities.Remove(tourCuriosity);
        //    await _context.SaveChangesAsync();

        //    return tourCuriosity;
        //}
        private bool TourExists(int id)
        {
            return _context.Tours.Any(e => e.Id == id);
        }

        private bool TourCuriosityExists(int id)
        {
            return _context.tourCuriosities.Any(e => e.TourId == id);
        }

        private bool CuriosityExists(int id)
        {
            return _context.Curiosities.Any(e => e.Id == id);
        }

    }
}
