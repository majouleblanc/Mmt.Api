using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Mmt.Api.Models
{
    public class MmtUser : IdentityUser
    {

        [Required]
        [MaxLength(50, ErrorMessage = "Voornaam mag niet langer dan 50 charakters zijn")]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(120, ErrorMessage = "Achternaam mag niet langer dan 120 charakters zijn")]
        public string LastName { get; set; }

        [MaxLength(255, ErrorMessage = "Straat mag niet langer dan 255 charakters zijn")]
        public string Street { get; set; }

        [MaxLength(20, ErrorMessage = "Postcode mag niet langer dan 20 charakters zijn")]
        public string PostalCode { get; set; }

        [MaxLength(80, ErrorMessage = "Stad mag niet langer dan 80 charakters zijn")]
        public string City { get; set; }

        public string Country { get; set; }

        [MaxLength(25, ErrorMessage = "Telefoon thuis mag niet langer dan 25 charakters zijn")]
        public string PhoneHome { get; set; }

        [MaxLength(25, ErrorMessage = "Telefoon werk mag niet langer dan 25 charakters zijn")]
        public string PhoneWork { get; set; }

        [MaxLength(255, ErrorMessage = "Functie mag niet langer dan 255 charakters zijn")]
        public string Function { get; set; }
    }
}
