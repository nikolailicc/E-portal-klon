namespace Eportal.Shared;

public interface IUserLookupService
{
    Task<UserSummaryDto?> FindByEmailAsync(string email);
    Task<UserSummaryDto?> FindByUserIdAsync(string userId);
    Task<List<UserSummaryDto>> GetProfessorsAsync();
}