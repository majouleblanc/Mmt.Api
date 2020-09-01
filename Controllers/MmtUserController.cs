using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Mmt.Api.DTO.MmtUser;
using Mmt.Api.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Mmt.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "DIRECTOR")]
    [AllowAnonymous]
    public class MmtUserController : ControllerBase
    {
        private readonly UserManager<MmtUser> _UserManager;
        private readonly RoleManager<IdentityRole> _RoleManager;

        public MmtUserController(UserManager<MmtUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _UserManager = userManager;
            _RoleManager = roleManager;
        }

        // GET: api/<MmtUserController>
        [HttpGet("MmtUsers")]
        public ActionResult<IEnumerable<MmtUser>> GetMmtUsers()
        {
            var users = _UserManager.Users;
            return Ok(users);
        }

        // GET api/<MmtUserController>/5
        [HttpGet("{Id}")]
        public async Task<ActionResult<MmtUserGetDTO>> GetMmtUser(string Id)
        {
            var mmtUser = await _UserManager.FindByIdAsync(Id);

            if (mmtUser == null)
            {
                return NotFound($"User with Id : {Id} not found ");
            }

            var mmtUserGetDTO = new MmtUserGetDTO()
            {
                Id = mmtUser.Id,
                UserName = mmtUser.UserName,
                FirstName = mmtUser.FirstName,
                LastName = mmtUser.LastName,
                Email = mmtUser.Email,
                EmailConfirmed = mmtUser.EmailConfirmed,
                PostalCode = mmtUser.PostalCode,
                City = mmtUser.City,
                Country = mmtUser.Country,
                Mobile = mmtUser.PhoneNumber,
                PhoneNumberConfirmed = mmtUser.PhoneNumberConfirmed,
                PhoneHome = mmtUser.PhoneHome,
                PhoneWork = mmtUser.PhoneWork,
                Function = mmtUser.Function,
                Street = mmtUser.Street,
                
            };
            var allRoles = _RoleManager.Roles;
            var rolsAsString = await _UserManager.GetRolesAsync(mmtUser);
            List<IdentityRole> roles = new List<IdentityRole>();
            foreach (var item in rolsAsString)
            {
                foreach (var role in allRoles)
                {
                    if (item == role.Name)
                    {
                        roles.Add(role);
                    }
                }
            }

            //mmtUserGetDTO.Roles= await _UserManager.GetRolesAsync(mmtUser);
            mmtUserGetDTO.Roles = roles;

            return Ok(mmtUserGetDTO);
        }

        // POST api/<MmtUserController>
        [HttpPost("PostMmtUser")]
        public async Task<ActionResult<MmtUserGetDTO>> PostMmtUser([FromForm] MmtUserPostDTO model)
        {
            var mmtUser = await _UserManager.FindByEmailAsync(model.Email);

            if (mmtUser != null)
            {
                ModelState.AddModelError("", "there is already a user with the same Email");
                return BadRequest(ModelState);
            }
            if (model.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError("", "password doent match his confirmation");
                return BadRequest(ModelState);
            }

            mmtUser = new MmtUser
            {
                UserName = model.UserName,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                PostalCode = model.PostalCode,
                City = model.City,
                Country = model.Country,
                PhoneNumber = model.Mobile,
                PhoneHome = model.PhoneHome,
                PhoneWork = model.PhoneWork,
                Function = model.Function,
                Street = model.Street,
                EmailConfirmed = model.ConfirmEmail,
            };

            var result = await _UserManager.CreateAsync(mmtUser, model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return BadRequest(ModelState);
            }

            if (model.Roles != null)
            {
                await _UserManager.AddToRolesAsync(mmtUser, model.Roles);
            }


            var mmtUserGetDTO = new MmtUserGetDTO
            {
                Id = mmtUser.Id,
                UserName = mmtUser.UserName,
                FirstName = mmtUser.FirstName,
                LastName = mmtUser.LastName,
                Email = mmtUser.Email,
                PostalCode = mmtUser.PostalCode,
                City = mmtUser.City,
                Country = mmtUser.Country,
                Mobile = mmtUser.PhoneNumber,
                PhoneHome = mmtUser.PhoneHome,
                PhoneWork = mmtUser.PhoneWork,
                Function = mmtUser.Function,
                Street = mmtUser.Street,
            };

            var allRoles = _RoleManager.Roles;
            var rolsAsString = await _UserManager.GetRolesAsync(mmtUser);
            List<IdentityRole> roles = new List<IdentityRole>();
            foreach (var item in rolsAsString)
            {
                foreach (var role in allRoles)
                {
                    if (item == role.Name)
                    {
                        roles.Add(role);
                    }
                }
            }
            //mmtUserGetDTO.Roles = await _UserManager.GetRolesAsync(mmtUser);
            mmtUserGetDTO.Roles = roles;

            return Ok(mmtUserGetDTO);
        }

        // PUT api/<MmtUserController>/5
        [HttpPut("{MmtUserId}")]
        public async Task<ActionResult<MmtUser>> PutMmtUser(string MmtUserId, [FromForm] MmtUserPutDTO model)
        {
            var mmtUser = await _UserManager.FindByIdAsync(MmtUserId);

            if (mmtUser == null)
            {
                return NotFound();
            };

            mmtUser.UserName = model.UserName;
            mmtUser.FirstName = model.FirstName;
            mmtUser.LastName = model.LastName;
            mmtUser.Email = model.Email;
            mmtUser.PostalCode = model.PostalCode;
            mmtUser.City = model.City;
            mmtUser.Country = model.Country;
            mmtUser.PhoneNumber = model.Mobile;
            mmtUser.PhoneHome = model.PhoneHome;
            mmtUser.PhoneWork = model.PhoneWork;
            mmtUser.Function = model.Function;
            mmtUser.EmailConfirmed = model.EmailConfirmed;
            // check if password is not empty
            if (!string.IsNullOrWhiteSpace(model.Password))
            {

                if (model.Password != model.ConfirmPassword)
                {
                    return BadRequest(ModelState);
                }

                // if password is not empty and password and his confirmation matchs we validate the password according to the password policy
                foreach (var v in _UserManager.PasswordValidators)
                {
                    var result = await v.ValidateAsync(_UserManager, null, model.Password);
                    if (!result.Succeeded)
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                        return BadRequest(ModelState);
                    }
                }

                //removing the old password
                await _UserManager.RemovePasswordAsync(mmtUser);

                //adding the new validated password
                await _UserManager.AddPasswordAsync(mmtUser, model.Password);
            };

            
            // if there is any roles in the model then we validate them and check if they exist in the db
            if (model.Roles != null)
            {
                foreach (var role in model.Roles)
                {
                    if (!(await _RoleManager.RoleExistsAsync(role)))
                    {
                        ModelState.AddModelError("", $"{role} does not exists");
                        return BadRequest(ModelState);
                    }
                }

                // we delete the old roles for the user
                var oldUserRoles = await _UserManager.GetRolesAsync(mmtUser);
                var result = await _UserManager.RemoveFromRolesAsync(mmtUser, oldUserRoles);

                if (!result.Succeeded)
                {
                    return BadRequest(ModelState);
                }

                // we add the new roles to the user
                var addToRolesResult = await _UserManager.AddToRolesAsync(mmtUser, model.Roles);
                if (!addToRolesResult.Succeeded)
                {
                    return BadRequest(ModelState);
                }
            }

            var updateMmtUserResult = await _UserManager.UpdateAsync(mmtUser);

            if (!updateMmtUserResult.Succeeded)
            {
                return BadRequest(ModelState);
            }

            return Ok(mmtUser);
        }

        // DELETE api/<MmtUserController>/5
        [HttpDelete("{mmtUserId}")]
        public async Task<ActionResult<MmtUser>> Delete(string mmtUserId)
        {
            var mmtUser = await _UserManager.FindByIdAsync(mmtUserId);

            if (mmtUser == null)
            {
                return NotFound();
            }

            var result = await _UserManager.DeleteAsync(mmtUser);
            if (!result.Succeeded)
            {
                return BadRequest(ModelState);
            }

            return Ok(mmtUser);

        }
    }
}
