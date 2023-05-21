using GradeCenter.Data.Models.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Security.Claims;

public class RequestValidator
{
    private readonly UserManager<AspNetUser> _userManager;
    private ClaimsPrincipal _user;

    public RequestValidator(UserManager<AspNetUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IActionResult?> ValidateRequest(ModelStateDictionary modelState, ClaimsPrincipal user)
    {
        var loggedUser = await GetLoggedUser(user);

        if (loggedUser == null || !IsAdmin(loggedUser))
            return new UnauthorizedResult();

        if (!modelState.IsValid)
            return new BadRequestObjectResult("Invalid model state.");

        return null;
    }

    public async Task<AspNetUser> GetLoggedUser(ClaimsPrincipal user)
    {
        return await _userManager.FindByNameAsync(user.Identity.Name);
    }

    public bool IsAdmin(AspNetUser user)
    {
        return user.UserRole.Equals(UserRoles.Admin);
    }
}
