using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Mmt.Api.DTO.Auth;
using Mmt.Api.Models;
using Mmt.Api.services;

namespace Mmt.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMailService _MailService;
        private readonly IConfiguration _Configuration;
        private readonly UserManager<MmtUser> _UserManager;
        private readonly RoleManager<IdentityRole> _RoleManager;

        public AuthController(IUserService userService, IMailService mailService, IConfiguration configuration, UserManager<MmtUser> manager, RoleManager<IdentityRole> roleManager)
        {
            _userService = userService;
            _MailService = mailService;
            _Configuration = configuration;
            _UserManager = manager;
            _RoleManager = roleManager;
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromForm] ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.ResetPasswordAsync(model);

            if (result.IsSucces)
                return Ok(result);

            return BadRequest(result);

        }

        // api/auth/forgetpassword
        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword(string email, string returnUrl)
        {
            if (string.IsNullOrEmpty(email))
                return NotFound("Email is empty");
            
            if (string.IsNullOrEmpty(returnUrl))
                return BadRequest("the return url is empty");

            var result = await _userService.ForgetPasswordAsync(email, returnUrl);
            if (!result.IsSucces)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        // api/ath/confirmEmail
        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token, string returnUrl)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
                return NotFound();

            if (string.IsNullOrWhiteSpace(token))
                return Unauthorized();
            

            var result = await _userService.ConfirmEmailAsync(userId, token);

            if (result.IsSucces)
            {
                //return Redirect(returnUrl);
                return Ok(result);
            }

            return BadRequest(result);
        }

        // api/auth/login
        [HttpPost("Login")]
        public async Task<IActionResult> LoginUserAsync([FromForm] LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userService.LoginUserAsync(model);
            if (result.IsSucces)
            {
                var user = await _UserManager.FindByEmailAsync(model.Email);

                result.FirstName = user.FirstName;
                result.LastName = user.LastName;
                result.Functie = user.Function;
                result.Roles = (await _UserManager.GetRolesAsync(user)).ToList();
                return Ok(result);
            }

            return BadRequest(result);

        }

        // api/auth/register
        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAsync([FromForm] RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userService.RegisterUserAsync(model);

            if (result.IsSucces)
            {
                return Ok(result);
            }
            return BadRequest(result);

        }

        // api/auth/IsEmailInUse
        [HttpGet("IsEmailInUse")]
        [HttpPost("IsEmailInUse")]
        public async Task<IActionResult> IsEmailInUse(string Email)
        {
            var result = await _userService.IsEmailInUser(Email);
            return Ok(result);
        }
    }
}
