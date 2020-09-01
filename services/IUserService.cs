using Mmt.Api.DTO.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mmt.Api.services
{
    public interface IUserService
    {
        Task<UserManagerResponse> RegisterUserAsync(RegisterViewModel model);

        Task<UserManagerResponse> LoginUserAsync(LoginViewModel model);

        Task<UserManagerResponse> ConfirmEmailAsync(string userId, string token);

        Task<UserManagerResponse> ForgetPasswordAsync(string email, string returnUrl);

        Task<UserManagerResponse> ResetPasswordAsync(ResetPasswordViewModel model);

        Task<UserManagerResponse> IsEmailInUser(string email);

    }
}
