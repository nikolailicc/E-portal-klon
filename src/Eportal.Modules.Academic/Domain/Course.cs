namespace Eportal.Modules.Academic.Domain;

public class Course
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public int Espb { get; set; }
    public int Semester { get; set; }

    public string ProfessorUserId { get; set; } = string.Empty;
}