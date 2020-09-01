using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Mmt.Api.DTO.Auth
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [MaxLength(255)]
        [Required]
        public string Password { get; set; }
    }
}
