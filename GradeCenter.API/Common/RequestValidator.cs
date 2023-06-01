using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using GradeCenter.Data.Models.Account;
using System.Data;

public class RequestValidator
{
    private readonly UserManager<AspNetUser> _userManager;

    public RequestValidator(UserManager<AspNetUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IActionResult?> ValidateRequest(ModelStateDictionary modelState, ClaimsPrincipal user, List<UserRoles>? roles = null)
    {
        roles ??= new List<UserRoles> { UserRoles.Admin };

        var loggedUser = await GetLoggedUser(user);

        if (loggedUser == null || !IsInRole(loggedUser, roles))
            return new UnauthorizedResult();

        if (!modelState.IsValid)
            return new BadRequestObjectResult("Invalid model state.");

        return null;
    }

    public async Task<AspNetUser> GetLoggedUser(ClaimsPrincipal user)
    {
        return await _userManager.FindByNameAsync(user.Identity.Name);
    }

    public bool IsInRole(AspNetUser user, List<UserRoles>? roles)
    {
        return roles.Contains((UserRoles)user.UserRole);
    }
}
