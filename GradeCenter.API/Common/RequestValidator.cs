using GradeCenter.Data.Models.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Security.Claims;

public class RequestValidator
{
    private readonly UserManager<AspNetUser> _userManager;
    private readonly ClaimsPrincipal _user;

    public RequestValidator(UserManager<AspNetUser> userManager, ClaimsPrincipal user)
    {
        _userManager = userManager;
        _user = user;
    }

    public async Task<IActionResult?> ValidateRequest(ModelStateDictionary modelState)
    {
        var loggedUser = await GetLoggedUser();

        if (loggedUser == null || !IsAdmin(loggedUser))
            return new UnauthorizedResult();

        if (!modelState.IsValid)
            return new BadRequestObjectResult("Invalid model state.");

        return null;
    }

    public async Task<AspNetUser> GetLoggedUser()
    {
        return await _userManager.FindByNameAsync(_user.Identity.Name);
    }

    public bool IsAdmin(AspNetUser user)
    {
        return user.UserRole.Equals(UserRoles.Admin);
    }
}
