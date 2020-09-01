using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Mmt.Api.DTO.Role;
using Mmt.Api.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Mmt.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "DIRECTOR")]
    public class RolesController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _RoleManager;

        public RolesController(RoleManager<IdentityRole> roleManager)
        {
            _RoleManager = roleManager;
        }
        // GET: api/<RolesController>
        [HttpGet]
        public ActionResult<IEnumerable<IdentityRole>> GetRoles()
        {

            var roles = _RoleManager.Roles;
            return Ok(roles);
        }

        // GET api/<RolesController>/5
        [HttpGet("{roleId}")]
        public async Task<ActionResult<IdentityRole>> GetRoleById(string roleId)
        {
            var role = await _RoleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                return NotFound($"Role with id =  {roleId} does not exist!");
            }
            return Ok(role);
        }

        // POST api/<RolesController>
        [HttpPost]
        public async Task<ActionResult<IdentityRole>> Post([FromForm] RolePostDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityRole role = new IdentityRole
            {
                Name = model.RoleName,
            };

            var result = await _RoleManager.CreateAsync(role);

            if (result.Succeeded)
            {
                return Ok(role);
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return BadRequest(ModelState);
        }

        // PUT api/<RolesController>/5
        [HttpPut("{roleId}")]
        public async Task<ActionResult<IdentityRole>> PutRole(string roleId, [FromForm] RolePutDTO model)
        {
            var role = await _RoleManager.FindByIdAsync(roleId);

            if (role==null)
            {
                ModelState.AddModelError("", $"role with Id {roleId} does not exist!");
                return NotFound(ModelState);
            }

            if (string.IsNullOrWhiteSpace(model.RoleName))
            {
                ModelState.AddModelError("", "Invalid role name");
                return BadRequest(ModelState);
            }

            role.Name = model.RoleName;

            var result =await _RoleManager.UpdateAsync(role);
            if (result.Succeeded)
            {
                return Ok(role);
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return BadRequest(ModelState);
        }

        // DELETE api/<RolesController>/5
        [HttpDelete("{roleId}")]
        public async Task<ActionResult<IdentityRole>> Delete(string roleId)
        {
            var role = await _RoleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                return NotFound($"Role with id =  {roleId} does not exist!");
            }

            var result = await _RoleManager.DeleteAsync(role);

            if (result.Succeeded)
            {
                return Ok(role);
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return BadRequest(ModelState);
        }
    }
}
