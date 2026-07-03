namespace Eportal.Modules.Academic.Domain;

public class Student
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;

    public string IndexNumber { get; set; } = string.Empty;
    public int StudyProgramId { get; set; }
    public StudyProgram StudyProgram { get; set; } = null!;
    public int Espb { get; set; }
    public StudentStatus Status { get; set; } = StudentStatus.Aktivan;
}

public enum StudentStatus
{
    Aktivan = 0,
    Neaktivan = 1,
    Diplomirao = 2
}