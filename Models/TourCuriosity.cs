using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Mmt.Api.Models
{
    public class TourCuriosity
    {
        //[NotMapped]
        public int TourId { get; set; }

        //[NotMapped]
        public int CuriosityId { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public Tour Tour { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public Curiosity Curiosity { get; set; }
    }
}
