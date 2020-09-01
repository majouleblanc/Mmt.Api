using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Mmt.Api.DTO.Auth;
using Mmt.Api.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Mmt.Api.services
{

    //in this class i want to extract the logic from the controller for reusability pursoses
    public class UserService : IUserService
    {
        private readonly UserManager<MmtUser> _UserManager;
        private readonly IConfiguration _Configuration;
        private readonly IMailService _MailService;

        //inject the identity usermanager class that wil help us manage the users, mailService to send emails
        public UserService(UserManager<MmtUser> userManager, IConfiguration configuration, IMailService mailService)
        {
            _UserManager = userManager;
            _Configuration = configuration;
            _MailService = mailService;
        }


        public async Task<UserManagerResponse> ResetPasswordAsync(ResetPasswordViewModel model)
        {
            var user = await _UserManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return new UserManagerResponse
                {
                    IsSucces = false,
                    Result = "No user associated with the email",
                };
            }

            if (model.NewPassword != model.ConfirmPassword)
            {
                return new UserManagerResponse
                {
                    IsSucces = false,
                    Result = "password doesn't match its confirmation",
                };
            }

            // decoding the token and converting it to normal string as it was when created
            var decodedToken = WebEncoders.Base64UrlDecode(model.Token);
            var normalToken = Encoding.UTF8.GetString(decodedToken);

            var result = await _UserManager.ResetPasswordAsync(user, normalToken, model.NewPassword);

            if (!result.Succeeded)
            {
                return new UserManagerResponse
                {
                    Result = "something went wrong",
                    IsSucces = false,
                    Errors = result.Errors.Select(e => e.Description)
                };
            }

            return new UserManagerResponse
            {
                Result = "Password has been reset successfully!",
                IsSucces = true
            };
        }



        public async Task<UserManagerResponse> ForgetPasswordAsync(string email, string returnUrl)
        {
            var user = await _UserManager.FindByEmailAsync(email);

            if (user == null)
            {
                return new UserManagerResponse
                {
                    IsSucces = false,
                    Result = "No user associated with email",
                };
            }

            var token = await _UserManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = Encoding.UTF8.GetBytes(token);
            var validToken = WebEncoders.Base64UrlEncode(encodedToken);

            string url = returnUrl + $"?email={user.Email}&&toke={validToken}";
            await _MailService.SendEmailAsync(user.Email, "Reset Password",
                "<h1>Follow the instruction to reset your password</h1>" + $"<p>To reset the password <a href = {url}> click here</a></p>");
            return new UserManagerResponse
            {
                IsSucces = true,
                Result = "Reset password URL has been sent to the email successfully"
            };
        }



        public async Task<UserManagerResponse> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _UserManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new UserManagerResponse
                {
                    IsSucces = false,
                    Result = "user not found",
                };
            }

            var decodedToken = WebEncoders.Base64UrlDecode(token);
            string normalToken = Encoding.UTF8.GetString(decodedToken);

            var result = await _UserManager.ConfirmEmailAsync(user, normalToken);

            if (!result.Succeeded)
            {
                return new UserManagerResponse
                {
                    IsSucces = false,
                    Result = "Email dit not confirm",
                    Errors = result.Errors.Select(e => e.Description)
                };
            }

            return new UserManagerResponse
            {
                IsSucces = true,
                Result = "Email confirmed successfully"
            };
        }


        //logic to login a user
        public async Task<UserManagerResponse> LoginUserAsync(LoginViewModel model)
        {
            var user = await _UserManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return new UserManagerResponse
                {
                    IsSucces = false,
                    Result = "no user with this email found",
                };
            }

            // this function wil hash the password provided in the model and checks if it matches the password hash in the db
            var result = await _UserManager.CheckPasswordAsync(user, model.Password);

            if (!result)
            {
                return new UserManagerResponse
                {
                    IsSucces = false,
                    Result = "Invalid Password",
                };
            }

            if (!user.EmailConfirmed)
            {
                return new UserManagerResponse
                {
                    IsSucces = false,
                    Result = "Email not confirmed yet"
                };
            }

            //here are the steps to generate a security token
            //1st step is to generate a claim array for the user
            var userClaims = new List<Claim>
            {
                new Claim("Email", user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
            };


            var userRole = (await _UserManager.GetRolesAsync(user));

            foreach (var role in userRole)
            {
                userClaims.Add(new Claim(ClaimTypes.Role, role));
            }


            //2nd step generate the security key that we are going to use to encrypte the token with
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_Configuration["AuthSettings:Key"]));

            //3de step is to generate the token itself with those parametres
            //we set the expire date to 30 days and the encryption algorithm to HmacSha256
            var token = new JwtSecurityToken(
                issuer : _Configuration["AuthSettings:Issuer"],
                audience : _Configuration["AuthSettings:Audience"],
                claims : userClaims,
                expires : DateTime.Now.AddDays(30),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
                );

            // 4th step we convert the token to string because the token can contains special characters that are not allowed in the url such / +  etc
            string TokenAsString = new JwtSecurityTokenHandler().WriteToken(token);

            //here we send the access token to the client so he can use it to access the protected resources
            return new UserManagerResponse
            {
                Result = TokenAsString,
                IsSucces = true,
                ExpireDate = token.ValidTo
            };
        }


        //logic to register a user
        public async Task<UserManagerResponse> RegisterUserAsync(RegisterViewModel model)
        {
            if (model == null)
            {
                throw new NullReferenceException("model is null");
            }

            if (model.Password != model.ConfirmPassword)
                return new UserManagerResponse
                {
                    Result = "Confirm password dont match password",
                    IsSucces = false,
                };

            //i should realy user automapper hier -lot of properties to manualy map
            var mmtUser = new MmtUser
            {
                UserName = model.UserName,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                City = model.City,
                Country = model.Country,
                Function = model.Function,
                PhoneHome = model.PhoneHome,
                PhoneWork = model.PhoneWork,
                PhoneNumber = model.Mobile,
                Street = model.Street,
                PostalCode = model.PostalCode,
            };

            //here we create a user and add it to aspnetUser table
            var result = await _UserManager.CreateAsync(mmtUser, model.Password);

            if (!result.Succeeded)
            {
                return new UserManagerResponse
                {
                    Result = "failed to create the user",
                    IsSucces = false,
                    Errors = result.Errors.Select(e => e.Description),
                };
            }

            await _UserManager.AddToRoleAsync(mmtUser, "user");

            //here we generate an email confirmation token, identity wil take care of that
            var confirmEmailTokenn = await _UserManager.GenerateEmailConfirmationTokenAsync(mmtUser);

            //we need to convert the token to an array of bytes to we able to encode it to base64
            var encodedEmailToken = Encoding.UTF8.GetBytes(confirmEmailTokenn);

            //we encode the token to base64 to get rid of all special charcters that the browsers doesnt accept in the url, because wil wil send the token in the url
            var validEmailToken = WebEncoders.Base64UrlEncode(encodedEmailToken);

            string url = $"{_Configuration["AppUrl:Dev"]}/api/auth/confirmEmail?userid={mmtUser.Id}&token={validEmailToken}";

            //sending the email to the user
            await _MailService.SendEmailAsync(mmtUser.Email, "Confirm your email", $"<h1>Welcome to Mmt</h1>" +
                $"<p>Please confirm your email by <a href={url}>Clicking here</a></p>");
            return new UserManagerResponse
            {
                Result = $"The user : {model.UserName} has been created successfully",
                IsSucces = true,
            };

        }


        // logic to check if the email is already in use during the registration of a new user
        public async Task<UserManagerResponse> IsEmailInUser(string email)
        {
            var user = await _UserManager.FindByEmailAsync(email);

            if (user == null) return new UserManagerResponse{
                                                              IsSucces = true,
                                                              Result = "false"};

            return new UserManagerResponse
            {
                IsSucces = true,
                Result = "true"
            };
        }
    }
}
