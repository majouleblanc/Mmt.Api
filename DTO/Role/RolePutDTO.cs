using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Mmt.Api.DTO.Role
{
    public class RolePutDTO
    {
        [Required]
        [MaxLength(50, ErrorMessage = "Rol mag niet langer dan 50 charakters zijn")]
        public string RoleName { get; set; }
    }
}
