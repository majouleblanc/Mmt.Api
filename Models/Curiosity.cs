using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Mmt.Api.Models
{
    public class Curiosity
    {
        public int Id { get; set; }

        public int CuriosityLikeId { get; set; }

        [Required]
        [MaxLength(255, ErrorMessage = "Naam mag niet langer dan 255 charakters zijn")]
        public string Name { get; set; }

        [Required]
        [MaxLength(255, ErrorMessage = "AfbeeldingURL mag niet langer dan 255 charakters zijn")]
        public string Image { get; set; }

        [MaxLength(50, ErrorMessage = "Coördinaten mag niet langer dan 50 charakters zijn")]
        public string Coordinates { get; set; }

        [MaxLength(25, ErrorMessage = "Lengtegraad mag niet langer dan 25 charakters zijn")]
        public string Longitude { get; set; }

        [MaxLength(25, ErrorMessage = "Breedtegraad	 mag niet langer dan 25 charakters zijn")]
        public string Latitude { get; set; }

        [MaxLength(60, ErrorMessage = "Type mag niet langer dan 60 charakters zijn")]
        public string Type { get; set; }

        [MaxLength(255, ErrorMessage = "Periode mag niet langer dan 255 charakters zijn")]
        public string Period { get; set; }

        [Required]
        [MaxLength(60, ErrorMessage = "Land mag niet langer dan 60 charakters zijn")]
        public string Country { get; set; }

        [MaxLength(120, ErrorMessage = "Regio mag niet langer dan 120 charakters zijn")]
        public string Region { get; set; }

        [MaxLength(120, ErrorMessage = "Provincie mag niet langer dan 120 charakters zijn")]
        public string Province { get; set; }

        [MaxLength(120, ErrorMessage = "Stad mag niet langer dan 120 charakters zijn")]
        public string City { get; set; }

        public string Description { get; set; }

        public ICollection<Comment> Comments { get; set; }

        public ICollection<Photo> Photos { get; set; }

        public ICollection<CuriosityLike> CuriosityLikes { get; set; }
        //public CuriosityLike CuriosityLikes { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public ICollection<TourCuriosity> ToursCuriosities { get; set; }

    //    [NotMapped]
    //    [JsonProperty(
    //    ObjectCreationHandling = ObjectCreationHandling.Replace
    //)]
    //    public IEnumerable<Tour> Tours
    //    {
    //        get => ToursCuriosities.Select(c => c.Tour);
    //        set => ToursCuriosities = value.Select(tc => new TourCuriosity()
    //        {
    //            TourId = tc.Id
    //        }).ToList();
    //    }
    }


}
