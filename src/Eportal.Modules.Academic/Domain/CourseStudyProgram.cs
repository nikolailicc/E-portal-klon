namespace Eportal.Modules.Academic.Domain;

public class CourseStudyProgram
{
    public int CourseId { get; set; }
    public Course Course { get; set; } = null!;

    public int StudyProgramId { get; set; }
    public StudyProgram StudyProgram { get; set; } = null!;
}