using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Mmt.Api.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Mmt.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "DIRECTOR")]
    public class AdministrationController : ControllerBase
    {
        private readonly UserManager<MmtUser> _UserManager;
        private readonly RoleManager<IdentityRole> _RoleManager;

        public AdministrationController(RoleManager<IdentityRole> roleManager , UserManager<MmtUser> userManager)
        {
            _RoleManager = roleManager;
            _UserManager = userManager;
        }


        [HttpGet("userRoles/{userId}")]
        public async Task<IActionResult> GetUserRoles(string userId)
        {
            var mmtUser = await _UserManager.FindByIdAsync(userId);

            if (mmtUser == null)
            {
                return NotFound($"the user with id = {userId} not found");
            }
            var allRoles = _RoleManager.Roles;

            var userRolesNames = await _UserManager.GetRolesAsync(mmtUser);

            List<IdentityRole> roles = new List<IdentityRole>();
            foreach (var roleName in userRolesNames)
            {
                foreach (var role in allRoles)
                {
                    if (role.Name == roleName)
                    {
                        roles.Add(role);
                    }
                }
            }

            return Ok(roles);
        }

        // api/roles/GetUsersInRole/20-20klnls
        [HttpGet("GetUsersInRole/{roleId}")]
        public async Task<IActionResult> GetUsersInRole(string roleId)
        {
            var role = await _RoleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                ModelState.AddModelError("", $"role with id = {roleId} not found");
                return NotFound(ModelState);
            }

            var mmtUsers = await _UserManager.GetUsersInRoleAsync(role.Name);

            return Ok(mmtUsers);
        }


    }
}
