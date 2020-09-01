using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Mmt.Api.DTO.Tours;
using Mmt.Api.Models;

namespace Mmt.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToursController : ControllerBase
    {
        private readonly MmtContext _context;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IConfiguration _Configuration;

        public ToursController(MmtContext context, IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
        {
            _context = context;
            this.webHostEnvironment = webHostEnvironment;
            _Configuration = configuration;
        }

       


        // GET: api/Tours
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Tour>>> GetTours()
        {
            var tours = await _context.Tours.ToListAsync();
            tours.ForEach(t => t.Image = $"{_Configuration["ToursImagesPath"]}" + t.Image);

            return Ok(tours);

        }

        // GET: api/Tours/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Tour>> GetTour(int id)
        {
            var tour = await _context.Tours
                .FirstOrDefaultAsync(t => t.Id == id);

            if (tour == null)
            {
                return NotFound();
            }
            tour.Image = $"{_Configuration["ToursImagesPath"]}" + tour.Image;
            return tour;
        }

        // PUT: api/Tours/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        [Authorize(Roles = "SUPERVISOR, DIRECTOR")]
        public async Task<IActionResult> PutTour(int id, [FromForm] TourPutDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tour = await _context.Tours.FindAsync(id);

            if (tour == null)
            {
                ModelState.AddModelError("", $"Tour with id : {id} not found");
                return NotFound(ModelState);
            }

            if (model.Image != null)
            {
                if (tour.Image !=null)
                {
                    string filePath = Path.Combine(webHostEnvironment.WebRootPath,
                        "TourImages", tour.Image);
                    System.IO.File.Delete(filePath);
                }

                tour.Image = ProcessUploadedFile(model);
            }

            tour.City = model.City;
            tour.Coordinates = model.Coordinates;
            tour.Country = model.Country;
            tour.Description = model.Description;
            tour.Latitude = model.Latitude;
            tour.Longitude = model.Longitude;
            tour.Name = model.Name;
            tour.Province = model.Province;
            tour.Region = model.Region;
            tour.Type = model.Type;

            _context.Entry(tour).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TourExists(id))
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

        // POST: api/Tours
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        [Authorize(Roles = "SUPERVISOR, DIRECTOR")]
        public async Task<ActionResult<Tour>> PostTour([FromForm] TourPostDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string uniqueFileName = ProcessUploadedFile(model);

            Tour tour = new Tour
            {

                City = model.City,
                Coordinates = model.Coordinates,
                Country = model.Country,
                Latitude = model.Latitude,
                Longitude = model.Longitude,
                Province = model.Province,
                Region = model.Region,
                Type = model.Type,
                Name = model.Name,
                Image = uniqueFileName,
                Description = model.Description
            };

            _context.Tours.Add(tour);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTour", new { id = tour.Id }, tour);
        }

        // DELETE: api/Tours/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "SUPERVISOR, DIRECTOR")]
        public async Task<ActionResult<Tour>> DeleteTour(int id)
        {
            //var tour = await _context.Tours.FindAsync(id);

            var tour = await _context.Tours
                .FirstOrDefaultAsync(t=>t.Id==id);

            if (tour == null)
            {
                ModelState.AddModelError("", $"tour with id : {id} not found");
                return NotFound(ModelState);
            }

            if (tour.Image !=null)
            {
                System.IO.File.Delete(webHostEnvironment.WebRootPath + "//tourImages//" + tour.Image);
            }

            _context.Tours.Remove(tour);
            await _context.SaveChangesAsync();

            return Ok(tour);
        }

        private string ProcessUploadedFile(TourPostDTO model)
        {
            string uniqueFileName = null;
            if (model.Image != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "TourImages");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Image.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.Image.CopyTo(fileStream);
                }
            }

            return uniqueFileName;
        }
        private string ProcessUploadedFile(TourPutDTO model)
        {
            string uniqueFileName = null;
            if (model.Image != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "TourImages");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Image.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.Image.CopyTo(fileStream);
                }
            }

            return uniqueFileName;
        }

        private bool TourExists(int id)
        {
            return _context.Tours.Any(e => e.Id == id);
        }
    }
}
