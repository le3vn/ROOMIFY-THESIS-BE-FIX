using MediatR;
using Microsoft.AspNetCore.Identity;
using Roomify.Contracts.RequestModels.Authentication;
using Roomify.Contracts.ResponseModels.Authentication;
using Roomify.Entities;
using Roomify.Services;

namespace Accelist.Career.Commons.RequestHandlers.Authentication;

public class EmailLoginHandler : IRequestHandler<EmailLoginRequestModel, EmailLoginResponseModel>
{
    private readonly CustomSignInManager _signInManager;
    private readonly UserManager<User> _userManager; // Make sure to inject UserManager

    public EmailLoginHandler(CustomSignInManager signInManager, UserManager<User> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager; // Initialize UserManager
    }

    public async Task<EmailLoginResponseModel> Handle(EmailLoginRequestModel request, CancellationToken cancellationToken)
    {
        SignInResult result = await _signInManager.PasswordSignInAsync(request.Email, request.Password, request.RememberMe, lockoutOnFailure: false);

        if (result.Succeeded)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            var roles = await _userManager.GetRolesAsync(user); // Fetch user roles

            return new EmailLoginResponseModel
            {
                Success = true,
                Roles = roles.ToList(), // Convert roles to list
                ErrorMessage = string.Empty
            };
        }

        return new EmailLoginResponseModel
        {
            Success = false,
            ErrorMessage = await GetErrorMessage(result, request.Email),
            Roles = new List<string>() // Return an empty list if login fails
        };
    }

    private async Task<string> GetErrorMessage(SignInResult signInResult, string userEmail)
    {
        if (signInResult.Succeeded)
            return string.Empty;

        string errorMessage;
        var user = await _signInManager.UserManager.FindByNameAsync(userEmail);
        if (user is null)
            errorMessage = "Account not found";
        else if (signInResult.IsNotAllowed)
            errorMessage = "User Email not confirmed. Please confirm your email before logging in.";
        else
            errorMessage = "Wrong password. Try again or click Forgot Password to reset it";
        return errorMessage;
    }
}