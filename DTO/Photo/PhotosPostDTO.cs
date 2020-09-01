using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Mmt.Api.DTO.Photo
{
    public class PhotosPostDTO
    {
        [Required]
        public int CuriosityId { get; set; }

        public IFormFile Photo { get; set; }
    }
}
