namespace Eportal.Shared;

public interface IUserLookupService
{
    Task<UserSummaryDto?> FindByEmailAsync(string email);
    Task<List<UserSummaryDto>> GetProfessorsAsync();
}