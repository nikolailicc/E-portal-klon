namespace Eportal.Modules.Exams.Domain;

public class ExamPeriod
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}