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
using Mmt.Api.DTO.Photo;
using Mmt.Api.Models;
using Mmt.Api.services;

namespace Mmt.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "DIRECTOR, SUPERVISOR, USER")]
    public class PhotosController : ControllerBase
    {
        private readonly MmtContext _context;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IConfiguration _Configuration;
        private readonly IAzureFileService _AzureFileService;

        public PhotosController(MmtContext context,
            IWebHostEnvironment webHostEnvironment,
            IConfiguration configuration,
            IAzureFileService azureFileService)
        {
            _context = context;
            this.webHostEnvironment = webHostEnvironment;
            _Configuration = configuration;
            _AzureFileService = azureFileService;
        }

        // GET: api/Photos
        [HttpGet("CuriosityWithId/{curiosityId}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<PhotoGetDTO>>> GetPhotos(int curiosityId)
        {
            if (curiosityId <= 0)
            {
                ModelState.AddModelError("", $"invalid curiosity id, curiosity id = {curiosityId}");
                return BadRequest(ModelState);
            }

            var curiosity = await _context.Curiosities.FirstOrDefaultAsync(c => c.Id == curiosityId);
            if (curiosity == null)
            {
                return BadRequest($"no curiosity with id {curiosityId} was found");
            }

            IQueryable<Photo> query = _context.Photos;

            query = query.Where(p => p.CuriosityId == curiosityId);


            var photos = await query.ToListAsync();

            List<PhotoGetDTO> photosDTO = new List<PhotoGetDTO>();

            foreach (var photo in photos)
            {
                photosDTO.Add(ProcessPhotoDTO(photo).Result);
            }
            return Ok(photosDTO);

            //return NotFound($"no photos for curiosity with id {curiosityId} were found");

        }

        //GET: api/Photos/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<PhotoGetDTO>> GetPhoto(int id)
        {
            var photo = await _context.Photos.FindAsync(id);

            if (photo == null)
            {
                return NotFound();
            }

            return await ProcessPhotoDTO(photo);
        }

        private async Task<PhotoGetDTO> ProcessPhotoDTO(Photo photo)
        {
            return new PhotoGetDTO()
            {
                CuriosityId = photo.CuriosityId,
                Id = photo.Id,
                //PhotoUrl = "/uploads/gallery/" + photo.Id + "/" + photo.PhotoPath
                //PhotoUrl = _Configuration["CuriositiesPhotosPath"] + photo.CuriosityId + "/" + photo.PhotoPath
                PhotoUrl = _Configuration["CuriositiesPhotosPath"] + photo.PhotoPath
            };
        }



        // POST: api/Photos
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.


        [HttpPost]
        public async Task<ActionResult<Photo>> PostPhoto([FromForm] PhotosPostDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //string uniqueFileName = ProcessFoto(model);
            string uniqueFileName = await _AzureFileService.ProcessPhotoGalleryForCuriosityAsync(model);

            Photo photo = new Photo()
            {
                CuriosityId = model.CuriosityId,
                PhotoPath = uniqueFileName
            };

            _context.Photos.Add(photo);
            await _context.SaveChangesAsync();

            var photoDTO = await ProcessPhotoDTO(photo);

            return CreatedAtAction("GetPhoto", new { id = photo.Id }, photoDTO);


            //return Ok(newMmtFoto);
            //return CreatedAtAction("GetPhoto", new { CuiosityId = photo.CuriosityId }, photo);





            //_context.Photos.Add(photo);
            //await _context.SaveChangesAsync();

            //return CreatedAtAction("GetPhoto", new { id = photo.Id }, photo);
        }

        // DELETE: api/Photos/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "DIRECTOR, SUPERVISOR")]
        public async Task<ActionResult<Photo>> DeletePhoto(int id)
        {
            var photo = await _context.Photos.FindAsync(id);
            if (photo == null)
            {
                return NotFound();
            }

            if (photo.PhotoPath != null)
            {
                //System.IO.File.Delete(webHostEnvironment.WebRootPath + "/uploads//gallery/" + photo.CuriosityId + "/" + photo.PhotoPath);
                await _AzureFileService.DeleteCuriosityPhotoGalleryAsync(photo.PhotoPath); 
            }


            _context.Photos.Remove(photo);
            await _context.SaveChangesAsync();

            return photo;
        }



        private bool PhotoExists(int id)
        {
            return _context.Photos.Any(e => e.Id == id);
        }


        private string ProcessFoto(PhotosPostDTO model)
        {
            string uniqueFileName = null;
            if (model.Photo != null)
            {
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;


                var uploadsFolder = webHostEnvironment.WebRootPath + "/uploads/gallery/" + model.CuriosityId;

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }


                //string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.Photo.CopyTo(fileStream);
                }
            }

            return uniqueFileName;
        }

        // PUT: api/Photos/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutPhoto(int id, Photo photo)
        //{
        //    if (id != photo.Id)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(photo).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!PhotoExists(id))
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
    }
}
