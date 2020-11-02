using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Mmt.Api.DTO.Curiosity;
using Mmt.Api.Models;
using Mmt.Api.services;

namespace Mmt.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "DIRECTOR, SUPERVISOR, USER")]
    public class CuriositiesController : ControllerBase
    {
        private readonly MmtContext _context;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IConfiguration _Configuration;
        private readonly IAzureFileService _AzureFileService;

        public CuriositiesController(MmtContext context, IWebHostEnvironment webHostEnvironment, IConfiguration configuration, IAzureFileService azureFileService)
        {
            _context = context;
            this.webHostEnvironment = webHostEnvironment;
            _Configuration = configuration;
            _AzureFileService = azureFileService;
        }

         
        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Curiosity>>> Search(string? name)
        {
            try
            {
                IQueryable<Curiosity> query = _context.Curiosities;
                if (!string.IsNullOrEmpty(name))
                {
                    query = query.Where(c => c.Name.Contains(name));
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


        // GET: api/Curiosities
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Curiosity>>> GetCuriosities()
        {
            var curiosities = await _context.Curiosities
                .Include(c=>c.Comments)
                .Include(c=>c.CuriosityLikes)
                .Include(c=>c.Photos)
                .ToListAsync();
            curiosities.ForEach(c => c.Image = _Configuration["CuriositiesImagePath"] + c.Image);
            return Ok(curiosities);
        }

        // GET: api/Curiosities/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Curiosity>> GetCuriosity(int id)
        {
            var curiosity = await _context.Curiosities
                .Include(c => c.Comments)
                .Include(c => c.CuriosityLikes)
                .Include(c => c.Photos)
                .FirstOrDefaultAsync(c => c.Id == id);
                //.FindAsync(id);

            if (curiosity == null)
            {
                return NotFound();
            }
            curiosity.Image = _Configuration["CuriositiesImagePath"] + curiosity.Image;
            return Ok(curiosity);
        }

        // PUT: api/Curiosities/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        [Authorize(Roles = "DIRECTOR, SUPERVISOR")]
        public async Task<IActionResult> PutCuriosity(int id, [FromForm] CuriositiesPutDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }


            var curiosity = await _context.Curiosities.FindAsync(id);

            if (curiosity == null)
            {
                ModelState.AddModelError("", $"Curiosity with id : ${id} not found");
                return NotFound(ModelState);
            }

            string uniqueFileName = "";
            if (model.Image != null)
            {
                if (curiosity.Image != null)
                {
                    await _AzureFileService.DeleteCuriosityImageAsync(curiosity.Image);
                    //string filePath = Path.Combine(webHostEnvironment.WebRootPath,
                    //    "images", curiosity.Image);
                    //System.IO.File.Delete(filePath);
                }

                //if (model.ExistingImage != null)
                //{
                //    string filePath = Path.Combine(webHostEnvironment.WebRootPath,
                //        "images", model.ExistingImage);
                //    System.IO.File.Delete(filePath);
                //}
                //curiosity.Image = ProcessUploadedFile(model);
                uniqueFileName = await _AzureFileService.ProcessFotoAsync(model);

                curiosity.Image = uniqueFileName;
            }
            curiosity.City = model.City;
            curiosity.Coordinates = model.Coordinates;
            curiosity.Country = model.Country;
            curiosity.Description = model.Description;
            curiosity.Latitude = model.Latitude;
            curiosity.Longitude = model.Longitude;
            curiosity.Name = model.Name;
            curiosity.Period = model.Period;
            curiosity.Province = model.Province;
            curiosity.Region = model.Region;
            curiosity.Type = model.Type;

            //_context.Update(curiosity);


            _context.Entry(curiosity).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CuriosityExists(id))
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

        // POST: api/Curiosities
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        //[Authorize(Roles = "SUPERVISOR, DIRECTOR, ADMIN")]
        [Authorize(Roles = "DIRECTOR, SUPERVISOR")]
        public async Task<ActionResult<Curiosity>> PostCuriosity([FromForm] CuriositiesPostDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //string uniqueFileName = ProcessUploadedFile(model);
            string uniqueFileName = await _AzureFileService.ProcessFotoAsync(model);

            Curiosity curiosity = new Curiosity
            {
                City = model.City,
                Coordinates = model.Coordinates,
                Country = model.Country,
                Latitude = model.Latitude,
                Longitude = model.Longitude,
                Period = model.Period,
                Province = model.Province,
                Region = model.Region,
                Type = model.Type,
                Name = model.Name,
                Image = uniqueFileName,
                Description = model.Description
            };

            _context.Curiosities.Add(curiosity);
            await _context.SaveChangesAsync();

            var curiosityLike = new CuriosityLike { CuriosityId = curiosity.Id, Likes = 0 };
            _context.CuriosityLikes.Add(curiosityLike);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCuriosity", new { id = curiosity.Id }, curiosity);

        }

        // DELETE: api/Curiosities/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "SUPERVISOR, DIRECTOR")]
        [AllowAnonymous]
        public async Task<ActionResult<Curiosity>> DeleteCuriosity(int id)
        {
            //var curiosity = await _context.Curiosities.FindAsync(id);

            var curiosity = await _context.Curiosities
                .Include(c => c.Comments)
                .Include(c => c.CuriosityLikes)
                .Include(c => c.Photos)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (curiosity == null)
            {
                ModelState.AddModelError("", $"curiosity with id : {id} not found");
                return NotFound(ModelState);
            }

            var tourCuriosities = _context.tourCuriosities.Where(tc => tc.Curiosity == curiosity).ToArray();
            _context.tourCuriosities.RemoveRange(tourCuriosities);

            if (curiosity.Image != null)
            {
                await _AzureFileService.DeleteCuriosityImageAsync(curiosity.Image);
                //System.IO.File.Delete(webHostEnvironment.WebRootPath + "//images//" + curiosity.Image);
            }

            //removing the photos folder from the uploads/gallery folder
            if (curiosity.Photos != null && curiosity.Photos.Count !=0)
            {
                //System.IO.Directory.Delete(webHostEnvironment.WebRootPath + "//uploads//gallery//" + curiosity.Id, true);
                foreach (var photo in curiosity.Photos)
                {
                    await _AzureFileService.DeleteCuriosityPhotoGalleryAsync(photo.PhotoPath);

                }
                _context.Photos.RemoveRange(curiosity.Photos);
            }

            //removing the related records from the child tables
            if (curiosity.Comments != null)
            {
                _context.Comments.RemoveRange(curiosity.Comments);
            }

            if (curiosity.CuriosityLikes != null)
            {
                _context.CuriosityLikes.RemoveRange(curiosity.CuriosityLikes);
            }


      

            //removing the entity self from the db
            _context.Curiosities.Remove(curiosity);
            await _context.SaveChangesAsync();

            return curiosity;
        }


        private bool CuriosityExists(int id)
        {
            return _context.Curiosities.Any(e => e.Id == id);
        }



        private string ProcessUploadedFile(CuriositiesPostDTO model)
        {
            string uniqueFileName = null;
            if (model.Image != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Image.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.Image.CopyTo(fileStream);
                }
            }

            return uniqueFileName;
        }

        private string ProcessUploadedFile(CuriositiesPutDTO model)
        {
            string uniqueFileName = null;
            if (model.Image != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Image.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.Image.CopyTo(fileStream);
                }
            }

            return uniqueFileName;
        }
    }
}
