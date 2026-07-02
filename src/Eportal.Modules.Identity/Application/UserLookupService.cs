using Eportal.Modules.Identity.Domain;
using Eportal.Shared;
using Microsoft.AspNetCore.Identity;


namespace Eportal.Modules.Identity.Application;

public class UserLookupService : IUserLookupService
{
    private readonly UserManager<AppUser> _userManager;

    public UserLookupService(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<UserSummaryDto?> FindByEmailAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
        {
            return null;
        }

        return new UserSummaryDto(user.Id, user.FirstName, user.LastName, user.Email ?? "");
    }
    public async Task<List<UserSummaryDto>> GetProfessorsAsync()
    {
        var professors = await _userManager.GetUsersInRoleAsync("Profesor");

        return professors
            .Select(p => new UserSummaryDto(p.Id, p.FirstName, p.LastName, p.Email ?? ""))
            .ToList();
    }
}