﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Mmt.Api.DTO.Tours
{
    public class TourPutDTO
    {
        [Required]
        [MaxLength(255, ErrorMessage = "Naam mag niet langer dan 255 charakters zijn")]
        public string Name { get; set; }

        public  IFormFile Image { get; set; }

        [MaxLength(50, ErrorMessage = "Coördinaten mag niet langer dan 50 charakters zijn")]
        public string Coordinates { get; set; }

        [MaxLength(25, ErrorMessage = "Lengtegraad mag niet langer dan 25 charakters zijn")]
        public string Longitude { get; set; }

        [MaxLength(25, ErrorMessage = "Breedtegraad	 mag niet langer dan 25 charakters zijn")]
        public string Latitude { get; set; }

        [MaxLength(60, ErrorMessage = "Type mag niet langer dan 60 charakters zijn")]
        public string Type { get; set; }

        [Required]
        [MaxLength(60, ErrorMessage = "Land mag niet langer dan 60 charakters zijn")]
        public string Country { get; set; }

        [MaxLength(120, ErrorMessage = "Regio mag niet langer dan 120 charakters zijn")]
        public string Region { get; set; }

        [MaxLength(120, ErrorMessage = "Provincie mag niet langer dan 120 charakters zijn")]
        public string Province { get; set; }

        [MaxLength(120, ErrorMessage = "Stad mag niet langer dan 120 charakters zijn")]
        public string City { get; set; }

        [MaxLength(255, ErrorMessage = "Periode mag niet langer dan 255 charakters zijn")]
        public string Description { get; set; }
    }
}
