using Microsoft.AspNetCore.Identity;

namespace Eportal.Modules.Identity.Domain;

public class AppUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public UserRole Role { get; set; }
}

public enum UserRole
{
    Student = 0,
    Profesor = 1,
    StudentskaSluzba = 2,
    Administrator = 3
}