namespace Eportal.Modules.Exams.Domain;

public class ExamRegistration
{
    public int Id { get; set; }

    public int ExamId { get; set; }
    public Exam Exam { get; set; } = null!;

    public int StudentId { get; set; }

    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

    public int? Grade { get; set; }
    public DateTime? GradedAt { get; set; }
}