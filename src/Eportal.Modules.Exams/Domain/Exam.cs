namespace Eportal.Modules.Exams.Domain;

public class Exam
{
    public int Id { get; set; }

    public int ExamPeriodId { get; set; }
    public ExamPeriod ExamPeriod { get; set; } = null!;

    public int CourseId { get; set; }
    public DateTime ExamDate { get; set; }
}