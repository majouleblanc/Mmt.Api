using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Mmt.Api.DTO.Photo
{
    public class PhotoGetDTO
    {
        public int Id { get; set; }

        [Required]
        public int CuriosityId { get; set; }

        public string PhotoUrl { get; set; }
    }
}
